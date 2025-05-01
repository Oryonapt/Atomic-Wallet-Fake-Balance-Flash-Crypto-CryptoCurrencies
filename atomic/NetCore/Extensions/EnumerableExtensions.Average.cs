namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector, long defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector, decimal? defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector, float defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector, int? defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector, float? defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, double defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector, double? defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector, long? defaultValue)
        => source.Any() ? source.Average(selector) : defaultValue;

    public static float? Average(this IEnumerable<float?> source, float? defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static decimal Average(this IEnumerable<decimal> source, decimal defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static double Average(this IEnumerable<int> source, double defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static double Average(this IEnumerable<long> source, double defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static double Average(this IEnumerable<double> source, double defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static double? Average(this IEnumerable<double?> source, double? defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static double? Average(this IEnumerable<int?> source, double? defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static double? Average(this IEnumerable<long?> source, double? defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static decimal? Average(this IEnumerable<decimal?> source, decimal? defaultValue)
        => source.Any() ? source.Average() : defaultValue;

    public static float Average(this IEnumerable<float> source, float defaultValue)
        => source.Any() ? source.Average() : defaultValue;
}
