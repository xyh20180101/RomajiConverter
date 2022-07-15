using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AduSkin.Controls.Metro;
using AduSkin.Themes;
using Microsoft.Win32;
using Newtonsoft.Json;
using RomajiConverter.Controls;
using RomajiConverter.Helper;
using RomajiConverter.Models;
using Clipboard = System.Windows.Clipboard;
using Color = System.Windows.Media.Color;

namespace RomajiConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<ConvertedLine> _convertedLineList = new List<ConvertedLine>();

        public MainWindow()
        {
            InitializeComponent();
            CloudMusicHelper.Init();
            RomajiHelper.Init();
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
            this.Title = $"RomajiConverter ({System.Reflection.Assembly.GetExecutingAssembly().GetName().Version})";
        }

        private async void ImportCloudMusicButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLrc(await CloudMusicHelper.GetLrc(CloudMusicHelper.GetLastSongId()));
        }

        private void ShowLrc(List<ReturnLrc> lrc)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in lrc)
            {
                stringBuilder.AppendLine(item.JLrc);
                stringBuilder.AppendLine(item.CLrc);
            }

            InputTextBox.Text = stringBuilder.ToString();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            _convertedLineList.Clear();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        private void Convert()
        {
            _convertedLineList = RomajiHelper.ToRomaji(InputTextBox.Text);
            RenderEditPanel();
        }

        private void RenderEditPanel()
        {
            EditPanel.Children.Clear();
            for (var i = 0; i < _convertedLineList.Count; i++)
            {
                var item = _convertedLineList[i];

                var line = new WrapPanel { };
                foreach (var unit in item.Units)
                {
                    var group = new EditableLabelGroup(unit);
                    line.Children.Add(group);
                }

                EditPanel.Children.Add(line);
                if (item.Units.Any() && i < _convertedLineList.Count - 1)
                    EditPanel.Children.Add(new Separator());
            }
        }

        private bool GetBool(bool? value)
        {
            return value.HasValue && value == true;
        }

        #region 复选框事件

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }

        #endregion

        private void MetroWindow_Closing(object sender, EventArgs e)
        {
            //Environment.Exit(0);
        }

        private void MetroTitleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new InfoWindow().ShowDialog();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputTextBox.Text);
        }

        private void ConvertTextButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }

        private string GetResultText()
        {
            string GetString(IEnumerable<string> array)
            {
                return string.Join(GetBool(SpaceCheckBox.IsChecked) ? " " : "", array);
            }

            var output = new StringBuilder();
            for (var i = 0; i < _convertedLineList.Count; i++)
            {
                var item = _convertedLineList[i];
                if (GetBool(RomajiCheckBox.IsChecked))
                    output.AppendLine(GetString(item.Units.Select(p => p.Romaji)));
                if (GetBool(JPCheckBox.IsChecked))
                    output.AppendLine(item.Japanese);
                if (GetBool(CHCheckBox.IsChecked) && !string.IsNullOrWhiteSpace(item.Chinese))
                    output.AppendLine(item.Chinese);
                if (GetBool(NewLineCheckBox.IsChecked) && i < _convertedLineList.Count - 1)
                    output.AppendLine();
            }
            if (_convertedLineList.Any()) output.Remove(output.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return output.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "json|*.json";
            if (sfd.ShowDialog().Value)
                File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(_convertedLineList));
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "json|*.json";
            ofd.Multiselect = false;
            if (ofd.ShowDialog().Value)
            {
                _convertedLineList = JsonConvert.DeserializeObject<List<ConvertedLine>>(File.ReadAllText(ofd.FileName));
                RenderEditPanel();
            }
        }

        private void ConvertPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "png|*.png";
            if (sfd.ShowDialog().Value)
            {
                using var image = _convertedLineList.ToImage(GenerateImageHelper.ImageSetting.Default);
                image.Save(sfd.FileName, ImageFormat.Png);
            }
        }
    }
}
