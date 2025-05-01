namespace Microsoft.EntityFrameworkCore.Metadata.Conventions;

internal sealed class ColumnDefaultValueSqlConvention(ProviderConventionSetBuilderDependencies dependencies) : PropertyAttributeConventionBase<DefaultValueSqlAttribute>(dependencies)
{
    protected override void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        DefaultValueSqlAttribute attribute,
        MemberInfo clrMember,
        IConventionContext context) 
        => propertyBuilder.HasDefaultValueSql(attribute.Value, fromDataAnnotation: true);
}