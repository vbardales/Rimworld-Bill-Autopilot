$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$checks = 0
function Check([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
    $script:checks++
}
function Read-Language([string]$language) {
    $path = Join-Path $root "Mod/Languages/$language/Keyed/BillAutopilot.xml"
    [xml]$document = Get-Content -LiteralPath $path -Raw -Encoding UTF8
    Check ($document.DocumentElement.Name -ceq 'LanguageData') "$language root"
    $entries = @{}
    foreach ($node in $document.DocumentElement.ChildNodes) {
        if ($node.NodeType -ne 'Element') { continue }
        Check (-not $entries.ContainsKey($node.Name)) "$language duplicate key: $($node.Name)"
        Check (-not [string]::IsNullOrWhiteSpace($node.InnerText)) "$language empty value: $($node.Name)"
        $entries[$node.Name] = $node.InnerText
    }
    return $entries
}

$xmlFiles = @(Get-ChildItem -LiteralPath (Join-Path $root 'Mod') -Filter *.xml -Recurse)
foreach ($file in $xmlFiles) {
    [xml]$document = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8
    Check ($null -ne $document.DocumentElement) "Invalid XML: $($file.FullName)"
}
[xml]$about = Get-Content -LiteralPath (Join-Path $root 'Mod/About/About.xml') -Raw -Encoding UTF8
Check ($about.DocumentElement.LocalName -ceq 'ModMetaData') 'About root'
Check ($about.ModMetaData.packageId -ceq 'nelim.billautopilot') 'Stable packageId'
Check ($about.ModMetaData.supportedVersions.li -contains '1.6') 'RimWorld 1.6 support'
Check (@($about.ModMetaData.modDependencies.li).Count -eq 1) 'Only Harmony is required'
Check ($about.ModMetaData.modDependencies.li.packageId -eq 'brrainz.harmony') 'Harmony dependency'
Check ($about.ModMetaData.loadAfter.li -contains 'brrainz.harmony') 'Load after Harmony'
$repo = 'https://github.com/vbardales/Rimworld-Bill-Autopilot'
Check ($about.ModMetaData.url -eq $repo) 'Repository URL'
Check ($about.ModMetaData.description.Contains($repo)) 'GitHub link in description'

$english = Read-Language 'English'
$french = Read-Language 'French'
Check ($english.Count -eq $french.Count) 'Translation key counts differ'
foreach ($key in $english.Keys) {
    Check ($french.ContainsKey($key)) "French key missing: $key"
    $enArgs = @([regex]::Matches($english[$key], '\{\d+\}') | ForEach-Object Value | Sort-Object -Unique)
    $frArgs = @([regex]::Matches($french[$key], '\{\d+\}') | ForEach-Object Value | Sort-Object -Unique)
    Check (($enArgs -join ',') -ceq ($frArgs -join ',')) "Translation placeholders differ: $key"
}
foreach ($file in Get-ChildItem -LiteralPath (Join-Path $root 'Source') -Filter *.cs -Recurse) {
    $source = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8
    foreach ($match in [regex]::Matches($source, '"(BillAutopilot\.[A-Za-z][A-Za-z0-9_.]*)"')) {
        $key = $match.Groups[1].Value
        Check ($english.ContainsKey($key)) "Source key missing from English: $key"
    }
}
Write-Host "$checks XML CHECKS PASSED ($($xmlFiles.Count) files)"
