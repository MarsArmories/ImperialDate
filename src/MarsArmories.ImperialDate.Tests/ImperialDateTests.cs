using FluentAssertions;
using FluentAssertions.Common;
using System;
using System.Globalization;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

namespace MarsArmories.ImperialDate.Tests
{
    public class ImperialDateTests
    {
        private readonly ITestOutputHelper _output;

        public ImperialDateTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void Now_IsNotNull()
        {
            var current = ImperialDate.Now;
            _output.WriteLine($"Testing {current}");
            current.Should().NotBeNull();
        }

        [Fact]
        public void From_DateTime_Now_IsNotNull()
        {
            var current = ImperialDate.From(DateTime.Now);
            _output.WriteLine($"Testing {current}");
            current.Should().NotBeNull();
        }

        [Fact]
        public void From_DateTime_Now_Should_Equal_Now()
        {
            var dateTimeNow = ImperialDate.From(DateTime.Now);
            var now = ImperialDate.Now;

            _output.WriteLine($"Testing Now: {now} vs UTCNow: {dateTimeNow}");

            now.Should().Be(dateTimeNow);
        }

        [Fact]
        public void DateTime_MinValue_IsValid()
        {
            var minValue = ImperialDate.From(DateTime.MinValue);
            _output.WriteLine($"Testing {minValue}");
            minValue.Should().NotBeNull();
        }

        [Fact]
        public void DateTime_MaxValue_IsValid()
        {
            var maxValue = ImperialDate.From(DateTime.MaxValue);
            _output.WriteLine($"Testing {maxValue}");
            maxValue.Should().NotBeNull();
        }

        [Fact]
        public void Valid_Ticks_Should_Not_Throw_Errors()
        {
            long dateAsTicks = 003_020_510_001;
            var expected = new ImperialDate(CheckValue.Sol, 510, 20, 3);
            _output.WriteLine($"Testing Ticks: {dateAsTicks} vs {expected}");
            Invoking(() => ImperialDate.From(dateAsTicks)).Should().NotThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1_000)]
        [InlineData(1_000_000_000)]
        public void Invalid_Ticks_Should_Throw_Exception(long badDateAsTicks)
        {
            var expected = new ImperialDate(CheckValue.Sol, 510, 20, 3);
            _output.WriteLine($"Testing Ticks: {badDateAsTicks} vs {expected}");
            Invoking(() => ImperialDate.From(badDateAsTicks)).Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Date_In_Ticks_Should_Equal_Date()
        {
            long dateAsTicks = 003_020_510_001;
            var expected = new ImperialDate(CheckValue.Sol, 510, 20, 3);
            _output.WriteLine($"Testing Ticks: {dateAsTicks} vs {expected}");
            var imperialDateFromTicks = ImperialDate.From(dateAsTicks);
            imperialDateFromTicks.Should().Be(expected);
        }

        [Theory]
        [InlineData(411111)]
        [InlineData(1000)]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-10000)]
        public void ImperialDate_Should_Not_Accept_Invalid_Years(int year)
            => Invoking(() => ImperialDate.From(0, 1, year, 1)).Should().ThrowExactly<ArgumentException>();

        [Theory]
        [InlineData(411111)]
        [InlineData(1000)]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-10000)]
        public void ImperialDate_Should_Not_Accept_Invalid_YearFractions(int yearFraction)
            => Invoking(() => ImperialDate.From(0, yearFraction, 1, 1)).Should().ThrowExactly<ArgumentException>();

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-10000)]
        public void ImperialDate_Should_Not_Accept_Invalid_Millenniums(int millennium) => Invoking(()
            => ImperialDate.From(0, 1, 1, millennium)).Should().ThrowExactly<ArgumentException>();

        [Theory]
        [InlineData(11)]
        [InlineData(10)]
        [InlineData(-1)]
        [InlineData(-2)]
        public void ImperialDate_Should_Not_Accept_Invalid_CheckValue(int checkValue)
        {
            _output.WriteLine($"CheckValue is {(CheckValue)checkValue}");
            Invoking(() => ImperialDate.From((CheckValue)checkValue, 1, 1, 1)).Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ImperialDate_MaxValue_Is_Not_Null()
        {
            var maxValue = ImperialDate.MaxValue;
            _output.WriteLine($"Testing {maxValue}");
            maxValue.Should().NotBeNull();
        }

        [Fact]
        public void ImperialDate_MaxValue_Year_Should_Be_999()
        {
            var maxValue = ImperialDate.MaxValue;
            _output.WriteLine($"Testing {maxValue}");
            maxValue.Year.Should().Be(999);
        }

        [Fact]
        public void ImperialDate_MaxValue_YearFraction_Should_Be_999()
        {
            var maxValue = ImperialDate.MaxValue;
            _output.WriteLine($"Testing {maxValue}");
            maxValue.YearFraction.Should().Be(999);
        }

        [Fact]
        public void ImperialDate_MaxValue_Millennium_Should_Be_999_999()
        {
            var maxValue = ImperialDate.MaxValue;
            maxValue.Millennium.Should().Be(Internal.Constants.MaxMillenium);
        }

        [Fact]
        public void ImperialDate_MinValue_Is_Not_Null()
        {
            var minValue = ImperialDate.MinValue;
            _output.WriteLine($"Testing {minValue}");
            minValue.Should().NotBeNull();
        }

        [Fact]
        public void ImperialDate_MinValue_Year_Should_Be_1()
        {
            var minValue = ImperialDate.MinValue;
            minValue.Year.Should().Be(1);
        }

        [Fact]
        public void ImperialDate_MinValue_YearFraction_Should_Be_1()
        {
            var minValue = ImperialDate.MinValue;
            minValue.YearFraction.Should().Be(1);
        }

        [Fact]
        public void ImperialDate_MinValue_Millennium_Should_Be_0()
        {
            var minValue = ImperialDate.MinValue;
            minValue.Millennium.Should().Be(0);
        }

        [Fact]
        public void ImperialDate_MinValue_Should_Be_Less_Than_MaxValue()
        {
            var minValue = ImperialDate.MinValue;
            var maxValue = ImperialDate.MaxValue;
            _output.WriteLine($"Testing minValue: {minValue} vs maxValue: {maxValue}");
            minValue.Should().BeLessThan(maxValue);
        }

        [Fact]
        public void ImperialDate_MaxValue_Should_Be_Greater_Than_MinValue()
        {
            var minValue = ImperialDate.MinValue;
            var maxValue = ImperialDate.MaxValue;
            _output.WriteLine($"Testing minValue: {minValue} vs maxValue: {maxValue}");
            _output.WriteLine($"MinValue: {minValue} has ticks: {minValue.Ticks}");
            _output.WriteLine($"MaxValue: {maxValue} has ticks: {maxValue.Ticks}");
            maxValue.Should().BeGreaterThan(minValue);
        }

        [Fact]
        public void ImperialDate_Should_Greater_Than_Equal_Itself()
        {
            var now = ImperialDate.Now;
            var otherNow = ImperialDate.From(DateTime.Now);
            _output.WriteLine($"Testing now: {now} vs otherNow: {otherNow}");
            now.Should().BeGreaterOrEqualTo(otherNow);
        }

        [Fact]
        public void ImperialDate_Should_Less_Than_Equal_Itself()
        {
            var now = ImperialDate.Now;
            var otherNow = ImperialDate.From(DateTime.Now);
            _output.WriteLine($"Testing now: {now} vs otherNow: {otherNow}");
            now.Should().BeLessOrEqualTo(otherNow);
        }

        [Fact]
        public void ImperialDate_Should_Not_Equal_Null()
        {
            var now = ImperialDate.Now;
            now.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void ImperialDate_Should_Not_Compare_To_Null()
        {
            var minValue = ImperialDate.MinValue;
            // Returning 1 here means that this comes after null
            minValue.CompareTo(null).Should().Be(1);
        }

        [Fact]
        public void ImperialDate_ObjectCompare_To_Itself_Should_Be_Equal()
        {
            var now = ImperialDate.Now;
            object otherNow = ImperialDate.From(DateTime.Now);
            _output.WriteLine($"Testing now: {now} vs otherNow: {otherNow}");
            // 0 here means that these objects are equal
            now.CompareTo(otherNow).Should().Be(0);
        }

        [Fact]
        public void ImperialDate_Should_Throw_Exception_When_Comparing_Other_Objects()
        {
            var object1 = ImperialDate.Now;
            object object2 = new();
            _output.WriteLine($"Testing object1 tpe: {object1.GetType()} vs object2 type: {object2.GetType()}");
            Invoking(() => object1.CompareTo(object2)).Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ImperialDate_Should_Equal_Itself()
        {
            var now = ImperialDate.Now;
            now.Equals(now).Should().BeTrue();
        }

        [Fact]
        public void ImperialDate_Should_Equal_Itself_EqualsMethod()
        {
            var now = ImperialDate.Now;
            ImperialDate.Equals(now, ImperialDate.Now).Should().BeTrue();
        }

        [Fact]
        public void ImperialDate_NowAndDateTime_Now_Should_Have_Same_HashCode()
        {
            var now = ImperialDate.Now;
            object otherNow = ImperialDate.From(DateTime.Now);
            _output.WriteLine($"Testing now: {now} vs otherNow: {otherNow}");
            now.GetHashCode().Should().IsSameOrEqualTo(otherNow.GetHashCode());
        }

        [Fact]
        public void ImperialDate_Now_Should_Return_Correct_Standard_Format()
        {
            var now = ImperialDate.Now;
            var standardFormat = DateTime.Now.ToString("hh:mm dd/MM/yyyy", CultureInfo.InvariantCulture);
            _output.WriteLine($"Testing now: {now.ToString("S")} vs otherNow: {standardFormat}");
            // We skip the first 5 digits of this to avoid comparing times. Since we don't have the same precision as DateTime, 
            // we'll always be a few hours off.
            now.ToString("S", CultureInfo.InvariantCulture)[5..].Should().Be(standardFormat[5..]);
        }

        [Fact]
        public void ImperialDate_Now_Should_Return_Correct_Imperial_Format()
        {
            var now = ImperialDate.From(3021119000);
            now.ToString("I").Should().Be("0 119 021.M3");
        }
        
        [Fact]
        public void ImperialDate_Null_Format_Should_Return_Imperial_Format()
        {
            var now = ImperialDate.From(3021119000);
            now.ToString(null).Should().Be("0 119 021.M3");
        }
        
        [Theory]
        [InlineData("o")]
        [InlineData("O")]
        [InlineData("Standard")]
        [InlineData("Imperial")]
        public void ImperialDate_Invalid_String_Formats_Should_Throw_FormatException(string format)
        {
            Invoking(() => ImperialDate.Now.ToString(format)).Should().ThrowExactly<FormatException>();
        }
    }
}