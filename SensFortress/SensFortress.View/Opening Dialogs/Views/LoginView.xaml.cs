using SensFortress.Data.Database;
using SensFortress.Utility;
using SensFortress.View.Helper;
using SensFortress.View.Opening_Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
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
            if (string.IsNullOrEmpty(MasterKey_PasswordBox.Password))
                return;

            if (Fortress_TreeView.SelectedItem_ == null)
                return;

            Login_ProgressBar.IsIndeterminate = true;

            var pw = MasterKey_PasswordBox.Password;
            var fortressVm = (FortressViewModel)Fortress_TreeView.SelectedItem_;

            await Task.Run(() => IsValidMasterKey(fortressVm, pw));

            pw = string.Empty;
            Login_ProgressBar.IsIndeterminate = false;
        }

        /// <summary>
        /// Checks if the login is authorized.
        /// </summary>
        /// <param name="fortressVm"></param>
        /// <param name="pw"></param>
        private void IsValidMasterKey(FortressViewModel fortressVm, string pw)
        {
            var result = false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed < TimeSpan.FromSeconds(2))
                ;

            result = Factory.Instance.BuildFortress(fortressVm.FullName, fortressVm.Name, pw);

            if (result)
            {
                pw = string.Empty;
                Application.Current.Dispatcher.Invoke(() => Navigation.NavigateTo(NavigationViews.HomeView));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => UIHelper.InformUser("You shall not pass!"));
            }
        }
    }
}

