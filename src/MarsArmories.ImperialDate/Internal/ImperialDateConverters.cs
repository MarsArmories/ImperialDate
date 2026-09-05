namespace MarsArmories.ImperialDate.Internal;

internal static class ImperialDateConverters
{
    private const long MillenniumScale = 1_000_000_000;
    private const long YearScale = 1_000_000;
    private const long FractionScale = 1_000;
    private const long MaxTicks = Constants.MaxMillennium * MillenniumScale + Constants.MaxYear * YearScale + Constants.MaxYearFraction * FractionScale + (int)CheckValue.Warp;

    internal static ImperialDate ConvertFromDateTime(DateTime date)
    {
        int year = date.Year;
        var startOfYear = new DateTime(year, 1, 1);
        var daysInYear = DateTime.IsLeapYear(year) ? Constants.DaysInYearLeap : Constants.DaysInYear;
        long ticksInYear = daysInYear * TimeSpan.TicksPerDay;
        // Integer arithmetic preserves subsecond precision at fraction boundaries.
        var yearFraction = (int)((date.Ticks - startOfYear.Ticks) * 1000 / ticksInYear);

        return ImperialDate.From(CheckValue.Terra, yearFraction, year % 1000, year / 1000 + 1);
    }

    internal static ImperialDate ConvertFromTicks(long ticks)
    {
        if (ticks < 0 || ticks > MaxTicks)
        {
            throw new ArgumentOutOfRangeException(nameof(ticks), ticks, "Ticks are outside the supported Imperial date range.");
        }

        var millennium = (int)(ticks / MillenniumScale);
        var year = (int)(ticks / YearScale % 1000);
        var yearFraction = (int)(ticks / FractionScale % 1000);
        var check = (int)(ticks % FractionScale);
        if (check > (int)CheckValue.Warp)
        {
            throw new ArgumentException("The check component of ticks must be between 0 and 9.", nameof(ticks));
        }

        return ImperialDate.From((CheckValue)check, yearFraction, year, millennium);
    }

    internal static long ConvertToTicks(ImperialDate date) =>
        date.Millennium * MillenniumScale + date.Year * YearScale + date.YearFraction * FractionScale + (int)date.Check;
}
