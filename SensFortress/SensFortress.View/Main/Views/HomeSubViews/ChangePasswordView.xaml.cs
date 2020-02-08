using SensFortress.Security;
using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private string _pwAtStart;

        public ChangePasswordView(byte[] encryptedBytes)
        {
            InitializeComponent();
            Password_Textbox.Text = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(encryptedBytes));
            _pwAtStart = Password_Textbox.Text;
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            // If the pw hasnt been changed, return false.
            if (_pwAtStart == Password_Textbox.Text)
            {
                Password_Textbox.Text = string.Empty;
                _pwAtStart = string.Empty;
                DialogResult = false;
                return;
            }
            else if(Password_Textbox.Text == string.Empty)
            {
                Information_Textblock.Text = "An empty password is not an option.";
                return;
            }

            // A bit ugly to put 3 methods into each other, but I want to avoid creating many extra variables when handling sensible data.
            ChangedPasswordEncrypted = CryptMemoryProtection.EncryptInMemoryData(ByteHelper.StringToByteArray(Password_Textbox.Text));
            Password_Textbox.Text = string.Empty;
            _pwAtStart = string.Empty;
            DialogResult = true;
        }

        private void GeneratePassword_Button_Click(object sender, RoutedEventArgs e)
        {
            Password_Textbox.Text = PasswordHelper.GenerateSecurePassword();
        }
    }
}
