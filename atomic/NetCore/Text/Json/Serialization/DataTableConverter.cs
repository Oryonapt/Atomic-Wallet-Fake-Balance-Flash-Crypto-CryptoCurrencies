using System.Data;

namespace System.Text.Json.Serialization;

public sealed class DataTableConverter : JsonConverter<DataTable>
{
    public override DataTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (DataRow row in value.Rows)
        {
            writer.WriteStartObject();
            foreach (DataColumn column in row.Table.Columns)
            {
                writer.WritePropertyName(
                    options.PropertyNamingPolicy?.ConvertName(column.ColumnName) ?? column.ColumnName);
                JsonSerializer.Serialize(writer, row[column], options);
            }
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
