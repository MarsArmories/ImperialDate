using System;

namespace MarsArmories.ImperialDate
{
    using MarsArmories.ImperialDate.Internal;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Auto)]
    public struct ImperialDate : IComparable, IFormattable, IComparable<ImperialDate>, IEquatable<ImperialDate>
    {
        public CheckValue Check { get; private set; }
        public int YearFraction { get; private set; }
        public int Year { get; private set; }
        public int Millennium { get; private set; }

        public long Ticks => ImperialDateConverters.ConvertToTicks(this);

        public static ImperialDate MinValue => new ImperialDate(0, Constants.MinYearFraction, Constants.MinYear, Constants.MinMillenium);

        public static ImperialDate MaxValue => new ImperialDate(CheckValue.Warp, Constants.MaxYearFraction, Constants.MaxYear, Constants.MaxMillenium);

        public static ImperialDate Now => new ImperialDate(DateTime.Now);

        internal ImperialDate(CheckValue check, int yearFraction, int year, int millennium)
        {
            if (!Enum.IsDefined(typeof(CheckValue), check))
            {
                throw new ArgumentException("Invalid value for CheckValue", nameof(CheckValue));
            }
            if (yearFraction <= 0 || yearFraction > Constants.MaxYearFraction)
            {
                throw new ArgumentException("YearFraction must be between 0 and 999", nameof(YearFraction));
            }
            if (year <= 0 || year > Constants.MaxYear)
            {
                throw new ArgumentException("Year must be between 0 and 999", nameof(Year));
            }
            if (millennium < 0)
            {
                throw new ArgumentException("Millenium must be greater than or equal to 0", nameof(Millennium));
            }
            Check = check;
            YearFraction = yearFraction;
            Year = year;
            Millennium = millennium;
        }

        private ImperialDate(DateTime date)
        {
            var imperialDate = ImperialDateConverters.ConvertFromDateTime(date);
            Check = imperialDate.Check;
            YearFraction = imperialDate.YearFraction;
            Year = imperialDate.Year;
            Millennium = imperialDate.Millennium;
        }

        public static ImperialDate From(CheckValue checkValue, int yearFraction, int year, int millennium) => new ImperialDate(checkValue, yearFraction, year, millennium);

        public static ImperialDate From(DateTime dateTime) => new ImperialDate(dateTime);

        public static ImperialDate From(long ticks) => ImperialDateConverters.ConvertFromTicks(ticks);

        public override string ToString() => $"{(int)Check} {YearFraction:000} {Year:000}.M{Millennium}";

        public string ToString(string format, IFormatProvider provider = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "I";
            }

            // Currently unused, will be useful for culture aware Standard date formatting
            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }

            // TODO: Improve formatting to handle better more culture-aware formatting
            switch (format.ToUpperInvariant())
            {
                case "S":
                    var (year, month, day, hour, minute) = ToStandardFormat();
                    return $"{hour:00}:{minute:00} {day:00}/{month:00}/{year}";
                case "I":
                    return ToString();
                default:
                    throw new FormatException($"The {format} format string is not supported.");
            }
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return 1;

                case ImperialDate date:
                    return CompareTo(date);

                default:
                    throw new ArgumentException("Argument must be an ImperialDate");
            }
        }

        public int CompareTo(ImperialDate date)
        {
            if (date.Ticks < Ticks)
            {
                return 1;
            }
            if (date.Ticks > Ticks)
            {
                return -1;
            }
            return 0;
        }

        public override bool Equals(object value) => value is ImperialDate date && Ticks == date.Ticks;

        public bool Equals(ImperialDate value) => Ticks == value.Ticks;

        public static bool Equals(ImperialDate date1, ImperialDate date2) => date1.Ticks == date2.Ticks;

        public override int GetHashCode() => HashCode.Combine(Check, YearFraction, Year, Millennium);

        private (int year, int month, int day, int hour, int minute) ToStandardFormat()
        {
            int standardYear = (Millennium - 1)* 1000 + Year;
            bool IsLeapYear(int year) => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
            int Days(int year) => IsLeapYear(year) ? Constants.DaysInYearLeap : Constants.DaysInYear;

            int daysInYear = Days(standardYear);

            long maxSeconds = daysInYear * Constants.SecondsInDay;
            var currentSeconds = YearFraction * maxSeconds / 1000d;
            var days = currentSeconds / Constants.SecondsInDay;
            var secondsRemaining = currentSeconds % Constants.SecondsInDay;
            var hour = secondsRemaining / Constants.SecondsInHour;
            secondsRemaining %= Constants.SecondsInHour;
            var minute = secondsRemaining / 60;

            int[] monthDays = IsLeapYear(standardYear) ? Constants.DaysToMonth366 : Constants.DaysToMonth365;

            int month = ((int)days >> 5) + 1;

            while ((int)days >= monthDays[month])
            {
                month++;
            }
            var day = days - monthDays[month - 1] + 1;
            return (standardYear, month, (int)day, (int)hour, (int)minute);
        }
    }
}