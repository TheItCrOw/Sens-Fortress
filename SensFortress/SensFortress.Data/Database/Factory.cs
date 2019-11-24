using SensFortress.Data.Custom;
using SensFortress.Models.Fortress;
using SensFortress.Models.Interfaces;
using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensFortress.Data.Database
{
    /// <summary>
    /// Builds models out of data async.
    /// </summary>
    public sealed class Factory
    {
        #region Enums
        public enum FactoryTaskType
        {
            [Description("Task when building known models.")]
            Build,
            [Description("Task when creating new models.")]
            Create,
            [Description("Task when reading data.")]
            Read,
            [Description("Task when saving data.")]
            Save,
            [Description("Task when loading integrated data.")]
            Load,
        }
        #endregion

        #region Implement Lazy behaviour
        /// <summary>
        /// Implement the lazy pattern.
        /// </summary>
        private static readonly Lazy<Factory>
            lazy =
            new Lazy<Factory>
            (() => new Factory());

        /// <summary>
        /// Instance of the <see cref="Factory"/> class.
        /// </summary>
        public static Factory Instance { get { return lazy.Value; } }
        #endregion

        /// <summary>
        /// <see cref="_factoryQueue"/> of the <see cref="Factory"/>. Every defined task given to it will be exceuted.
        /// </summary>
        private FactoryQueue<FactoryTaskType, object[]> _factoryQueue;

        private XmlDataCache _xmlDatacache;

        /// <summary>
        /// Starts the <see cref="FactoryQueue{T1, T2}"/>. If called when queue has already been built, it resets the queue.
        /// </summary>
        public void StartFactoryQueue(string fortressPath)
        {
            _factoryQueue = new FactoryQueue<FactoryTaskType, object[]>();
            _xmlDatacache = new XmlDataCache(fortressPath);

            Thread continousThread = new Thread(new ThreadStart(ContinousThread));
            continousThread.IsBackground = true;
            continousThread.Name = "Factory queue executing thread.";
            continousThread.Start();
        }

        /// <summary>
        /// Authorize the task and put it at the end of the <see cref="FactoryQueue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public void EnqueueTask(FactoryTaskType taskType, object[] param) => _factoryQueue.Enqueue(Tuple.Create(taskType, param));

        /// <summary>
        /// Dequeue the last authorized task in the <see cref="FactoryQueue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private Tuple<FactoryTaskType, object[]> DequeueTask() => _factoryQueue.Dequeue().Result; 

        /// <summary>
        /// The continous thread that checks in a specific intervall if there are any unfinished tasks in the queue.
        /// </summary>
        private void ContinousThread()
        {
            while (true)
            {
                Trace.WriteLine($"[{DateTime.Now}]: Current unfinished tasks: {_factoryQueue.Count}");
                if(_factoryQueue.Count > 0)
                {
                    var currentTask = DequeueTask();
                    Trace.WriteLine($"[{DateTime.Now}]: Handling a {currentTask.Item1} operation.");
                    if(HandleTask(currentTask))
                    {
                        //Continue only when the task is complete.
                    }
                }
                Thread.Sleep(100);
            }
        }

        private bool HandleTask(Tuple<FactoryTaskType, object[]> task)
        {
            switch (task.Item1)
            {
                case FactoryTaskType.Build:
                    // Have to later consider other models than fortresses
                    _xmlDatacache.BuildFortress((string)task.Item2[0], (string)task.Item2[1], (string)task.Item2[2]);
                    break;
                case FactoryTaskType.Create:

                    // Have to later consider other models than fortresses
                    _xmlDatacache.CreateNewFortress((Fortress)task.Item2[0]);
                    return true;
                case FactoryTaskType.Read:
                    break;
                case FactoryTaskType.Save:
                    break;
                case FactoryTaskType.Load:
                    break;
                default:
                    break;
            }

            return false;
        }

    }
}
