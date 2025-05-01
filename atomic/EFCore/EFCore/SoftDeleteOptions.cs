namespace Microsoft.EntityFrameworkCore;

public sealed class SoftDeleteOptions(string name, string? comment)
{
    public SoftDeleteOptions() : this("Deleted", string.Empty)
    {
    }

    public string Name { get; init; } = name;

    public string? Comment { get; init; } = comment;

    public int Order { get; init; } = 100;

    public bool Enabled { get; init; } = false;
}
