﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AduSkin.Controls.Metro;
using AduSkin.Themes;
using RomajiConverter.Helper;

namespace RomajiConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            BorderBrush = new SolidColorBrush(Color.FromRgb(255, 102, 102));
            InitializeComponent();
            KuromojiHelper.Init();
            CloudMusicHelper.Init();
            SpaceCheckBox.Checked += CheckBox_Checked;
            SpaceCheckBox.Unchecked += CheckBox_Unchecked;
            NewLineCheckBox.Checked += CheckBox_Checked;
            NewLineCheckBox.Unchecked += CheckBox_Unchecked;
            RomajiCheckBox.Checked += CheckBox_Checked;
            RomajiCheckBox.Unchecked += CheckBox_Unchecked;
            JPCheckBox.Checked += CheckBox_Checked;
            JPCheckBox.Unchecked += CheckBox_Unchecked;
            CHCheckBox.Checked += CheckBox_Checked;
            CHCheckBox.Unchecked += CheckBox_Unchecked;
        }

        private async void ImportCloudMusicButton_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("测试");
            var result = await CloudMusicHelper.GetLrc(CloudMusicHelper.GetLastSongId());
            var stringBuilder = new StringBuilder();
            foreach (var item in result)
            {
                stringBuilder.AppendLine(item.JLrc);
                stringBuilder.AppendLine(item.CLrc);
            }

            InputTextBox.Text = stringBuilder.ToString();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        private void Convert()
        {
            var result = RomajiHelper.ToRomaji(InputTextBox.Text, GetBool(SpaceCheckBox.IsChecked));
            var output = new StringBuilder();
            foreach (var item in result)
            {
                if (GetBool(RomajiCheckBox.IsChecked))
                    output.AppendLine(item.Romaji);
                if (GetBool(JPCheckBox.IsChecked))
                    output.AppendLine(item.Japanese);
                if (GetBool(CHCheckBox.IsChecked) && !string.IsNullOrWhiteSpace(item.Chinese))
                    output.AppendLine(item.Chinese);
                if (GetBool(NewLineCheckBox.IsChecked))
                    output.AppendLine();
            }
            OutputTextBox.Text = output.ToString();
        }

        private bool GetBool(bool? value)
        {
            return value.HasValue && value == true;
        }

        #region 复选框事件

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        #endregion
    }
}
