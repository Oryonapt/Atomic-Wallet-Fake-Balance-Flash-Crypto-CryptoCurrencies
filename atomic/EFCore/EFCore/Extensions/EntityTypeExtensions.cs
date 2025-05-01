namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeExtensions
{
    public static string? GetSoftDelete(this IEntityType entityType)
        => entityType.FindAnnotation(EntityFrameworkCoreAnnotationNames.SoftDelete) is { Value: not null } annotation
            && annotation.Value is string column and not null && !string.IsNullOrWhiteSpace(column) ? column : null;

    public static IConventionAnnotation? SetOrRemoveSoftDelete(this IConventionEntityType conventionEntityType, string column)
        => conventionEntityType.SetOrRemoveAnnotation(EntityFrameworkCoreAnnotationNames.SoftDelete, column, true);

    public static IReadOnlyDictionary<object, LambdaExpression> GetStoredQueryFilter(this IMutableEntityType mutableEntityType)
        => mutableEntityType.FindAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter) is { Value: not null } annotation
            && annotation.Value is Dictionary<object, LambdaExpression> filter and not null && filter.Count != 0 ? filter : [];

    public static IReadOnlyDictionary<object, LambdaExpression> GetStoredQueryFilter(this IEntityType entityType)
        => entityType.FindAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter) is { Value: not null } annotation
            && annotation.Value is Dictionary<object, LambdaExpression> filter and not null && filter.Count != 0 ? filter : [];
}
