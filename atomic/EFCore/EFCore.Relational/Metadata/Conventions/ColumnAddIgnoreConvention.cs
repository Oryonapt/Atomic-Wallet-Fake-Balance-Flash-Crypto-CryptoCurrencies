namespace Microsoft.EntityFrameworkCore.Metadata.Conventions;

internal sealed class ColumnAddIgnoreConvention(ProviderConventionSetBuilderDependencies dependencies) : PropertyAttributeConventionBase<AddIgnoreAttribute>(dependencies)
{
    protected override void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        AddIgnoreAttribute attribute,
        MemberInfo clrMember,
        IConventionContext context) 
        => propertyBuilder.BeforeSave(PropertySaveBehavior.Ignore, fromDataAnnotation: true);
}