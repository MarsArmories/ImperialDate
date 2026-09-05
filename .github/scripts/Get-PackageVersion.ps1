function Get-PackageVersion {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)][string] $BaseVersion,
        [string] $MajorMinor,
        [string[]] $PublishedVersions = @(),
        [int] $PullRequestNumber = 0,
        [int] $RunNumber = 0,
        [int] $RunAttempt = 1
    )

    if ([string]::IsNullOrEmpty($MajorMinor)) {
        if ($BaseVersion -notmatch '^(\d+)\.(\d+)(?:\.\d+)?(?:[-+].*)?$') {
            throw "Invalid base version: $BaseVersion"
        }
        $MajorMinor = "$($Matches[1]).$($Matches[2])"
    }
    if ($MajorMinor -notmatch '^\d+\.\d+$') {
        throw "Invalid major/minor version: $MajorMinor"
    }

    $pattern = '^' + [regex]::Escape($MajorMinor) + '\.(\d+)$'
    [long] $latestPatch = -1
    foreach ($version in $PublishedVersions) {
        if ($version -match $pattern) {
            $latestPatch = [Math]::Max($latestPatch, [long]$Matches[1])
        }
    }
    $resolved = "$MajorMinor.$($latestPatch + 1)"
    if ($PullRequestNumber -gt 0) {
        if ($RunNumber -le 0 -or $RunAttempt -le 0) {
            throw 'PR versions require positive run and attempt numbers.'
        }
        $resolved += "-pr.$PullRequestNumber.$RunNumber.$RunAttempt"
    }
    return $resolved
}
