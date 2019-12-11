using SensFortress.View.Main.ViewModel;
using SensFortress.View.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SensFortress.View.Helper
{
    public static class Navigation
    {
        public static Frame MainFrame { get; set; }

        /// <summary>
        /// Navigates to the given view.
        /// </summary>
        /// <param name="view"></param>
        public static bool NavigateTo(NavigationViews view)
        {
            switch (view)
            {
                case NavigationViews.HomeView:
                    var homeView = new HomeView();
                    homeView.DataContext = new HomeViewModel();
                    MainFrame.Content = homeView;
                    return true;
                default:
                    return false;
            }
        }

    }

    /// <summary>
    /// Enumeration of all navigatable views.
    /// </summary>
    public enum NavigationViews
    {
        HomeView,

    }
}
