using SensFortress.Utility;
using SensFortress.Utility.Log;
using SensFortress.View.Main.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SensFortress.View.TaskLog
{
    /// <summary>
    /// The logger that displays in the HomeView for the User. Use it carefully.
    /// </summary>
    public sealed class TaskLogger
    {
        /// <summary>
        /// Implement the lazy pattern.
        /// </summary>
        private static readonly Lazy<TaskLogger>
            lazy =
            new Lazy<TaskLogger>
            (() => new TaskLogger());

        /// <summary>
        /// Instance of the <see cref="Factory"/> class.
        /// </summary>
        public static TaskLogger Instance { get { return lazy.Value; } }

        /// <summary>
        /// For communcating with the current HomeVm.
        /// </summary>
        private HomeViewModel _currentHome;

        public void SetHomeView(HomeViewModel homeVm)
        {
            _currentHome = homeVm;
            // Subscribe to locked event
            CurrentFortressData.FortressLockedStatusChanged += IsLocked_Changed;
        }

        /// <summary>
        /// Track when the locked status changes.
        /// </summary>
        public void IsLocked_Changed()
        {
            if (CurrentFortressData.IsLocked)
                Track("The fortress has been locked.");
            else
                Track("The fortress has been unlocked.");
        }

        public void Track(string message)
        {
            if (_currentHome == null)
            {
                Logger.log.Error("CurrentHome of TaskLogger hasn't been set.");
                return;
            }

            Application.Current.Dispatcher.Invoke(() => _currentHome.TaskLogs.Add($"{DateTime.Now.ToShortTimeString()}: {message}"));
        }

    }
}
