namespace Microsoft.EntityFrameworkCore;

public static partial class DbSetExtensions
{
    public static EntityEntry<TEntity> AddOrUpdate<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, object value) where TEntity : class
        => entity.GetOrCreateEntry(value);

    public static EntityEntry<TEntity> AddOrUpdate<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, TEntity value) where TEntity : class
        => entity.GetOrCreateEntry(value);

    public static EntityEntry<TEntity> AddOrUpdate<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IDictionary<string, object?> value) where TEntity : class
        => entity.GetOrCreateEntry(value);

    public static async ValueTask<EntityEntry<TEntity>> AddOrUpdateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, object value) where TEntity : class
        => await entity.GetOrCreateEntryAsync(value);

    public static async ValueTask<EntityEntry<TEntity>> AddOrUpdateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, TEntity value) where TEntity : class
        => await entity.GetOrCreateEntryAsync(value);

    public static async ValueTask<EntityEntry<TEntity>> AddOrUpdateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IDictionary<string, object?> value) where TEntity : class
        => await entity.GetOrCreateEntryAsync(value);
}