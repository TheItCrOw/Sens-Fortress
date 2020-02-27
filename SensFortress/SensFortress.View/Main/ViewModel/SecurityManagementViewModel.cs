using SensFortress.Guardian;
using SensFortress.Guardian.Models;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SensFortress.View.Main.ViewModel
{
    public class SecurityManagementViewModel : ViewModelManagementBase
    {
        private bool _isLocked;

        public ObservableCollection<GuardianLogEntry> GuardianLogs { get; set; } = new ObservableCollection<GuardianLogEntry>();

        public override bool IsLocked 
        { 
            get => _isLocked; 
            set => SetProperty(ref _isLocked, value);
        }

        /// <summary>
        /// Use this instead of ctor for more control over when it's called.
        /// </summary>
        public void Initialize()
        {

        }

        public SecurityManagementViewModel()
        {
            GuardianLogger.Instance.GuardianLogEntryAdded += GuardianLogger_EntryAdded;
        }

        private void GuardianLogger_EntryAdded(GuardianLogEntry entry) => Application.Current.Dispatcher?.Invoke(() => GuardianLogs.Add(entry));
    }
}
