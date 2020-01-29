using mshtml;
using SensFortress.Data.Database;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.View.Helper;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using SensFortress.View.Test;
using SensFortress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
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

        #region Edit Username
        private void EditUsername_Button_Click(object sender, RoutedEventArgs e)
        {
            Username_Textbox.Visibility = Visibility.Visible;
            Username_Textblock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// For handling edit username
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Username_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Username_Textbox.Visibility = Visibility.Collapsed;
            Username_Textblock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// For handling edit username
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Username_Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Username_Textbox.Visibility = Visibility.Collapsed;
                Username_Textblock.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Edit Description
        private void EditDescription_Button_Click(object sender, RoutedEventArgs e)
        {
            Description_Textbox.Visibility = Visibility.Visible;
            Description_Textblock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// For handling edit description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Description_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Description_Textbox.Visibility = Visibility.Collapsed;
            Description_Textblock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// For handling edit description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Description_Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Description_Textbox.Visibility = Visibility.Collapsed;
                Description_Textblock.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
