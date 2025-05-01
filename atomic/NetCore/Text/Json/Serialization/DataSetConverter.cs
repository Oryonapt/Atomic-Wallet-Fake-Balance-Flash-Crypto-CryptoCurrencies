using System.Data;

namespace System.Text.Json.Serialization;

public sealed class DataSetConverter : JsonConverter<DataSet>
{
    public override DataSet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (DataTable table in value.Tables)
        {
            writer.WritePropertyName(
                options.PropertyNamingPolicy?.ConvertName(table.TableName) ?? table.TableName);
            JsonSerializer.Serialize(writer, table, options);
        }

        writer.WriteEndObject();
    }
}
