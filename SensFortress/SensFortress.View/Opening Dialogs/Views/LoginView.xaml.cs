using SensFortress.Utility;
using SensFortress.View.Helper;
using SensFortress.View.Opening_Dialogs.ViewModels;
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
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            if(IsValidMasterkey())
            {
                Navigation.NavigateTo(NavigationViews.HomeView);
            }
            else
            {
                ExceptionHelper.InformUser("You may not enter - mellon.");
            }
        }

        private bool IsValidMasterkey()
        {
            if (string.IsNullOrEmpty(MasterKey_PasswordBox.Password))
                return false;

            return true;
        }
    }
}
