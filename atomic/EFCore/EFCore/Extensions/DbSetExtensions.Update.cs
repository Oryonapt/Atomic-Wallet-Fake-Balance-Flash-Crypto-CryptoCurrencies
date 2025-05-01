namespace Microsoft.EntityFrameworkCore;

public static partial class DbSetExtensions
{
    public static EntityEntry<TEntity> Update<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, object value) where TEntity : class
        => entity.GetOrCreateEntryUntypedLocal(value, entry => entry.State = EntityState.Modified);

    public static EntityEntry<TEntity> Update<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TProperty>(this DbSet<TEntity> entity, IDictionary<string, TProperty> value) where TEntity : class
        => entity.GetOrCreateEntryUntypedLocal(value, entry => entry.State = EntityState.Modified);

    public static void UpdateRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params object[] values) where TEntity : class
        => entity.UpdateRange((IEnumerable<object>)values);

    public static void UpdateRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params IDictionary<string, object?>[] values) where TEntity : class
        => entity.UpdateRange((IEnumerable<IDictionary<string, object?>>)values);

    public static void UpdateRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<object> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Update(value);
        }
    }

    public static void UpdateRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TProperty>(this DbSet<TEntity> entity, IEnumerable<IDictionary<string, TProperty>> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Update(value);
        }
    }
}