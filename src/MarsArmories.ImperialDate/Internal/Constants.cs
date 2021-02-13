using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("MarsArmories.ImperialDate.Tests")]
namespace MarsArmories.ImperialDate.Internal
{
    internal static class Constants
    {
        internal const int MinYearFraction = 1;
        internal const int MinYear = 1;
        internal const int MinMillenium = 0;
        internal const int MaxYearFraction = 999;
        internal const int MaxYear = 999;
        internal const int MaxMillenium = 999_999;

        internal const int SecondsInHour = 3600;
        internal const int SecondsInDay = 86400;
        internal const int DaysInYearLeap = 366;
        internal const int DaysInYear = 365;

        internal static readonly int[] DaysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        internal static readonly int[] DaysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
    }
}
