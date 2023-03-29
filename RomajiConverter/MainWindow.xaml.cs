using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
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
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            IsDetailMode = ((App)Application.Current).Config.IsDetailMode;
            SimpleRadioButton.IsChecked = !IsDetailMode;
            DetailRadioButton.IsChecked = IsDetailMode;

            InputTextBox.FontSize = ((App)Application.Current).Config.InputTextBoxFontSize;
            OutputTextBox.FontSize = ((App)Application.Current).Config.OutputTextBoxFontSize;

            EditHiraganaCheckBox.Checked += EditHiraganaCheckBox_Checked;
            EditHiraganaCheckBox.Unchecked += EditHiraganaCheckBox_Unchecked;
            EditRomajiCheckBox.Checked += EditRomajiCheckBox_Checked;
            EditRomajiCheckBox.Unchecked += EditRomajiCheckBox_UnChecked;
            IsOnlyShowKanjiCheckBox.Checked += IsOnlyShowKanjiCheckBox_Checked;
            IsOnlyShowKanjiCheckBox.Unchecked += IsOnlyShowKanjiCheckBox_Unchecked;

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
            KanjiHiraganaCheckBox.Checked += ThirdCheckBox_Checked;
            KanjiHiraganaCheckBox.Unchecked += ThirdCheckBox_Unchecked;
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
                    if (GetBool(EditHiraganaCheckBox.IsChecked) == true)
                    {
                        if (GetBool(IsOnlyShowKanjiCheckBox.IsChecked) == true && group.Unit.IsKanji == false)
                        {
                            group.HiraganaVisibility = Visibility.Hidden;
                        }
                        else
                        {
                            group.HiraganaVisibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        group.HiraganaVisibility = Visibility.Collapsed;
                    }
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
                    if (GetBool(IsOnlyShowKanjiCheckBox.IsChecked) && !editableLabelGroup.Unit.IsKanji)
                        editableLabelGroup.HiraganaVisibility = Visibility.Hidden;
                    else
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

        private void IsOnlyShowKanjiCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (EditHiraganaCheckBox.IsChecked == false)
                return;

            foreach (object children in EditPanel.Children)
            {
                WrapPanel wrapPanel;
                if (children.GetType() == typeof(WrapPanel))
                    wrapPanel = (WrapPanel)children;
                else
                    continue;

                foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                {
                    if (editableLabelGroup.Unit.IsKanji == false)
                        editableLabelGroup.HiraganaVisibility = Visibility.Hidden;
                }
            }
        }

        private void IsOnlyShowKanjiCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (EditHiraganaCheckBox.IsChecked == false)
                return;

            foreach (object children in EditPanel.Children)
            {
                WrapPanel wrapPanel;
                if (children.GetType() == typeof(WrapPanel))
                    wrapPanel = (WrapPanel)children;
                else
                    continue;

                foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                {
                    if (editableLabelGroup.Unit.IsKanji == false)
                        editableLabelGroup.HiraganaVisibility = Visibility.Visible;
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
                {
                    if (GetBool(KanjiHiraganaCheckBox.IsChecked))
                    {
                        var japanese = item.Japanese;
                        var leftParenthesis = ((App)Application.Current).Config.LeftParenthesis;
                        var rightParenthesis = ((App)Application.Current).Config.RightParenthesis;

                        var kanjiUnitList = item.Units.Where(p => p.IsKanji);
                        foreach (var kanjiUnit in kanjiUnitList)
                        {
                            var kanjiIndex = japanese.IndexOf(kanjiUnit.Japanese);
                            var hiraganaIndex = kanjiIndex + kanjiUnit.Japanese.Length;
                            japanese = japanese.Insert(hiraganaIndex, $"{leftParenthesis}{kanjiUnit.Hiragana}{rightParenthesis}");
                        }
                        output.AppendLine(japanese);
                    }
                    else
                    {
                        output.AppendLine(item.Japanese);
                    }
                }
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
                try
                {
                    _convertedLineList = JsonConvert.DeserializeObject<List<ConvertedLine>>(File.ReadAllText(ofd.FileName));
                    RenderEditPanel();
                }
                catch (JsonSerializationException exception)
                {
                    throw new Exception("不是合法的json文件", exception);
                }
            }
        }

        private void ConvertPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "png|*.png";
            if (sfd.ShowDialog().Value)
            {
                var renderData = new List<string[][]>();
                foreach (var line in _convertedLineList)
                {
                    var renderLine = new List<string[]>();
                    foreach (var unit in line.Units)
                    {
                        var renderUnit = new List<string>();
                        if (GetBool(EditRomajiCheckBox.IsChecked))
                            renderUnit.Add(unit.Romaji);
                        if (GetBool(EditHiraganaCheckBox.IsChecked))
                        {
                            if (GetBool(IsOnlyShowKanjiCheckBox.IsChecked))
                            {
                                renderUnit.Add(unit.IsKanji ? unit.Hiragana : " ");
                            }
                            else
                            {
                                renderUnit.Add(unit.Hiragana);
                            }
                        }
                        renderUnit.Add(unit.Japanese);
                        renderLine.Add(renderUnit.ToArray());
                    }
                    renderData.Add(renderLine.ToArray());
                }
                using var image = GenerateImageHelper.ToImage(renderData, new GenerateImageHelper.ImageSetting(((App)Application.Current).Config));
                image.Save(sfd.FileName, ImageFormat.Png);
                if (((App)Application.Current).Config.IsOpenExplorerAfterSaveImage)
                    Process.Start("explorer.exe", $"/select,\"{sfd.FileName}\"");
            }
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

        private ColumnDefinition _editColumnDefinition = new ColumnDefinition();
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
                    if (MainGrid.ColumnDefinitions.Count == 2) MainGrid.ColumnDefinitions.Insert(1, _editColumnDefinition);
                    ReadButton.Visibility = Visibility.Visible;
                    SaveButton.Visibility = Visibility.Visible;
                    EditHiraganaCheckBox.Visibility = Visibility.Visible;
                    EditRomajiCheckBox.Visibility = Visibility.Visible;
                    IsOnlyShowKanjiCheckBox.Visibility = Visibility.Visible;
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
                    IsOnlyShowKanjiCheckBox.Visibility = Visibility.Collapsed;
                    ConvertPictureButton.Visibility = Visibility.Collapsed;
                    ConvertTextButton.Visibility = Visibility.Collapsed;
                    EditBorder.Visibility = Visibility.Collapsed;
                    _editColumnDefinition = MainGrid.ColumnDefinitions[1];
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

        private void SettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingWindow().Show();
        }

        #endregion

        #region 文本缩放相关

        /// <summary>
        /// 输入文本框缩放值
        /// </summary>
        public string InputTextBoxScale
        {
            get
            {
                return _inputTextBoxScale;
            }
            set
            {
                _inputTextBoxScale = value;
                OnPropertyChanged("InputTextBoxScale");
                ((App)Application.Current).Config.InputTextBoxFontSize = InputTextBox.FontSize;
            }
        }

        private string _inputTextBoxScale;

        /// <summary>
        /// 输入文本框缩放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0 && InputTextBox.FontSize > 2.64)
                {
                    InputTextBox.FontSize /= 1.1;
                }
                else if (e.Delta > 0 && InputTextBox.FontSize < 41.5)
                {
                    InputTextBox.FontSize *= 1.1;
                }
                InputTextBoxScale = (int)Math.Round(InputTextBox.FontSize / 12 * 100) + "%";
                e.Handled = true;
            }
        }

        /// <summary>
        /// 编辑区缩放值
        /// </summary>
        public string EditPanelScale
        {
            get
            {
                return _editPanelScale;
            }
            set
            {
                _editPanelScale = value;
                OnPropertyChanged("EditPanelScale");
                ((App)Application.Current).Config.EditPanelFontSize = _editPanelFontSize;
            }
        }

        private string _editPanelScale;

        private double _editPanelFontSize = 12;

        /// <summary>
        /// 编辑区缩放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0 && _editPanelFontSize > 2.64)
                {
                    _editPanelFontSize /= 1.1;
                    foreach (object children in EditPanel.Children)
                    {
                        WrapPanel wrapPanel;
                        if (children.GetType() == typeof(WrapPanel))
                            wrapPanel = (WrapPanel)children;
                        else
                            continue;

                        foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                        {
                            editableLabelGroup.MyFontSize = _editPanelFontSize;
                        }
                    }
                }
                else if (e.Delta > 0 && _editPanelFontSize < 41.5)
                {
                    _editPanelFontSize *= 1.1;
                    foreach (object children in EditPanel.Children)
                    {
                        WrapPanel wrapPanel;
                        if (children.GetType() == typeof(WrapPanel))
                            wrapPanel = (WrapPanel)children;
                        else
                            continue;

                        foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                        {
                            editableLabelGroup.MyFontSize = _editPanelFontSize;
                        }
                    }
                }
                EditPanelScale = (int)Math.Round(_editPanelFontSize / 12 * 100) + "%";
                e.Handled = true;
            }
        }

        /// <summary>
        /// 输出文本框缩放值
        /// </summary>
        public string OutputTextBoxScale
        {
            get
            {
                return _outputTextBoxScale;
            }
            set
            {
                _outputTextBoxScale = value;
                OnPropertyChanged("OutputTextBoxScale");
                ((App)Application.Current).Config.OutputTextBoxFontSize = OutputTextBox.FontSize;
            }
        }

        private string _outputTextBoxScale;

        /// <summary>
        /// 输出文本框缩放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0 && OutputTextBox.FontSize > 2.64)
                {
                    OutputTextBox.FontSize /= 1.1;
                }
                else if (e.Delta > 0 && OutputTextBox.FontSize < 41.5)
                {
                    OutputTextBox.FontSize *= 1.1;
                }
                OutputTextBoxScale = (int)Math.Round(OutputTextBox.FontSize / 12 * 100) + "%";
                e.Handled = true;
            }
        }

        #endregion
    }
}
