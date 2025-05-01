namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static IEnumerable<TSource> Replace<TSource>(this IEnumerable<TSource> source, int index, TSource present)
    {
        var i = -1;
        foreach (var element in source)
        {
            checked { i++; }
            yield return i == index ? present : element;
        }
    }

    public static IEnumerable<TSource> ReplaceAll<TSource>(this IEnumerable<TSource> source, TSource original, TSource present)
    {
        foreach (var element in source)
        {
            yield return Equals(element, original) ? present : element;
        }
    }

    public static IEnumerable<TSource> ReplaceAll<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource present)
    {
        foreach (var element in source)
        {
            yield return predicate(element) ? present : element;
        }
    }
}
