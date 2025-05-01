using System.ComponentModel;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions;

internal sealed class ColumnDefaultValueConvention(ProviderConventionSetBuilderDependencies dependencies) : PropertyAttributeConventionBase<DefaultValueAttribute>(dependencies)
{
    protected override void ProcessPropertyAdded(
       IConventionPropertyBuilder propertyBuilder,
       DefaultValueAttribute attribute,
       MemberInfo clrMember,
       IConventionContext context)
       => propertyBuilder.HasDefaultValue(attribute.Value, fromDataAnnotation: true);
}