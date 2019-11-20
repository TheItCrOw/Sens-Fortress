using SensFortress.Models.BaseClasses;
using SensFortress.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensFortress.Data.Custom
{
    /// <summary>
    /// Async Queue for the factory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FactoryQueue<T1, T2> : IEnumerable
    {
        private protected ConcurrentQueue<Tuple<T1, T2>> _queue = new ConcurrentQueue<Tuple<T1, T2>>();

        /// <summary>
        /// Authorize and enqueue a tuple async.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async void Enqueue(Tuple<T1, T2> task)
        {
            await Task.Run(() =>
            {
                var authorizedTask = AuthorizeEnqeue(task);
                _queue.Enqueue(authorizedTask);
            });

        }

        /// <summary>
        /// Dequeue an authorized tuple async.
        /// </summary>
        /// <returns></returns>
        public async void Dequeue()
        {
            await Task.Run(() =>
            {
                _queue.TryPeek(out var task);
                AuthorizeDequeue(task);
                _queue.TryDequeue(out var authorizedTask);
            });
        }

        /// <summary>
        /// Authorize a given model before enqueuing it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Tuple<T1, T2> AuthorizeEnqeue(Tuple<T1, T2> task)
        {
            // Implement the rest later
            return task;                
        }

        /// <summary>
        /// Authorizes a given model before dequeueing it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Tuple<T1, T2> AuthorizeDequeue(Tuple<T1, T2> task)
        {
            if (task.Item1 == null || task.Item2 == null || task == null)
                throw new NullReferenceException($"FactoryQueue found empty data:{Environment.NewLine}" +
                    $"{task}" +
                    $"{task.Item1}" +
                    $"{task.Item2}.");

            return task;
        }

        public int Count => _queue.Count;
        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}
