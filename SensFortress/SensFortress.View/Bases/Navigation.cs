using SensFortress.View.Main.ViewModel;
using SensFortress.View.Main.Views;
using SensFortress.View.Opening_Dialogs.ViewModels;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SensFortress.View.Helper
{
    public static class Navigation
    {
        public static Frame MainFrame { get; set; }

        public static LoginViewModel LoginManagementInstance { get; set; }

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
                    var homeVm = new HomeViewModel();
                    homeView.DataContext = homeVm;
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
