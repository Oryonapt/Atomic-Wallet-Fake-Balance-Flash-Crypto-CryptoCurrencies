namespace Microsoft.AspNetCore.Mvc;

public class ExceptionResult
{
    public int Code { get; set; }

    public string Message { get; set; } = default!;
}