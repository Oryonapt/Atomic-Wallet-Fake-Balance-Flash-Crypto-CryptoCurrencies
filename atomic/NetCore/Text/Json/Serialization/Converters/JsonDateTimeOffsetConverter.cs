using System.Globalization;

namespace System.Text.Json.Serialization;

public sealed class JsonDateTimeOffsetConverter(string dateFormatString) : JsonConverter<DateTimeOffset>
{
    private readonly JsonConverter<DateTimeOffset> s_defaultConverter =
        (JsonConverter<DateTimeOffset>)JsonSerializerOptions.Default.GetConverter(typeof(DateTimeOffset));

    public JsonDateTimeOffsetConverter() : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetDateTimeOffset(out var result)
            ? result
            : DateTimeOffset.TryParse(reader.GetString(), out result)
            ? result
            : DateTimeOffset.TryParseExact(reader.GetString(), dateFormatString, CultureInfo.CurrentCulture, DateTimeStyles.None, out result)
            ? result
            : s_defaultConverter.Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString(dateFormatString));
}
