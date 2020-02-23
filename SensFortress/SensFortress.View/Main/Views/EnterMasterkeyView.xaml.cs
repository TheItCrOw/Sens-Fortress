using SensFortress.Data.Database;
using SensFortress.Utility;
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

namespace SensFortress.View.Main.Views
{
    /// <summary>
    /// Interaktionslogik für EnterMasterkeyView.xaml
    /// </summary>
    public partial class EnterMasterkeyView : Window
    {
        public string PotFortressPath { get; set; }
        public string PotFortressName { get; set; }

        public EnterMasterkeyView()
        {
            InitializeComponent();
        }

        private void Varify_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataAccessService.Instance.ValidateMasterkey(Masterkey_Textbox.Text, false, PotFortressPath, PotFortressName))
            {
                DialogResult = true;
                Close();
            }
            else
                Communication.InformUser("Couldn't validate the given masterkey.");
        }
    }
}
