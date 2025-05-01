using System.Globalization;

namespace System.Text.Json.Serialization;

public sealed class JsonDateTimeConverter(string dateFormatString) : JsonConverter<DateTime>
{
    private readonly JsonConverter<DateTime> s_defaultConverter =
        (JsonConverter<DateTime>)JsonSerializerOptions.Default.GetConverter(typeof(DateTime));

    public JsonDateTimeConverter() : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TryGetDateTime(out var result)
            ? result
            : DateTime.TryParse(reader.GetString(), out result)
            ? result
            : DateTime.TryParseExact(reader.GetString(), dateFormatString, CultureInfo.CurrentCulture, DateTimeStyles.None, out result)
            ? result
            : s_defaultConverter.Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(dateFormatString));

}
