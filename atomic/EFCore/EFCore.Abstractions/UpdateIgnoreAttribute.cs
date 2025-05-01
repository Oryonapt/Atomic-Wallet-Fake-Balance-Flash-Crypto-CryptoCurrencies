namespace Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UpdateIgnoreAttribute : Attribute;