using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AduSkin.Controls.Metro;

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
        }
        
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            string msg = String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            AduMessageBox.Show(msg, "UI线程异常");

            //e.Handled = true;//表示异常已处理，可以继续运行
        }
    }
}
