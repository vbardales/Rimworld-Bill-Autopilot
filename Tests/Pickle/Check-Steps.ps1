<#
.SYNOPSIS
  Compile every step pattern of this suite, and match every step line of every feature against
  them. No game, about two seconds.

.DESCRIPTION
  Written after a whole run was lost to a pattern that looked right and was not.

  On 2026-09-21 the first WSL pass of this suite played zero scenarios and reported
  `exitReason: infrastructure-error`. The cause was one character class: in a Cucumber Expression,
  parentheses mean OPTIONAL TEXT, so `at ({int}, {int})` is not "a cell" but an optional group
  containing parameters - which is illegal. Pickle refused to build its step table at all, so not
  one scenario of the nineteen ran. The fix is to escape them, `at \({int}, {int}\)`, which in a C#
  literal is written `\\(` and `\\)`.

  An invalid pattern costs the whole run, not the scenario using it, and the machine is shared: a
  lost run is also everybody else's forty minutes in the queue. That is worth two seconds here.

  This checks two things, both against Pickle's own engine rather than against a guess:

    1. Every pattern this suite declares COMPILES, using the same PickleParameterTypeRegistry the
       game uses. This is the check that would have caught the failure above.
    2. Every `Given/When/Then/And` line in every feature either MATCHES one of those patterns or is
       left over for Pickle's own vocabulary. The leftovers are listed so a person can read them
       against the built-in catalogue (Docs/steps.md in the Pickle repository); they are not an
       error here, because this script does not load Pickle's own step assembly.

  It also reports a pattern this suite declares and no feature ever uses. A step nobody calls is
  weight, and the doctrine says to delete it rather than keep it for later.

.EXAMPLE
  powershell.exe -ExecutionPolicy Bypass -File Tests/Pickle/Check-Steps.ps1
#>
param(
    [string]$PickleAssemblies = 'C:\Program Files (x86)\Steam\steamapps\workshop\content\294100\3791648678\Assemblies'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent          # ...\Tests
$suite = $PSScriptRoot                            # ...\Tests\Pickle

foreach ($dll in 'CucumberExpressions.dll', 'RimWorks.Pickle.Core.dll') {
    $path = Join-Path $PickleAssemblies $dll
    if (-not (Test-Path $path)) {
        throw "$dll not found under $PickleAssemblies. Pass -PickleAssemblies with the path to the installed Pickle mod's Assemblies folder."
    }
    [Reflection.Assembly]::LoadFrom($path) | Out-Null
}

$core = [AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.GetName().Name -eq 'RimWorks.Pickle.Core' }
$registryType = $core.GetType('RimWorks.Pickle.Core.Steps.PickleParameterTypeRegistry')
if (-not $registryType) { throw 'RimWorks.Pickle.Core.Steps.PickleParameterTypeRegistry no longer exists: Pickle renamed it, update this script.' }
$registry = [Activator]::CreateInstance($registryType)

# --- 1. every declared pattern compiles ---------------------------------------------------------

# The attribute argument is a C# literal, so its backslashes are doubled in the source. Undoing
# that here is what makes this script check the pattern Pickle will really see.
$declared = @()
foreach ($file in Get-ChildItem -LiteralPath (Join-Path $suite 'Source') -Filter *.cs) {
    $text = [IO.File]::ReadAllText($file.FullName)
    foreach ($m in [regex]::Matches($text, '\[(?:Given|When|Then)\("((?:[^"\\]|\\.)*)"\)\]')) {
        $declared += [pscustomobject]@{
            File    = $file.Name
            Pattern = $m.Groups[1].Value -replace '\\\\', '\' -replace '\\"', '"'
        }
    }
}

if ($declared.Count -eq 0) { throw "no step patterns found under $suite\Source: the attribute shape this script looks for has changed." }

# Two definitions carrying the same expression text are an "Ambiguous step" at run time, and the
# scenarios they fail are perfectly healthy ones. Pickle matches on the expression text alone, so a
# Given setter and a Then assertion spelled identically collide even though they read differently in
# a feature file - which is exactly how three of them got in here.
$bad = 0
$dupes = $declared | Group-Object Pattern | Where-Object { $_.Count -gt 1 }
foreach ($g in $dupes) {
    Write-Host "DUPLICATE  $($g.Name)" -ForegroundColor Red
    Write-Host "           declared $($g.Count) times, in $(($g.Group.File | Sort-Object -Unique) -join ', ')" -ForegroundColor DarkRed
    Write-Host "           Pickle matches on the expression text alone: this is an Ambiguous step, and it fails healthy scenarios." -ForegroundColor DarkRed
    $bad++
}

$compiled = @()
foreach ($d in $declared) {
    try {
        $compiled += [pscustomobject]@{
            Pattern    = $d.Pattern
            Expression = New-Object CucumberExpressions.CucumberExpression($d.Pattern, $registry)
            Used       = $false
        }
    } catch {
        $e = $_.Exception
        while ($e.InnerException) { $e = $e.InnerException }
        Write-Host "INVALID  $($d.File): $($d.Pattern)" -ForegroundColor Red
        foreach ($line in ($e.Message -split "`n")) { if ($line.Trim()) { Write-Host "         $($line.TrimEnd())" -ForegroundColor DarkRed } }
        $bad++
    }
}

# --- 2. every feature line resolves --------------------------------------------------------------

$leftover = @()
$lines = 0
foreach ($file in Get-ChildItem -LiteralPath (Join-Path $suite 'Mod\Pickle\Features') -Filter *.feature) {
    foreach ($raw in [IO.File]::ReadAllLines($file.FullName)) {
        $line = $raw.Trim()
        if ($line -notmatch '^(Given|When|Then|And|But)\s+(.+)$') { continue }
        $step = $Matches[2].Trim()
        $lines++

        # The compiled expression exposes the regex it rewrote to, which is the same one Pickle
        # matches a step line with. Matching here rather than re-implementing the rewrite is the
        # point: a check that parsed the pattern itself would share whatever misreading put the
        # bug in the pattern.
        $hit = $false
        foreach ($c in $compiled) {
            if ($c.Expression.Regex.IsMatch($step)) { $c.Used = $true; $hit = $true; break }
        }
        if (-not $hit) { $leftover += "$($file.Name): $step" }
    }
}

$unused = @($compiled | Where-Object { -not $_.Used })

# --- report --------------------------------------------------------------------------------------

Write-Host ''
Write-Host "$($declared.Count) patterns declared, $($declared.Count - $bad) compile, $lines step lines across the features."

if ($unused.Count -gt 0) {
    Write-Host ''
    Write-Host "$($unused.Count) pattern(s) no feature uses - weight, not coverage:" -ForegroundColor Yellow
    foreach ($u in $unused) { Write-Host "  $($u.Pattern)" -ForegroundColor Yellow }
}

$distinct = @($leftover | Sort-Object -Unique)
if ($distinct.Count -gt 0) {
    Write-Host ''
    Write-Host "$($distinct.Count) step line(s) left for Pickle's own vocabulary - read them against its catalogue:"
    foreach ($l in $distinct) { Write-Host "  $l" -ForegroundColor DarkGray }
}

Write-Host ''
if ($bad -gt 0) {
    Write-Host "$bad PROBLEM(S). Pickle builds its whole step table before it runs anything, so an invalid pattern makes a run play zero scenarios and report infrastructure-error, and a duplicate fails healthy scenarios as Ambiguous step." -ForegroundColor Red
    exit 1
}

Write-Host 'ALL PATTERNS COMPILE, NONE DECLARED TWICE' -ForegroundColor Green
exit 0
