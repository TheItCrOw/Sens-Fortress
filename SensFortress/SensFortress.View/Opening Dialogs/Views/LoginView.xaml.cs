using SensFortress.Data.Database;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Helper;
using SensFortress.View.Opening_Dialogs.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SensFortress.View.Opening_Dialogs.Views
{
    /// <summary>
    /// Interaktionslogik für LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel();
        }

        /// <summary>
        /// The event that handles the login process. We do that in code behind since we are handling passwords.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(MasterKey_PasswordBox.Password) || Fortress_TreeView.SelectedItem_ == null)
                {
                    Communication.InformUser("You shall not pass!");
                    return;
                }

                Login_ProgressBar.IsIndeterminate = true;

                var pw = MasterKey_PasswordBox.Password;
                var fortressVm = (FortressViewModel)Fortress_TreeView.SelectedItem_;
                Logger.log.Info($"Login attempt in fortress: {fortressVm.ToString()}");

                await Task.Run(() => IsValidMasterKey(fortressVm, pw));

                pw = string.Empty;
                Login_ProgressBar.IsIndeterminate = false;
            }
            catch (Exception ex)
            {
                ex.SetUserMessage("An error occured while trying to escort you into the fortress.");
                Communication.InformUserAboutError(ex);
                Logger.log.Error($"Error while trying to log in {ex}");
            }
        }

        /// <summary>
        /// Checks if the login is authorized.
        /// </summary>
        /// <param name="fortressVm"></param>
        /// <param name="pw"></param>
        private void IsValidMasterKey(FortressViewModel fortressVm, string pw)
        {
            var result = false;

            result = DataAccessService.Instance.BuildFortress(fortressVm.FullName, fortressVm.Name, pw);

            if (result)
            {
                pw = string.Empty;
                Application.Current.Dispatcher.Invoke(() => Navigation.NavigateTo(NavigationViews.HomeView));
                Logger.log.Info($"Login successfull!");
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => Communication.InformUser("You shall not pass!"));
                Logger.log.Info($"Login failed.");
            }
        }

    }
}

