using Microsoft.Win32;
using Prism.Commands;
using SensFortress.Utility;
using SensFortress.View.Bases;
using System;
using System.Collections.ObjectModel;

namespace SensFortress.View.Main.ViewModel.SettingsSubVms
{
    /// <summary>
    /// Interaction logic for DataBackup sub settings view.
    /// </summary>
    public class SettingsDataBackupViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        private bool _b_AutomaticBackupIntervall;
        private SettingInterval _i_AutomaticBackupIntervall;
        private DateTime _d_AutomaticBackupIntervall;
        private string _p_AutomaticBackupIntervall;
        public DelegateCommand SaveSettingsCommand => new DelegateCommand(SaveSettings);
        public DelegateCommand ChooseBackupPathCommand => new DelegateCommand(ChooseBackupPath);
        /// <summary>
        /// I know that binding in XAML to a enum in ItemSource is possible,... It didnt work here, so I hack my way through it.
        /// </summary>
        public ObservableCollection<SettingInterval> SettingIntervals { get; set; } = new ObservableCollection<SettingInterval>()
        {
            SettingInterval.Hourly,
            SettingInterval.Daily,
            SettingInterval.Weekly,
            SettingInterval.Monthly,
            SettingInterval.Yearly
        };

        /// => These 5 are one setting: Backup at given interval to path
        public bool B_AutomaticBackupIntervall
        {
            get => _b_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _b_AutomaticBackupIntervall, value);
            }
        }
        public SettingInterval I_AutomaticBackupIntervall
        {
            get => _i_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _i_AutomaticBackupIntervall, value);
            }
        }
        public DateTime D_AutomaticBackupIntervall
        {
            get => _d_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _d_AutomaticBackupIntervall, value);
            }
        }
        public string P_AutomaticBackupIntervall
        {
            get => _p_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _p_AutomaticBackupIntervall, value);
            }
        }
        private string DIP_AutomaticBackupIntervall()
        {
            if (!B_AutomaticBackupIntervall)
                return "Void";
            else
                return $"{D_AutomaticBackupIntervall}, {I_AutomaticBackupIntervall}, {P_AutomaticBackupIntervall}";
        }
        // <=

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
        /// Lets the user choose a path where he wants his backups to stay.
        /// </summary>
        private void ChooseBackupPath()
        {
            var dlg = new SaveFileDialog();
            dlg.ShowDialog();

            if (dlg.FileName == string.Empty)
                return;

            P_AutomaticBackupIntervall = dlg.FileName;
        }

        /// <summary>
        /// Saves the settings to the configs file
        /// </summary>
        private void SaveSettings()
        {
            // Save automatic backup
            Settings.SaveSetting("DIP_AutomaticBackupIntervall", DIP_AutomaticBackupIntervall());
        }

        /// <summary>
        /// Loads the settings form the config
        /// </summary>
        private void LoadSettings()
        {
            // Backup
            string dip_AutomaticBackup = Settings.GetSettingValue<string>("DIP_AutomaticBackupIntervall");

            if (dip_AutomaticBackup == default)
            {
                B_AutomaticBackupIntervall = false;
                I_AutomaticBackupIntervall = SettingInterval.Daily;
                D_AutomaticBackupIntervall = DateTime.Now;
                P_AutomaticBackupIntervall = IOPathHelper.GetDesktopPath();
            }
            else
            {
                B_AutomaticBackupIntervall = true;
                var splited = dip_AutomaticBackup.Split(',');
                D_AutomaticBackupIntervall = Convert.ToDateTime(splited[0]);
                if (Enum.TryParse(splited[1], out SettingInterval interval))
                    I_AutomaticBackupIntervall = interval;
                P_AutomaticBackupIntervall = splited[2];
            }
        }

    }
}
