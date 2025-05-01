namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
        => ExceptByIterator(first, second, keySelector, null);

    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        => ExceptByIterator(first, second, keySelector, comparer);

    private static IEnumerable<TSource> ExceptByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
    {
        var set = new HashSet<TKey>(second.Select(keySelector), comparer);

        foreach (TSource element in first)
        {
            if (set.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}
