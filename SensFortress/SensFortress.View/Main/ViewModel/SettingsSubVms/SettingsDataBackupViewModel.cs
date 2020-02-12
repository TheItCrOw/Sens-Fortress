using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel.SettingsSubVms
{
    /// <summary>
    /// Interaction logic for DataBackup sub settings view.
    /// </summary>
    public class SettingsDataBackupViewModel : ViewModelManagementBase
    {
        private bool _isLocked;

        public override bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        public SettingsDataBackupViewModel()
        {

        }

    }
}
