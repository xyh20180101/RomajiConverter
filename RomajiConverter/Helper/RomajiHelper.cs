using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ja;
using Lucene.Net.Analysis.Ja.TokenAttributes;
using Lucene.Net.Analysis.TokenAttributes;
using MeCab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WanaKanaSharp;

namespace RomajiConverter.Helper
{
    public static class RomajiHelper
    {
        public static List<ReturnText> ToRomaji(string text, bool isSpace = true, float chineseRate = 1f)
        {
            try
            {
                var lineTextList = text.RemoveEmptyLine().Split(Environment.NewLine);

                var returnList = new List<ReturnText>();


                for (var index = 0; index < lineTextList.Length; index++)
                {
                    var line = lineTextList[index];

                    var returnText = new ReturnText();

                    if (IsChinese(line, chineseRate))
                    {
                        continue;
                    }

                    returnText.Japanese = line;
                    returnText.Romaji = UnitToRomaji(line, isSpace);
                    if (index + 1 < lineTextList.Length &&
                        IsChinese(lineTextList[index + 1], chineseRate))
                    {
                        returnText.Chinese = lineTextList[index + 1];
                    }

                    returnText.Index = returnList.Count;
                    returnList.Add(returnText);
                }

                return returnList;
            }
            catch (Exception e)
            {
                return new List<ReturnText>();
            }
        }

        /// <summary>
        /// 判断字符串(句子)是否简体中文
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rate">容错率(0-1)</param>
        /// <returns></returns>
        public static bool IsChinese(string str, float rate)
        {
            if (rate > 1 || rate < 0)
                throw new Exception("容错率超出范围");

            if (str.Length < 2)
                return false;

            var wordArray = str.ToCharArray();

            var total = wordArray.Length;

            var chCount = 0f;

            var enCount = 0f;

            foreach (var word in wordArray)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(word.ToString(), @"^[\u3040-\u30ff]+$"))
                {
                    //含有日文直接返回否
                    return false;
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var gbBytes = Encoding.GetEncoding("GB2312").GetBytes(word.ToString());

                if (gbBytes.Length == 2)  // double bytes char.  
                {
                    if (gbBytes[0] >= 0xB0 && gbBytes[0] <= 0xF7 && gbBytes[1] >= 0xA1 && gbBytes[1] <= 0xFE)//简体中文
                    {
                        chCount++;
                    }
                    else if (gbBytes[0] >= 0xA0 && gbBytes[0] <= 0xA3 && gbBytes[1] >= 0xA1 && gbBytes[1] <= 0xFE)//中文符号
                    {
                        total--;
                    }
                }
                else if (gbBytes.Length == 1)
                {
                    var byteAscii = int.Parse(gbBytes[0].ToString());
                    if ((byteAscii >= 65 && byteAscii <= 90) || (byteAscii >= 97 && byteAscii <= 122)) //英文字母
                    {
                        enCount++;
                    }
                    else//英文符号
                    {
                        total--;
                    }
                }
            }

            if (chCount == 0) return false;//一个简体中文都没有

            return (chCount + enCount) / total >= rate;
        }

        public static string GetPronunciation(string str)
        {
            var result = new StringBuilder();
            using (var sr = new StringReader(str))
            {
                Tokenizer tokenizer = new JapaneseTokenizer(sr, null, true, JapaneseTokenizerMode.NORMAL);
                using (var tokenStream = new TokenStreamComponents(tokenizer, tokenizer).TokenStream)
                {
                    tokenStream.Reset();
                    while (tokenStream.IncrementToken())
                    {
                        result.Append(tokenStream.GetAttribute<IReadingAttribute>().GetPronunciation());
                    }
                }
            }

            return result.ToString();
        }

        public static string UnitToRomaji(string str, bool isSpace)
        {
            var parameter = new MeCabParam
            {
                LatticeLevel = MeCabLatticeLevel.Two
            };
            var tagger = MeCabTagger.Create(parameter);
            var list = tagger.ParseToNodes(str);

            var result = "";

            foreach (var item in list)
            {
                var space = (isSpace &&(item.Next?.Feature?.Split(',')[0]?? "記号") != "記号") ? " " : "";
                if (item.CharType > 0)
                {
                    var features = item.Feature.Split(',');
                    if (features[0] == "記号")
                    {
                        result += item.Surface;
                    }
                    else if (features[6] == "*")
                    {
                        result += item.Surface;
                    }
                    else
                    {
                        result += WanaKana.ToRomaji(features[8]) + space;
                    }
                }
                else if (item.Stat!=MeCabNodeStat.Bos)
                {
                    result += item.Surface + space;
                }
            }

            if (result.LastIndexOf(' ') == -1)
            {
                return result;
            }

            if (result.LastIndexOf(' ') == result.Length - 1)
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }
    }

    public class ReturnText
    {
        public int Index { get; set; }

        public string Chinese { get; set; }

        public string Japanese { get; set; }

        public string Romaji { get; set; }

        public ReturnText()
        {
            Index = 0;
            Chinese = "";
            Japanese = "";
            Romaji = "";
        }
    }
}