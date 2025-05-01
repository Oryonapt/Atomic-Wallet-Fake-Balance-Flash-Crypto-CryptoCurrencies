using System.Globalization;

namespace System;

public readonly record struct DateCalculation
{
    public readonly Day Day { get; init; }

    public readonly Month Month { get; init; }

    public readonly Week Week { get; init; }

    public readonly Quarter Quarter { get; init; }

    public readonly Year Year { get; init; }

    public DateCalculation(DateTime current)
    {
        Day = new Day(current);
        Month = new Month(current.AddDays(1 - current.Day));
        Week = new Week(current.AddDays(1 - (int)current.DayOfWeek));
        Quarter = new Quarter(current.AddMonths(0 - (current.Month - 1) % 3).AddDays(1 - current.Day));
        Year = new Year(new DateTime(current.Year, 1, 1));
    }
};

public readonly record struct Day(DateTime Start)
{
    public readonly DateTime Start { get; init; } = Start.Date;

    public readonly int DayOfYear { get; init; } = Start.DayOfYear;

    public readonly DayOfWeek DayOfWeek { get; init; } = Start.DayOfWeek;

    public readonly DateTime End { get; init; } = Start.Date.AddDays(1).AddSeconds(-1);
};

public readonly record struct Week(DateTime Start)
{
    public readonly DateTime Start { get; init; } = Start.Date;

    public readonly int WeekOfYear { get; init; } = new GregorianCalendar().GetWeekOfYear(Start, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

    public readonly DateTime End { get; init; } = Start.Date.AddDays(1 - (int)Start.DayOfWeek).AddDays(7).AddSeconds(-1);
}

public readonly record struct Month(DateTime Start)
{
    public readonly DateTime Start { get; init; } = Start.Date;

    public readonly DateTime End { get; init; } = Start.Date.AddDays(1 - Start.Day).AddMonths(1).AddSeconds(-1);
}

public readonly record struct Quarter(DateTime Start)
{
    public readonly DateTime Start { get; init; } = Start.Date;

    public readonly int QuarterOfYear { get; init; } = Start.Month / 3 + 1;

    public readonly DateTime End { get; init; } = Start.Date.AddMonths(0 - (Start.Month - 1) % 3).AddDays(1 - Start.Day).AddMonths(3).AddSeconds(-1);
}

public readonly record struct Year(DateTime Start)
{
    public readonly DateTime End { get; init; } = Start.AddYears(1).AddSeconds(-1).Date;
}