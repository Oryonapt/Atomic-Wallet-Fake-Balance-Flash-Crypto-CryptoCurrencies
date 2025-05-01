namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonDateTimeOffsetConverterAttribute(string dateFormatString) : JsonConverterAttribute
{
    public JsonDateTimeOffsetConverterAttribute() : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    public override JsonConverter? CreateConverter(Type typeToConvert)
        => new JsonDateTimeOffsetConverter(dateFormatString);
}