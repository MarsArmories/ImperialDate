# ImperialDate

A .NET 10 library for representing dates in the Warhammer 40,000 Imperial date style. `ImperialDate` is an immutable value type with conversion from `DateTime`, formatting, equality, ordering, and a packed integer representation.

## Installation

```sh
dotnet add package MarsArmories.ImperialDate
```

Your application must target .NET 10 or a compatible later framework.

## Usage

```csharp
using MarsArmories.ImperialDate;

var date = ImperialDate.From(new DateTime(2024, 7, 2));
Console.WriteLine(date);               // 0 500 024.M3
Console.WriteLine(date.ToString("S")); // 00:00 02/07/2024

var campaignDate = ImperialDate.From(CheckValue.Warp, 123, 456, 41);
Console.WriteLine(campaignDate);       // 9 123 456.M41

long packed = campaignDate.Ticks;
var restored = ImperialDate.From(packed);
Console.WriteLine(restored == campaignDate); // True

var now = ImperialDate.Now;
bool isEarlier = date.CompareTo(now) < 0;
```

## Date fields and conventions

The Imperial format is `C FFF YYY.MN`:

| Field | Property | Accepted values |
| --- | --- | --- |
| Check / source classification | `Check` | `CheckValue`, from `Terra` (0) through `Warp` (9) |
| Thousandth of the year | `YearFraction` | 0–999 |
| Year within the millennium | `Year` | 0–999 |
| Millennium | `Millennium` | 0–999,999 |

The library uses `standardYear = (Millennium - 1) * 1000 + Year`. For example, 1999 is `999.M2`, 2000 is `000.M3`, and 2024 is `024.M3`. Conversion from `DateTime` always assigns `CheckValue.Terra`.

`From(DateTime)` uses the supplied calendar and clock fields without time-zone conversion. `Now` uses the local clock; use `ImperialDate.From(DateTime.UtcNow)` for UTC.

### Precision and formatting

A year is divided into 1,000 equal intervals. Conversion rounds down to the containing interval and accounts for Gregorian leap years. An interval lasts 8 hours, 45 minutes, 36 seconds in a common year, or 8 hours, 47 minutes, 2.4 seconds in a leap year.

| Format | Output |
| --- | --- |
| `ToString()`, `"I"`, null, or empty | Imperial date, such as `0 500 024.M3` |
| `"S"` | Start of the interval, as `HH:mm dd/MM/year` with a 24-hour clock |

Format letters are case-insensitive. Unsupported formats throw `FormatException`. Formatting defaults to invariant culture; an explicit `IFormatProvider` controls numeric formatting, while field order and separators stay fixed.

Standard formatting discards seconds and can show the previous calendar day compared with the original `DateTime`. The original timestamp and its `DateTimeKind` are not retained.

### Range, ticks, and equality

All accepted component combinations are supported, including dates outside the `DateTime` range. Standard formatting extends Gregorian leap-year rules to those years, including zero and negative years.

- `MinValue` is `0 000 000.M0`, equivalent to `default(ImperialDate)`, with packed ticks of zero. Its standard year is -1000.
- `MaxValue` is `9 999 999.M999999`, with standard year 999,998,999.
- `DateTime.MinValue` converts to `0 000 001.M1` and formats as `00:00 01/01/1`.
- `DateTime.MaxValue` converts to `0 999 999.M10`.

`Ticks` is a packed Imperial value, **not `DateTime.Ticks` or an elapsed duration**:

```text
Millennium * 1,000,000,000 + Year * 1,000,000 + YearFraction * 1,000 + Check
```

`From(long)` reverses this encoding. Values outside the supported range and packed check components above 9 are rejected. Invalid component ranges throw `ArgumentOutOfRangeException`; malformed packed check components throw `ArgumentException`.

Equality and hashing include all four fields. Comparison orders by millennium, year, fraction, then check value. Two records with different check values are distinct even if their date fields match.

### Changes from the earlier implementation

Zero year fractions and year components are now accepted. The minimum value is now all zeros, conversion at millennium boundaries follows the formula above, and millennia above 999,999 are rejected. If you persist packed dates, the encoding of previously valid component combinations is unchanged; persisted values outside the enforced range must be corrected before loading.

## Repository layout

```text
MarsArmories.ImperialDate.slnx     Solution
Directory.Build.props            Shared .NET and build-output settings
global.json                      .NET SDK and test-runner selection
src/MarsArmories.ImperialDate/    Library and package metadata
tests/MarsArmories.ImperialDate.Tests/
                                 xUnit tests on Microsoft.Testing.Platform
.github/                         CI, publishing, and dependency updates
.artifacts/                      Generated build, test, and package output
```

## Build and test

Install a stable .NET 10 SDK. Run these commands from the repository root:

```sh
dotnet restore MarsArmories.ImperialDate.slnx
dotnet build MarsArmories.ImperialDate.slnx --configuration Release --no-restore
dotnet test --solution MarsArmories.ImperialDate.slnx --configuration Release --no-build
dotnet pack src/MarsArmories.ImperialDate/MarsArmories.ImperialDate.csproj --configuration Release --no-build --output .artifacts/packages
```

Tests cover calendar boundaries, leap years, fraction precision, tick validation, formatting, equality, and ordering. The library has no NuGet runtime dependencies. The NuGet package includes this README and XML documentation for IntelliSense. Public API documentation warnings fail the library build.

CI builds and tests pushes and pull requests to `main`. It derives the next patch version from NuGet.org for the configured major/minor version, applies that version to both the assembly and package, and retains packages and symbols as workflow artifacts. Publishing runs in a separate job after validation. Pushes publish to NuGet.org when `NUGET_API_KEY` is configured and to GitHub Packages using the workflow token. Pull requests from the same repository publish prerelease packages to GitHub Packages, with the PR, run, and attempt numbers in their versions. Fork and Dependabot pull requests build, test, and pack without publishing.

Release-version logic lives in `.github/scripts/Get-PackageVersion.ps1`. Run its offline regression checks with `pwsh -File .github/scripts/Test-PackageVersion.ps1`; CI runs these checks too.

## License

[MIT](https://github.com/MarsArmories/ImperialDate/blob/main/LICENSE)
