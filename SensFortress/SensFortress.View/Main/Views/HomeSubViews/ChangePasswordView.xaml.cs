using SensFortress.Security;
using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SensFortress.View.Main.Views.HomeSubViews
{
    /// <summary>
    /// Interaktionslogik für ChangePasswordView.xaml
    /// </summary>
    public partial class ChangePasswordView : Window
    {
        public byte[] ChangedPasswordEncrypted { get; set; }

        public ChangePasswordView(byte[] encryptedBytes)
        {
            InitializeComponent();
            Password_Textbox.Text = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(encryptedBytes));
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            // A bit ugly to put 3 methods into each other, but I want to avoid creating many extra variables when handling sensible data.
            ChangedPasswordEncrypted = CryptMemoryProtection.EncryptInMemoryData(ByteHelper.StringToByteArray(Password_Textbox.Text));
            Password_Textbox.Text = string.Empty;
            DialogResult = true;
        }
    }
}
