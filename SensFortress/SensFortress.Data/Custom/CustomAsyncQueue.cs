using SensFortress.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensFortress.Data.Custom
{
    public class CustomAsyncQueue<T> : IEnumerable
    {

        private Queue<ISerializable> _queue = new Queue<ISerializable>(); 

        public IEnumerator GetEnumerator() => _queue.GetEnumerator();

        public Task<ISerializable> Enqueue(ISerializable model)
        {
            _queue.Enqueue(model);
            return Enqueue(model);
        }

        public Task<ISerializable> Dequeue()
        {
            _queue.TryDequeue(out var model);
            return Dequeue();
        }

    }
}
