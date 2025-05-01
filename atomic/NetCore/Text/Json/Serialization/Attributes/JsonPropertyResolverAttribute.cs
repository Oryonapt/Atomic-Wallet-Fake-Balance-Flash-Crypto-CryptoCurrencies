namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonPropertyResolverAttribute : JsonAttribute
{
    public JsonPropertyResolverAttribute()
    {
    }

    public JsonPropertyResolverAttribute(string name)
    {
        Name = name;
    }

    public JsonPropertyResolverAttribute(params string[] values)
    {
        Values = values;
    }

    public string? Name { get; set; }

    public string? Default { get; set; }

    public string[] Values { get; set; } = [];
}
