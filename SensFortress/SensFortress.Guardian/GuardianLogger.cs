using SensFortress.Guardian.Bases;
using SensFortress.Guardian.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian
{
    /// <summary>
    /// Logger that tracks actions of the guardian. Subscribe to the <see cref="GuardianLogEntryAdded"/> event.
    /// </summary>
    public sealed class GuardianLogger
    {
        #region lazy behaviour
        /// <summary>
        /// Implement the lazy pattern.
        /// </summary>
        private static readonly Lazy<GuardianLogger>
            lazy =
            new Lazy<GuardianLogger>
            (() => new GuardianLogger());

        /// <summary>
        /// Instance of the <see cref="Factory"/> class.
        /// </summary>
        public static GuardianLogger Instance { get { return lazy.Value; } }
        #endregion

        public delegate void GuardianLogEntryAddedEvent(GuardianLogEntry entry);
        /// <summary>
        /// Raises, when the guardian enters a new log entry
        /// </summary>
        public event GuardianLogEntryAddedEvent GuardianLogEntryAdded;

        public GuardianLogger()
        {
            GuardianController.GuardianStarted += Log_GuardianStarted;
            GuardianController.GuardianStopped += Log_GuardianStopped;
            GuardianController.GuardianThrewException += Log_GuardianThrewEx;
            GuardianController.GuardianHandledTask += Log_GuardianHandledTask;
        }

        private void Log_GuardianStarted() => Log("Guardian started", "The guardian has been successfully launched.", EntryType.Success);
        private void Log_GuardianStopped(string message) => Log("Guardian stopped", message, EntryType.Danger);
        private void Log_GuardianThrewEx(Exception ex, string description) => Log("Guardian error", description, EntryType.Error);
        private void Log_GuardianHandledTask(GuardianTask task) => Log("Guardian executed a task", $"Executed: {task.Description}", EntryType.Success);

        /// <summary>
        /// Log a <see cref="GuardianLogEntry"/>
        /// </summary>
        /// <param name="entry"></param>
        public void Log(string name) => 
            GuardianLogEntryAdded?.Invoke(new GuardianLogEntry()
            {
                    Name = name,
                    Date = DateTime.Now
            });

        public void Log(string name, string description) =>
            GuardianLogEntryAdded?.Invoke(new GuardianLogEntry()
            {
                Name = name,
                Description = description,
                Date = DateTime.Now
            });

        public void Log(string name, string description, EntryType logType) =>
            GuardianLogEntryAdded?.Invoke(new GuardianLogEntry()
            {
                Name = name,
                Description = description,
                LogType = logType,
                Date = DateTime.Now
            });

    }
}
