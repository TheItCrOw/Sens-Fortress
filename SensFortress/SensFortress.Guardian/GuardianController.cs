using SensFortress.Guardian.Bases;
using SensFortress.Guardian.Events;
using SensFortress.Guardian.Exceptions;
using SensFortress.Guardian.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SensFortress.Guardian
{
    /// <summary>
    /// The CronJob that handles configs, settings, scans, etc. It always runs in the background.
    /// </summary>
    public static class GuardianController
    {        
        public delegate void GuardianHandledTaskEvent();
        /// <summary>
        /// Fires, when the guardian has handled a task.
        /// </summary>
        public static event GuardianHandledTaskEvent GuardianHandledTask;
        /// <summary>
        ///stores, how much time may differ between now and  a task to be handled. e.g. 10 equals: 
        /// We already prepare to handle the task that is being scheduled in 10 Minutes.
        /// </summary>
        private const int HANDLE_NEXT_TASK_MINUTES = 10;
        private static Thread _guardianThread;
        private static bool _guardianIsRunning;

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
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// The continous thread that stays in the background and completes task after task.
        /// </summary>
        private static void GuardianThread()
        {
            try
            {
                while (_guardianIsRunning)
                {
                    CheckUpcomingTasks();

                    if (_upcomingTasks.Count > 0)
                    {
                        // Take the next possible task which
                        var nextTask = _upcomingTasks.FirstOrDefault(t => t.ExecutionDate == DateTime.Now);
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
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
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
                        if (File.Exists(UtilityParameters.FortressPath))
                            File.Copy(UtilityParameters.FortressPath, (string)config.Parameters[2]);
                        GuardianHandledTask?.Invoke();
                        break;
                    case "DI_AutomaticScans":
                        //not yet implemented
                        break;
                    case "DI_AutomaticSaves":
                        // not yet implemented
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if there are any upcomingly scheduled tasks to complete.
        /// </summary>
        private static void CheckUpcomingTasks()
        {
            // This gets the Ids from the task pool that are being scheduled in the next HANDLE_NEXT_TASK_MINUTES (e.g. 10) minutes or less.
            var nextTasksIds = _taskPool.Keys.Where(k => IsUpcomingDate(k.ExecutionDate)).Select(t => t);
            // INFORM THE USER THAT A CONFIG IS BEING SCHEDULED IN THE NEXT 10 MINUTES!
            // Add them to new upcoming tasks list.
            foreach (var taskId in nextTasksIds)
                _upcomingTasks.Add(taskId);
        }

        /// <summary>
        /// Takes in a scheduled date and returns true, if that date is scheduled within the next HANDLE_NEXT_TASK_MINUTES.
        /// </summary>
        /// <param name="executionDate"></param>
        /// <returns></returns>
        private static bool IsUpcomingDate(DateTime executionDate)
        {
            if (executionDate.Year == DateTime.Now.Year &&
                executionDate.Month == DateTime.Now.Month &&
                executionDate.Day == DateTime.Now.Day &&
                executionDate.Minute - DateTime.Now.Minute < HANDLE_NEXT_TASK_MINUTES)
                return true;
            return false;
        }

    }
}
