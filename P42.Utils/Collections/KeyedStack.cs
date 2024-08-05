using System.Collections.Generic;
namespace P42.Utils
{
    /// <summary>
    /// Dictionary of stacks, indexed by key
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class KeyedStack<K, V> : Dictionary<K, Stack<V>> where V : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public KeyedStack()
        {
        }

        /// <summary>
        /// Push member on stack tagged with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Push(K key, V value)
        {
            if (!ContainsKey(key))
                Add(key, new Stack<V>());
            this[key].Push(value);
        }

        /// <summary>
        /// Pop item off stack tagged with key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V Pop(K key)
        {
            if (!ContainsKey(key))
                return null;
            var stack = this[key];
            if (stack.Count > 0)
                return stack.Pop();
            return null;
        }

        /// <summary>
        /// Count of stack tagged with key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new int Count(K key)
        {
            if (!ContainsKey(key))
                return 0;
            return this[key].Count;
        }
    }
}
