using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace System.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions ApplyWebDefault(this JsonSerializerOptions serializerOptions)
    {
        if (serializerOptions.IsReadOnly)
        {
            serializerOptions = new JsonSerializerOptions(serializerOptions);
        }

        serializerOptions.WriteIndented = true;
        serializerOptions.AllowTrailingCommas = true;
        serializerOptions.PropertyNameCaseInsensitive = true;
        serializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        serializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        serializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        serializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        serializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;

        serializerOptions.Converters.Add(new DataSetConverter());
        serializerOptions.Converters.Add(new JsonGuidConverter());
        serializerOptions.Converters.Add(new DataTableConverter());
        serializerOptions.Converters.Add(new JsonStringConverter());
        serializerOptions.Converters.Add(new JsonDateOnlyConverter());
        serializerOptions.Converters.Add(new JsonDateTimeConverter());
        serializerOptions.Converters.Add(new JsonDateTimeOffsetConverter());

        serializerOptions.TypeInfoResolver = serializerOptions.TypeInfoResolver?
            .WithAddedModifier(JsonPropertyResolver.AddEnumModifier)
            .WithAddedModifier(JsonPropertyResolver.AddIntegerModifier);

        return serializerOptions;
    }
}
