﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RomajiConverter.Extensions
{
    public static class StringExtension
    {
        public static string RemoveEmptyLine(this string str)
        {
            var strArray = str.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            strArray = strArray.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            return string.Join(Environment.NewLine, strArray);
        }

        public static string[] LineToUnits(this string str)
        {
            return str.Split(new char[] { ' ', '　' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
