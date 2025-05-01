namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonDateTimeConverterAttribute(string dateFormatString) : JsonConverterAttribute
{
    public JsonDateTimeConverterAttribute() : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    public override JsonConverter? CreateConverter(Type typeToConvert)
        => new JsonDateTimeConverter(dateFormatString);
}