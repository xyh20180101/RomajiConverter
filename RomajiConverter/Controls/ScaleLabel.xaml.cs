using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RomajiConverter.Controls
{
    /// <summary>
    /// ScaleLabel.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleLabel : UserControl
    {
        public ScaleLabel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty ScaleTextProperty = DependencyProperty.Register("ScaleText", typeof(string), typeof(ScaleLabel), new PropertyMetadata(default(string)));

        [Category("Extension")]
        public string ScaleText
        {
            get => (string)GetValue(ScaleTextProperty);
            set => SetValue(ScaleTextProperty, value);
        }
    }
}
