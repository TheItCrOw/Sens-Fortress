using MaterialDesignThemes.Wpf;
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

namespace SensFortress.View.Main.Views.HomeSubViews
{
    /// <summary>
    /// Interaktionslogik für HubView.xaml
    /// </summary>
    public partial class HubView : UserControl
    {
        public HubView()
        {
            InitializeComponent();
        }

        public void InformAboutDragDrop()
        {
            Quickbar_Card.Visibility = Visibility.Collapsed;
            DragAdd_Card.Visibility = Visibility.Visible;
        }

        private void Quickbar_ListBox_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeItemViewModel)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                //((ListBox)sender).Background = Brushes.Gray;
                //((ListBox)sender).BorderThickness = new Thickness(3,3,3,3);
                e.Effects = DragDropEffects.Move;
            }
        }

        private void Quickbar_ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeItemViewModel)))
            {
                // do whatever you want do with the dropped element
                var droppedThingie = e.Data.GetData(typeof(TreeItemViewModel)) as TreeItemViewModel;
            }

            Quickbar_Card.Visibility = Visibility.Visible;
            DragAdd_Card.Visibility = Visibility.Collapsed;
        }

        private void Quickbar_ListBox_DragLeave(object sender, DragEventArgs e)
        {
            //((ListBox)sender).Background = Brushes.GhostWhite;
            //((ListBox)sender).BorderThickness = new Thickness(0);
            //Quickbar_Card.Visibility = Visibility.Visible;
            //DragAdd_Card.Visibility = Visibility.Collapsed;
        }
    }
}
