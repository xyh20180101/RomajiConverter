﻿using RomajiConverter.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RomajiConverter
{
    /// <summary>
    /// EditableLabel.xaml 的交互逻辑
    /// </summary>
    public partial class EditableLabel : UserControl, INotifyPropertyChanged
    {
        public EditableLabel()
        {
            InitializeComponent();
            DataContext = this;
            IsEdit = false;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableLabel), new PropertyMetadata(default(string)));

        [Category("Extension")]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty MyFontSizeProperty = DependencyProperty.Register("MyFontSize", typeof(double), typeof(EditableLabel), new PropertyMetadata(12d));

        [Category("Extension")]
        public double MyFontSize
        {
            get => (double)GetValue(MyFontSizeProperty);
            set => SetValue(MyFontSizeProperty, value);
        }

        private bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
                OnPropertyChanged("EditLabelVisibility");
                OnPropertyChanged("EditBoxVisibility");
            }
        }

        public Visibility EditLabelVisibility => IsEdit ? Visibility.Hidden : Visibility.Visible;

        public Visibility EditBoxVisibility => IsEdit ? Visibility.Visible : Visibility.Hidden;

        public void ToEdit()
        {
            IsEdit = true;
            EditBox.Focus();
            EditBox.SelectionStart = EditBox.Text.Length;
        }

        public void ToSave()
        {
            IsEdit = false;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                ToEdit();
                e.Handled = true;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                EditBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void EditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ToSave();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
