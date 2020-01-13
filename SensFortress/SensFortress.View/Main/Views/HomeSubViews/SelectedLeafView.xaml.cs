﻿using SensFortress.Data.Database;
using SensFortress.Utility;
using SensFortress.View.Helper;
using SensFortress.View.Main.ViewModel.HomeSubVms;
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

namespace SensFortress.View.Main.Views.HomeSubViews
{
    /// <summary>
    /// Interaktionslogik für SelectedLeafView.xaml
    /// </summary>
    public partial class SelectedLeafView : UserControl
    {
        public SelectedLeafView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Give the user the possibilty to unlock the fortress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrontContent_Button_Click(object sender, RoutedEventArgs e)
        {
            Navigation.HomeManagementInstance.LockUnlockFortressCommand.Execute();
        }
    }
}
