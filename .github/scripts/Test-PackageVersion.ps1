$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/Get-PackageVersion.ps1"

$cases = @(
    @{ Args = @{ BaseVersion = '2.0.0' }; Expected = '2.0.0' },
    @{ Args = @{ BaseVersion = '2.0.0'; PublishedVersions = @('2.0.0', '2.0.7', '2.0.12', '2.0.13-pr.1', '2.1.99') }; Expected = '2.0.13' },
    @{ Args = @{ BaseVersion = '2.0.0'; PublishedVersions = @('2.0.9', '2.0.10', '2.0.2') }; Expected = '2.0.11' },
    @{ Args = @{ BaseVersion = '2.0.0'; MajorMinor = '3.1'; PublishedVersions = @('3.1.4', '2.0.99') }; Expected = '3.1.5' },
    @{ Args = @{ BaseVersion = '2.0.0-preview.1'; PublishedVersions = @('2.0.1') }; Expected = '2.0.2' },
    @{ Args = @{ BaseVersion = '2.0.0'; PullRequestNumber = 12; RunNumber = 34; RunAttempt = 2 }; Expected = '2.0.0-pr.12.34.2' }
)
foreach ($case in $cases) {
    $arguments = $case.Args
    $actual = Get-PackageVersion @arguments
    if ($actual -ne $case.Expected) { throw "Expected $($case.Expected), got $actual" }
}
foreach ($arguments in @(@{ BaseVersion = 'bad' }, @{ BaseVersion = '2.0.0'; MajorMinor = '2x0' }, @{ BaseVersion = '2.0.0'; PullRequestNumber = 1 })) {
    $rejected = $false
    try { $null = Get-PackageVersion @arguments } catch { $rejected = $true }
    if (!$rejected) { throw 'Invalid version arguments were accepted' }
}
Write-Output '9 package version checks passed.'
