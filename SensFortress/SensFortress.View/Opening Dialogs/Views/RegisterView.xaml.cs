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
using System.Linq;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Models.Fortress;
using SensFortress.Data.Database;
using SensFortress.View.Helper;
using SensFortress.Utility.Log;
using SensFortress.Utility.Exceptions;
using Microsoft.Win32;
using SensFortress.Security;

namespace SensFortress.View.Opening_Dialogs.Views
{
    /// <summary>
    /// Interaktionslogik für RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        private string _fullPath = string.Empty;
        private bool _pwIsVisible;

        public RegisterView()
        {
            InitializeComponent();
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reset();
                var currentPw = string.Empty;
                var currentReenteredPw = string.Empty;
                if(_pwIsVisible)
                {
                    currentPw = Master_VisibleTextbox.Text;
                    currentReenteredPw = MasterReentered_Textbox.Text;
                }
                else
                {
                    currentPw = Master_PasswordBox.Password;
                    currentReenteredPw = MasterReentered_PasswordBox.Password;
                }

                // Check if all criteria are correct
                if (Info_Checkbox.IsChecked == false)
                {
                    Output_Textblock.Text = "You need to varify first that you have read the text on the left.";
                    Info_Checkbox.Foreground = Brushes.Red;
                    return;
                }

                if (FortressName_Textbox.Text == string.Empty)
                {
                    Output_Textblock.Text = "You need to name your fortress.";
                    FortressName_Textbox.BorderBrush = Brushes.Red;
                    return;
                }

                if (WellKnownSpecialCharacters.ContainsSpecialCharacters(FortressName_Textbox.Text))
                {
                    Output_Textblock.Text = "Special characters in the name are not allowed.";
                    FortressName_Textbox.BorderBrush = Brushes.Red;
                    return;
                }

                if (currentPw.Length < 8 ||
                    !(currentPw.Any(char.IsUpper)) ||
                    !(currentPw.Any(char.IsDigit)))
                {
                    Output_Textblock.Text = "The masterkey has to match the following criteria: Minumum 8 characters long; Contain at least one upper case character and one digit.";
                    Master_PasswordBox.Foreground = Brushes.Red;
                    Master_VisibleTextbox.Foreground = Brushes.Red;
                    return;
                }

                if (currentReenteredPw != currentPw)
                {
                    Output_Textblock.Text = "Masterkey doesn't match the reentered one.";
                    MasterReentered_PasswordBox.Foreground = Brushes.Red;
                    MasterReentered_Textbox.Foreground = Brushes.Red;
                    return;
                }

                // If they are - continue to make the fortress.
                var aesHelper = new AesHelper();
                var salt = aesHelper.GenerateSalt();
                var hashedKey = aesHelper.CreateKey(Master_PasswordBox.Password, 512, salt);
                var fullPath = string.Empty;

                // If the user has entered a custom path -> Disabled for now
                if(_fullPath != string.Empty)
                    fullPath = $"{_fullPath}\\{FortressName_Textbox.Text}";
                else // else use the default.
                    fullPath = $"{IOPathHelper.GetDefaultFortressDirectory()}\\{FortressName_Textbox.Text}";

                var name = "NOT GIVEN";
                var lastName = "NOT GIVEN";
                var userName = "notGiven";
                var eMail = "NotGiven@xyz.com";
                var fortress = new Fortress(salt, hashedKey, fullPath, name, lastName, userName, eMail, Guid.NewGuid());

                DataAccessService.Instance.CreateNewFortress(fortress); // Create the new fortress.

                ClearPasswords();

                Navigation.LoginManagementInstance.LoadFortresses(); // Refresh the list.

                Communication.InformUser($"{FortressName_Textbox.Text} has been successfully built.");
            }
            catch (Exception ex)
            {
                ClearPasswords();
                Logger.log.Error($"Error while trying to register a new fortress: {ex}");
                ex.SetUserMessage("There was a problem creating the fortress. The given passwords have been flushed out of memory.");
                Communication.InformUserAboutError(ex);
            }
        }

        private void GeneratePassword_Button_Click(object sender, RoutedEventArgs e)
        {
            var randomPw = PasswordHelper.GenerateSecurePassword();
            //var infoText = "If you are satisfied with the generated masterkey and wish to use it as your own: REMEMBER and ARCHIVE this password now. If not: Generate a new one by closing this window and clicking the button again.";
            //Communication.ShowSensibleData(randomPw, infoText, "Your masterkey:");

            if (_pwIsVisible)
                Master_VisibleTextbox.Text = randomPw;
            else
                Master_PasswordBox.Password = randomPw;

            randomPw = string.Empty;
        }

        /// <summary>
        /// Show/Hide password. PasswordBox is very sensitive in that regard. It has not method for this and it doesn't allow password binding.
        /// So this is a bit hacky..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHide_Button_Click(object sender, RoutedEventArgs e)
        {
            if(_pwIsVisible)
            {
                Master_PasswordBox.Visibility = Visibility.Visible;
                Master_VisibleTextbox.Visibility = Visibility.Collapsed;
                MasterReentered_PasswordBox.Visibility = Visibility.Visible;
                MasterReentered_Textbox.Visibility = Visibility.Collapsed;
                Master_PasswordBox.Password = Master_VisibleTextbox.Text;
                MasterReentered_PasswordBox.Password = MasterReentered_Textbox.Text;
                _pwIsVisible = false;
            }
            else
            {
                Master_PasswordBox.Visibility = Visibility.Collapsed;
                Master_VisibleTextbox.Visibility = Visibility.Visible;
                MasterReentered_PasswordBox.Visibility = Visibility.Collapsed;
                MasterReentered_Textbox.Visibility = Visibility.Visible;
                Master_VisibleTextbox.Text = Master_PasswordBox.Password;
                MasterReentered_Textbox.Text = MasterReentered_PasswordBox.Password;
                _pwIsVisible = true;
            }
        }

        private void Reset()
        {
            Info_Checkbox.Foreground = Brushes.White;
            Master_PasswordBox.Foreground = Brushes.White;
            MasterReentered_PasswordBox.Foreground = Brushes.White;
            FortressName_Textbox.BorderBrush = Brushes.White;
            MasterReentered_Textbox.Foreground = Brushes.White;
            Master_VisibleTextbox.Foreground = Brushes.White;
        }

        private void ClearPasswords()
        {
            Master_PasswordBox.Password = string.Empty;
            MasterReentered_PasswordBox.Password = string.Empty;
            MasterReentered_Textbox.Text = string.Empty;
            Master_VisibleTextbox.Text = string.Empty;
        }
    }
}
