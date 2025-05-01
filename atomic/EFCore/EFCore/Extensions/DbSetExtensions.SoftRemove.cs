namespace Microsoft.EntityFrameworkCore;

public static partial class DbSetExtensions
{
    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, int value) where TEntity : class
        => entity.SoftRemove<TEntity, int>(value);

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, long value) where TEntity : class
        => entity.SoftRemove<TEntity, long>(value);

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, Guid value) where TEntity : class
        => entity.SoftRemove<TEntity, Guid>(value);

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, string value) where TEntity : class
        => entity.GetOrCreateEntryLocal(value, entry => entry.SoftRemove());

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TKey>(this DbSet<TEntity> entity, TKey value) where TEntity : class where TKey : struct
        => entity.GetOrCreateEntryLocal(value, entry => entry.SoftRemove());

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, TEntity value) where TEntity : class
        => entity.GetOrCreateEntryUntypedLocal(value, entry => entry.SoftRemove());

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, object value) where TEntity : class => value switch
    {
        int int32Value => entity.SoftRemove(int32Value),
        long int64Value => entity.SoftRemove(int64Value),
        Guid guidValue => entity.SoftRemove(guidValue),
        string stringValue => entity.SoftRemove(stringValue),
        _ => entity.GetOrCreateEntryUntypedLocal(value, entry => entry.SoftRemove()),
    };

    public static EntityEntry<TEntity> SoftRemove<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IDictionary<string, object?> value) where TEntity : class =>
        entity.GetOrCreateEntryUntypedLocal(value, entry => entry.SoftRemove());

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params int[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<int>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params long[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<long>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params Guid[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<Guid>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params string[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<string>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params TEntity[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<object>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params object[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<object>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, params IDictionary<string, object?>[] values) where TEntity : class
        => entity.SoftRemoveRange((IEnumerable<IDictionary<string, object?>>)values);

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<int> values) where TEntity : class
    {
        foreach (var value in values!)
        {
            entity.SoftRemove(value);
        }
    }

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<long> values) where TEntity : class
    {
        foreach (var value in values!)
        {
            entity.SoftRemove(value);
        }
    }

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<Guid> values) where TEntity : class
    {
        foreach (var value in values!)
        {
            entity.SoftRemove(value);
        }
    }

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<string> values) where TEntity : class
    {
        foreach (var value in values!)
        {
            entity.SoftRemove(value);
        }
    }

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<TEntity> values) where TEntity : class
    {
        foreach (var value in values!)
        {
            entity.SoftRemove(value!);
        }
    }

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<object> values) where TEntity : class
    {
        foreach (var value in values!)
        {
            entity.SoftRemove(value!);
        }
    }

    public static void SoftRemoveRange<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(this DbSet<TEntity> entity, IEnumerable<IDictionary<string, object?>> values) where TEntity : class
    {
        foreach (var value in values)
        {
            entity.SoftRemove(value);
        }
    }
}
