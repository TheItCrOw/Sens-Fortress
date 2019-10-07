using SensFortress.Utility;
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

namespace SensFortress.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LogHelper.Log(LogType.Info, "Building fortress...");
            InitializeComponent();
            // For some reason, VS is firing an exceptionn when trying to do this in XAML...
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LogHelper.Log(LogType.Info, "Sen's fortress has been successfully built!");
        }
    }
}
