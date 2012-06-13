using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity
{
    public static class Extensions
    {
        private static readonly Random Rnd = new Random();

        public static IEnumerable<T> TakeBest<T>(this IEnumerable<T> sequence,  Func<T,T,int> comparer)
        {
            IList<T> best = new List<T>();
            foreach (T item in sequence)
            {
                if(best.Count==0)
                {
                    best.Add(item);
                    continue;
                }
                int comparison = comparer(item, best[0]);
                if (comparison < 0)
                    continue;
                if (comparison>0)
                    best.Clear();
                best.Add(item);
            }
            return best;
        }

        public static IEnumerable<T> Highest<T>(this IEnumerable<T> sequence, Func<T, double?> selector)
        {
            KeyValuePair<T, double?>[] weights = sequence.Select(i => new KeyValuePair<T, double?>(i, selector(i))).ToArray();
            double? maximum = weights.MaxOrNull(kvp => kvp.Value);
            return weights.Where(kvp => kvp.Value == maximum).Select(kvp => kvp.Key);
        }

        public static double? MaxOrNull<T>(this IEnumerable<T> sequence, Func<T, double?> selector)
        {
            try
            {
                double?[] weights = sequence.Select(selector).ToArray();
                return weights.Where(d => d.HasValue).Max();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            return Random(collection.ToArray());
        }

        public static T Random<T>(this T[] collection)
        {
            return collection.Length == 0 ? default(T) : (collection[Rnd.Next(collection.Length)]);
        }

        public static IEnumerable<KeyValuePair<T, double?>> Reweight<T>(this IEnumerable<KeyValuePair<T, double?>> sequence,
                                                                        Func<T, double?> selector)
        {
            return Reweight(sequence, (i, w) => selector(i));
        }

        public static IEnumerable<KeyValuePair<T, double?>> Reweight<T>(this IEnumerable<KeyValuePair<T, double?>> sequence,
                                                                        Func<T, double?, double?> selector)
        {
            return sequence.Select(kvp => new KeyValuePair<T, double?>(kvp.Key, selector(kvp.Key, kvp.Value)));
        }

        public static IEnumerable<KeyValuePair<T, int>> Reweight<T>(this IEnumerable<KeyValuePair<T, int>> sequence,
                                                                    Func<T, int, int> selector)
        {
            return sequence.Select(kvp => new KeyValuePair<T, int>(kvp.Key, selector(kvp.Key, kvp.Value)));
        }

        public static double? SumOrNull<T>(this IEnumerable<T> sequence, Func<T, double?> selector)
        {
            try
            {
                return sequence.Sum(selector);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static IEnumerable<KeyValuePair<object, double>> Weight<T>(this IEnumerable<T> sequence, Func<T, double?> weight)
        {
            return
                sequence.Select(o => new KeyValuePair<T, double?>(o, weight(o))).Where(kvp => kvp.Value.HasValue).Select(
                    kvp => new KeyValuePair<object, double>(kvp.Key, kvp.Value.GetValueOrDefault()));
        }
    }
}