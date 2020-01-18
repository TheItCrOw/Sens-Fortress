using MSHTML;
using SensFortress.Data.Database;
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
            if(e.Key == Key.Enter)
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
            // Part 1: Use WebBrowser control to load web page
            Webbrowser.Navigate(new Uri("https://server.nitrado.net/deu/gameserver-mieten"));

            System.Threading.Thread.Sleep(2000);
            // Delay 2 seconds to render login page
            // Part 2: Automatically input username and password
            var theElementCollection = ((MSHTML.HTMLDocument)Webbrowser.Document).getElementsByTagName("input");
            foreach (var curElement in theElementCollection)
            {
                var castedCurElement = (HTMLDTElement)curElement;
                string controlName = (string)castedCurElement.getAttribute("name");
                if ((controlName == "UserNameTextBox"))
                {
                    castedCurElement.setAttribute("Value", "Username text here");
                }
                else if ((controlName == "PasswordTextBox"))
                {
                    castedCurElement.setAttribute("Value", "Password text here");
                    // In addition,you can get element value like this:
                    // MessageBox.Show(curElement.GetAttribute("Value"))
                }
            }
            //// Part 3: Automatically click that Login button
            //theElementCollection = Webbrowser.Document.GetElementsByTagName("input");
            //foreach (HtmlElement curElementin theElementCollection)
            //{
            //    if (curElement.GetAttribute("value").Equals("Login"))
            //    {
            //        curElement.InvokeMember("click");
            //        // javascript has a click method for you need to invoke on button and hyperlink elements.
            //    }
            //}
        }
    
        private bool _isRun;
        private void Webbrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!_isRun)
            {
                MSHTML.HTMLDocument document = (MSHTML.HTMLDocument)Webbrowser.Document;
                document.getElementById("login_name").innerText = "test";
                document.getElementById("passwd").innerText = "password";
                document.getElementById("buttonid_login").click();
                _isRun = true;
            }
        }
    }
}
