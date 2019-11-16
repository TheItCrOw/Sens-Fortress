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
    public sealed class ModelFactory
    {
        #region Implement Lazy behaviour
        /// <summary>
        /// Implement the lazy pattern.
        /// </summary>
        private static readonly Lazy<ModelFactory>
            lazy =
            new Lazy<ModelFactory>
            (() => new ModelFactory());

        /// <summary>
        /// Instance of the <see cref="ModelFactory"/> class.
        /// </summary>
        public static ModelFactory Instance { get { return lazy.Value; } }
        #endregion
        /// <summary>
        /// <see cref="FactoryQueue"/> of the <see cref="ModelFactory"/>. Entities in it will be build one after another.
        /// </summary>
        private FactoryQueue<Type, object> FactoryQueue = new FactoryQueue<Type, object>();

        /// <summary>
        /// Authorize the data and put it at the end of the <see cref="FactoryQueue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public async void AddToFactoryQueue<T>(object model)
        {
            var authorizedModelTuple = await FactoryQueue.Enqueue(Tuple.Create(typeof(T), model));
        }

        /// <summary>
        /// Dequeue the last authorized data in the <see cref="FactoryQueue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public async void DequeueFromFactory<T>()
        {
            await FactoryQueue.Dequeue();
        }

    }
}
