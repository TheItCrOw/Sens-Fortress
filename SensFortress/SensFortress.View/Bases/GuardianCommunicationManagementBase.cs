using SensFortress.Guardian;
using SensFortress.Guardian.Bases;
using SensFortress.Guardian.Models;
using SensFortress.Utility;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SensFortress.View.Bases
{
    /// <summary>
    /// "Interface" to communicate with the Guardian Events.
    /// </summary>
    public class GuardianCommunicationManagementBase : ViewModelManagementBase
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

        public GuardianCommunicationManagementBase()
        {
            GuardianController.GuardianHandledTask += Guardian_HandledTask;
        }

        /// <summary>
        /// Event that raises, when a task has been handled by the guardian.
        /// </summary>
        /// <param name="handledTask"></param>
        protected void Guardian_HandledTask(GuardianTask handledTask)
        {
            // Always use dispatcher when handling tasks from outside => you cant know what thread the caller is on.
            Application.Current.Dispatcher.Invoke(() =>
            {
                TaskLogger.Instance.Track($"Test: {handledTask.Gtid} has been handled!");
                // Testing
                if (handledTask is ScheduledConfig config)
                {
                    var value = $"{((DateTime)config.Parameters[0]).AddHours(1)}, {config.Parameters[1]},{config.Parameters[2]}";
                    Settings.SaveSetting(config.Name, value);
                }
            });
        }
    }
}
