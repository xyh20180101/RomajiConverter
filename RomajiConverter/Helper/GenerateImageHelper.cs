using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using RomajiConverter.Extensions;
using RomajiConverter.Models;

namespace RomajiConverter.Helper
{
    public static class GenerateImageHelper
    {
        public static Bitmap ToImage(this List<ConvertedLine> list, ImageSetting setting)
        {
            var fontSize = setting.FontPixelSize;
            var margin = setting.Margin;
            var paddingX = setting.PaddingX;
            var paddingY = setting.PaddingY;
            var paddingInnerY = setting.PaddingInnerY;

            var font = new Font(setting.FontFamilyName, fontSize, GraphicsUnit.Pixel);
            var brush = new SolidBrush(setting.FontColor);
            var background = setting.BackgroundColor;

            var maxLength = list.Any() ? list.Select(p => p.Units.Sum(q => q.GetUnitLength(font))).Max() : 0;
            var mL = list.Any() ? list.Select(p => p.Units.Length).Max() : 0;
            var width = maxLength + mL * paddingX + margin * 2;
            var height = list.Count * (2 * fontSize + paddingInnerY) + list.Count * paddingY + margin * 2;
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
                var frontUnitLength = margin + paddingX;
                foreach (var unit in line.Units)
                {
                    var x1 = frontUnitLength;
                    var x2 = frontUnitLength;

                    var s = unit.GetUnitShortOffset(font);
                    switch (s.Index)
                    {
                        case 0: x1 += s.Length; break;
                        case 1: x2 += s.Length; break;
                    }

                    var y1 = margin + (fontSize * 2 + paddingInnerY + paddingY) * i;
                    var y2 = margin + (fontSize * 2 + paddingInnerY + paddingY) * i + fontSize + paddingInnerY;

                    {
                        g1.DrawString(unit.Romaji, font, brush, new PointF(x1, y1));
                        g1.DrawString(unit.Japanese, font, brush, new PointF(x2, y2));
                    }

                    frontUnitLength += unit.GetUnitLength(font) + paddingX;
                }
            }

            return image;
        }

        private static int GetUnitLength(this ConvertedUnit unit, Font font)
        {
            using var g = Graphics.FromImage(new Bitmap(1, 1));
            var r = (int)g.MeasureString(unit.Romaji, font).Width;
            var j = (int)g.MeasureString(unit.Japanese, font).Width;
            return r > j ? r : j;
        }

        private static (int Index, int Length) GetUnitShortOffset(this ConvertedUnit unit, Font font)
        {
            using var g = Graphics.FromImage(new Bitmap(1, 1));
            var r = (int)g.MeasureString(unit.Romaji, font).Width;
            var j = (int)g.MeasureString(unit.Japanese, font).Width;
            var o = Math.Abs((r - j)) / 2;
            return r < j ? (0, o) : (1, o);
        }

        public class ImageSetting
        {
            public static ImageSetting Default => new ImageSetting
            {
                FontFamilyName = "微软雅黑",
                FontPixelSize = 48,
                Margin = 24,
                PaddingX = 0,
                PaddingY = 48,
                PaddingInnerY = 12,
                BackgroundColor = Color.White,
                FontColor = Color.Black
            };

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
