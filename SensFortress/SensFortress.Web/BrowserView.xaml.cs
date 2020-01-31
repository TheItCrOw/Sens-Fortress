using Microsoft.Toolkit.Wpf.UI.Controls;
using mshtml;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
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

/* Important for the Webbrowser to not suck: Make changes in the registry and add them into the installation wizard.
 * See here: https://weblog.west-wind.com/posts/2011/may/21/web-browser-control-specifying-the-ie-version
 */

namespace SensFortress.Web
{
    /// <summary>
    /// Interaktionslogik für BrowserView.xaml
    /// </summary>
    public partial class BrowserView : Window
    {
        private bool _isLoginAttempt;
        private bool _isRun;
        private string _username;
        private byte[] _encryptedPassword;

        public BrowserView(Uri adress, string username, byte[] encryptedPassword)
        {
            InitializeComponent();
            _username = username;
            _encryptedPassword = encryptedPassword;
            _isLoginAttempt = true;
            Webbrowser.Navigate(adress.AbsoluteUri);
        }

        private void Webbrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                if (!_isRun && _isLoginAttempt)
                {
                    // Get the website document first
                    mshtml.HTMLDocument document = (mshtml.HTMLDocument)Webbrowser.Document;

                    // Set the username
                    var username = document.getElementById("ap_email");
                    if (username != null)
                        username.innerText = _username;
                    // Now for amazon we need to click first
                    var theElementCollection = document.getElementsByTagName("input");
                    if (theElementCollection != null)
                    {
                        foreach (var el in theElementCollection)
                        {

                            if (((HTMLDTElement)el).id == "continue")
                                ((HTMLDTElement)el).click();
                        }
                    }

                    // Let the page load
                    System.Threading.Thread.Sleep(2000);

                    // Get the newly loaded document
                    document = (mshtml.HTMLDocument)Webbrowser.Document;

                    // Fill in password
                    var pw = document.getElementById("ap_password");
                    if (pw != null)
                        pw.innerText = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(_encryptedPassword));

                    theElementCollection = document.getElementsByTagName("input");
                    if (theElementCollection != null)
                    {
                        // Click login button
                        foreach (var el in theElementCollection)
                        {
                            if (((HTMLDTElement)el).id == "signInSubmit")
                                ((HTMLDTElement)el).click();
                        }
                    }

                    Navigation_Textblock.Text = Webbrowser.Source.AbsoluteUri;
                    _isRun = true;
                }
            }
            catch (Exception ex)
            {
                Communication.InformUser("Couldn't log into page.");
                Logger.log.Error($"Couldn't login to page: {ex}");
                Close();
            }
        }

        private void Webbrowser_Navigated(object sender, NavigationEventArgs e)
        {
            //HideScriptErrors(Webbrowser, true);
        }

        /// <summary>
        /// WPF shows tons of script errors - we want to surpress them since they spam the screen.
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="hide"></param>
        private void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private void GoBack_Button_Click(object sender, RoutedEventArgs e)
        {
            Webbrowser.GoBack();
        }
    }
}
