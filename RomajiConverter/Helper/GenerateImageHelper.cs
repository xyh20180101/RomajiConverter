using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows;
using RomajiConverter.Models;

namespace RomajiConverter.Helper
{
    public static class GenerateImageHelper
    {
        public static Bitmap ToImage(this List<ConvertedLine> list)
        {
            var scaling = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice.M11;
            var size = 24;
            var font = new Font("微软雅黑", size, GraphicsUnit.Pixel);
            var brush = new SolidBrush(Color.Black);

            var maxLength = list.SelectMany(p => p.Units).Select(p => p.Romaji.Length).Max();
            var width = maxLength * size;
            var height = list.Count * size;
            var image = new Bitmap(width, height);
            using var bg = Graphics.FromImage(image);
            bg.Clear(Color.White);
            for (var i = 0; i < list.Count; i++)
            {
                var line = list[i];
                using var g1 = Graphics.FromImage(image);
                g1.InterpolationMode = InterpolationMode.High;
                g1.SmoothingMode = SmoothingMode.HighQuality;
                g1.CompositingQuality = CompositingQuality.HighQuality;
                g1.TextRenderingHint = TextRenderingHint.AntiAlias;
                g1.DrawString(line.Japanese, font, brush, new PointF(0, i * size));
            }

            return image;
        }
    }
}
