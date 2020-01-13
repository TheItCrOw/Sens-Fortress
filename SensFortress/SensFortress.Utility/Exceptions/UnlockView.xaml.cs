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
using System.Windows.Shapes;

namespace SensFortress.Utility.Exceptions
{
    /// <summary>
    /// Interaktionslogik für UnlockView.xaml
    /// </summary>
    public partial class UnlockView : Window
    {
        private bool _pwIsShowing;
        public UnlockView()
        {
            InitializeComponent();
            _pwIsShowing = false;
        }

        private void ShowHide_Button_Click(object sender, RoutedEventArgs e)
        {
            if(_pwIsShowing)
            {
                Master_Textbox.Text = Master_PasswordBox.Password;
                Master_Textbox.Visibility = Visibility.Visible;
                Master_PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                Master_PasswordBox.Password = Master_Textbox.Text;
                Master_Textbox.Visibility = Visibility.Collapsed;
                Master_PasswordBox.Visibility = Visibility.Visible;
            }
        }
    }
}
