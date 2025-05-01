using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class EntityFrameworkCoreConventionSetPlugin(
    IEntityFrameworkCoreSingletonOptions entityFrameworkCoreSingletonOptions,
    ProviderConventionSetBuilderDependencies dependencies) : IConventionSetPlugin
{
    protected ProviderConventionSetBuilderDependencies Dependencies { get; } = dependencies;

    public virtual ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        if (!entityFrameworkCoreSingletonOptions.EnableForeignKeyIndex)
        {
            conventionSet.Remove(typeof(ForeignKeyIndexConvention));
        }

        var columnDefaultValueConvention = new ColumnDefaultValueConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnDefaultValueConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnDefaultValueConvention);

        var columnDefaultValueSqlConvention = new ColumnDefaultValueSqlConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnDefaultValueSqlConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnDefaultValueSqlConvention);

        var columnUpdateIgnoreConvention = new ColumnUpdateIgnoreConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnUpdateIgnoreConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnUpdateIgnoreConvention);

        var columnInsertIgnoreConvention = new ColumnAddIgnoreConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnInsertIgnoreConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnInsertIgnoreConvention);

        var tableAndColumnCommentConvention = new TableAndColumnCommentConvention(entityFrameworkCoreSingletonOptions);
        conventionSet.ModelFinalizingConventions.Add(tableAndColumnCommentConvention);

        return conventionSet;
    }
}
