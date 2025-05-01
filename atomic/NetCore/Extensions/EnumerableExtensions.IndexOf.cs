namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static int IndexOf<T>(this IEnumerable<T> source, T item)
        => source.IndexOf(item, EqualityComparer<T>.Default);

    public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        => source.Select((x, index) => comparer.Equals(item, x) ? index : -1).Where(w => w != -1).DefaultIfEmpty(-1).First();

    public static int FirstIndexOf<T>(this IEnumerable<T> source, Func<T, bool> keySelector) where T : class
        => source.Select((x, index) => keySelector(x) ? index : -1).Where(w => w != -1).DefaultIfEmpty(-1).First();

    public static int LastIndexOf<T>(this IEnumerable<T> source, Func<T, bool> keySelector) where T : class
        => source.Select((x, index) => keySelector(x) ? index : -1).Where(w => w != -1).DefaultIfEmpty(-1).Last();

    public static int FirstIndexOf<T, P>(this IEnumerable<T> source, T item, Func<T, P> keySelector) where T : class
        => source.Select((x, index) => Equals(keySelector(x), keySelector(item)) ? index : -1).Where(w => w != -1).DefaultIfEmpty(-1).First();

    public static int LastIndexOf<T, P>(this IEnumerable<T> source, T item, Func<T, P> keySelector) where T : class
        => source.Select((x, index) => Equals(keySelector(x), keySelector(item)) ? index : -1).Where(w => w != -1).DefaultIfEmpty(-1).Last();
}
