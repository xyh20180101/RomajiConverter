using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RomajiConverter.Models
{
    public class ConvertedUnit : INotifyPropertyChanged
    {
        public ConvertedUnit(string japanese, string hiragana, string romaji)
        {
            Japanese = japanese;
            Romaji = romaji;
            Hiragana = hiragana;
        }

        private string _japanese;
        public string Japanese
        {
            get => _japanese;
            set
            {
                _japanese = value;
                OnPropertyChanged("Japanese");
            }
        }

        private string _romaji;

        public string Romaji
        {
            get => _romaji;
            set
            {
                _romaji = value;
                OnPropertyChanged("Romaji");
            }
        }

        private string _hiragana;
        public string Hiragana
        {
            get => _hiragana;
            set
            {
                _hiragana = value;
                OnPropertyChanged("Hiragana");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
