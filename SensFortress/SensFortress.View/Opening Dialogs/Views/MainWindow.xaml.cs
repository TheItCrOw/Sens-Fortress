using log4net;
using log4net.Config;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Security.AES;
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
            Logger.log.Info("Building the gates...");
            InitializeComponent();
            Logger.log.Info("Successfully built!");
            // For some reason, VS is firing an exceptionn when trying to do this in XAML...
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Testing();
        }

        private void Testing()
        {
            //ZipHelper.ZipSavedArchives("C:\\Users\\Nutzer\\Desktop\\DateTickets", "C:\\Users\\Nutzer\\Desktop\\TestZip.sfzf");
            //var testZip = ZipHelper.UnzipSavedZip("C:\\Users\\Nutzer\\Desktop\\TestZip.sfzf");
            //AesTests.TestFileEncryption();
            //AesTests.TestFileDecryption();
            var userMasterKey = "diesIstEinTest123";

            var aesHelper = new AesHelper();
            var salt = aesHelper.GenerateSalt();
            var hashedKey = aesHelper.CreateKey(userMasterKey, 512, salt);
            var fullPath = "C:\\Users\\Nutzer\\Desktop\\testFortress";
            var name = "Max";
            var lastName = "Mustermann";
            var userName = "mMuster";
            var eMail = "test@web.de";


            var fortress = new Fortress(salt, hashedKey, fullPath, name, lastName, userName, eMail, Guid.NewGuid());
            DataAccessService.Instance.CreateNewFortress(fortress);
            DataAccessService.Instance.BuildFortress("C:\\Users\\Nutzer\\Desktop\\testFortress.sfzf", "testFortress", "diesIstEinTest123");
        }

    }

}
