using log4net;
using log4net.Config;
using SensFortress.Security.Testing;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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








            //Testing
            string original = "Here is some data to encrypt!";

            // Create a new instance of the Aes
            // class.  This generates a new key and initialization 
            // vector (IV).
            using (Aes myAes = Aes.Create())
            {

                // Encrypt the string to an array of bytes.
                byte[] encrypted = AesTests.EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

                var testVI = "testVI123";

                var testByteVI = Encoding.ASCII.GetBytes(testVI);

                // Decrypt the bytes to a string.
                string roundtrip = AesTests.DecryptStringFromBytes_Aes(encrypted, myAes.Key, testByteVI);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("Round Trip: {0}", roundtrip);
            }
        }
    }

}
