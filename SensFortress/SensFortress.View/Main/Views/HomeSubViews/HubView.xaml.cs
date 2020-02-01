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

        private void AddDragItem_Card_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeItemViewModel)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                ((Card)sender).Background = Brushes.GhostWhite;
                e.Effects = DragDropEffects.Move;
            }
        }

        private void AddDragItem_Card_DragLeave(object sender, DragEventArgs e)
        {
            ((Card)sender).Background = Brushes.DarkGray;
        }

        private void AddDragItem_Card_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeItemViewModel)))
            {
                // do whatever you want do with the dropped element
                var droppedThingie = e.Data.GetData(typeof(TreeItemViewModel)) as TreeItemViewModel;
            }
            ((Card)sender).Background = Brushes.DarkGray;
        }
    }
}
