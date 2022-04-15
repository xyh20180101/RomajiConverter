using MeCab;
using RomajiConverter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WanaKanaSharp;
using NTextCat;

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
                LatticeLevel = MeCabLatticeLevel.Zero
            };
            _tagger = MeCabTagger.Create(parameter);

            _customizeDict = new Dictionary<string, string>();
            foreach (var item in File.ReadAllText("customizeDict.txt").Split(Environment.NewLine))
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
        public static List<ConvertedLine> ToRomaji(string text, bool isSpace = true, float chineseRate = 1f)
        {
            var lineTextList = text.RemoveEmptyLine().Split(Environment.NewLine);

            var convertedText = new List<ConvertedLine>();


            for (var index = 0; index < lineTextList.Length; index++)
            {
                var line = lineTextList[index];

                var convertedLine = new ConvertedLine();

                if (IsChinese2(line, chineseRate))
                {
                    continue;
                }

                convertedLine.Japanese = line;

                var sentences = line.LineToSentences();//将行拆分为分句
                var multiUnits = new List<ConvertedUnit[]>();
                foreach (var sentence in sentences)
                {
                    if (IsEnglish(sentence))
                    {
                        multiUnits.Add(new[] { new ConvertedUnit(sentence, sentence) });
                        continue;
                    }
                    var units = SentenceToRomaji(sentence);
                    multiUnits.Add(units);
                }

                convertedLine.Units = multiUnits.SelectMany(p => p).ToArray();

                if (index + 1 < lineTextList.Length &&
                    IsChinese(lineTextList[index + 1], chineseRate))
                {
                    convertedLine.Chinese = lineTextList[index + 1];
                }

                convertedLine.Index = convertedText.Count;
                convertedText.Add(convertedLine);
            }

            return convertedText;
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
                var gbBytes = Encoding.Unicode.GetBytes(word.ToString());

                if (gbBytes.Length == 2)  // double bytes char.  
                {
                    if (gbBytes[1] >= 0x4E && gbBytes[1] <= 0x9F)//中文
                    {
                        chCount++;
                    }
                    else
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
                    else
                    {
                        total--;
                    }
                }
            }

            if (chCount == 0) return false;//一个简体中文都没有

            return (chCount + enCount) / total >= rate;
        }

        public static bool IsChinese2(string str, float rate = 0)
        {
            var factory = new RankedLanguageIdentifierFactory();
            var identifier = factory.Load("Core14.profile.xml");
            var languages = identifier.Identify(str).ToArray();
            var mostCertainLanguage = languages.FirstOrDefault();
            if (mostCertainLanguage != null)
                return mostCertainLanguage.Item1.Iso639_3 == "zho";
            else
                return false;
        }

        /// <summary>
        /// 判断字符串是否全为单字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEnglish(string str)
        {
            return new Regex("^[\x20-\x7E]+$").IsMatch(str);
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
        /// 分句转为罗马音
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ConvertedUnit[] SentenceToRomaji(string str)
        {
            var list = _tagger.ParseToNodes(str).ToArray();

            var result = new List<ConvertedUnit>();

            foreach (var item in list)
            {
                if (item.CharType > 0)
                {
                    string[] features;
                    features = item.Feature.Split(',');
                    if (TryCustomConvert(item.Surface, out var customResult))
                    {
                        //用户自定义词典
                        result.Add(new ConvertedUnit(item.Surface, customResult));
                    }
                    else if (features.Length > 0 && features[0] != "助詞" && IsJapanese(item.Surface))
                    {
                        //纯假名
                        result.Add(new ConvertedUnit(item.Surface, WanaKana.ToRomaji(item.Surface)));
                    }
                    else if (features.Length <= 6 || new string[] { "補助記号" }.Contains(features[0]))
                    {
                        //标点符号
                        result.Add(new ConvertedUnit(item.Surface, item.Surface));
                    }
                    else if (IsEnglish(item.Surface))
                    {
                        //英文
                        result.Add(new ConvertedUnit(item.Surface, item.Surface));
                    }
                    else
                    {
                        //汉字
                        result.Add(new ConvertedUnit(item.Surface, WanaKana.ToRomaji(features[ChooseIndexByType(features[0])])));
                    }
                }
                else if (item.Stat != MeCabNodeStat.Bos && item.Stat != MeCabNodeStat.Eos)
                {
                    result.Add(new ConvertedUnit(item.Surface, item.Surface));
                }
            }

            return result.ToArray();
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
}