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
        public async Task<Tuple<T1, T2>> Enqueue(Tuple<T1, T2> model)
        {
            Tuple<T1, T2> authorizedModel = null;
            await Task.Run(() =>
            {
                authorizedModel = AuthorizeEnqeue(model);
                _queue.Enqueue(authorizedModel);
            });

            return authorizedModel;
        }

        /// <summary>
        /// Dequeue an authorized tuple async.
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<T1, T2>> Dequeue()
        {
            Tuple<T1, T2> authorizedModel = null;
            await Task.Run(() =>
            {
                _queue.TryDequeue(out var model);
                AuthorizeDequeue(model);
                authorizedModel = model;
            });

            return authorizedModel;
        }

        /// <summary>
        /// Authorize a given model before enqueuing it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Tuple<T1, T2> AuthorizeEnqeue(Tuple<T1, T2> model)
        {
            // Implement the rest later
            return model;                
        }

        /// <summary>
        /// Authorizes a given model before dequeueing it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Tuple<T1, T2> AuthorizeDequeue(Tuple<T1, T2> model)
        {
            if (model.Item1 == null || model.Item2 == null || model == null)
                throw new NullReferenceException($"FactoryQueue found empty data:{Environment.NewLine}" +
                    $"{model}" +
                    $"{model.Item1}" +
                    $"{model.Item2}.");

            return model;
        }

        public int Count => _queue.Count;
        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}
