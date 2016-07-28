using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LewisTech.Utils.Collections
{

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> items)
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
        public static void ForEach<T>(this IEnumerable<T> values, Action<int, T> action)
        {
            var x = 0;
            foreach (var value in values)
            {
                action(x, value);
                x++;
            }
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> values)
        {
            values.ForEach(list.Add);
        }



        public static bool SequenceEqual<T>(this IEnumerable<T> list1, IEnumerable<T> list2, bool ignoreOrder)
        {
            return SequenceEqual(list1, list2, null, ignoreOrder);
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer, bool ignoreOrder)
        {
            if (!ignoreOrder)
            {
                return list1.SequenceEqual(list2, comparer);
            }
            var cnt = new Dictionary<T, int>(comparer);
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        //public static T BinarySearch<T>(this IReadOnlyList<T> items, Func<T, int> prop, int searchValue)
        //{

        //}
        public static T BinarySearch<T, TProp>(this IReadOnlyList<T> items, Func<T, TProp> prop, TProp searchValue) where TProp : IComparable<TProp>
        {
            var low = 0;
            var high = items.Count - 1;

            //Console.WriteLine(string.Format("BinarySearch<{0}> starting: {1} items, looking for: {2}", typeof(T).Name, itemCount, searchValue));
            //TProp prevHigh = default(TProp);
            //TProp prevLow = default(TProp);
            TProp prevValue = default(TProp);
            int comp = 0;
            //var count = 0;
            while (low <= high)
            {
                //count++;
                var mid = (low + high) / 2;


                var test = items[mid];

                var check = prop(test);
                //comp = check.CompareTo(searchValue);
                
                if ((comp == -1 && check.CompareTo(prevValue)==-1) || (comp == 1 && check.CompareTo(prevValue)==1))
                {
                    throw new Exception("Array not sorted");
                }
                
                comp = check.CompareTo(searchValue);

                prevValue = check;

                //Console.WriteLine(string.Format("low: {0}, mid: {1}, high: {2}, check: {3}, comp: {4}", low, mid, high, check, comp));

                if (comp == 1)
                {
                    high = mid - 1;

                    //if (check.CompareTo(prevHigh) == 1)
                    //{
                    //    throw new Exception();
                    //}
                    //prevHigh = check;
                }
                else if (comp == -1)
                {
                    low = mid + 1;
                    //prevLow = check;

                }
                else
                {
                    //Console.WriteLine(string.Format("BinarySearch<{0}> found match in {1} tries", typeof(T).Name, count));
                    return test;
                }
            }
            //Console.WriteLine(string.Format("BinarySearch<{0}> no match found in {1} tries", typeof(T).Name, count));

            return default(T);
        }
        public static double Median(this IReadOnlyList<int> items)
        {
            int numberCount = items.Count;

            if (numberCount == 0)
            {
                return 0;
            }
            if (numberCount == 1)
            {
                return items.First();
            }

            var halfIndex = (int)Math.Floor(numberCount / 2d);
            var sortedNumbers = items.OrderBy(n => n);

            if ((numberCount % 2) == 0)
            {
                return sortedNumbers.Skip(halfIndex - 1).Take(2).Average();
            }
            return sortedNumbers.ElementAt(halfIndex);
        }
        public static double Median(this IReadOnlyList<double> items)
        {
            int numberCount = items.Count;

            if (numberCount == 0)
            {
                return 0;
            }
            if (numberCount == 1)
            {
                return items.First();
            }

            var halfIndex = (int)Math.Floor(numberCount / 2d);
            var sortedNumbers = items.OrderBy(n => n);

            if ((numberCount % 2) == 0)
            {
                return sortedNumbers.Skip(halfIndex - 1).Take(2).Average();
            }
            return sortedNumbers.ElementAt(halfIndex);
        }


        public static IEnumerable<T> Randomize<T>(this IReadOnlyList<T> source)
        {
            var rand = new Random();
            var values = source.ToArray();
            var num_values = values.Length;
            for (var i = 0; i <= num_values - 2; i++)
            {
                var j = rand.Next(i, num_values);
                var temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }

            return values;
        }


        public static IEnumerable<T> Ancestors<T>(this T item, Func<T, T> parentProperty, bool includeSelf) where T : class
        {
            if (includeSelf)
            {
                yield return item;
            }

            foreach (var a in item.Ancestors(parentProperty))
            {
                yield return a;
            }

        }

        public static IEnumerable<T> Ancestors<T>(this T item, Func<T, T> parentProperty) where T : class
        {

            var current = item;

            do
            {
                var parent = parentProperty(current);
                if (parent == null)
                {
                    yield break;
                }
                yield return parent;
                current = parent;

            }
            while (true);

        }

        public static IEnumerable<T> Descendants<T>(this T item, Func<T, IEnumerable<T>> childrenProperty, bool includeSelf)
        {
            if (includeSelf)
            {
                yield return item;
            }

            foreach (var a in item.Descendants(childrenProperty))
            {
                yield return a;
            }

        }

        public static IEnumerable<T> Descendants<T>(this T item, Func<T, IEnumerable<T>> childrenProperty)
        {

            var children = childrenProperty(item);

            foreach (var n in children)
            {
                yield return n;

                foreach (var n2 in n.Descendants(childrenProperty))
                {
                    yield return n2;
                }

            }

        }


        public static void RemoveMissing<T>(this ICollection<T> collection, IEnumerable<T> lookIn, Func<T, int> compare)
        {
            collection.RemoveAll(m => lookIn.All(l => compare(m) != compare(l)));

        }
        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var toRemove = collection.Where(predicate).ToArray();

            toRemove.ForEach(t => collection.Remove(t));
        }

    }
}