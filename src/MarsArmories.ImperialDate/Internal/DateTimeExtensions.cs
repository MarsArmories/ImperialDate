namespace MarsArmories.ImperialDate.Internal;

internal static class DateTimeExtensions
{
    extension(DateTime date)
    {
        internal int SecondsInYear() => DateTime.IsLeapYear(date.Year) ? 31622400 : 31536000;
        internal DateTime LastDayOfYear() => new DateTime(date.Year, 1, 1).AddDays(-1);
    }
}