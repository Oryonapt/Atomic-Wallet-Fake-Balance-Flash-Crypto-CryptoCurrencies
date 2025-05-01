namespace Microsoft.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbContext context, TEntity value) where TEntity : class
        => context.Set<TEntity>().SoftRemove(value);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbContext context, params TEntity[] values) where TEntity : class
        => context.SoftRemoveRange((IEnumerable<TEntity>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbContext context, IEnumerable<TEntity> values) where TEntity : class
    {
        foreach (var objectInstance in values)
        {
            context.SoftRemove(objectInstance);
        }
    }
}
