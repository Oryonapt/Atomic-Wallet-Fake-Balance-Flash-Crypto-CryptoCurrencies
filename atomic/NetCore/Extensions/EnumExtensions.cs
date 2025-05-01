namespace System;

public static class EnumExtensions
{
    public static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        => enumValue.GetType().GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static)?.GetCustomAttribute<T>(false);

    public static string? GetDescription(this Enum enumValue)
        => enumValue.GetAttributeOfType<DescriptionAttribute>()?.Description;
}