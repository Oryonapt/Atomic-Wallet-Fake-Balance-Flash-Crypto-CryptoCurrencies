namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, object storedKey, LambdaExpression queryFilters)
        => entityTypeBuilder.HasStoredQueryFilter(storedQueryFilter => storedQueryFilter.SetFilter(storedKey, queryFilters));

    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, params LambdaExpression[] queryFilters)
        => entityTypeBuilder.HasStoredQueryFilter((IEnumerable<LambdaExpression>)queryFilters);

    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, IEnumerable<LambdaExpression> queryFilters)
    {
        var setQueryFilterCalls = new SetQueryFilterCalls();
        foreach (var queryFilter in queryFilters)
        {
            setQueryFilterCalls.SetFilter(queryFilter);
        }

        return entityTypeBuilder.HasStoredQueryFilter(setQueryFilterCalls.Filters);
    }

    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, Func<SetQueryFilterCalls, SetQueryFilterCalls> setQueryFilterCallsFunc)
    {
        var setQueryFilterCalls = new SetQueryFilterCalls();
        setQueryFilterCallsFunc.Invoke(setQueryFilterCalls);

        return entityTypeBuilder.HasStoredQueryFilter(setQueryFilterCalls.Filters);
    }

    private static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, IReadOnlyDictionary<object, LambdaExpression> storedQueryFilter)
        => entityTypeBuilder.HasAnnotation(
            EntityFrameworkCoreAnnotationNames.StoredQueryFilter,
            new Dictionary<object, LambdaExpression>([.. storedQueryFilter, .. entityTypeBuilder.Metadata.GetStoredQueryFilter()]));

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, object storedKey, Expression<Func<TEntity, bool>> queryFilters) where TEntity : class
        => entityTypeBuilder.HasStoredQueryFilter(storedQueryFilter => storedQueryFilter.SetFilter(storedKey, queryFilters));

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, params Expression<Func<TEntity, bool>>[] queryFilters) where TEntity : class
        => entityTypeBuilder.HasStoredQueryFilter((IEnumerable<Expression<Func<TEntity, bool>>>)queryFilters);

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, IEnumerable<Expression<Func<TEntity, bool>>> queryFilters) where TEntity : class
    {
        var setQueryFilterCalls = new SetQueryFilterCalls<TEntity>();
        foreach (var queryFilter in queryFilters)
        {
            setQueryFilterCalls.SetFilter(queryFilter);
        }

        return entityTypeBuilder.HasStoredQueryFilter(setQueryFilterCalls.Filters);
    }

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, Func<SetQueryFilterCalls<TEntity>, SetQueryFilterCalls<TEntity>> setQueryFilterCallsFunc) where TEntity : class
    {
        var setQueryFilterCalls = new SetQueryFilterCalls<TEntity>();
        setQueryFilterCallsFunc.Invoke(setQueryFilterCalls);

        return entityTypeBuilder.HasStoredQueryFilter(setQueryFilterCalls.Filters);
    }

    private static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, IReadOnlyDictionary<object, LambdaExpression> storedQueryFilter) where TEntity : class
        => entityTypeBuilder.HasAnnotation(
            EntityFrameworkCoreAnnotationNames.StoredQueryFilter,
            new Dictionary<object, LambdaExpression>([.. storedQueryFilter, .. entityTypeBuilder.Metadata.GetStoredQueryFilter()]));
}
