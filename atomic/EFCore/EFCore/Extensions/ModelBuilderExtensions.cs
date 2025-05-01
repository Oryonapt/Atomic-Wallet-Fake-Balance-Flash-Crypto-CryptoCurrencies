namespace Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, string assemblyName)
        => builder.ApplyEntitiesFromAssembly(Assembly.Load(assemblyName));

    public static ModelBuilder ApplyEntitiesFromAssembly<TBaseEntity>(this ModelBuilder builder, string assemblyName) where TBaseEntity : class
        => builder.ApplyEntitiesFromAssembly<TBaseEntity>(Assembly.Load(assemblyName));

    public static ModelBuilder ApplyEntitiesFromAssembly<TBaseEntity>(this ModelBuilder builder, Assembly assembly, params Expression<Func<TBaseEntity, bool>>[] baseQueryFilters) where TBaseEntity : class
        => builder.ApplyEntitiesFromAssembly(assembly, typeof(TBaseEntity), baseQueryFilters);

    public static ModelBuilder ApplyEntitiesFromAssembly<TBaseEntity>(this ModelBuilder builder, Assembly assembly, Type baseType, params Expression<Func<TBaseEntity, bool>>[] baseQueryFilters) where TBaseEntity : class
        => builder.ApplyEntitiesFromAssembly(assembly, w => baseType.IsAssignableFrom(w) && w.IsClass && !w.IsAbstract || w.IsDefined<DbEntityAttribute>(), baseQueryFilters);

    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, Assembly assembly, Func<Type, bool>? predicate = null, params LambdaExpression[] baseQueryFilters)
    {
        IEnumerable<Type> assemblyTypes = assembly.GetTypes();
        if (null != predicate)
        {
            assemblyTypes = assemblyTypes.Where(predicate);
        }
        return builder.ApplyEntitiesFromAssembly(assemblyTypes, baseQueryFilters);
    }

    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, IEnumerable<Type> clrTypes, params LambdaExpression[] baseQueryFilters)
    {
        foreach (var clrType in clrTypes)
        {
            var entityTypeBuilder = builder.Entity(clrType);
            if (baseQueryFilters != null && baseQueryFilters.Length > 0)
            {
                var queryFilters = baseQueryFilters.Select(baseQueryFilter =>
                {
                    var parameterExpression = Expression.Parameter(
                        clrType,
                        baseQueryFilter.Parameters[0].Name);

                    var expressionFilter = ReplacingExpressionVisitor.Replace(
                        baseQueryFilter.Parameters[0],
                        parameterExpression,
                        baseQueryFilter.Body);
                    return Expression.Lambda(expressionFilter, parameterExpression);
                });
                entityTypeBuilder.HasStoredQueryFilter(queryFilters);
            }

            foreach (var mutableProperty in
                            entityTypeBuilder.Metadata.GetProperties()
                                .Where(w => !w.IsShadowProperty() && !w.IsForeignKey() && w.ClrType == typeof(decimal)))
            {
                mutableProperty.SetPrecision(18);
                mutableProperty.SetScale(2);
            }
        }
        return builder;
    }
}