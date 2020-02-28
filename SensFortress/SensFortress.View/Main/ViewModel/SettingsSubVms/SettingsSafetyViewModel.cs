using Prism.Commands;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.View.Bases;
using SensFortress.View.Helper;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel.SettingsSubVms
{
    /// <summary>
    /// Interaction logic for DataBackup sub settings view.
    /// </summary>
    public class SettingsSafetyViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        private bool _b_LockingIncludeQuickBar;
        private bool _b_LockingIncludeHomeHub;
        private bool _b_MasterkeyAskForConfigSettings;
        private bool _b_LockingIncludeSettings;
        private bool _b_LockingIncludeSecurityManagement;

        public DelegateCommand SaveSettingsCommand => new DelegateCommand(SaveSettings);

        /// <summary>
        /// Naming conventions here are adjusted to the settings name conventions.
        /// </summary>
        #region settingsProperties
        public bool HasUnsavedChanges { get; set; }

        public bool B_LockingIncludeQuickBar
        {
            get => _b_LockingIncludeQuickBar;
            set
            {
                SetProperty(ref _b_LockingIncludeQuickBar, value);
                if (B_LockingIncludeQuickBar)
                {
                    B_LockingIncludeHomeHub = false;
                }
                HasUnsavedChanges = true;
            }
        }
        public bool B_LockingIncludeHomeHub
        {
            get => _b_LockingIncludeHomeHub;
            set
            {
                SetProperty(ref _b_LockingIncludeHomeHub, value);
                if (B_LockingIncludeHomeHub)
                {
                    B_LockingIncludeQuickBar = false;
                }
                HasUnsavedChanges = true;
            }
        }
        public bool B_LockingIncludeSettings
        {
            get => _b_LockingIncludeSettings;
            set
            {
                SetProperty(ref _b_LockingIncludeSettings, value);
                HasUnsavedChanges = true;
            }
        }
        public bool B_LockingIncludeSecurityManagement
        {
            get => _b_LockingIncludeSecurityManagement;
            set
            {
                SetProperty(ref _b_LockingIncludeSecurityManagement, value);
                HasUnsavedChanges = true;
            }
        }

        public bool B_MasterkeyAskForConfigSettings
        {
            get => _b_MasterkeyAskForConfigSettings;
            set
            {
                SetProperty(ref _b_MasterkeyAskForConfigSettings, value);
                HasUnsavedChanges = true;
            }
        }

        #endregion

        public override bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        public void Initialize()
        {
            LoadSettings();
        }

        /// <summary>
        /// Loads the settings initally from the file.
        /// </summary>
        private void LoadSettings()
        {
            //Locking
            B_LockingIncludeQuickBar = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeQuickBar));
            B_LockingIncludeHomeHub = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeHomeHub));
            B_LockingIncludeSettings = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeSettings));
            B_LockingIncludeSecurityManagement = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeSecurityManagement));

            //Masterkey
            B_MasterkeyAskForConfigSettings = Settings.GetSettingValue<bool>(nameof(B_MasterkeyAskForConfigSettings));

            HasUnsavedChanges = false;
        }

        /// <summary>
        /// Saves the settings into the config file
        /// </summary>
        private void SaveSettings()
        {
            //Locking
            Settings.SaveSetting(nameof(B_LockingIncludeQuickBar), B_LockingIncludeQuickBar.ToString());
            Settings.SaveSetting(nameof(B_LockingIncludeHomeHub), B_LockingIncludeHomeHub.ToString());
            Settings.SaveSetting(nameof(B_LockingIncludeSettings), B_LockingIncludeSettings.ToString());
            Settings.SaveSetting(nameof(B_LockingIncludeSecurityManagement), B_LockingIncludeSecurityManagement.ToString());

            //Masterkey
            Settings.SaveSetting(nameof(B_MasterkeyAskForConfigSettings), B_MasterkeyAskForConfigSettings.ToString());

            //Inform the home about saved settings.
            HasUnsavedChanges = false;
            Navigation.HomeManagementInstance.ReloadGuardianTasks();
            TaskLogger.Instance.Track($"Safety settings have been saved.");
            SecurityParameterProvider.Instance.UpdateHash(nameof(Settings), IOPathHelper.GetSettingsFile());
        }
    }
}
