using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace P42.Utils
{
    /// <summary>
    /// Circular buffer : fixed capacity 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T> : ICollection<T>
    {
        /// <summary>
        /// Next index
        /// </summary>
        public int NextIndex { get; private set; }

        /// <summary>
        /// Count of items
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// True
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Capacity (set on instantiation)
        /// </summary>
        readonly public int Capacity;

        readonly T[] _buffer;
        readonly object _lock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity"></param>
        public CircularBuffer(int capacity)
        {
            Capacity = capacity;
            _buffer = new T[capacity];
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            lock (_lock)
            {
                _buffer[NextIndex++] = item;
                Count++;
                if (Count > Capacity)
                    Count = Capacity;
                if (NextIndex >= Capacity)
                    NextIndex = 0;
            }
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                NextIndex = 0;
                Count = 0;
                for (int i = 0; i < Capacity; i++)
                    _buffer[i] = default;
            }
        }

        /// <summary>
        /// Contains item test
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _buffer.Contains(item);
        }

        /// <summary>
        /// Copy items to T[] array 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _buffer.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var result = new T[Count];

            lock (_lock)
            {

                int index = 0;
                for (int i = NextIndex; i < Count; i++)
                    result[index++] = _buffer[i];
                for (int i = 0; i < NextIndex; i++)
                    result[index++] = _buffer[i];
            }

            return result.Cast<T>().GetEnumerator();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            var result = new T[Count];

            lock (_lock)
            {

                int index = 0;
                for (int i = NextIndex; i < Count; i++)
                    result[index++] = _buffer[i];
                for (int i = 0; i < NextIndex; i++)
                    result[index++] = _buffer[i];
            }

            return result.GetEnumerator();
        }
    }
}
