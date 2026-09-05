using Xunit;

namespace MarsArmories.ImperialDate.Tests;

public class ConversionTests
{
    [Theory]
    [InlineData(1, 1, 1, 0, 1, 1)]
    [InlineData(1, 7, 1, 495, 1, 1)]
    [InlineData(2026, 1, 1, 0, 26, 3)]
    [InlineData(2026, 7, 2, 498, 26, 3)]
    [InlineData(2024, 7, 2, 500, 24, 3)]
    [InlineData(1900, 3, 1, 161, 900, 2)]
    [InlineData(2000, 3, 1, 163, 0, 3)]
    [InlineData(2100, 3, 1, 161, 100, 3)]
    [InlineData(1000, 1, 1, 0, 0, 2)]
    [InlineData(2000, 7, 1, 497, 0, 3)]
    public void DateTimeProducesExpectedComponents(int year, int month, int day, int fraction, int imperialYear, int millennium)
    {
        var actual = ImperialDate.From(new DateTime(year, month, day));
        Assert.Equal(ImperialDate.From(CheckValue.Terra, fraction, imperialYear, millennium), actual);
    }

    [Fact]
    public void DateTimeExtremesConvertWithoutSpecialCases()
    {
        Assert.Equal("0 000 001.M1", ImperialDate.From(DateTime.MinValue).ToString());
        Assert.Equal("00:00 01/01/1", ImperialDate.From(DateTime.MinValue).ToString("S"));
        Assert.Equal("0 999 999.M10", ImperialDate.From(DateTime.MaxValue).ToString());
    }

    [Theory]
    [InlineData(2026)]
    [InlineData(2024)]
    public void FractionBoundariesPreserveSubsecondPrecision(int year)
    {
        var start = new DateTime(year, 1, 1);
        long fractionTicks = (DateTime.IsLeapYear(year) ? 366L : 365L) * TimeSpan.TicksPerDay / 1000;
        for (int fraction = 1; fraction < 1000; fraction++)
        {
            var boundary = start.AddTicks(fraction * fractionTicks);
            Assert.Equal(fraction - 1, ImperialDate.From(boundary.AddTicks(-1)).YearFraction);
            Assert.Equal(fraction, ImperialDate.From(boundary).YearFraction);
        }
    }

    [Theory]
    [InlineData(999)]
    [InlineData(1999)]
    [InlineData(2999)]
    public void MillenniumRolloverPreservesOrdering(int year)
    {
        var next = new DateTime(year + 1, 1, 1);
        var before = ImperialDate.From(next.AddTicks(-1));
        var after = ImperialDate.From(next);
        Assert.True(before.CompareTo(after) < 0);
        Assert.Equal(0, after.Year);
        Assert.Equal(before.Millennium + 1, after.Millennium);
        Assert.Equal($"00:00 01/01/{year + 1}", after.ToString("S"));
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Utc)]
    [InlineData(DateTimeKind.Unspecified)]
    public void DateTimeUsesSuppliedClockFields(DateTimeKind kind)
    {
        Assert.Equal("0 500 024.M3", ImperialDate.From(new DateTime(2024, 7, 2, 0, 0, 0, kind)).ToString());
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(1000L)]
    [InlineData(1000000L)]
    [InlineData(1000000000L)]
    [InlineData(3020510001L)]
    [InlineData(999999999999009L)]
    public void PackedTicksRoundTrip(long ticks)
    {
        Assert.Equal(ticks, ImperialDate.From(ticks).Ticks);
    }

    [Fact]
    public void AllComponentBoundariesRoundTrip()
    {
        foreach (int millennium in new[] { 0, 1, 3, 2147, 2148, 999999 })
        {
            foreach (int year in new[] { 0, 1, 999 })
            {
                foreach (int fraction in new[] { 0, 1, 999 })
                {
                    foreach (CheckValue check in Enum.GetValues<CheckValue>())
                    {
                        var date = ImperialDate.From(check, fraction, year, millennium);
                        Assert.Equal(date, ImperialDate.From(date.Ticks));
                    }
                }
            }
        }
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    [InlineData(1000000000000000L)]
    public void OutOfRangeTicksAreRejected(long ticks)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ImperialDate.From(ticks));
    }

    [Theory]
    [InlineData(10L)]
    [InlineData(999L)]
    [InlineData(3020510010L)]
    public void InvalidPackedCheckIsRejected(long ticks)
    {
        Assert.Throws<ArgumentException>(() => ImperialDate.From(ticks));
    }
}
