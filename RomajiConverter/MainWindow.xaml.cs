using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AduSkin.Controls.Metro;
using Microsoft.Win32;
using Newtonsoft.Json;
using RomajiConverter.Controls;
using RomajiConverter.Helper;
using RomajiConverter.Models;
using Clipboard = System.Windows.Clipboard;

namespace RomajiConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// 当前的转换结果集合
        /// </summary>
        private List<ConvertedLine> _convertedLineList = new List<ConvertedLine>();

        /// <summary>
        /// 简易模式最窄宽度
        /// </summary>
        private const int _simpleModeMinWidth = 900;

        /// <summary>
        /// 详细模式最窄宽度
        /// </summary>
        private const int _detailModeMinWidth = 1200;

        public MainWindow()
        {
            InitializeComponent();

            IsDetailMode = ((App)Application.Current).Config.IsDetailMode;
            SimpleRadioButton.IsChecked = !IsDetailMode;
            DetailRadioButton.IsChecked = IsDetailMode;

            EditHiraganaCheckBox.Checked += EditHiraganaCheckBox_Checked;
            EditHiraganaCheckBox.Unchecked += EditHiraganaCheckBox_Unchecked;
            EditRomajiCheckBox.Checked += EditRomajiCheckBox_Checked;
            EditRomajiCheckBox.Unchecked += EditRomajiCheckBox_UnChecked;

            SpaceCheckBox.Checked += ThirdCheckBox_Checked;
            SpaceCheckBox.Unchecked += ThirdCheckBox_Unchecked;
            NewLineCheckBox.Checked += ThirdCheckBox_Checked;
            NewLineCheckBox.Unchecked += ThirdCheckBox_Unchecked;
            RomajiCheckBox.Checked += ThirdCheckBox_Checked;
            RomajiCheckBox.Unchecked += ThirdCheckBox_Unchecked;
            HiraganaCheckBox.Checked += ThirdCheckBox_Checked;
            HiraganaCheckBox.Unchecked += ThirdCheckBox_Unchecked;
            JPCheckBox.Checked += ThirdCheckBox_Checked;
            JPCheckBox.Unchecked += ThirdCheckBox_Unchecked;
            CHCheckBox.Checked += ThirdCheckBox_Checked;
            CHCheckBox.Unchecked += ThirdCheckBox_Unchecked;
            SimpleRadioButton.Checked += SimpleRadioButton_Checked;
            DetailRadioButton.Checked += DetailRadioButton_Checked;

            this.Title = $"RomajiConverter ({System.Reflection.Assembly.GetExecutingAssembly().GetName().Version})";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CloudMusicHelper.Init();
            RomajiHelper.Init();
            VariantHelper.Init();
        }

        private void MetroWindow_Closing(object sender, EventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }
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

        private void ImportQQMusicButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLrc(QQMusicHelper.GetLrc(QQMusicHelper.GetCurrentSongmid()));
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
            _convertedLineList = RomajiHelper.ToRomaji(InputTextBox.Text, GetBool(SpaceCheckBox.IsChecked), GetBool(AutoVariantCheckBox.IsChecked));

            if (IsDetailMode)
                RenderEditPanel();
            else
                OutputTextBox.Text = GetResultText();
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
                    group.RomajiVisibility = GetBool(EditRomajiCheckBox.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
                    group.HiraganaVisibility = GetBool(EditHiraganaCheckBox.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
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

        #region 第二网格复选框事件

        private void EditRomajiCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (object children in EditPanel.Children)
            {
                WrapPanel wrapPanel;
                if (children.GetType() == typeof(WrapPanel))
                    wrapPanel = (WrapPanel)children;
                else
                    continue;

                foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                {
                    editableLabelGroup.RomajiVisibility = Visibility.Visible;
                }
            }
        }

        private void EditRomajiCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            foreach (object children in EditPanel.Children)
            {
                WrapPanel wrapPanel;
                if (children.GetType() == typeof(WrapPanel))
                    wrapPanel = (WrapPanel)children;
                else
                    continue;

                foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                {
                    editableLabelGroup.RomajiVisibility = Visibility.Collapsed;
                }
            }
        }

        private void EditHiraganaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (object children in EditPanel.Children)
            {
                WrapPanel wrapPanel;
                if (children.GetType() == typeof(WrapPanel))
                    wrapPanel = (WrapPanel)children;
                else
                    continue;

                foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                {
                    editableLabelGroup.HiraganaVisibility = Visibility.Visible;
                }
            }
        }

        private void EditHiraganaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (object children in EditPanel.Children)
            {
                WrapPanel wrapPanel;
                if (children.GetType() == typeof(WrapPanel))
                    wrapPanel = (WrapPanel)children;
                else
                    continue;

                foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                {
                    editableLabelGroup.HiraganaVisibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region 第三网格复选框事件

        private void ThirdCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }

        private void ThirdCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }

        #endregion

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
                if (GetBool(HiraganaCheckBox.IsChecked))
                    output.AppendLine(GetString(item.Units.Select(p => p.Hiragana)));
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
                File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(_convertedLineList, Formatting.Indented));
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
            new PictureSettingWindow(_convertedLineList).ShowDialog();
        }

        private void ConvertTextButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputTextBox.Text);
        }

        #region 菜单顶栏

        private bool _isDetailMode;
        public bool IsDetailMode
        {
            get => _isDetailMode;
            set
            {
                _isDetailMode = value;
                ((App)Application.Current).Config.IsDetailMode = _isDetailMode;
                SwitchMode(_isDetailMode);
            }
        }

        /// <summary>
        /// 切换简易/详细界面
        /// </summary>
        /// <param name="isDetailMode"></param>
        private void SwitchMode(bool isDetailMode)
        {
            if (this.IsInitialized)
            {
                if (isDetailMode)
                {
                    this.Width = _detailModeMinWidth;
                    this.MinWidth = _detailModeMinWidth;
                    if (MainGrid.ColumnDefinitions.Count == 2) MainGrid.ColumnDefinitions.Insert(1, new ColumnDefinition());
                    ReadButton.Visibility = Visibility.Visible;
                    SaveButton.Visibility = Visibility.Visible;
                    EditHiraganaCheckBox.Visibility = Visibility.Visible;
                    EditRomajiCheckBox.Visibility = Visibility.Visible;
                    ConvertPictureButton.Visibility = Visibility.Visible;
                    ConvertTextButton.Visibility = Visibility.Visible;
                    EditBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    ReadButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                    EditHiraganaCheckBox.Visibility = Visibility.Collapsed;
                    EditRomajiCheckBox.Visibility = Visibility.Collapsed;
                    ConvertPictureButton.Visibility = Visibility.Collapsed;
                    ConvertTextButton.Visibility = Visibility.Collapsed;
                    EditBorder.Visibility = Visibility.Collapsed;
                    if (MainGrid.ColumnDefinitions.Count == 3) MainGrid.ColumnDefinitions.RemoveAt(1);
                    this.MinWidth = _simpleModeMinWidth;
                    this.Width = _simpleModeMinWidth;
                }
            }
        }

        private void SimpleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsDetailMode = false;
        }

        private void DetailRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsDetailMode = true;
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new HelpWindow().Show();
        }

        private void InfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new InfoWindow().Show();
        }

        #endregion

        private void InputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0 && InputTextBox.FontSize > 8)
                {
                    InputTextBox.FontSize /= 1.1;
                }
                else if (e.Delta > 0 && InputTextBox.FontSize < 40)
                {
                    InputTextBox.FontSize *= 1.1;
                }
                e.Handled = true;
            }
        }

        private double _editableLabelGroupFontSize = 12;

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0 && _editableLabelGroupFontSize > 8)
                {
                    _editableLabelGroupFontSize /= 1.1;
                    foreach (object children in EditPanel.Children)
                    {
                        WrapPanel wrapPanel;
                        if (children.GetType() == typeof(WrapPanel))
                            wrapPanel = (WrapPanel)children;
                        else
                            continue;

                        foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                        {
                            editableLabelGroup.MyFontSize = _editableLabelGroupFontSize;
                        }
                    }
                }
                else if (e.Delta > 0 && _editableLabelGroupFontSize < 40)
                {
                    _editableLabelGroupFontSize *= 1.1;
                    foreach (object children in EditPanel.Children)
                    {
                        WrapPanel wrapPanel;
                        if (children.GetType() == typeof(WrapPanel))
                            wrapPanel = (WrapPanel)children;
                        else
                            continue;

                        foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                        {
                            editableLabelGroup.MyFontSize = _editableLabelGroupFontSize;
                        }
                    }
                }
                e.Handled = true;
            }
        }

        private void OutputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0 && OutputTextBox.FontSize > 8)
                {
                    OutputTextBox.FontSize /= 1.1;
                }
                else if (e.Delta > 0 && OutputTextBox.FontSize < 40)
                {
                    OutputTextBox.FontSize *= 1.1;
                }
                e.Handled = true;
            }
        }
    }
}
