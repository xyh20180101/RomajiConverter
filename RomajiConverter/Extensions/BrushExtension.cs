using System;
using System.Collections.Generic;
using System.Text;

namespace RomajiConverter.Extensions
{
    public static class BrushExtension
    {
        public static System.Windows.Media.SolidColorBrush ToMediaSolidBrush(System.Drawing.SolidBrush drawingBrush)
        {
            return new System.Windows.Media.SolidColorBrush(drawingBrush.Color.ToMediaColor());
        }

        public static System.Drawing.SolidBrush ToDrawingSolidBrush(System.Windows.Media.SolidColorBrush mediaBrush)
        {
            return new System.Drawing.SolidBrush(mediaBrush.Color.ToDrawingColor());
        }
    }
}
