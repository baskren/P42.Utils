using System;
using System.Collections.Generic;
using System.Linq;

namespace P42.Utils
{
    public static class CopyingExtensions
    {
        /// <summary>
        /// Deep copy (make copies of members) of list
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> DeepCopy<T>(this IEnumerable<T> source) where T : ICopiable<T>, new()
            => source.Select(member => member.Copy()).ToList();
        

        /// <summary>
        /// Make a copy of an ICopiable
        /// </summary>
        /// <param name="source">source</param>
        /// <typeparam name="T">ICopiable</typeparam>
        /// <returns>copy</returns>
        public static T Copy<T>(this T source) where T : ICopiable<T>, new()
        {
            var copy = Activator.CreateInstance<T>();
            copy.PropertiesFrom(source);
            return copy;
        }
    }
}
