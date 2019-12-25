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
    /// Interaktionslogik für AskForAnswerView.xaml
    /// </summary>
    public partial class AskForAnswerView : Window
    {
        public AskForAnswerView(string message)
        {
            InitializeComponent();

            Info_Textblock.Text = message;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Topmost = true;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
