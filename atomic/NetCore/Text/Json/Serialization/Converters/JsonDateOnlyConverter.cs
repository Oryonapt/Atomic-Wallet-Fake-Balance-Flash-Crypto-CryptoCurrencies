using System.Globalization;

namespace System.Text.Json.Serialization;

public sealed class JsonDateOnlyConverter(string dateFormatString) : JsonConverter<DateOnly>
{
    private readonly JsonConverter<DateOnly> s_defaultConverter =
        (JsonConverter<DateOnly>)JsonSerializerOptions.Default.GetConverter(typeof(DateOnly));

    public JsonDateOnlyConverter() : this("yyyy-MM-dd")
    {
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.TryParseExact(reader.GetString(), dateFormatString, CultureInfo.CurrentCulture, DateTimeStyles.None, out var result)
            ? result : s_defaultConverter.Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(dateFormatString));

}
