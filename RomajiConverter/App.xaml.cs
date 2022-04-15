using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AduSkin.Controls.Metro;
using Newtonsoft.Json;

namespace RomajiConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            InitConfig();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            AduMessageBox.Show($"{e.Exception.Message}", "异常");

            e.Handled = true;//表示异常已处理，可以继续运行
        }

        private void InitConfig()
        {
            var myConfig = new MyConfig
            {
                DefaultBrush = "#FFFF6666"
            };
            if (File.Exists("config"))
            {
                myConfig = JsonConvert.DeserializeObject<MyConfig>(File.ReadAllText("config"));
                Current.Resources["DefaultBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(myConfig.DefaultBrush));
            }
            else
            {
                var file = File.Create("config");
                using var sw = new StreamWriter(file);
                sw.Write(JsonConvert.SerializeObject(myConfig));
            }
        }

        private class MyConfig
        {
            public string DefaultBrush { get; set; }
        }
    }
}
