using AduSkin.Controls.Metro;
using RomajiConverter.Extensions;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace RomajiConverter
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : MetroWindow
    {
        public SettingWindow()
        {
            InitializeComponent();

            InitFontFamily();
            InitHelpLabel();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitAllConfig(((App)Application.Current).Config);
        }

        private void InitAllConfig(App.MyConfig config)
        {
            IsOpenExplorerAfterSaveImageCheckBox.IsChecked = config.IsOpenExplorerAfterSaveImage;
            LeftParenthesisTextBox.Text = config.LeftParenthesis;
            RightParenthesisTextBox.Text = config.RightParenthesis;

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

            config.IsOpenExplorerAfterSaveImage = IsOpenExplorerAfterSaveImageCheckBox.IsChecked == true;
            config.LeftParenthesis = LeftParenthesisTextBox.Text;
            config.RightParenthesis = RightParenthesisTextBox.Text;

            config.FontFamilyName = (string)FontFamilyComboBox.SelectedValue;
            config.FontPixelSize = int.Parse(FontPixelSizeTextBox.Text);
            config.FontColor = ((SolidColorBrush)FontColorPicker.Background).Color.ToDrawingColor().ToHexString();
            config.BackgroundColor = ((SolidColorBrush)BackgroundColorPicker.Background).Color.ToDrawingColor().ToHexString();
            config.Margin = int.Parse(MarginTextBox.Text);
            config.PaddingX = int.Parse(PaddingXTextBox.Text);
            config.PaddingY = int.Parse(PaddingYTextBox.Text);
            config.PaddingInnerY = int.Parse(PaddingInnerYTextBox.Text);
        }

        #region 通用设置



        #endregion

        #region 图片导出设置

        private void InitFontFamily()
        {
            foreach (var font in new InstalledFontCollection().Families)
            {
                FontFamilyComboBox.Items.Add(font.Name);
            }
        }

        private void InitImageConfig(App.MyConfig config)
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

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            var result = AduMessageBox.ShowOKCancel("确认恢复默认设置？(需要手动保存)", "提示", "是", "否");
            if (result == MessageBoxResult.OK)
            {
                InitImageConfig(new App.MyConfig());
            }
        }

        #endregion

        #region 帮助

        private void InitHelpLabel()
        {
            HelpLabel.Text = File.ReadAllText("Help.md");
        }

        #endregion

        #region 关于

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri) { UseShellExecute = true });
        }

        #endregion

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            Close();
        }
    }
}
