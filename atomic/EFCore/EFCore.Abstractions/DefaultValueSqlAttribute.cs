namespace Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class DefaultValueSqlAttribute(string? value = null) : Attribute
{
    public string? Value { get; } = value;
}