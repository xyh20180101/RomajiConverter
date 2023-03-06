using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AduSkin.Controls.Metro;
using Newtonsoft.Json;
using RomajiConverter.Extensions;

namespace RomajiConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MyConfig Config { get; set; }

        private const string _configFileName = "config.json";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            InitConfig();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            File.WriteAllText(_configFileName, JsonConvert.SerializeObject(Config, Formatting.Indented));
            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            AduMessageBox.Show($"{e.Exception.Message}", "异常");

            e.Handled = true;//表示异常已处理，可以继续运行
        }

        private void InitConfig()
        {
            if (File.Exists(_configFileName))
            {
                Config = JsonConvert.DeserializeObject<MyConfig>(File.ReadAllText(_configFileName));
                Current.Resources["DefaultBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.DefaultBrush));
            }
            else
            {
                Config = new MyConfig();
                var file = File.Create(_configFileName);
                using var sw = new StreamWriter(file);
                sw.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
        }

        public class MyConfig
        {
            /// <summary>
            /// 默认设置
            /// </summary>
            public MyConfig()
            {
                DefaultBrush = "#FFFF6666";
                IsDetailMode = false;
                InputTextBoxFontSize = 12;
                EditPanelFontSize = 12;
                OutputTextBoxFontSize = 12;
                IsOpenExplorerAfterSaveImage = true;

                InitImageSetting();
            }

            /// <summary>
            /// 生成图片默认设置
            /// </summary>
            public void InitImageSetting()
            {
                FontFamilyName = "微软雅黑";
                FontPixelSize = 48;
                FontColor = System.Drawing.Color.Black.ToHexString();
                BackgroundColor = System.Drawing.Color.White.ToHexString();
                Margin = 24;
                PaddingX = 0;
                PaddingY = 48;
                PaddingInnerY = 12;
            }

            #region 通用设置

            public string DefaultBrush { get; set; }

            public bool IsDetailMode { get; set; }

            public double InputTextBoxFontSize { get; set; }

            public double EditPanelFontSize { get; set; }

            public double OutputTextBoxFontSize { get; set; }

            public bool IsOpenExplorerAfterSaveImage { get; set; }

            #endregion

            #region 导出图片设置

            public string FontFamilyName { get; set; }

            public int FontPixelSize { get; set; }

            public string FontColor { get; set; }

            public string BackgroundColor { get; set; }

            public int Margin { get; set; }

            public int PaddingX { get; set; }

            public int PaddingY { get; set; }

            public int PaddingInnerY { get; set; }

            #endregion
        }
    }
}
