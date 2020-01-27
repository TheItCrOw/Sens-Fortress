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

        private void LogintoBrowser_Button_Click(object sender, RoutedEventArgs e)
        {
            var adress = new Uri("https://www.amazon.de/ap/signin?showRememberMe=false&openid.pape.max_auth_age=0&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&pageId=deflex&ignoreAuthState=1&openid.return_to=https%3A%2F%2Fwww.amazon.de%2F%3Fref_%3Dnav_signin&prevRID=T028TS6C1NMGVZ4Y8B9B&openid.assoc_handle=deflex&openid.mode=checkid_setup&openid.ns.pape=http%3A%2F%2Fspecs.openid.net%2Fextensions%2Fpape%2F1.0&prepopulatedLoginId=eyJjaXBoZXIiOiJ0d1VzTS9wRTQrcGM4WFY1NzQrdFp3PT0iLCJ2ZXJzaW9uIjoxLCJJViI6Ims3Sm9GMWJncVFjS29kaWIyQk1RK2c9PSJ9&failedSignInCount=0&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&timestamp=1579545861000");
            var browser = new BrowserView(adress);
            browser.Show();
        }

    }
}
