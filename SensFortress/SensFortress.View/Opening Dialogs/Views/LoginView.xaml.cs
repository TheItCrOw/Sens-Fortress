using SensFortress.Data.Database;
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
                UIHelper.InformUser("Welcome back.");
                Navigation.NavigateTo(NavigationViews.HomeView);
            }
            else
            {
                UIHelper.InformUser("You may not enter - mellon.");
            }
        }

        /// <summary>
        /// Try to login under given password
        /// </summary>
        /// <returns></returns>
        private bool IsValidMasterkey()
        {
            if (string.IsNullOrEmpty(MasterKey_PasswordBox.Password))
                return false;

            if(Fortress_TreeView.SelectedItem_ == null)
                return false;

            var currentFortress = (FortressViewModel)Fortress_TreeView.SelectedItem_;

            if (Factory.Instance.BuildFortress(currentFortress.FullName, currentFortress.Name, MasterKey_PasswordBox.Password))
            {
                MasterKey_PasswordBox.Password = string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
