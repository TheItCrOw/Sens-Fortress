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

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Password_Textblock.Visibility = Visibility.Collapsed;
            Password_Textbox.Visibility = Visibility.Visible;
        }

        private void Password_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Password_Textblock.Visibility = Visibility.Visible;
            Password_Textbox.Visibility = Visibility.Collapsed;
        }
    }
}
