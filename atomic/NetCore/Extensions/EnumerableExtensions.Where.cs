namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        => condition ? source.Where(predicate) : source;

    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
        => condition ? source.Where(predicate) : source;
}