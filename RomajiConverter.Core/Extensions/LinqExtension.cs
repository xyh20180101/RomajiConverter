using System;
using System.Collections.Generic;

namespace RomajiConverter.Core.Extensions
{
    public static class LinqExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    HashSet<TKey> set = new HashSet<TKey>(7, null);
                    do
                    {
                        TSource current = enumerator.Current;
                        if (set.Add(keySelector(current)))
                            yield return current;
                    }
                    while (enumerator.MoveNext());
                    set = (HashSet<TKey>)null;
                }
            }
        }
    }
}
