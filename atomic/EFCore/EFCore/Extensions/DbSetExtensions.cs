namespace Microsoft.EntityFrameworkCore;

public static partial class DbSetExtensions
{
    private static EntityEntry<TEntity> GetOrCreateEntryLocal<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TKey>(
        this DbSet<TEntity> entity, TKey value, Action<EntityEntry<TEntity>> setAction) where TEntity : class
    {
        var entityEntry = entity.Local.FindEntry(value);

        if (null == entityEntry)
        {
            entityEntry = entity.Entry(Activator.CreateInstance<TEntity>());

            foreach (var item in entity.EntityType.FindPrimaryKey()?.Properties ?? [])
            {
                entityEntry.Property(item).CurrentValue = value;
            }
        }

        setAction.Invoke(entityEntry);

        return entityEntry;
    }

    private static EntityEntry<TEntity> GetOrCreateEntryUntypedLocal<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(
        this DbSet<TEntity> entity, object value, Action<EntityEntry<TEntity>> setAction) where TEntity : class
    {
        var entityEntry = entity.Local.FindEntryUntyped(GetPrimaryValues(entity, value))
            ?? entity.Entry(Activator.CreateInstance<TEntity>());

        entityEntry.CurrentValues.SetValues(value);

        setAction(entityEntry);

        return entityEntry;
    }

    private static EntityEntry<TEntity> GetOrCreateEntryUntypedLocal<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TProperty>(
        this DbSet<TEntity> entity, IDictionary<string, TProperty> value, Action<EntityEntry<TEntity>> setAction) where TEntity : class
    {
        var entityEntry = entity.Local.FindEntryUntyped(GetPrimaryValues(entity, value))
            ?? entity.Entry(Activator.CreateInstance<TEntity>());

        entityEntry.CurrentValues.SetValues(value);

        setAction(entityEntry);

        return entityEntry;
    }

    private static EntityEntry<TEntity> GetOrCreateEntry<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(
        this DbSet<TEntity> entity, object value) where TEntity : class
    {
        var dbEntity = entity.Find(GetPrimaryValues(entity, value));

        var entityEntry = entity.Entry(dbEntity ?? Activator.CreateInstance<TEntity>());

        entityEntry.CurrentValues.SetValues(value);

        entityEntry.State = dbEntity is not null ? EntityState.Modified : EntityState.Added;

        return entityEntry;
    }

    private static async ValueTask<EntityEntry<TEntity>> GetOrCreateEntryAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(
        this DbSet<TEntity> entity, object value) where TEntity : class
    {
        var dbEntity = await entity.FindAsync(GetPrimaryValues(entity, value));

        var entityEntry = entity.Entry(dbEntity ?? Activator.CreateInstance<TEntity>());

        entityEntry.CurrentValues.SetValues(value);

        entityEntry.State = dbEntity is not null ? EntityState.Modified : EntityState.Added;

        return entityEntry;
    }

    private static object?[] GetPrimaryValues<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity>(DbSet<TEntity> entity, object value) where TEntity : class
    {
        List<object?> propertyValues = [];

        foreach (var property in entity.EntityType.FindPrimaryKey()?.Properties ?? [])
        {
            var propertyValue = value.GetType().IsClass || value.GetType().IsAnonymousType()
                ? (value.GetType().GetAnyProperty(property.Name)?.GetValue(value))
                : throw new ArgumentException(string.Format("The object value '{0}' cannot find any property for entitytype '{1}'.", value.GetType().Name, entity.EntityType.Name));
            if (propertyValue is not null)
            {
                propertyValues.Add(propertyValue);
            }
        }

        return [.. propertyValues];
    }

    private static object?[] GetPrimaryValues<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.EntityType)] TEntity, TProperty>(DbSet<TEntity> entity, IDictionary<string, TProperty> value) where TEntity : class
    {
        List<object?> propertyValues = [];

        foreach (var property in entity.EntityType.FindPrimaryKey()?.Properties ?? [])
        {
            if (value is IDictionary<string, TProperty> dictionaryValue && dictionaryValue.TryGetValue(property.Name, out var propertyValue))
            {
                propertyValues.Add(propertyValue);
            }
            else
            {
                throw new ArgumentException(string.Format("The object value '{0}' cannot find any property for entitytype '{1}'.", value.GetType().Name, entity.EntityType.Name));
            }
        }

        return [.. propertyValues];
    }
}
