using System;
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
            InitializeComponent();
            KuromojiHelper.Init();
            BorderBrush = new SolidColorBrush(Color.FromRgb(255, 102, 102));
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            var result = RomajiHelper.ToRomaji(InputTextBox.Text);
            var output = new StringBuilder();
            foreach (var item in result)
            {
                output.AppendLine(item.Romaji);
            }
            OutputTextBox.Text = output.ToString();
        }
    }
}
