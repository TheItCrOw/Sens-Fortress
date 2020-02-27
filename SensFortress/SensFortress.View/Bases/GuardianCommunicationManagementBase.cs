using SensFortress.Data.Database;
using SensFortress.Guardian;
using SensFortress.Guardian.Bases;
using SensFortress.Guardian.Models;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using SensFortress.View.Helper;
using SensFortress.View.Main.ViewModel.HomeSubVms;
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
            GuardianController.GuardianStopped += Guardian_Stopped;
            GuardianController.GuardianStarted += Guardian_Started;
        }

        /// <summary>
        /// Launches/Restarts the guardian
        /// </summary>
        public bool LaunchGuardian() => GuardianController.LaunchGuardian(Settings.GetSettingsForGuardian(), BuildGuardianParams());

        /// <summary>
        /// Stops the guardian
        /// </summary>
        /// <returns></returns>
        public bool StopGuardian(bool supressMessage = false) => GuardianController.StopGuardian(supressMessage);

        /// <summary>
        /// Builds a parameter package the guardian can work with
        /// </summary>
        /// <returns></returns>
        private object[] BuildGuardianParams()
        {
            return new object[1]
            {
                CurrentFortressData.FullPath
            };
        }

        /// <summary>
        /// Makes the guardian reload the settings for updated properties.
        /// </summary>
        /// <param name="tasks"></param>
        public void ReloadGuardianTasks() => GuardianController.ReloadTasks(Settings.GetSettingsForGuardian());

        /// <summary>
        /// Events that triggers, when the guardian has been stopped.
        /// </summary>
        /// <param name="message"></param>
        protected void Guardian_Stopped(string message)
        {
            // Always use dispatcher when handling tasks from outside => you cant know what thread the caller is on.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logger.log.Error($"Guardian has been stopped: {message}");
                TaskLogger.Instance.Track(message);
                // Change that later to a guardian info box
                Communication.InformUser(message);
            });
        }

        /// <summary>
        /// Fires, when the guardian has been started.
        /// </summary>
        private void Guardian_Started()
        {
            // Always use dispatcher when handling tasks from outside => you cant know what thread the caller is on.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logger.log.Error($"Started Guardian.");
                TaskLogger.Instance.Track("Guardian has been launched.");
                // Change that later to a guardian info box
            });
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
                Logger.log.Info($"{handledTask.Name} started execution at {DateTime.Now}.");

                if (handledTask is ScheduledConfig config)
                {
                    Navigation.SettingsMangementInstance.ReloadSettings(); // Call this before raising the task!
                    var raisedTask = Settings.RaiseHandledSetting(handledTask);
                    // Add the raisedTask. The old version has already been removed by the guardian when he handled the task.
                    GuardianController.AddTask(raisedTask);
                    // Update the hash
                    SecurityParameterProvider.Instance.UpdateHash(nameof(Settings), IOPathHelper.GetSettingsFile());
                }
            });
        }

        /// <summary>
        /// Event that raises when the guardian threw an exception.
        /// </summary>
        /// <param name="ex"></param>
        protected void Guardian_ThrewExecption(Exception ex, string description)
        {
            Logger.log.Error($"Guardian threw an exception: {ex}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Change that later to a guardian info box
                Communication.InformUser(ex.Message);
            });
        }

        /// <summary>
        /// Event that raises when the Guardian wants a request to be executed.
        /// Let the guardian do the exception handling.
        /// </summary>
        /// <param name="request"></param>
        protected void Guardian_Request(RequestTypes request)
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                switch (request)
                {
                    case RequestTypes.Save:
                        Navigation.HomeManagementInstance.SaveTreeChangesCommand.Execute();
                        break;
                    case RequestTypes.Backup:
                        // Backup the fortress. We "split" since we only want the path value.
                        DataAccessService.Instance.BackupFortress(Settings.GetSettingValue<string>("DIP_AutomaticBackupIntervall").Split(',')[2]);
                        TaskLogger.Instance.Track("Guardian backed up fortress successfully!");
                        break;
                    case RequestTypes.Scan:
                        Navigation.HomeManagementInstance?.GetCurrentHubViewModel().QuickScanFortressCommand.Execute();
                        TaskLogger.Instance.Track("Guardian scanned the fortress.");
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
