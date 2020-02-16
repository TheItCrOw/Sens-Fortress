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
        private bool _b_AutomaticScans;
        private SettingInterval _i_AutomaticScans;
        private DateTime _d_AutomaticScans;
        private bool _b_AutomaticSaves;
        private SettingInterval _i_AutomaticSaves;
        private DateTime _t_AutomaticBackupIntervall;

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
        public bool HasUnsavedChanges { get; set; }

        /// => These 5 are one setting: Backup at given interval to path
        public bool B_AutomaticBackupIntervall
        {
            get => _b_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _b_AutomaticBackupIntervall, value);
                HasUnsavedChanges = true;
            }
        }
        public SettingInterval I_AutomaticBackupIntervall
        {
            get => _i_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _i_AutomaticBackupIntervall, value);
                HasUnsavedChanges = true;
            }
        }
        public DateTime D_AutomaticBackupIntervall
        {
            get => _d_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _d_AutomaticBackupIntervall, value);
                HasUnsavedChanges = true;
            }
        }
        public DateTime T_AutomaticBackupIntervall
        {
            get => _t_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _t_AutomaticBackupIntervall, value);
                HasUnsavedChanges = true;
            }
        }
        public string P_AutomaticBackupIntervall
        {
            get => _p_AutomaticBackupIntervall;
            set
            {
                SetProperty(ref _p_AutomaticBackupIntervall, value);
                HasUnsavedChanges = true;
            }
        }
        private string DIP_AutomaticBackupIntervall()
        {
            if (!B_AutomaticBackupIntervall)
                return "Void";
            else
                return $"{D_AutomaticBackupIntervall},{I_AutomaticBackupIntervall},{P_AutomaticBackupIntervall}";
        }
        // <=

        // These 4 are one setting: Scan at given itnerval
        public bool B_AutomaticScans
        {
            get => _b_AutomaticScans;
            set
            {
                SetProperty(ref _b_AutomaticScans, value);
                HasUnsavedChanges = true;
            }
        }
        public SettingInterval I_AutomaticScans
        {
            get => _i_AutomaticScans;
            set
            {
                SetProperty(ref _i_AutomaticScans, value);
                HasUnsavedChanges = true;
            }
        }
        public DateTime D_AutomaticScans
        {
            get => _d_AutomaticScans;
            set
            {
                SetProperty(ref _d_AutomaticScans, value);
                HasUnsavedChanges = true;
            }
        }
        private string DI_AutomaticScans()
        {
            if (!B_AutomaticScans)
                return "Void";
            else
                return $"{D_AutomaticScans},{I_AutomaticScans}";
        }
        // <=

        // These 3 are on setting: Auto saves
        public bool B_AutomaticSaves
        {
            get => _b_AutomaticSaves;
            set
            {
                SetProperty(ref _b_AutomaticSaves, value);
                HasUnsavedChanges = true;
            }
        }
        public SettingInterval I_AutomaticSaves
        {
            get => _i_AutomaticSaves;
            set
            {
                SetProperty(ref _i_AutomaticSaves, value);
                HasUnsavedChanges = true;
            }
        }
        private string DI_AutomaticSaves()
        {
            if (!B_AutomaticSaves)
                return "Void";
            else // for atuo saving, we do not need a date. So jsut use the empty dateTime.
                return $"{DateTime.MinValue},{I_AutomaticSaves}";
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
            dlg.Filter = $"Fortress|*{TermHelper.GetZippedFileEnding()}";
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
            Settings.SaveSetting("DI_AutomaticScans", DI_AutomaticScans());
            Settings.SaveSetting("DI_AutomaticSaves", DI_AutomaticSaves());

            HasUnsavedChanges = false;
        }

        /// <summary>
        /// Loads the settings form the config
        /// </summary>
        private void LoadSettings()
        {
            LoadDIPAutomaticBackup();
            LoadDIAutomaticScans();
            LoadDIAutomaticSaves();

            HasUnsavedChanges = false;
        }
        private void LoadDIPAutomaticBackup()
        {
            // Backup
            var dip_AutomaticBackup = Settings.GetSettingValue<string>("DIP_AutomaticBackupIntervall");

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
        private void LoadDIAutomaticScans()
        {
            //Scan
            var di_AutomaticScans = Settings.GetSettingValue<string>("DI_AutomaticScans");
            if (di_AutomaticScans == default)
            {
                B_AutomaticScans = false;
                I_AutomaticScans = SettingInterval.Daily;
                D_AutomaticScans = DateTime.Now;
            }
            else
            {
                B_AutomaticScans = true;
                var splited = di_AutomaticScans.Split(',');
                D_AutomaticScans = Convert.ToDateTime(splited[0]);
                if (Enum.TryParse(splited[1], out SettingInterval interval))
                    I_AutomaticScans = interval;
            }
        }
        private void LoadDIAutomaticSaves()
        {
            //Scan
            var di_AutomaticSaves = Settings.GetSettingValue<string>("DI_AutomaticSaves");
            if (di_AutomaticSaves == default)
            {
                B_AutomaticSaves = false;
                I_AutomaticSaves = SettingInterval.Hourly;
            }
            else
            {
                B_AutomaticSaves = true;
                var splited = di_AutomaticSaves.Split(',');
                //D_AutomaticScans = Convert.ToDateTime(splited[0]); We do not need a date for autoamtic saving.
                if (Enum.TryParse(splited[1], out SettingInterval interval))
                    I_AutomaticSaves = interval;
            }
        }
    }
}
