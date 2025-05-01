namespace System.Text.Json.Serialization;

public sealed class JsonStringConverter : JsonConverter<string>
{
    private readonly JsonConverter<string> s_defaultConverter =
        (JsonConverter<string>)JsonSerializerOptions.Default.GetConverter(typeof(string));

    public override bool HandleNull => true;

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => s_defaultConverter.Read(ref reader, typeToConvert, options) ?? string.Empty;

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        => s_defaultConverter.Write(writer, value ?? string.Empty, options);
}
