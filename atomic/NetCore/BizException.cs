namespace System;

public class BizException : Exception
{
    public BizException()
    {
    }

    public BizException(string message) : base(message)
    {
    }

    public static void Throw(string message) => throw new BizException(message);

    public static void ThrowIfNull(object? argument, string message)
    {
        if (argument is null)
        {
            Throw(message);
        }
    }
}