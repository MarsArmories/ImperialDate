using System;

namespace MarsArmories.ImperialDate.Internal
{
    internal static class DateTimeExtensions
    {
        internal static int SecondsInYear(this DateTime date) => DateTime.IsLeapYear(date.Year) ? 31622400 : 31536000;
        internal static DateTime LastDayOfYear(this DateTime date) => new DateTime(date.Year, 1, 1).AddDays(-1);
    }

}
