[CmdletBinding()]
param(
    [string] $Configuration = 'Release'
)

$RootDir = "$PsScriptRoot\.." | Resolve-Path

@("$RootDir\NGit.Test\bin\$Configuration\netcoreapp2.1\NGit.Test.dll",
"$RootDir\Sharpen.Test\bin\$Configuration\netcoreapp2.1\Sharpen.Test.dll") | Resolve-Path | ForEach-Object {
    try {
        if ($env:TEAMCITY_VERSION) {
            dotnet vstest $_ --testcasefilter:"TestCategory!=Explicit" --logger:teamcity
        } else {
            dotnet vstest $_ --testcasefilter:"TestCategory!=Explicit"
        }
    }
    catch {
        Write-Host "Error while running tests: $_"
    }
}