using SensFortress.Guardian.Bases;
using SensFortress.Guardian.Exceptions;
using SensFortress.Guardian.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SensFortress.Guardian
{
    /// <summary>
    /// List of all possible request types the guardian may ask the View
    /// </summary>
    public enum RequestTypes
    {
        Save,
        Backup,
        Scan
    }
    /// <summary>
    /// The CronJob that handles configs, settings, scans, etc. It always runs in the background.
    /// </summary>
    public static class GuardianController
    {
        public delegate void GuardianHandledTaskEvent(GuardianTask handledTask);
        public delegate void GuardianThrewExceptionEvent(Exception ex, string description);
        public delegate void GuardianRequestEvent(RequestTypes request);
        public delegate void GuardianStoppedEvent(string reason);
        public delegate void GuardianStartedEvent();
        /// <summary>
        /// Fires, when the guardian has handled a task.
        /// </summary>
        public static event GuardianHandledTaskEvent GuardianHandledTask;
        public static event GuardianThrewExceptionEvent GuardianThrewException;
        public static event GuardianRequestEvent GuardianRequest;
        public static event GuardianStoppedEvent GuardianStopped;
        public static event GuardianStartedEvent GuardianStarted;

        /// <summary>
        /// Stores, how much time may differ between now and a task to be handled. e.g. 10 equals: 
        /// We already prepare to handle the task that is being scheduled in 10 Minutes.
        /// </summary>
        private const int HANDLE_NEXT_TASK_MINUTES = 10;
        private static Thread _guardianThread;
        private static bool _guardianIsRunning;
        private static readonly object _lock = new object();

        private static ConcurrentDictionary<GTIdentifier, GuardianTask> _taskPool;
        private static HashSet<GTIdentifier> _upcomingTasks;

        /// <summary>
        /// Adds a task to the task pool of the guardian.
        /// </summary>
        /// <param name="task"></param>
        public static void AddTask(GuardianTask task)
        {
            if (_taskPool == null)
                throw new GuardianException($"The guardian has not been launched yet!");

            if (!_taskPool.TryAdd(task.Gtid, task))
                throw new GuardianException($"Given task can not be added into the task-pool. The Gtid must be unique.");
        }

        /// <summary>
        /// Removes a task from the <see cref="_taskPool"/>
        /// </summary>
        /// <param name="taskId"></param>
        public static void RemoveTask(GTIdentifier taskId)
        {
            if (_taskPool == null)
                throw new GuardianException($"The guardian has not been launched yet!");

            if (!_taskPool.TryRemove(taskId, out var task))
                throw new GuardianException($"Given task can not be removed from the task-pool.");
        }

        /// <summary>
        /// Launches the guardian Thread and overwrites a possible existing one.
        /// </summary>
        /// <returns></returns>
        public static bool LaunchGuardian(HashSet<GuardianTask> initialTasks, object[] parameters)
        {
            try
            {
                _taskPool = new ConcurrentDictionary<GTIdentifier, GuardianTask>();
                _upcomingTasks = new HashSet<GTIdentifier>();

                // Add the intial tasks
                foreach (var task in initialTasks)
                    AddTask(task);

                UtilityParameters.HandleFortressParameters(parameters);

                _guardianThread = new Thread(new ThreadStart(GuardianThread))
                {
                    Name = "GUARDIAN_THREAD",
                    IsBackground = true
                };
                _guardianIsRunning = true; // Call this before starting the thread!
                _guardianThread.Start();
                GuardianStarted?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                GuardianThrewException?.Invoke(ex, "Couldn't launch the guardian.");
                return false;
            }
        }

        /// <summary>
        /// Stops the gurdian.
        /// </summary>
        /// <returns></returns>
        public static bool StopGuardian(bool supressMessage = false)
        {
            try
            {
                _guardianIsRunning = false;
                _taskPool.Clear();
                _upcomingTasks.Clear();
                if (!supressMessage)
                    GuardianStopped?.Invoke("Guardian has been manually stopped.");
                return true;
            }
            catch (Exception ex)
            {
                GuardianThrewException?.Invoke(ex, "Couldn't stop the guardian.");
                return false;
            }
        }

        /// <summary>
        /// Makes the guardian reloads it's task pool with the given List
        /// </summary>
        public static void ReloadTasks(HashSet<GuardianTask> initialTasks)
        {
            try
            {
                _taskPool.Clear();

                foreach (var task in initialTasks)
                    AddTask(task);
            }
            catch (Exception ex)
            {
                GuardianThrewException?.Invoke(ex, "There was an error reloading your scheduled configs.");
            }
        }

        /// <summary>
        /// The continous thread that stays in the background and completes task after task.
        /// </summary>
        private static void GuardianThread()
        {
            lock (_lock)
            {
                try
                {
                    while (_guardianIsRunning)
                    {
                        CheckUpcomingTasks();

                        if (_upcomingTasks.Count > 0)
                        {
                            // Take the next possible task
                            var nextTask = _upcomingTasks.FirstOrDefault(t => t.ExecutionDate <= DateTime.Now);
                            if (nextTask != default)
                            {
                                //Handle the task now!
                                if (_taskPool.TryGetValue(nextTask, out var task))
                                    HandleTask(task);
                                else
                                    ;// Inform someone that a task is missing.
                            }
                        }
                        // Maybe let the user decide the frequency of the Guardian later.
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    GuardianStopped?.Invoke($"The guardian detected an error and has been shut down. " +
                        $"After the error has been fixed, try to restart him manually via the Home-Hub. {Environment.NewLine}" +
                        $"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handles and executes a task.
        /// </summary>
        /// <param name="task"></param>
        private static void HandleTask(GuardianTask task)
        {
            if (task is ScheduledConfig config)
            {
                // This is obv not gerenic. Maybe if the settings are getting way more, we could be more generic here.
                switch (config.Name)
                {
                    case "DIP_AutomaticBackupIntervall":
                        GuardianRequest?.Invoke(RequestTypes.Backup);
                        break;
                    case "DI_AutomaticScans":
                        GuardianRequest?.Invoke(RequestTypes.Scan);
                        break;
                    case "DI_AutomaticSaves":
                        GuardianRequest?.Invoke(RequestTypes.Save);
                        break;
                    default:
                        throw new GuardianException($"{task.Name} could not be handled.");
                }
                // Task has been handled => Inform the View and delete this instance.
                _taskPool.Remove(config.Gtid, out var obsolete);
                _upcomingTasks.Remove(config.Gtid);
                GuardianHandledTask?.Invoke(config);
            }
        }

        /// <summary>
        /// Checks if there are any upcomingly scheduled tasks to complete.
        /// </summary>
        private static void CheckUpcomingTasks()
        {
            // This gets the Ids from the task pool that are being scheduled in the next HANDLE_NEXT_TASK_MINUTES (e.g. 10) minutes or less.
            var nextTasksIds = _taskPool.Keys.Where(k => IsUpcomingDate(k)).Select(t => t);
            // INFORM THE USER THAT A CONFIG IS BEING SCHEDULED IN THE NEXT 10 MINUTES!
            // Add them to new upcoming tasks list.
            foreach (var taskId in nextTasksIds)
                _upcomingTasks.Add(taskId);
        }

        /// <summary>
        /// Filters the exectuion date: Foreach interval the day must be today. Then see if the exectuionTime is still less than today, even when the interval has been added.
        /// </summary>
        /// <param name="executionDate"></param>
        /// <returns></returns>
        private static bool IsUpcomingDate(GTIdentifier task)
        {
            if (task.ExecutionDate <= DateTime.Now)
            {
                switch (task.Interval.Trim())
                {
                    case "Hourly":
                        if (task.ExecutionDate.TimeOfDay <= DateTime.Now.TimeOfDay)
                            return true;
                        return false;
                    case "Daily":
                        if (task.ExecutionDate.TimeOfDay <= DateTime.Now.TimeOfDay)
                            return true;
                        return false;
                    case "Weekly":
                        if (task.ExecutionDate.Date < DateTime.Now.Date)
                            return true;
                        else if (task.ExecutionDate.Date == DateTime.Now.Date && task.ExecutionDate.TimeOfDay <= DateTime.Now.TimeOfDay)
                            return true;
                        return false;
                    case "Monthly":
                        if (task.ExecutionDate.Date < DateTime.Now)
                            return true;
                        else if (task.ExecutionDate.Date == DateTime.Now.Date && task.ExecutionDate.TimeOfDay <= DateTime.Now.TimeOfDay)
                            return true;
                        return false;
                    case "Yearly":
                        if (task.ExecutionDate.Date < DateTime.Now)
                            return true;
                        else if (task.ExecutionDate.Date == DateTime.Now.Date && task.ExecutionDate.TimeOfDay <= DateTime.Now.TimeOfDay)
                            return true;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }

    }
}
