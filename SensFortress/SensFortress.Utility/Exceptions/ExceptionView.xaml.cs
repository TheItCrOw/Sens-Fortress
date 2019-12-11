using SensFortress.Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.Media;
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
    /// Interaktionslogik für ExceptionView.xaml
    /// </summary>
    public partial class ExceptionView : Window
    {
        public ExceptionView(Exception ex)
        {
            InitializeComponent();

            Error_Textblock.Text = ex.GetUserMessage();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Topmost = true;
            SystemSounds.Hand.Play();
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
