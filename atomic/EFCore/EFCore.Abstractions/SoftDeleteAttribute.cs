namespace Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SoftDeleteAttribute(string name, string comment) : Attribute
{
    public SoftDeleteAttribute() : this("Deleted", string.Empty)
    {
    }

    public SoftDeleteAttribute(string name) : this(name, string.Empty)
    {
    }

    public string Name { get; init; } = name;

    public string Comment { get; init; } = comment;

    public int Order { get; init; } = 100;

    public bool Enable { get; init; } = true;
}