﻿using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using SensFortress.View.Main.ViewModel;
using SensFortress.View.Main.ViewModel.HomeSubVms;
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

            DataContext = this;
        }

        #region DragDrop
        /// <summary>
        ///  When draggin over => Dragging gets started in <see cref="HomeView"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDragItem_Card_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeItemViewModel)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                ((Card)sender).Background = Brushes.LightGray;
                e.Effects = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// When leaving the drag 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDragItem_Card_DragLeave(object sender, DragEventArgs e)
        {
            ((Card)sender).Background = Brushes.GhostWhite;
        }

        /// <summary>
        /// Drop the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDragItem_Card_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeItemViewModel)))
            {
                // do whatever you want do with the dropped element
                var droppedItem = e.Data.GetData(typeof(TreeItemViewModel)) as TreeItemViewModel;
                ((HubViewModel)DataContext).AddQuickBarItemCommand.Execute(droppedItem);
            }
            ((Card)sender).Background = Brushes.GhostWhite;
        }
        #endregion

        /// <summary>
        /// Flip the flipper card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TotalPwStrengh_Card_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Flipper.FlipCommand.Execute(null,PWAnalysis_Flipper);
        //}
    }
}
