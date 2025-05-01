namespace System.Text.Json.Serialization;

public sealed class JsonGuidConverter(JsonGuidHandling? jsonGuidHandling) : JsonConverter<Guid>
{
    private readonly JsonConverter<Guid> s_defaultConverter =
        (JsonConverter<Guid>)JsonSerializerOptions.Default.GetConverter(typeof(Guid));

    public JsonGuidConverter() : this(null)
    {
    }

    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType == JsonTokenType.String
            ? Guid.TryParseExact(reader.GetString(), jsonGuidHandling switch
            {
                JsonGuidHandling.Digits => "N",
                JsonGuidHandling.Braces => "B",
                JsonGuidHandling.Parentheses => "P",
                JsonGuidHandling.Hexadecimal => "X",
                JsonGuidHandling.Hyphens or _ => "D",
            }, out var result) ? result : s_defaultConverter.Read(ref reader, typeToConvert, options)
            : s_defaultConverter.Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        switch (jsonGuidHandling)
        {
            case JsonGuidHandling.Digits:
                writer.WriteStringValue(value.ToString("N"));
                break;
            case JsonGuidHandling.Hyphens:
                writer.WriteStringValue(value.ToString("D"));
                break;
            case JsonGuidHandling.Braces:
                writer.WriteStringValue(value.ToString("B"));
                break;
            case JsonGuidHandling.Parentheses:
                writer.WriteStringValue(value.ToString("P"));
                break;
            case JsonGuidHandling.Hexadecimal:
                writer.WriteStringValue(value.ToString("X"));
                break;
            default:
                s_defaultConverter.Write(writer, value, options);
                break;
        }
    }
}
