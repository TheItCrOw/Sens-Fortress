﻿using SensFortress.View.Helper;
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
        }

        /// <summary>
        /// The event that handles the login process. We do that in code behind since we are handling passwords.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateTo(NavigationViews.HomeView);
        }
    }
}
