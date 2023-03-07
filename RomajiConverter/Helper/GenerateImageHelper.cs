﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Media;
using RomajiConverter.Extensions;
using RomajiConverter.Models;
using Color = System.Drawing.Color;

namespace RomajiConverter.Helper
{
    public static class GenerateImageHelper
    {
        public static Bitmap ToImage(this List<string[][]> list, ImageSetting setting)
        {
            if (list.Any() == false || list[0].Any() == false || list[0][0].Any() == false)
                return new Bitmap(1, 1);

            var fontSize = setting.FontPixelSize;
            var margin = setting.Margin;
            var paddingX = setting.PaddingX;
            var paddingY = setting.PaddingY;
            var paddingInnerY = setting.PaddingInnerY;

            var font = new Font(setting.FontFamilyName, fontSize, GraphicsUnit.Pixel);
            var brush = new SolidBrush(setting.FontColor);
            var background = setting.BackgroundColor;

            //(最长句的渲染长度,该句的单元数)
            var longestLine = list.Select(p => new { MaxLength = p.Sum(q => GetUnitLength(q, font)), UnitCount = p.Length })
                .OrderByDescending(p => p.MaxLength).FirstOrDefault();

            //最长句子的渲染长度
            var maxLength = longestLine?.MaxLength ?? 0;
            //最大单元数
            var maxUnitCount = longestLine?.UnitCount ?? 0;
            //图片宽度
            var width = maxLength + maxUnitCount * paddingX + margin * 2;
            //图片高度
            var height = list.Count * (list[0][0].Length * fontSize + paddingInnerY) + list.Count * paddingY + margin * 2;
            var image = new Bitmap(width, height);

            using var g1 = Graphics.FromImage(image);
            g1.Clear(background);
            g1.InterpolationMode = InterpolationMode.High;
            g1.SmoothingMode = SmoothingMode.HighQuality;
            g1.CompositingQuality = CompositingQuality.HighQuality;
            g1.TextRenderingHint = TextRenderingHint.AntiAlias;
            if (brush.Color.A == 0)
                g1.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            for (var i = 0; i < list.Count; i++)
            {
                var line = list[i];
                var startX = margin + paddingX;
                foreach (var unit in line)
                {
                    var unitLength = GetUnitLength(unit, font);
                    var renderXArray = unit.Select(str => startX + GetStringXOffset(str, font, unitLength)).ToArray();
                    var renderYArray = unit.Select((str, index) =>
                    margin + (fontSize * unit.Length + paddingInnerY + paddingY) * i + index * (fontSize + paddingInnerY)).ToArray();
                    for (var j = 0; j < unit.Length; j++)
                    {
                        g1.DrawString(unit[j], font, brush, new PointF(renderXArray[j], renderYArray[j]));
                    }
                    startX += unitLength + paddingX;
                }
            }

            return image;
        }

        /// <summary>
        /// 获取单元长度(最长字符串渲染长度)
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private static int GetUnitLength(string[] unit, Font font)
        {
            using var g = Graphics.FromImage(new Bitmap(1, 1));
            return unit.Any() ? unit.Max(p => (int)g.MeasureString(p, font).Width) : 0;
        }

        /// <summary>
        /// 获取字符串的渲染坐标x轴偏移值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <param name="unitLength"></param>
        /// <returns></returns>
        private static int GetStringXOffset(string str, Font font, int unitLength)
        {
            using var g = Graphics.FromImage(new Bitmap(1, 1));
            return (unitLength - (int)g.MeasureString(str, font).Width) / 2;
        }

        public class ImageSetting
        {
            public ImageSetting()
            {

            }

            public ImageSetting(App.MyConfig config)
            {
                FontFamilyName = config.FontFamilyName;
                FontPixelSize = config.FontPixelSize;
                Margin = config.Margin;
                PaddingX = config.PaddingX;
                PaddingY = config.PaddingY;
                PaddingInnerY = config.PaddingInnerY;
                BackgroundColor = ((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.BackgroundColor)).ToDrawingColor();
                FontColor = ((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.FontColor)).ToDrawingColor();
            }

            public string FontFamilyName { get; set; }

            public int FontPixelSize { get; set; }

            public int Margin { get; set; }

            public int PaddingX { get; set; }

            public int PaddingY { get; set; }

            public int PaddingInnerY { get; set; }

            public Color BackgroundColor { get; set; }

            public Color FontColor { get; set; }
        }
    }
}
