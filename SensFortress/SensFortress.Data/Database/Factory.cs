using SensFortress.Data.Custom;
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
            [Description("Task when creating models.")]
            Build,
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
        private FactoryQueue<FactoryTaskType, object> _factoryQueue;

        /// <summary>
        /// Starts the <see cref="FactoryQueue{T1, T2}"/>. If called when queue has already been built, it resets the queue.
        /// </summary>
        public void StartFactoryQueue()
        {
            _factoryQueue = new FactoryQueue<FactoryTaskType, object>();

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
        public void AddTask<T>(FactoryTaskType taskType, object param) => _factoryQueue.Enqueue(Tuple.Create(taskType, param));

        /// <summary>
        /// Dequeue the last authorized task in the <see cref="FactoryQueue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DequeueTask() => _factoryQueue.Dequeue();

        /// <summary>
        /// The continous thread that checks in a specific intervall if there are any unfinished tasks in the queue.
        /// </summary>
        private void ContinousThread()
        {
            while (true)
            {
                Trace.WriteLine("Test");
                Trace.WriteLine(_factoryQueue.Count);
                Thread.Sleep(100);
            }
        }

    }
}
