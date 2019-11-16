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

        /// <summary>
        /// CreationQueue of the <see cref="ModelFactory"/>. Entities in it will be build one after another.
        /// </summary>
        private static FactoryQueue<Type, object> CreationQueue = new FactoryQueue<Type, object>();

        /// <summary>
        /// Authorize the data and put it at the end of the <see cref="ModelFactory"/> queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public async static void AddToFactoryQueue<T>(object model)
        {
            var authorizedModelTuple = await CreationQueue.Enqueue(Tuple.Create(typeof(T), model));
        }

        /// <summary>
        /// Dequeue the last authorized data in the <see cref="ModelFactory"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static async Tuple<Type, object> DequeueFromFactory<T>()
        {
            return await CreationQueue.Dequeue();
        }

    }
}
