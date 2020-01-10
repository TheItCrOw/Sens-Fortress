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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Linq;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Models.Fortress;
using SensFortress.Data.Database;
using SensFortress.View.Helper;
using SensFortress.Utility.Log;
using SensFortress.Utility.Exceptions;

namespace SensFortress.View.Opening_Dialogs.Views
{
    /// <summary>
    /// Interaktionslogik für RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            // Check if all criteria are correct
            if (Info_Checkbox.IsChecked == false)
            {
                Output_Textblock.Text = "You need to varify first that you have read the text on the left.";
                Info_Checkbox.Foreground = Brushes.Red;
                return;
            }

            if(FortressName_Textbox.Text == string.Empty)
            {
                Output_Textblock.Text = "You need to name your fortress.";
                FortressName_Textbox.BorderBrush = Brushes.Red;
                return;
            }

            var foundSpecial = false;
            // Check for sepcial charaters. We dont want them in the name
            foreach(var ch in WellKnownSpecialCharacters.SpecialCharacters)
            {
                if(FortressName_Textbox.Text.Contains(ch))
                {
                    foundSpecial = true;
                }
            }
            if(foundSpecial)
            {
                Output_Textblock.Text = "Special characters in the name are not allowed.";
                FortressName_Textbox.BorderBrush = Brushes.Red;
                return;
            }

            if (Master_PasswordBox.Password.Length < 8 ||
                !(Master_PasswordBox.Password.Any(char.IsUpper)) ||
                !(Master_PasswordBox.Password.Any(char.IsDigit)))
            {
                Output_Textblock.Text = "The masterkey has to match the following criteria: Minumum 8 characters long; Contain at least one upper case character and one digit.";
                Master_PasswordBox.Foreground = Brushes.Red;
                return;
            }

            if (MasterReentered_PasswordBox.Password != Master_PasswordBox.Password)
            {
                Output_Textblock.Text = "Masterkey doesn't match the reentered one.";
                Master_PasswordBox.Foreground = Brushes.Red;
                MasterReentered_PasswordBox.Foreground = Brushes.Red;
                return;
            }

            try
            {
                // If they are - continue to make the fortress.
                var aesHelper = new AesHelper();
                var salt = aesHelper.GenerateSalt();
                var hashedKey = aesHelper.CreateKey(Master_PasswordBox.Password, 512, salt);
                var fullPath = $"{IOPathHelper.GetDefaultFortressDirectory()}\\{FortressName_Textbox.Text}";
                var name = "NOT GIVEN";
                var lastName = "NOT GIVEN";
                var userName = "notGiven";
                var eMail = "NotGiven@xyz.com";
                var fortress = new Fortress(salt, hashedKey, fullPath, name, lastName, userName, eMail, Guid.NewGuid());

                DataAccessService.Instance.CreateNewFortress(fortress);

                Navigation.LoginManagementInstance.LoadFortresses();

                Communication.InformUser($"{FortressName_Textbox.Name} has been successfully built.");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while trying to register a new fortress: {ex}");
                ex.SetUserMessage("There was a problem creating the fortress. The given passwords have been flushed out of memory.");
                Communication.InformUserAboutError(ex);
            }
            finally
            {
                Master_PasswordBox.Password = string.Empty;
                MasterReentered_PasswordBox.Password = string.Empty;
                FortressName_Textbox.Text = string.Empty;
            }
        }

        private void Reset()
        {
            Info_Checkbox.Foreground = Brushes.White;
            Master_PasswordBox.Foreground = Brushes.White;
            MasterReentered_PasswordBox.Foreground = Brushes.White;
            FortressName_Textbox.BorderBrush = Brushes.White;
        }
    }
}
