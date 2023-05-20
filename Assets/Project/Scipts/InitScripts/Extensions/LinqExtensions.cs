using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public static class LinqExtensions
    {
        public static void Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                Debug.LogError("enumerable is null");
                return;
            }

            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static T RandomElement<T>(this IList<T> enumerable)
        {
            return enumerable[UnityEngine.Random.Range(0, enumerable.Count)];
        }
        
        public static List<T> ToList<T>(this IEnumerable<T> enumerable)
        {
            return new List<T>(enumerable);
        }
        
        public static IEnumerable<T> DuplicateIntersect<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {
            var dict = new Dictionary<T, int>(comparer);

            foreach (T item in second)
            {
                int hits;
                dict.TryGetValue(item, out hits);
                dict[item] = hits + 1;
            }

            foreach (T item in first)
            {
                int hits;
                dict.TryGetValue(item, out hits);
                if (hits > 0)
                {
                    yield return item;
                    dict[item] = hits - 1;
                }
            }
        }
        
        public static T Max<T, TR>(this IEnumerable<T> source, Func<T, TR> selector)
        {
            if (source == null)
            {
                Debug.LogError("Enumerable is null.");
                return default(T);
            }

            var @default = Comparer<TR>.Default;
            T y = default(T);
            if ((object)y == null)
            {
                foreach (T x in source)
                {
                    if ((object)x != null && ((object)y == null || @default.Compare(selector(x), selector(y)) > 0))
                        y = x;
                }
                return y;
            }
            else
            {
                using (IEnumerator<T> enumerator = source.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                        return default(T);

                    y = enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        T x = enumerator.Current;
                        if (@default.Compare(selector(x), selector(y)) > 0)
                            y = x;
                    }
                    return y;
                }
            }
        }
        
        public static T Min<T, TR>(this IEnumerable<T> source, Func<T, TR> selector)
        {
            if (source == null)
            {
                Debug.LogError("Enumerable is null.");
                return default(T);
            }

            var @default = Comparer<TR>.Default;
            T y = default(T);
            if ((object)y == null)
            {
                foreach (T x in source)
                {
                    if ((object)x != null && ((object)y == null || @default.Compare(selector(x), selector(y)) < 0))
                        y = x;
                }
                return y;
            }
            else
            {
                using (IEnumerator<T> enumerator = source.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                        return default(T);

                    y = enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        T x = enumerator.Current;
                        if (@default.Compare(selector(x), selector(y)) < 0)
                            y = x;
                    }
                    return y;
                }
            }
        }
    }
}