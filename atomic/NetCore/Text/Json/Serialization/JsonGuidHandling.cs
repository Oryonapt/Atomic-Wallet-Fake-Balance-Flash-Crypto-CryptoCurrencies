namespace System.Text.Json.Serialization;

[Flags]
public enum JsonGuidHandling
{
    /// <summary>
    /// 00000000000000000000000000000000
    /// </summary>
    Digits = 0x1,

    /// <summary>
    /// 00000000-0000-0000-0000-000000000000
    /// </summary>
    Hyphens = 0x2,

    /// <summary>
    /// {00000000-0000-0000-0000-000000000000}
    /// </summary>
    Braces = 0x4,

    /// <summary>
    /// (00000000-0000-0000-0000-000000000000)
    /// </summary>
    Parentheses = 0x8,

    /// <summary>
    /// {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
    /// </summary>
    Hexadecimal = 0xA,
}