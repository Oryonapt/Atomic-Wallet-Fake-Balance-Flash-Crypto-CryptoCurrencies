using System.Data;

namespace Microsoft.EntityFrameworkCore;

public static class DataRecordExtension
{
    public static T GetValue<T>(this IDataRecord record, string name)
      => (T)record.GetValue(record.GetOrdinal(name));

    public static T? GetValueOrDefault<T>(this IDataRecord record, string name)
    {
        var idx = record.GetOrdinal(name);
        return record.IsDBNull(idx) ? default : (T)record.GetValue(idx);
    }

    public static T? GetValueOrDefault<T>(this IDataRecord record, string name, T defaultValue)
    {
        var idx = record.GetOrdinal(name);
        return record.IsDBNull(idx) ? defaultValue : (T)record.GetValue(idx);
    }
}
