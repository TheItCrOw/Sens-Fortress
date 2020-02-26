using SensFortress.View.Main.ViewModel;
using SensFortress.View.Main.Views;
using SensFortress.View.Opening_Dialogs.ViewModels;
using SensFortress.View.TaskLog;
using SensFortress.View.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using SensFortress.View.Main.Views.HomeSubViews;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using SensFortress.View.Opening_Dialogs.Views;

namespace SensFortress.View.Helper
{
    public static class Navigation
    {
        public static HomeView HomeView;

        public static SettingsView SettingsView;

        public static SecurityManagementView SecurityView;
        public static Frame MainFrame { get; set; }
        public static LoginViewModel LoginManagementInstance { get; set; }
        public static HomeViewModel HomeManagementInstance { get; set; }
        public static SettingsViewModel SettingsMangementInstance { get; set; }
        public static SecurityManagementViewModel SecurityMangementInstance { get; set; }
        /// <summary>
        /// The view that contains the MainFrame and where all others views are being showed in.
        /// </summary>
        public static MainWindow MainWindowInstance { get; set; }
        /// <summary>
        /// Navigates to the given view.
        /// </summary>
        /// <param name="view"></param>
        public static bool NavigateTo(NavigationViews view)
        {
            switch (view)
            {
                case NavigationViews.HomeView:
                    if (HomeView == null)
                    {
                        HomeView = new HomeView();
                        HomeManagementInstance = new HomeViewModel();
                        HomeView.DataContext = HomeManagementInstance;
                    }
                    MainFrame.Content = HomeView;
                    HomeManagementInstance.Initialize();
                    return true;
                case NavigationViews.Settings:
                    if(SettingsView == null)
                    {
                        SettingsView = new SettingsView();
                        SettingsMangementInstance = new SettingsViewModel();
                        SettingsView.DataContext = SettingsMangementInstance;
                    }
                    SettingsMangementInstance.Initialize();
                    HomeManagementInstance.SelectedContent = SettingsView;
                    return true;
                case NavigationViews.Security:
                    if(SecurityView == null)
                    {
                        SecurityView = new SecurityManagementView();
                        SecurityMangementInstance = new SecurityManagementViewModel();
                        SecurityView.DataContext = SecurityMangementInstance;
                    }
                    SecurityMangementInstance.Initialize();
                    HomeManagementInstance.SelectedContent = SecurityView;
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
        Settings,
        Security
    }
}
