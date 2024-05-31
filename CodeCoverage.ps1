$DirectoryrName = ".\TestResults\manual"

if (Test-Path $DirectoryrName) 
{
    Remove-Item $DirectoryrName -Force -Confirm:$false -Recurse
}

try
{
    reportgenerator ? | Out-Null
}
catch
{
    dotnet tool install -g dotnet-reportgenerator-globaltool --ignore-failed-sources
}

dotnet test CssWebApi.sln --collect "XPlat Code Coverage" -s .\CodeCoverage.runsettings --results-directory .\TestResults\manual

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

reportgenerator "-reports:.\TestResults\manual\*\coverage.cobertura.xml" "-sourcedirs:.\src" "-targetdir:TestResults\manual\report" "-reporttypes:HtmlInline;JsonSummary"

$coverage = Get-Content -Raw .\TestResults\manual\report\Summary.json | ConvertFrom-Json

Write-Host "Line Coverage: $($coverage.summary.linecoverage)%"
Write-Host "Branch coverage: $($coverage.summary.branchcoverage)%"
Write-Host "Method coverage : $($coverage.summary.methodcoverage)%"

TestResults\manual\report\index.html
