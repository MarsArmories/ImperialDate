using Xunit;

namespace MarsArmories.ImperialDate.Tests;

public class ImperialDateTests
{
    [Fact]
    public void DefaultValueIsMinimum()
    {
        Assert.Equal(default, ImperialDate.MinValue);
        Assert.Equal(0, ImperialDate.MinValue.Ticks);
        Assert.Equal("0 000 000.M0", ImperialDate.MinValue.ToString());
        Assert.Equal("9 999 999.M999999", ImperialDate.MaxValue.ToString());
    }

    [Theory]
    [InlineData(-1, 0, 0, 0)]
    [InlineData(10, 0, 0, 0)]
    [InlineData(0, -1, 0, 0)]
    [InlineData(0, 1000, 0, 0)]
    [InlineData(0, 0, -1, 0)]
    [InlineData(0, 0, 1000, 0)]
    [InlineData(0, 0, 0, -1)]
    [InlineData(0, 0, 0, 1000000)]
    [InlineData(0, 0, 0, int.MaxValue)]
    public void InvalidComponentsAreRejected(int check, int fraction, int year, int millennium)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ImperialDate.From((CheckValue)check, fraction, year, millennium));
    }

    [Fact]
    public void EqualityHashingAndComparisonAgree()
    {
        var date = ImperialDate.From(CheckValue.Terra, 510, 20, 3);
        var copy = ImperialDate.From(date.Ticks);
        var laterCheck = ImperialDate.From(CheckValue.Sol, 510, 20, 3);
        var laterFraction = ImperialDate.From(CheckValue.Terra, 511, 20, 3);
        Assert.True(date == copy);
        Assert.False(date != copy);
        Assert.True(date.Equals((object)copy));
        Assert.True(ImperialDate.Equals(date, copy));
        Assert.Equal(date.GetHashCode(), copy.GetHashCode());
        Assert.Contains(copy, new HashSet<ImperialDate> { date });
        Assert.Equal(0, date.CompareTo(copy));
        Assert.Equal(0, date.CompareTo((object)copy));
        Assert.True(date != laterCheck);
        Assert.True(date.CompareTo(laterCheck) < 0);
        Assert.True(laterFraction.CompareTo(laterCheck) > 0);
        Assert.True(ImperialDate.MinValue.CompareTo(ImperialDate.MaxValue) < 0);
        Assert.True(ImperialDate.MaxValue.CompareTo(ImperialDate.MinValue) > 0);
        Assert.False(date.Equals(null));
        Assert.False(date.Equals("date"));
        Assert.Equal(1, date.CompareTo(null));
        Assert.Throws<ArgumentException>(() => date.CompareTo("date"));
    }

    [Fact]
    public void NowFallsBetweenSurroundingClockReads()
    {
        var before = ImperialDate.From(DateTime.Now);
        var now = ImperialDate.Now;
        var after = ImperialDate.From(DateTime.Now);
        Assert.InRange(now, before, after);
    }
}
