using SensFortress.View.Main.ViewModel;
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

namespace SensFortress.View.Main.Views
{
    /// <summary>b
    /// Interaktionslogik für HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public void ShowUnlockCard()
        {
            Unlock_Card.Visibility = Visibility.Visible;
        }

        private void CloseLockCard_Button_Click(object sender, RoutedEventArgs e)
        {
            MasterLock_PasswordBox.Password = string.Empty;
            MasterLock_Textbox.Text = string.Empty;
            ((HomeViewModel)DataContext).ShowLockCard = false;
        }
    }
}
