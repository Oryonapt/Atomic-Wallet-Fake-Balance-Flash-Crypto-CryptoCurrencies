namespace System.Linq;

public static partial class QueryableExtensions
{
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        => condition ? source.Where(predicate) : source;

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, int, bool>> predicate)
        => condition ? source.Where(predicate) : source;
}