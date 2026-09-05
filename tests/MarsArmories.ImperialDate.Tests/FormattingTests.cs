using System.Globalization;
using Xunit;

namespace MarsArmories.ImperialDate.Tests;

public class FormattingTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("I")]
    [InlineData("i")]
    public void ImperialFormatIsStable(string? format)
    {
        var date = ImperialDate.From(3021119000);
        Assert.Equal("0 119 021.M3", date.ToString(format));
        Assert.Equal(date.ToString("I"), date.ToString());
        Assert.Equal(date.ToString("I"), $"{date:I}");
    }

    [Theory]
    [InlineData("o")]
    [InlineData("O")]
    [InlineData("Standard")]
    [InlineData("Imperial")]
    public void UnknownFormatsAreRejected(string format)
    {
        Assert.Throws<FormatException>(() => ImperialDate.MinValue.ToString(format));
    }

    [Theory]
    [InlineData(500, 24, 3, "00:00 02/07/2024")]
    [InlineData(500, 26, 3, "12:00 02/07/2026")]
    [InlineData(0, 0, 3, "00:00 01/01/2000")]
    [InlineData(0, 0, 0, "00:00 01/01/-1000")]
    [InlineData(999, 999, 999999, "15:14 31/12/999998999")]
    public void StandardFormatSupportsFullRange(int fraction, int year, int millennium, string expected)
    {
        Assert.Equal(expected, ImperialDate.From(CheckValue.Terra, fraction, year, millennium).ToString("s"));
    }

    [Fact]
    public void StandardFormatCanFallOnPreviousDay()
    {
        var date = ImperialDate.From(new DateTime(2026, 9, 5));
        Assert.Equal("17:45 04/09/2026", date.ToString("S"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1900)]
    [InlineData(2000)]
    [InlineData(2024)]
    [InlineData(2026)]
    [InlineData(9999)]
    public void EveryFractionFormatsToItsCalendarInterval(int year)
    {
        var start = new DateTime(year, 1, 1);
        long fractionTicks = (DateTime.IsLeapYear(year) ? 366L : 365L) * TimeSpan.TicksPerDay / 1000;
        for (int fraction = 0; fraction < 1000; fraction++)
        {
            var expected = start.AddTicks(fraction * fractionTicks);
            var date = ImperialDate.From(expected);
            Assert.Equal(expected.ToString("HH:mm dd/MM/", CultureInfo.InvariantCulture) + year, date.ToString("S"));
        }
    }

    [Fact]
    public void ExplicitNumericFormatProviderIsUsed()
    {
        var provider = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        provider.NegativeSign = "minus";
        Assert.Equal("00:00 01/01/minus1000", ImperialDate.MinValue.ToString("S", provider));
    }
}
