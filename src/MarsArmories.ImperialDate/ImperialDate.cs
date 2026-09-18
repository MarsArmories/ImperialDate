namespace MarsArmories.ImperialDate;

using Internal;
using System.Globalization;
using System.Runtime.InteropServices;

/// <summary>Represents an immutable Imperial date with a source check and thousandth-of-year precision.</summary>
/// <remarks>
/// The standard year is <c>(Millennium - 1) * 1000 + Year</c>.
/// Equality and ordering include the check value. The default value equals <see cref="MinValue"/>.
/// </remarks>
[StructLayout(LayoutKind.Auto)]
public readonly struct ImperialDate : IComparable, IFormattable, IComparable<ImperialDate>, IEquatable<ImperialDate>
{
    private static readonly ImperialDate Maximum = new(CheckValue.Warp, Constants.MaxYearFraction, Constants.MaxYear, Constants.MaxMillennium);

    /// <summary>Gets the source classification, from Terra (0) through Warp (9).</summary>
    public CheckValue Check { get; }
    /// <summary>Gets the zero-based thousandth of the year, from 0 through 999.</summary>
    public int YearFraction { get; }
    /// <summary>Gets the year component within the millennium, from 0 through 999.</summary>
    public int Year { get; }
    /// <summary>Gets the millennium component, from 0 through 999,999.</summary>
    public int Millennium { get; }

    /// <summary>Gets the reversible packed integer representation of this date.</summary>
    /// <remarks>
    /// Encoding: <c>Millennium * 1,000,000,000 + Year * 1,000,000 + YearFraction * 1,000 + Check</c>.
    /// These are not <see cref="DateTime.Ticks"/> and do not measure elapsed time.
    /// </remarks>
    public long Ticks => ImperialDateConverters.ConvertToTicks(this);

    /// <summary>Gets the minimum date, <c>0 000 000.M0</c>, whose standard year is -1000.</summary>
    public static ImperialDate MinValue => default;

    /// <summary>Gets the maximum date, <c>9 999 999.M999999</c>.</summary>
    public static ImperialDate MaxValue => Maximum;

    /// <summary>Gets the current local date and time converted to an Imperial date on Terra.</summary>
    /// <remarks>For UTC, pass <see cref="DateTime.UtcNow"/> to <see cref="From(DateTime)"/>.</remarks>
    public static ImperialDate Now => From(DateTime.Now);

    private ImperialDate(CheckValue checkValue, int yearFraction, int year, int millennium)
    {
        // CheckValue is a contiguous range; a range check avoids boxing the enum.
        if (checkValue is < CheckValue.Terra or > CheckValue.Warp)
        {
            throw new ArgumentOutOfRangeException(nameof(checkValue), checkValue, "Check must be between 0 and 9.");
        }
        if (yearFraction is < Constants.MinYearFraction or > Constants.MaxYearFraction)
        {
            throw new ArgumentOutOfRangeException(nameof(yearFraction), yearFraction, "Year fraction must be between 0 and 999.");
        }
        if (year is < Constants.MinYear or > Constants.MaxYear)
        {
            throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be between 0 and 999.");
        }
        if (millennium is < Constants.MinMillennium or > Constants.MaxMillennium)
        {
            throw new ArgumentOutOfRangeException(nameof(millennium), millennium, "Millennium must be between 0 and 999999.");
        }
        Check = checkValue;
        YearFraction = yearFraction;
        Year = year;
        Millennium = millennium;
    }

    /// <summary>Creates a date from its four Imperial components.</summary>
    /// <param name="checkValue">A source classification from 0 through 9.</param>
    /// <param name="yearFraction">A thousandth of the year, from 0 through 999.</param>
    /// <param name="year">A year component from 0 through 999.</param>
    /// <param name="millennium">A millennium component from 0 through 999,999.</param>
    /// <returns>The specified Imperial date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A component is outside its supported range.</exception>
    public static ImperialDate From(CheckValue checkValue, int yearFraction, int year, int millennium) => new(checkValue, yearFraction, year, millennium);

    /// <summary>Converts a Gregorian date to an Imperial date on Terra.</summary>
    /// <param name="dateTime">The date and clock fields to convert, without time-zone normalization.</param>
    /// <returns>A date rounded down to the containing thousandth of the year.</returns>
    /// <remarks>
    /// Leap years are respected. The original timestamp and <see cref="DateTime.Kind"/> are not retained.
    /// The entire <see cref="DateTime"/> range is supported; 2000 maps to <c>000.M3</c>.
    /// </remarks>
    public static ImperialDate From(DateTime dateTime) => ImperialDateConverters.ConvertFromDateTime(dateTime);

    /// <summary>Restores a date from the packed encoding exposed by <see cref="Ticks"/>.</summary>
    /// <param name="ticks">Packed Imperial ticks, not <see cref="DateTime.Ticks"/>.</param>
    /// <returns>The decoded Imperial date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is outside the minimum and maximum packed ticks.</exception>
    /// <exception cref="ArgumentException">The packed check component is greater than 9.</exception>
    public static ImperialDate From(long ticks) => ImperialDateConverters.ConvertFromTicks(ticks);

    /// <summary>Formats this date as <c>C FFF YYY.MN</c> using invariant culture.</summary>
    /// <returns>The Imperial representation, such as <c>0 500 024.M3</c>.</returns>
    public override string ToString() => ToString("I", CultureInfo.InvariantCulture);

    /// <summary>Formats this date in Imperial or standard calendar notation.</summary>
    /// <param name="format"><c>I</c>, null, or empty for Imperial notation; <c>S</c> for <c>HH:mm dd/MM/year</c>. Letters are case-insensitive.</param>
    /// <param name="provider">The numeric format provider, or null for invariant culture. Separators and field order are fixed.</param>
    /// <returns>The formatted date.</returns>
    /// <remarks>
    /// Standard notation shows the interval start, truncating seconds, and may precede the original calendar day.
    /// Gregorian leap-year rules extend to years outside the <see cref="DateTime"/> range, including zero and negative years.
    /// </remarks>
    /// <exception cref="FormatException">The format is unsupported.</exception>
    public string ToString(string? format, IFormatProvider? provider = null)
    {
        provider ??= CultureInfo.InvariantCulture;
        if (string.IsNullOrEmpty(format) || format is "I" or "i")
        {
            return string.Create(provider, $"{(int)Check} {YearFraction:000} {Year:000}.M{Millennium}");
        }
        if (format is "S" or "s")
        {
            var (year, month, day, hour, minute) = ToStandardFormat();
            return string.Create(provider, $"{hour:00}:{minute:00} {day:00}/{month:00}/{year}");
        }
        throw new FormatException($"The {format} format string is not supported.");
    }

    /// <summary>Compares this date with another Imperial date; a null object sorts first.</summary>
    /// <param name="obj">An Imperial date or null.</param>
    /// <returns>A negative value, zero, or a positive value when this date is earlier, equal, or later.</returns>
    /// <exception cref="ArgumentException">The object is not an Imperial date or null.</exception>
    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        ImperialDate date => CompareTo(date),
        _ => throw new ArgumentException("Argument must be an ImperialDate", nameof(obj))
    };

    /// <summary>Compares dates by millennium, year, fraction, then source check.</summary>
    /// <param name="date">The date to compare with this instance.</param>
    /// <returns>A negative value, zero, or a positive value when this date is earlier, equal, or later.</returns>
    public int CompareTo(ImperialDate date) => Ticks.CompareTo(date.Ticks);

    /// <summary>Determines whether an object is an Imperial date with the same four components.</summary>
    /// <param name="value">The object to compare.</param>
    /// <returns>True if the object is an equal Imperial date; otherwise false.</returns>
    public override bool Equals(object? value) => value is ImperialDate date && Equals(date);

    /// <summary>Determines whether another date has the same four components.</summary>
    /// <param name="value">The date to compare.</param>
    /// <returns>True if all components match; otherwise false.</returns>
    public bool Equals(ImperialDate value) => Ticks == value.Ticks;

    /// <summary>Determines whether two dates have the same four components.</summary>
    /// <param name="date1">The first date.</param>
    /// <param name="date2">The second date.</param>
    /// <returns>True if all components match; otherwise false.</returns>
    public static bool Equals(ImperialDate date1, ImperialDate date2) => date1.Equals(date2);

    /// <summary>Gets a hash code based on all four date components.</summary>
    /// <returns>A hash code suitable for hash-based collections, not persistent storage.</returns>
    public override int GetHashCode() => HashCode.Combine(Check, YearFraction, Year, Millennium);

    private (int year, int month, int day, int hour, int minute) ToStandardFormat()
    {
        int standardYear = (Millennium - 1) * 1000 + Year;
        bool isLeapYear = standardYear % 4 == 0 && (standardYear % 100 != 0 || standardYear % 400 == 0);
        int daysInYear = isLeapYear ? Constants.DaysInYearLeap : Constants.DaysInYear;
        long currentSeconds = (long)YearFraction * daysInYear * Constants.SecondsInDay / 1000;
        int days = (int)(currentSeconds / Constants.SecondsInDay);
        int secondsRemaining = (int)(currentSeconds % Constants.SecondsInDay);
        int hour = secondsRemaining / Constants.SecondsInHour;
        int minute = secondsRemaining % Constants.SecondsInHour / 60;
        int[] monthDays = isLeapYear ? Constants.DaysToMonth366 : Constants.DaysToMonth365;
        int month = (days >> 5) + 1;
        while (days >= monthDays[month])
        {
            month++;
        }
        return (standardYear, month, days - monthDays[month - 1] + 1, hour, minute);
    }

    /// <summary>Determines whether two dates have equal components, including their source checks.</summary>
    /// <param name="left">The first date.</param>
    /// <param name="right">The second date.</param>
    /// <returns>True if the dates are equal; otherwise false.</returns>
    public static bool operator ==(ImperialDate left, ImperialDate right) => left.Equals(right);

    /// <summary>Determines whether any component of two dates differs.</summary>
    /// <param name="left">The first date.</param>
    /// <param name="right">The second date.</param>
    /// <returns>True if the dates differ; otherwise false.</returns>
    public static bool operator !=(ImperialDate left, ImperialDate right) => !left.Equals(right);
}
