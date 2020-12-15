using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WanaKanaSharp;

namespace RomajiConverter.Helper
{
    public static class RomajiHelper
    {
        public static List<ReturnText> ToRomaji(string text, bool isSpace = true, float chineseRate = 0.51f)
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
                    returnText.Romaji = SingleLineToRomaji(line, isSpace);
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
        ///		单行文本转换为罗马音
        /// </summary>
        /// <param name="str">文本</param>
        /// <param name="isSpace">是否以空格分隔</param>
        /// <returns></returns>
        public static string SingleLineToRomaji(string str, bool isSpace)
        {
            var result = "";

            result += UnitToRomaji(str, isSpace);

            return result;
        }

        /// <summary>
        ///		文本转换为罗马音
        /// </summary>
        /// <param name="str">一个单元文本(空格之间)</param>
        /// <param name="isSpace">是否以空格分隔</param>
        /// <returns></returns>
        public static string UnitToRomaji(string str, bool isSpace)
        {
            var list = KuromojiHelper._tokenizer.Tokenize(str);

            var a = KuromojiHelper._tokenizer.MultiTokenizeNBest(str, 1);

            var result = "";

            foreach (var item in list)
            {
                var space = isSpace ? " " : "";
                if (item.LanguageType == "記号" || item.LanguageType == "*" || item.PartOfSpeechLevel1 == "空白")
                {
                    result += item.Surface;
                }
                else if (item.LanguageType == "外" && !WanaKana.IsJapanese(item.Surface))
                {
                    result += item.Surface;
                }
                else if (item.Surface == "私")
                {
                    result += "watashi" + space;
                }
                else
                {
                    result += WanaKana.ToRomaji(item.Pronunciation) + space;
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