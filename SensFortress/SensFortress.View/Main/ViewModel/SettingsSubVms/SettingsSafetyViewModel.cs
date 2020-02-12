using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.View.Bases;
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
        private bool _b_LockingIncludeAll;
        private bool _b_MasterkeyAskForSaving;
        private bool _b_MasterkeyAskForConfigSettings;

        /// <summary>
        /// Naming conventions here are adjusted to the settings name conventions.
        /// </summary>
        #region settingsProperties
        public bool B_LockingIncludeQuickBar
        {
            get => _b_LockingIncludeQuickBar;
            set
            {
                SetProperty(ref _b_LockingIncludeQuickBar, value);
                if (B_LockingIncludeQuickBar)
                {
                    B_LockingIncludeHomeHub = false;
                    B_LockingIncludeAll = false;
                }
                Settings.SaveSetting(nameof(B_LockingIncludeQuickBar), value.ToString());
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
                    B_LockingIncludeAll = false;
                }
                Settings.SaveSetting(nameof(B_LockingIncludeHomeHub), value.ToString());
            }
        }
        public bool B_LockingIncludeAll
        {
            get => _b_LockingIncludeAll;
            set
            {
                SetProperty(ref _b_LockingIncludeAll, value);
                if (B_LockingIncludeAll)
                {
                    B_LockingIncludeQuickBar = false;
                    B_LockingIncludeHomeHub = false;
                }
                Settings.SaveSetting(nameof(B_LockingIncludeAll), value.ToString());
            }
        }

        public bool B_MasterkeyAskForSaving
        {
            get => _b_MasterkeyAskForSaving;
            set
            {
                SetProperty(ref _b_MasterkeyAskForSaving, value);
                Settings.SaveSetting(nameof(B_MasterkeyAskForSaving), value.ToString());
            }
        }
        public bool B_MasterkeyAskForConfigSettings
        {
            get => _b_MasterkeyAskForConfigSettings;
            set
            {
                SetProperty(ref _b_MasterkeyAskForConfigSettings, value);
                Settings.SaveSetting(nameof(B_MasterkeyAskForConfigSettings), value.ToString());
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
            B_LockingIncludeQuickBar = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeQuickBar)).HasValue ?
                Settings.GetSettingValue<bool>(nameof(B_LockingIncludeQuickBar)).Value : false;

            B_LockingIncludeHomeHub = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeHomeHub)).HasValue ?
                Settings.GetSettingValue<bool>(nameof(B_LockingIncludeHomeHub)).Value : false;

            B_LockingIncludeAll = Settings.GetSettingValue<bool>(nameof(B_LockingIncludeAll)).HasValue ?
                Settings.GetSettingValue<bool>(nameof(B_LockingIncludeAll)).Value : false;

            B_MasterkeyAskForSaving = Settings.GetSettingValue<bool>(nameof(B_MasterkeyAskForSaving)).HasValue ?
                Settings.GetSettingValue<bool>(nameof(B_MasterkeyAskForSaving)).Value : false;

            B_MasterkeyAskForConfigSettings = Settings.GetSettingValue<bool>(nameof(B_MasterkeyAskForConfigSettings)).HasValue ?
                Settings.GetSettingValue<bool>(nameof(B_MasterkeyAskForConfigSettings)).Value : false;
        }
    }
}
