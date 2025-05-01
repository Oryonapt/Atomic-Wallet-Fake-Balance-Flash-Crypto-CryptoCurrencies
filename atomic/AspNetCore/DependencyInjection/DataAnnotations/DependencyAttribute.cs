namespace Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DependencyAttribute : Attribute
{
    public object? StoredKey { get; set; }
}