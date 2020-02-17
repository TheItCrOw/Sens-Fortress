using SensFortress.Data.Database;
using SensFortress.Guardian;
using SensFortress.Guardian.Bases;
using SensFortress.Guardian.Models;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using SensFortress.View.Helper;
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
            GuardianController.GuardianThrewException += Guardian_ThrewExecption;
            GuardianController.GuardianRequest += Guardian_Request;
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
                TaskLogger.Instance.Track(handledTask.Description);
                Logger.log.Info($"{handledTask.Name} has been executed at {DateTime.Now}.");

                if (handledTask is ScheduledConfig config)
                {
                    Navigation.SettingsMangementInstance.ReloadSettings(); // Call this before raising the task!
                    var raisedTask = Settings.RaiseHandledSetting(handledTask);
                    // Add the raisedTask. The old version has already been removed by the guardian when he handled the task.
                    GuardianController.AddTask(raisedTask);
                }
            });
        }

        /// <summary>
        /// Event that raises when the guardian threw an exception.
        /// </summary>
        /// <param name="ex"></param>
        protected void Guardian_ThrewExecption(Exception ex)
        {
            Logger.log.Error($"Guardian threw an exception: {ex}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Testing
                Communication.InformUser(ex.Message);
            });
        }

        /// <summary>
        /// Event that raises when the Guardian wants a request to be executed.
        /// </summary>
        /// <param name="request"></param>
        protected void Guardian_Request(RequestTypes request)
        {
            if (request == RequestTypes.Save)
                Navigation.HomeManagementInstance.SaveTreeChangesCommand.Execute();
        }
    }
}
