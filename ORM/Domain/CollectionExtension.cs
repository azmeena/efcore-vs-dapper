using System;
using System.Collections.Generic;
using System.Linq;
using GenFu;

namespace ORM.Domain
{
    public static class CollectionExtension
    {
        private static Random rnd = new Random();

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[rnd.Next(list.Count)];
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[rnd.Next(array.Length)];
        }

        public static List<T> Populate<T>(this List<T> list, Func<int, List<T>> func, int count) where T : class
        {
            while (list.Count < count)
                list.AddRange(func(count));

            return list.Take(count).ToList();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static GenFuConfigurator<TType> AsRandom<TType, TData>(
            this GenFuComplexPropertyConfigurator<TType, IEnumerable<TData>> configurator,
            IEnumerable<TData> data, int min, int max)
            where TType : new()
        {
            configurator.Maggie.RegisterFiller(
                new CustomFiller<IEnumerable<TData>>(
                    configurator.PropertyInfo.Name, typeof(TType),
                    () => data.GetRandom(min, max)));

            return configurator;
        }

        private static IEnumerable<T> GetRandom<T>(this IEnumerable<T> source, int min, int max)
        {
            var length = source.Count();
            var index = A.Random.Next(0, length - 1);
            var count = A.Random.Next(min, max);

            return source.Skip(index).Take(count);
        }
    }
}