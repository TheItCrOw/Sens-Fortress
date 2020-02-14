using MaterialDesignThemes.Wpf;
using SensFortress.Data.Database;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Helper;
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
        private bool _pwIsHidden;
        public HomeView()
        {
            InitializeComponent();
            _pwIsHidden = true;
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

        /// <summary>
        /// Unlocks the fortress if the masterkey has been entered correctly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnlockFortress_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentPw = string.Empty;

                if (_pwIsHidden)
                    currentPw = MasterLock_PasswordBox.Password;
                else
                    currentPw = MasterLock_Textbox.Text;

                if (DataAccessService.Instance.ValidateMasterkey(currentPw))
                {
                    CurrentFortressData.IsLocked = false;
                    MasterLock_PasswordBox.Password = string.Empty;
                    MasterLock_Textbox.Text = string.Empty;
                    ((HomeViewModel)DataContext).ShowLockCard = false;
                }
                else
                {
                    Communication.InformUser("You didn't say the magic words my friend.");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while trying to unlock fortress: {ex}");
                MasterLock_PasswordBox.Password = string.Empty;
                MasterLock_Textbox.Text = string.Empty;
                ex.SetUserMessage("An error occured while trying to unlock the fortress. The fortress will stay locked for now.");
                CurrentFortressData.IsLocked = true;
                Communication.InformUserAboutError(ex);
            }
        }

        private void ShowHide_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_pwIsHidden)
            {
                MasterLock_Textbox.Text = MasterLock_PasswordBox.Password;
                MasterLock_PasswordBox.Visibility = Visibility.Collapsed;
                MasterLock_Textbox.Visibility = Visibility.Visible;
                _pwIsHidden = !_pwIsHidden;
            }
            else
            {
                MasterLock_PasswordBox.Password = MasterLock_Textbox.Text;
                MasterLock_Textbox.Visibility = Visibility.Collapsed;
                MasterLock_PasswordBox.Visibility = Visibility.Visible;
                _pwIsHidden = !_pwIsHidden;
            }
        }

        //Start Drag and Drop
        private void TreeItem_Card_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // The dragging can be started from the card itself, the textbox name and the icon
                if (e.Source.GetType().Name.Equals("Card"))
                {
                    InitializeDragDrop<Card>(sender, e);
                }
                else if (e.Source.GetType().Name.Equals("TextBox"))
                {
                    InitializeDragDrop<TextBox>(sender, e);
                }
                else if (e.Source.GetType().Name.Equals("PackIcon"))
                {
                    InitializeDragDrop<PackIcon>(sender, e);
                }
            }
            catch (Exception ex)
            {
                // For now, we do not want to inform the user about possible small mistakes in here.
                Logger.log.Error($"Error while trying to start dragging: {ex}");
            }
        }

        private void InitializeDragDrop<T>(object sender, MouseButtonEventArgs e) where T : DependencyObject
        {
            // The source is the object that is being touched to drag
            var item = (T)e.Source;
            // This is the data behind
            var treeItem = ((Card)sender).DataContext;
            // If the textbox is currently being edited - we do not want to drag.
            if (((TreeItemViewModel)treeItem).IsEditable)
                return;

            if (item != null)
            {
                DragDrop_Popup.IsOpen = true;
                DragDrop.DoDragDrop(item, treeItem, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Searching through the treeView on the left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((HomeViewModel)DataContext).SearchThroughNodesCommand.Execute(Search_Textbox.Text);
        }

        /// <summary>
        /// Searching through the treeView on the left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                ((HomeViewModel)DataContext).SearchThroughNodesCommand.Execute(Search_Textbox.Text);
        }

        private void DragDrop_Popup_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(DragDrop_Popup.IsOpen)
            {
                var mousePos = e.GetPosition(this.DragDrop_Popup);
                DragDrop_Popup.HorizontalOffset = mousePos.X;
                DragDrop_Popup.VerticalOffset = mousePos.Y;
            }
        }
    }
}
