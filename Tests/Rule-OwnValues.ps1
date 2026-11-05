param([Parameter(Mandatory)][string]$ModRoot, [string]$SharedDir)

# Own values and facts of this mod's content: About identity, dependencies and the generated source link, the counted-text key
# families that Source/ names without the suffix, and the hidden settings shortcut (MainButtonDef) with its French DefInjected.
# One consumer, nothing shareable; Tests/Test-Mod.ps1 (generated) runs it after the shared checks. Prints findings, exit 1 on any.
# XML parse, About shape, key parity and placeholders live in Check-ModBasics and Check-Translations.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ModRoot).Path
$mod = Join-Path $root 'Mod'
$script:findings = @()

function It([string]$name, [scriptblock]$body) {
    try { $problems = @(& $body | Where-Object { $_ }) } catch { $problems = @("threw: $($_.Exception.GetBaseException().Message)") }
    foreach ($p in $problems) { $script:findings += "[$name] $p" }
}
function Read-Xml([string]$p) { [xml](Get-Content -LiteralPath $p -Raw -Encoding UTF8) }

It 'About identity, Harmony as the only dependency, repository link' {
    $about = (Read-Xml (Join-Path $mod 'About/About.xml')).ModMetaData
    if ($about.packageId -cne 'nelim.billautopilot') { "packageId is $($about.packageId)" }
    if ($about.supportedVersions.li -notcontains '1.6') { 'supportedVersions lacks 1.6' }
    if (@($about.modDependencies.li).Count -ne 1) { 'Only Harmony is required' }
    if ($about.modDependencies.li.packageId -ne 'brrainz.harmony') { 'Dependency is not brrainz.harmony' }
    if ($about.loadAfter.li -notcontains 'brrainz.harmony') { 'Not loaded after Harmony' }
    $repo = 'https://github.com/vbardales/Rimworld-Bill-Autopilot'
    if ($about.url -ne $repo) { "url is $($about.url)" }
    # Two accepted forms: the plain text generated from PUBLICATION.md, or the hand-written BBCode of a mod that has not migrated.
    $end = $about.description.TrimEnd()
    if (-not ($end.EndsWith("Source code on GitHub ($repo)") -or $end.EndsWith("[url=$repo]Source code on GitHub[/url]"))) { 'Description does not end with the required GitHub link' }
}

It 'Every Source key exists in English, or is a counted family (key.One and key.Many)' {
    $english = @{}
    foreach ($n in (Read-Xml (Join-Path $mod 'Languages/English/Keyed/BillAutopilot.xml')).DocumentElement.ChildNodes) {
        if ($n.NodeType -eq 'Element') { $english[$n.Name] = $true }
    }
    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $root 'Source') -Filter *.cs -Recurse) {
        $source = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8
        foreach ($m in [regex]::Matches($source, '"(BillAutopilot\.[A-Za-z][A-Za-z0-9_.]*)"')) {
            $key = $m.Groups[1].Value
            # A counted thing is chosen by CountForm at run time, so Source names the base and the base itself does not exist.
            $isFamily = $english.ContainsKey("$key.One") -and $english.ContainsKey("$key.Many")
            if (-not ($english.ContainsKey($key) -or $isFamily)) { "Source key missing from English: $key ($($file.Name))" }
        }
    }
}

It 'Settings shortcut is hidden, not disabled, opens without a map, and has French label and description' {
    $button = (Read-Xml (Join-Path $mod 'Defs/MainButtonDefs/BillAutopilot.xml')).Defs.MainButtonDef
    if ($button.defName -cne 'BillAutopilot_Settings') { 'Shortcut defName' }
    if ($button.buttonVisible -cne 'false') { 'Shortcut must be hidden, not disabled' }
    if ($button.workerClass -cne 'BillAutopilot.MainButtonWorker_Settings') { 'Shortcut worker' }
    if ($button.validWithoutMap -cne 'true') { 'Settings must be available without a map' }
    $injected = Read-Xml (Join-Path $mod 'Languages/French/DefInjected/MainButtonDef/BillAutopilot.xml')
    foreach ($field in 'label', 'description') {
        if ([string]::IsNullOrWhiteSpace($button.$field)) { "Shortcut English $field is empty" }
        $key = "$($button.defName).$field"
        if ([string]::IsNullOrWhiteSpace($injected.LanguageData.$key)) { "Shortcut French $key is empty" }
    }
}

if ($script:findings.Count) { $script:findings | ForEach-Object { Write-Host $_ }; exit 1 }
Write-Host 'Rule-OwnValues: OK'
