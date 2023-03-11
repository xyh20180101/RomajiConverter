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
using RomajiConverter.Models;

namespace RomajiConverter.Controls
{
    /// <summary>
    /// EditableLabelGroup.xaml 的交互逻辑
    /// </summary>
    public partial class EditableLabelGroup : UserControl
    {
        public EditableLabelGroup(ConvertedUnit unit)
        {
            InitializeComponent();
            DataContext = this;
            Unit = unit;
            MyFontSize = ((App)Application.Current).Config.EditPanelFontSize;
        }

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(ConvertedUnit), typeof(EditableLabelGroup));

        [Category("Extension")]
        public ConvertedUnit Unit
        {
            get => (ConvertedUnit)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public static readonly DependencyProperty RomajiVisibilityProperty = DependencyProperty.Register("RomajiVisibility", typeof(Visibility), typeof(EditableLabelGroup));

        [Category("Extension")]
        public Visibility RomajiVisibility
        {
            get => (Visibility)GetValue(RomajiVisibilityProperty);
            set => SetValue(RomajiVisibilityProperty, value);
        }

        public static readonly DependencyProperty HiraganaVisibilityProperty = DependencyProperty.Register("HiraganaVisibility", typeof(Visibility), typeof(EditableLabelGroup));

        [Category("Extension")]
        public Visibility HiraganaVisibility
        {
            get => (Visibility)GetValue(HiraganaVisibilityProperty);
            set => SetValue(HiraganaVisibilityProperty, value);
        }

        public static readonly DependencyProperty MyFontSizeProperty = DependencyProperty.Register("MyFontSize", typeof(double), typeof(EditableLabelGroup), new PropertyMetadata(12d));

        [Category("Extension")]
        public double MyFontSize
        {
            get => (double)GetValue(MyFontSizeProperty);
            set => SetValue(MyFontSizeProperty, value);
        }
    }
}
