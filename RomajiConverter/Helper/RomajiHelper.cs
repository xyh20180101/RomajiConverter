using MeCab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WanaKanaSharp;

namespace RomajiConverter.Helper
{
    public static class RomajiHelper
    {
        /// <summary>
        /// 分词器
        /// </summary>
        private static MeCabTagger _tagger;

        /// <summary>
        /// 自定义词典
        /// </summary>
        private static Dictionary<string, string> _customizeDict;

        public static void Init()
        {
            var parameter = new MeCabParam
            {
                DicDir = "unidic",//词典路径
                LatticeLevel = MeCabLatticeLevel.Zero,
            };
            _tagger = MeCabTagger.Create(parameter);

            var str = File.ReadAllText("customizeDict.txt");
            var list = str.Split(Environment.NewLine);
            _customizeDict = new Dictionary<string, string>();
            foreach (var item in list)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                var array = item.Split(" ");
                if (array.Length < 2) continue;
                _customizeDict.Add(array[0], array[1]);
            }
        }

        /// <summary>
        /// 生成转换结果列表
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isSpace"></param>
        /// <param name="chineseRate"></param>
        /// <returns></returns>
        public static List<ReturnText> ToRomaji(string text, bool isSpace = true, float chineseRate = 1f)
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
                returnText.Japanese = line.Replace("\0", "");//文本中如果包含\0，会导致复制只能粘贴到第一个\0处，需要替换为空，以下同理


                var units = line.LineToUnits();//将行文本拆分为单词
                var romajiUnits = new List<string>();
                foreach (var unit in units)
                {
                    romajiUnits.Add(UnitToRomaji(unit, isSpace));
                }

                returnText.Romaji = string.Join(" ", romajiUnits).Replace("\0", ""); ;


                if (index + 1 < lineTextList.Length &&
                    IsChinese(lineTextList[index + 1], chineseRate))
                {
                    returnText.Chinese = lineTextList[index + 1].Replace("\0", ""); ;
                }

                returnText.Index = returnList.Count;
                returnList.Add(returnText);
            }

            return returnList;
        }

        /// <summary>
        /// 判断字符串(句子)是否简体中文
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rate">容错率(0-1)</param>
        /// <returns></returns>
        public static bool IsChinese(string str, float rate)
        {
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

        /// <summary>
        /// 判断字符串是否全为英文数字空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEnglish(string str)
        {
            return new Regex("^[a-zA-Z0-9 ]+$").IsMatch(str);
        }

        /// <summary>
        /// 判断字符串是否全为假名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsJapanese(string str)
        {
            return Regex.IsMatch(str, @"^[\u3040-\u30ff]+$");
        }

        /// <summary>
        /// 单词转为罗马音
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isSpace"></param>
        /// <returns></returns>
        public static string UnitToRomaji(string str, bool isSpace)
        {
            var list = _tagger.ParseToNodes(str);

            var result = "";

            foreach (var item in list)
            {
                var nextFeatures = item.Next?.Feature?.Split(',') ?? new string[] { };
                var space = (!isSpace || nextFeatures.Length <= 6 || new string[] { "記号", "補助記号" }.Contains(nextFeatures[0] ?? "記号")) ? "" : " ";
                if (item.CharType > 0)
                {
                    var features = item.Feature.Split(',');
                    if (TryCustomConvert(item.Surface, out var customResult))
                    {
                        //用户自定义词典
                        result += customResult;
                    }
                    else if (IsJapanese(item.Surface))
                    {
                        //纯假名
                        result += WanaKana.ToRomaji(item.Surface) + space;
                    }
                    else if (features.Length <= 6 || new string[] { "補助記号" }.Contains(features[0]))
                    {
                        //标点符号
                        result += item.Surface;
                    }
                    else if (IsEnglish(item.Surface))
                    {
                        //英文
                        result += item.Surface;
                    }
                    else
                    {
                        //汉字
                        result += WanaKana.ToRomaji(features[ChooseIndexByType(features[0])]) + space;
                    }
                }
                else if (item.Stat != MeCabNodeStat.Bos)
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

        /// <summary>
        /// 根据不同的词型选择正确的索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int ChooseIndexByType(string type)
        {
            switch (type)
            {
                case "助詞": return 11;
                default: return 19;
            }
        }

        /// <summary>
        /// 自定义转换规则
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryCustomConvert(string str, out string result)
        {
            if (_customizeDict.ContainsKey(str))
            {
                result = _customizeDict[str];
                return true;
            }
            else
            {
                result = "";
                return false;
            }
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