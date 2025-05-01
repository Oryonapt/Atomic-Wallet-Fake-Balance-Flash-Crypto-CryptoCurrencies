namespace System;

public static partial class DateTimeExtensions
{
    public static bool HasOverlap(this DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        => start1 <= end2 && end1 >= start2;

    public static DateCalculation Calculation(this DateTime dateTime) => new(dateTime);
}
