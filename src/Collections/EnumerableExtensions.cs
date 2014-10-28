using System;
using System.Collections.Generic;
using System.Linq;

namespace LewisTech.Utils.Collections
{
    
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }
        
    }
}