﻿using System;

namespace RomajiConverter.Core.Models
{
    public class ConvertedLine
    {
        public ConvertedLine()
        {
            Index = 0;
            Chinese = "";
            Japanese = "";
            Units = Array.Empty<ConvertedUnit>();
        }

        public ushort Index { get; set; }

        public string Chinese { get; set; }

        public string Japanese { get; set; }

        public ConvertedUnit[] Units { get; set; }
    }
}