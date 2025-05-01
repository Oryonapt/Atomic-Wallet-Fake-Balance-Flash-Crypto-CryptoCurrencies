namespace System
{
    public static class ArrayExtensions
    {
        public static string Join(this string?[] value, char separator)
            => string.Join(separator, value);

        public static string Join(this string?[] value, string? separator)
            => string.Join(separator, value);

        public static string Join(this string?[] value, char separator, int startIndex, int count)
            => string.Join(separator, value, startIndex, count);

        public static string Join(this string?[] value, string? separator, int startIndex, int count)
            => string.Join(separator, value, startIndex, count);

        public static string Join(this object?[] values, char separator)
            => string.Join(separator, values);

        public static string Join(this object?[] values, string? separator)
            => string.Join(separator, values);
    }
}
