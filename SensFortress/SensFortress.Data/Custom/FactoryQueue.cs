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
        public int Count => _queue.Count;
        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
        /// <summary>
        /// Authorize and enqueue a task async.
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
        /// Dequeue an authorized task async.
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<T1, T2>> Dequeue()
        {
            Tuple<T1, T2> authorizedTask = null;
            await Task.Run(() =>
            {
                _queue.TryPeek(out var task);
                AuthorizeDequeue(task);
                _queue.TryDequeue(out authorizedTask);
            });
            return authorizedTask;
        }

        /// <summary>
        /// Authorize a given model before enqueuing it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Tuple<T1, T2> AuthorizeEnqeue(Tuple<T1, T2> task)
        {
            if (task.Item1 == null || task.Item2 == null || task == null)
                throw new NullReferenceException($"FactoryQueue found empty data:{Environment.NewLine}" +
                    $"{task}" +
                    $"{task.Item1}" +
                    $"{task.Item2}.");

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

    }
}
