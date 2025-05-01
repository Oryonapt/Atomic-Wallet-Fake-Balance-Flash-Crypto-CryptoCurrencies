namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonGuidHandlingAttribute(JsonGuidHandling jsonGuidHandling) : JsonConverterAttribute
{
    public override JsonConverter? CreateConverter(Type typeToConvert)
        => new JsonGuidConverter(jsonGuidHandling);
}