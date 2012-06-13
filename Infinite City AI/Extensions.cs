using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.AI
{
    public static class Extensions
    {
        private static readonly Random Rnd = new Random();

        public static T Random<T>(this IEnumerable<T> collection)
        {
            return Random(collection.ToArray());
        }

        public static T Random<T>(this T[] collection)
        {
            return (collection[Rnd.Next(collection.Length)]);
        }
    }
}