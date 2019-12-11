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
using SensFortress.View.Helper;
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
            Start();
        }

        void Start()
        {
            Logger.log.Info("Building the gates...");
            InitializeComponent();
            // Start the factory queue
            if(Factory.Instance.StartFactoryQueue(System.IO.Path.Combine(DirectoryHelper.GetDesktopPath(), "SensFortress Test Ordner", "testFortress")))
            {
                Logger.log.Info("Successfully built!");

                // For some reason, VS is firing an exceptionn when trying to do this in XAML...
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // Give the NavigationHelper access to the views.
                Navigation.MainFrame = MainFrame;

                Testing();
            }
            else
            {
                this.Close();
            }

        }

        private void Testing()
        {
            //ZipHelper.ZipSavedArchives("C:\\Users\\Nutzer\\Desktop\\DateTickets", "C:\\Users\\Nutzer\\Desktop\\TestZip.sfzf");
            //var testZip = ZipHelper.UnzipSavedZip("C:\\Users\\Nutzer\\Desktop\\TestZip.sfzf");
            //AesTests.TestFileEncryption();
            //AesTests.TestFileDecryption();

            var aesHelper = new AesHelper();
            var salt = aesHelper.GenerateSalt();
            var userMasterKey = "diesIstEinTest123";
            var hashedKey = aesHelper.CreateKey(userMasterKey, 512, salt);
            var fullPath = $"{DirectoryHelper.GetDefaultFortressDirectory()}\\testFortress25";
            var name = "Max";
            var lastName = "Mustermann";
            var userName = "mMuster";
            var eMail = "test@web.de";

            var fortress = new Fortress(salt, hashedKey, fullPath, name, lastName, userName, eMail, Guid.NewGuid());

            //DataAccessService.Instance.TestStoreOne(fortress);
            //DataAccessService.Instance.CreateNewFortress(fortress);
            //DataAccessService.Instance.BuildFortress(System.IO.Path.Combine(DirectoryHelper.GetDesktopPath(), "SensFortress Test Ordner\\testFortress.sfzf"), "testFortress", "diesIstEinTest123");


            //Factory.Instance.EnqueueTask(Factory.FactoryTaskType.Create, new object[1] { fortress});
            var param = new object[3] { System.IO.Path.Combine(DirectoryHelper.GetDesktopPath(), "SensFortress Test Ordner\\testFortress.sfzf"), "testFortress", "diesIstEinTest123" };
            Factory.Instance.EnqueueTask(Factory.FactoryTaskType.Build, param);

            //for (int i = 0; i < 100; i++)
            //{
            //    var userMasterKey2 = "diesIstEinTest123";

            //    var aesHelper2 = new AesHelper();
            //    var salt2 = aesHelper.GenerateSalt();
            //    var hashedKey2 = aesHelper.CreateKey(userMasterKey, 512, salt);
            //    var fullPath2 = (System.IO.Path.Combine(DirectoryHelper.GetDesktopPath(), "SensFortress Test Ordner", $"testFortress{i}"));
            //    var name2 = "Max";
            //    var lastName2 = "Mustermann";
            //    var userName2 = "mMuster";
            //    var eMail2 = "test@web.de";

            //    var fortress2 = new Fortress(salt2, hashedKey2, fullPath2, name2, lastName2, userName2, eMail2, Guid.NewGuid());
            //    Factory.Instance.EnqueueTask(Factory.FactoryTaskType.Create, new object[1] { fortress2 });
            //}
        }

    }

}
