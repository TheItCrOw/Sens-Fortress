using SensFortress.Security;
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

namespace SensFortress.View.Main.Views.HomeSubViews
{
    /// <summary>
    /// Interaktionslogik für ManagePasswordEntryView.xaml
    /// </summary>
    public partial class ManagePasswordEntryView : Window
    {
        public string Name { get => Name_Textbox.Text; set => Name_Textbox.Text = value; }
        public string Url { get => Url_Textbox.Text; set => Url_Textbox.Text = value; }
        public string Password { get => Password_Textbox.Text; set => Password_Textbox.Text = value; }
        public string Description { get => Password_Description.Text; set => Password_Description.Text = value; }
        public string Username { get => Username_Textbox.Text; set => Username_Textbox.Text = value; }
        public ManagePasswordEntryView()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            // Delete PW
            DialogResult = false;
            Password_Textbox.Text = string.Empty;
            Close();
        }

        /// <summary>
        /// We close from the HomeViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Clears the pw
        /// </summary>
        public void ClearPassword()
        {
            Password_Textbox.Text = string.Empty;
        }

        private void GeneratePw_Button_Click(object sender, RoutedEventArgs e)
        {
            Password_Textbox.Text = PasswordHelper.GenerateSecurePassword();
        }
    }
}
