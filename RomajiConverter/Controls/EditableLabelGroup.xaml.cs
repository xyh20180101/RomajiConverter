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
            GroupUnit = unit;
        }

        public static readonly DependencyProperty GroupUnitProperty = DependencyProperty.Register("GroupUnit", typeof(ConvertedUnit), typeof(EditableLabelGroup), new PropertyMetadata(default(string)));

        [Category("Extension")]
        public ConvertedUnit GroupUnit
        {
            get => (ConvertedUnit)GetValue(GroupUnitProperty);
            set => SetValue(GroupUnitProperty, value);
        }
    }
}
