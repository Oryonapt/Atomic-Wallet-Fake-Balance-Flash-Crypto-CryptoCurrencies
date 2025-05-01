namespace Microsoft.AspNetCore.Mvc;

public class GlobalObjectResult
{
    public int Code { get; set; }

    public object? Value { get; set; }

    public string Message { get; set; } = default!;
}
