using System;

namespace MarsArmories.ImperialDate.Internal
{
    internal static class ImperialDateConverters
    {
        private const int SecondsInDay = 86400;
        private const int SecondsInHour = 3600;
        private const int SecondsInMinute = 60;
        private const int DaysInLeapYear = 366;
        private const int DaysInYear     = 365;

        internal static ImperialDate ConvertFromDateTime(DateTime date)
        {
            // Since DateTime is an earth construct, assume Terra
            const CheckValue checkValue = CheckValue.Terra;

            if (date.Equals(DateTime.MinValue))
            {
                return new ImperialDate(checkValue, 1, 1, 0);
            }

            var yearRemaining = date - date.LastDayOfYear();
            var day = Math.Floor(yearRemaining.TotalMilliseconds / (1000.00 * SecondsInDay));

            var daySeconds = (day * SecondsInDay) - SecondsInDay;
            var hourSeconds = date.Hour * SecondsInHour;
            var minuteSeconds = date.Minute * SecondsInMinute;
            var seconds = date.Second;

            var secondsElapsed = daySeconds + hourSeconds + minuteSeconds + seconds;


            var yearFraction = (int)Math.Floor(secondsElapsed / date.SecondsInYear() * 1000);

            var year = date.Year % 1000;

            var millennium = (int)Math.Ceiling(date.Year / 1000.00);

            return new ImperialDate(checkValue, yearFraction, year, millennium);
        }

        internal static ImperialDate ConvertFromTicks(long ticks)
        {
            var millennium = (int)(ticks / 1_000_000_000);
            var year = (int)(ticks / 1_000_000) - (millennium * 1_000);
            var yearFraction = (int)(ticks / 1_000) - (millennium * 1_000_000 + year * 1_000);
            var check = (int)(ticks - (millennium * 1_000_000_000 + year * 1_000_000 + yearFraction * 1_000));
            if (millennium < Constants.MinMillennium
                || year <= Constants.MinYear
                || yearFraction <= Constants.MinYearFraction
                || !Enum.IsDefined(typeof(CheckValue), check))
            {
                throw new ArgumentException("Invalid ticks format");
            }
            return new ImperialDate((CheckValue)check, yearFraction, year, millennium);
        }

        internal static long ConvertToTicks(ImperialDate date)
        {
            long ticks = 0;
            ticks += (long)date.Millennium * 1_000_000_000;
            ticks += (long)date.Year * 1_000_000;
            ticks += (long)date.YearFraction * 1_000;
            ticks += (long)date.Check;
            return ticks;
        }
    }
}
