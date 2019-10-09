using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace SensFortress.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupLog();
            Logger.log.Info("Logger initialized...");
        }

        private void SetupLog()
        {
            var logHelper = new LogHelper();
            logHelper.InitializeLogger();
        }
    }
}
