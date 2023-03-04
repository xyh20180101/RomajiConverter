using AduSkin.Controls.Metro;
using System.Collections.Generic;
using System.Windows;
using System.Drawing.Text;
using System.Windows.Media;
using RomajiConverter.Extensions;
using Application = System.Windows.Application;
using RomajiConverter.Models;
using Microsoft.Win32;
using RomajiConverter.Helper;
using System.Drawing.Imaging;

namespace RomajiConverter
{
    /// <summary>
    /// PictureSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PictureSettingWindow : MetroWindow
    {
        private readonly List<ConvertedLine> _convertedLineList;

        public PictureSettingWindow(List<ConvertedLine> convertedLineList)
        {
            InitializeComponent();
            InitFontFamily();
            _convertedLineList = convertedLineList;
        }

        private void InitFontFamily()
        {
            foreach (var font in new InstalledFontCollection().Families)
            {
                FontFamilyComboBox.Items.Add(font.Name);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitConfig(((App)Application.Current).Config);
        }

        private void InitConfig(App.MyConfig config)
        {
            FontFamilyComboBox.SelectedValue = config.FontFamilyName;
            FontPixelSizeTextBox.Text = config.FontPixelSize.ToString();
            FontColorPicker.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(config.FontColor));
            BackgroundColorPicker.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(config.BackgroundColor));
            MarginTextBox.Text = config.Margin.ToString();
            PaddingXTextBox.Text = config.PaddingX.ToString();
            PaddingYTextBox.Text = config.PaddingY.ToString();
            PaddingInnerYTextBox.Text = config.PaddingInnerY.ToString();
        }

        private void SaveConfig()
        {
            var config = ((App)Application.Current).Config;
            config.FontFamilyName = (string)FontFamilyComboBox.SelectedValue;
            config.FontPixelSize = int.Parse(FontPixelSizeTextBox.Text);
            config.FontColor = ((SolidColorBrush)FontColorPicker.Background).Color.ToDrawingColor().ToHexString();
            config.BackgroundColor = ((SolidColorBrush)BackgroundColorPicker.Background).Color.ToDrawingColor().ToHexString();
            config.Margin = int.Parse(MarginTextBox.Text);
            config.PaddingX = int.Parse(PaddingXTextBox.Text);
            config.PaddingY = int.Parse(PaddingYTextBox.Text);
            config.PaddingInnerY = int.Parse(PaddingInnerYTextBox.Text);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfig();
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            var sfd = new SaveFileDialog();
            sfd.Filter = "png|*.png";
            if (sfd.ShowDialog().Value)
            {
                using var image = _convertedLineList.ToImage(new GenerateImageHelper.ImageSetting(((App)Application.Current).Config));
                image.Save(sfd.FileName, ImageFormat.Png);
                this.Close();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            var result = AduMessageBox.ShowOKCancel("确认恢复默认设置？", "提示", "是", "否");
            if (result == MessageBoxResult.OK)
            {
                ((App)Application.Current).Config.InitImageSetting();
                InitConfig(((App)Application.Current).Config);
            }
        }
    }
}
