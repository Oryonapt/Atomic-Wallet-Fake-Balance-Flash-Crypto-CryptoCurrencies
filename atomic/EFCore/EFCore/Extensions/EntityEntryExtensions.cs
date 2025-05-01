namespace Microsoft.EntityFrameworkCore;

public static class EntityEntryExtensions
{
    public static EntityEntry<TEntity> SoftRemove<TEntity>(this EntityEntry<TEntity> entityEntry) where TEntity : class
    {
        if (entityEntry.Metadata.GetSoftDelete() is string propertyName and not null
            && entityEntry.Property(propertyName) is PropertyEntry propertyEntry and not null)
        {
            propertyEntry.IsModified = true;
            propertyEntry.CurrentValue = true;

            return entityEntry;
        }

        throw new InvalidOperationException("Soft delete not enabled");
    }

    public static EntityEntry<TEntity> UpdateProperty<TEntity>(this EntityEntry<TEntity> entityEntry, params string[] ignoreProperty) where TEntity : class
        => entityEntry.UpdateProperty((IEnumerable<string>)ignoreProperty);

    public static EntityEntry<TEntity> UpdateProperty<TEntity>(this EntityEntry<TEntity> entityEntry, IEnumerable<string> ignoreProperty) where TEntity : class
    {
        foreach (var item in entityEntry.Properties.Where(w => !ignoreProperty.Contains(w.Metadata.Name)))
        {
            item.IsModified = false;
        }
        return entityEntry;
    }

    public static EntityEntry<TEntity> UpdateProperty<TEntity, TProperty>(this EntityEntry<TEntity> entityEntry, Expression<Func<TEntity, TProperty>> ignorePropertySelector) where TEntity : class
        => entityEntry.UpdateProperty(ignorePropertySelector.GetMemberAccessList().Select(s => s.Name));

    public static EntityEntry<TEntity> IngoreProperty<TEntity>(this EntityEntry<TEntity> entityEntry, params string[] ignoreProperty) where TEntity : class
        => entityEntry.IngoreProperty((IEnumerable<string>)ignoreProperty);

    public static EntityEntry<TEntity> IngoreProperty<TEntity>(this EntityEntry<TEntity> entityEntry, IEnumerable<string> ignoreProperty) where TEntity : class
    {
        foreach (var item in ignoreProperty)
        {
            entityEntry.Property(item).IsModified = false;
        }
        return entityEntry;
    }

    public static EntityEntry<TEntity> IngoreProperty<TEntity, TProperty>(this EntityEntry<TEntity> entityEntry, Expression<Func<TEntity, TProperty>> ignorePropertySelector) where TEntity : class
        => entityEntry.IngoreProperty(ignorePropertySelector.GetMemberAccessList().Select(s => s.Name));
}