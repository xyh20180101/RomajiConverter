using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kuromoji.NET.Tokenizers.UniDic;

namespace RomajiConverter.Helper
{
	public static class KuromojiHelper
	{
		public static Tokenizer _tokenizer;

		public static void Init()
        {
            var dictionaryPath = Environment.CurrentDirectory+"\\unidic.zip";
			_tokenizer = new Tokenizer(dictionaryPath);
		}

		/// <summary>
		/// 转换为片假名
		/// </summary>
		/// <returns></returns>
		public static string ToKatakana(string input)
		{
			return _tokenizer.Tokenize(input).Aggregate("", (current, token) => current + token.LemmaReadingForm);
		}
	}
}
