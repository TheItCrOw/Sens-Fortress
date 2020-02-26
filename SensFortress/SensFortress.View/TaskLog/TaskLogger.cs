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
        #region lazy behaviour
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
        #endregion

        public delegate void TaskLoggerEntryAddedEvent(string message);
        /// <summary>
        /// Raises, when a new entry has been added to the task logger.
        /// </summary>
        public event TaskLoggerEntryAddedEvent TaskLoggerEntryAdded;

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

        /// <summary>
        /// Tracks a message into the Task-Logger
        /// </summary>
        /// <param name="message"></param>
        public void Track(string message) => TaskLoggerEntryAdded?.Invoke(message);

    }
}
