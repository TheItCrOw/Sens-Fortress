﻿using log4net;
using log4net.Config;
using SensFortress.Security;
using SensFortress.Security.Testing;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using SensFortress.Utility.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;

namespace SensFortress.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Logger.log.Info("Building the fortress...");
            InitializeComponent();
            Logger.log.Info("Successfully built!");
            // For some reason, VS is firing an exceptionn when trying to do this in XAML...
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            ZipTests.CustomZipHelper("", "", "");
            AesTests.TestFileEncryption();
            AesTests.TestFileDecryption();
        }

    }

}
