#region License
// This class was written by StackOverflow user ChaosPandion (https://stackoverflow.com/users/156142/chaospandion)
// and is licensed under CC BY-SA 3.0 ( http://creativecommons.org/licenses/by-sa/3.0/ ).
// https://stackoverflow.com/a/2564523/3427520
#endregion

using System;
using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// Represents a pool of objects with a size limit.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public sealed class ObjectPool<T> : IDisposable
        where T : new()
    {
        private readonly int size;
        private readonly object locker;
        private readonly Queue<T> queue;
        private int count;


        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="size">The size of the object pool.</param>
        public ObjectPool(int size)
        {
            if (size <= 0)
            {
                const string message = "The size of the pool must be greater than zero.";
                throw new ArgumentOutOfRangeException("size", size, message);
            }

            this.size = size;
            locker = new object();
            queue = new Queue<T>();
        }


        /// <summary>
        /// Retrieves an item from the pool. 
        /// </summary>
        /// <returns>The item retrieved from the pool.</returns>
        public T Get()
        {
            lock (locker)
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }

                count++;
                return new T();
            }
        }

        /// <summary>
        /// Places an item in the pool.
        /// </summary>
        /// <param name="item">The item to place to the pool.</param>
        public void Put(T item)
        {
            lock (locker)
            {
                if (count < size)
                {
                    queue.Enqueue(item);
                }
                else
                {
                    using (item as IDisposable)
                    {
                        count--;
                    }
                }
            }
        }

        /// <summary>
        /// Disposes of items in the pool that implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            lock (locker)
            {
                count = 0;
                while (queue.Count > 0)
                {
                    using (queue.Dequeue() as IDisposable)
                    {

                    }
                }
            }
        }
    }
}
