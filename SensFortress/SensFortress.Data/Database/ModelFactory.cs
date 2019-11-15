using SensFortress.Data.Custom;
using SensFortress.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensFortress.Data.Database
{
    /// <summary>
    /// Builds models out of data async.
    /// </summary>
    public static class ModelFactory
    {
        private static object _lock = new object();
        private static CustomAsyncQueue<ISerializable> CreationQueue = new CustomAsyncQueue<ISerializable>();

        /// <summary>
        /// Authorize the data and put it at the end of the <see cref="ModelFactory"/> queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public static async void AddToQueue<T>(ISerializable model)
        {
            var authorizedModel = await CreationQueue.Enqueue(model);
        }

        /// <summary>
        /// Dequeue the last authorized data in the <see cref="ModelFactory"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static async void Dequeue<T>()
        {
            var authorizedModel = await CreationQueue.Dequeue();
        }

    }
}
