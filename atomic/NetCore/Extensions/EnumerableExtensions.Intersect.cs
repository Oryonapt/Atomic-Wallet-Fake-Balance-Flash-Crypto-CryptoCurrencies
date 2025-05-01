namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
        => IntersectByIterator(first, second, keySelector, null);

    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        => IntersectByIterator(first, second, keySelector, comparer);

    private static IEnumerable<TSource> IntersectByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
    {
        var set = new HashSet<TKey>(second.Select(keySelector), comparer);

        foreach (TSource element in first)
        {
            if (set.Remove(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}
