namespace Microsoft.EntityFrameworkCore;

public static partial class DbSetExtensions
{
    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, int value) where TEntity : class
        => entity.Remove<TEntity, int>(value);

    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, long value) where TEntity : class
        => entity.Remove<TEntity, long>(value);

    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, Guid value) where TEntity : class
        => entity.Remove<TEntity, Guid>(value);

    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, string value) where TEntity : class
        => entity.GetOrCreateEntryLocal(value, entry => entry.State = EntityState.Deleted);

    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TKey>(this DbSet<TEntity> entity, TKey value) where TEntity : class where TKey : struct
        => entity.GetOrCreateEntryLocal(value, entry => entry.State = EntityState.Deleted);

    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, object value) where TEntity : class => value switch
    {
        int int32Value => entity.Remove(int32Value),
        long int64Value => entity.Remove(int64Value),
        Guid guidValue => entity.Remove(guidValue),
        string stringValue => entity.Remove(stringValue),
        _ => entity.GetOrCreateEntryUntypedLocal(value, entry => entry.State = EntityState.Deleted),
    };

    public static EntityEntry<TEntity> Remove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IDictionary<string, object?> value) where TEntity : class
        => entity.GetOrCreateEntryUntypedLocal(value, entry => entry.State = EntityState.Deleted);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params int[] values) where TEntity : class
        => entity.RemoveRange((IEnumerable<int>)values);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params long[] values) where TEntity : class
        => entity.RemoveRange((IEnumerable<long>)values);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params Guid[] value) where TEntity : class
        => entity.RemoveRange((IEnumerable<Guid>)value);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params string[] values) where TEntity : class
        => entity.RemoveRange((IEnumerable<string>)values);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params object[] values) where TEntity : class
        => entity.RemoveRange((IEnumerable<object>)values);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params IDictionary<string, object?>[] values) where TEntity : class
        => entity.RemoveRange((IEnumerable<IDictionary<string, object?>>)values);

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<int> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Remove(value);
        }
    }

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<long> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Remove(value);
        }
    }

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<Guid> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Remove(value);
        }
    }

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<string> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Remove(value);
        }
    }

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<object> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Remove(value);
        }
    }

    public static void RemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<IDictionary<string, object?>> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.Remove(value);
        }
    }
}
