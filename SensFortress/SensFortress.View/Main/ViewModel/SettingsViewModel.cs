using Prism.Commands;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Helper;
using SensFortress.View.Main.ViewModel.SettingsSubVms;
using SensFortress.View.Main.Views.SettingsSubViews;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    /// <summary>
    /// Interaction logic with the SettingsView
    /// </summary>
    public class SettingsViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        private object _selectedContent;
        private SettingsSafetyView _safetyView;
        private SettingsSafetyViewModel _safetyVm;
        private SettingsDataBackupView _dataBackupView;
        private SettingsDataBackupViewModel _dataBackupVm;
        private string _currentTitle;

        public DelegateCommand<string> NavigateToSettingCategoryCommand => new DelegateCommand<string>(NavigateToSettingCategory);
        public override bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }
        public string CurrentTitel
        {
            get => _currentTitle;
            set
            {
                SetProperty(ref _currentTitle, value);
            }
        }
        /// <summary>
        /// Holds the view that is being shown in the right menu.
        /// </summary>
        public object SelectedContent
        {
            get => _selectedContent;
            set
            {
                SetProperty(ref _selectedContent, value);
            }
        }

        /// <summary>
        /// Like a ctor, but we want to explicitly control when this is called.
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (_safetyView == null)
                {
                    _safetyView = new SettingsSafetyView();
                    _safetyVm = new SettingsSafetyViewModel();
                    _safetyView.DataContext = _safetyVm;
                    ((SettingsSafetyViewModel)_safetyView.DataContext).Initialize();
                }
                if (_dataBackupView == null)
                {
                    _dataBackupView = new SettingsDataBackupView();
                    _dataBackupVm = new SettingsDataBackupViewModel();
                    _dataBackupView.DataContext = _dataBackupVm;
                    ((SettingsDataBackupViewModel)_dataBackupView.DataContext).Initialize();
                }

                // default view = safety
                NavigateToSettingCategory("Safety");
            }
            catch (Exception ex)
            {
                ex.SetUserMessage($"An error occured while trying to initialize the settings. Some functions may not fully work.");
                Logger.log.Error($"Error while loading the settings: {ex}");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Reloads all settings from the config file.
        /// </summary>
        public void ReloadSettings()
        {
            if(_safetyVm.HasUnsavedChanges || _dataBackupVm.HasUnsavedChanges)
            {
                // If he wants to save unsaved changes, do so.
                if(Communication.AskForAnswer("The guardian must update your settings, but there are still unsaved changes. " +
                    "Save changes before updating? Unsaved changes will be lost."))
                {
                    _safetyVm.SaveSettingsCommand.Execute();
                    _dataBackupVm.SaveSettingsCommand.Execute();
                }
            }

            _safetyView = null;
            _dataBackupView = null;
        }

        /// <summary>
        /// Changes the selected view after it's selected category
        /// </summary>
        private void NavigateToSettingCategory(string category)
        {
            switch (category)
            {
                case "Safety":
                    if (_safetyView != null)
                        SelectedContent = _safetyView;
                    CurrentTitel = "Safety configurations";
                    break;
                case "DataBackup":
                    if (_dataBackupView != null)
                        SelectedContent = _dataBackupView;
                    CurrentTitel = "Data backup configurations";
                    break;
                default:
                    break;
            }
        }

    }
}
