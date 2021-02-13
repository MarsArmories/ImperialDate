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
            var checkNumber = 0;

            if (date.Equals(DateTime.MinValue))
            {
                return new ImperialDate((CheckValue)checkNumber, 1, 1, 0);
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

            var millenium = (int)Math.Ceiling(date.Year / 1000.00);

            return new ImperialDate((CheckValue)checkNumber, yearFraction, year, millenium);
        }

        internal static ImperialDate ConvertFromTicks(long ticks)
        {
            int millenium = (int)(ticks / 1_000_000_000);
            int year = (int)(ticks / 1_000_000) - (millenium * 1_000);
            int yearFraction = (int)(ticks / 1_000) - (millenium * 1_000_000 + year * 1_000);
            int check = (int)(ticks - (millenium * 1_000_000_000 + year * 1_000_000 + yearFraction * 1_000));
            if (millenium < Constants.MinMillenium
                || year <= Constants.MinYear
                || yearFraction <= Constants.MinYearFraction
                || !Enum.IsDefined(typeof(CheckValue), check))
            {
                throw new ArgumentException("Invalid ticks format");
            }
            return new ImperialDate((CheckValue)check, yearFraction, year, millenium);
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
