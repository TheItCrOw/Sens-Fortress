﻿using SensFortress.Data.Database;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
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
            if (string.IsNullOrEmpty(MasterKey_PasswordBox.Password) || Fortress_TreeView.SelectedItem_ == null)
            {
                Communication.InformUser("You shall not pass!");
                return;
            }

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

            // => This can be deleted later, it's only for testing.
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed < TimeSpan.FromSeconds(2))
                ;
            // <= Testing

            result = DataAccessService.Instance.BuildFortress(fortressVm.FullName, fortressVm.Name, pw);

            if (result)
            {
                pw = string.Empty;
                Application.Current.Dispatcher.Invoke(() => Navigation.NavigateTo(NavigationViews.HomeView));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => Communication.InformUser("You shall not pass!"));
            }
        }

    }
}

