---
localization: complete
translation_en: complete
translation_fr: complete
mod:          Bill Autopilot
packageId:    nelim.billautopilot
repo:         Rimworld-Bill-Autopilot
visibility:   public
detached:     yes
stage:        dansMonoRepo
licence:      original
licence_at:   original work
licence_name: MIT
licence_file: LICENSE (identical copy in Mod/LICENSE)
dependencies: declared
showcase:     complete
settings_audit: partial
tested_on:    2026-09-01
workshop:
remaining:
  - defect: ATTRIBUTION.md is absent; document studied versus reused third-party material and provenance in English before horsMonoRepo
  - defect: no discoverable hidden MainButtons shortcut exists for the useful settings page
  - defect: changing a recipe count while inheriting a custom bench mode changes that recipe to Maintain (Dialog_BenchProfile.cs)
  - defect: About.xml ends with a raw repository URL instead of the required linked Source code on GitHub text
  - unverified: settings serialization round-trip, older stored values and complete applicable technical settings coverage
  - unverified: final current-build in-game scenarios, FR/EN UI, logs, new game and existing save, options persistence and RIMMSQOL shortcut integration
  - unverified: English and French runtime translation checks described in TESTING.md, including optional integrations and clipping
  - unverified: the mark in the bill label, in the tabs other mods redraw
  - unverified: a change made in the bills tab being kept as a profile override
  - unverified: the memory held per workbench, two benches of a kind set differently
  - unverified: a repeat mode from another mod, kept and asked rather than guessed at
  - unverified: all five compatibility bridges, Better Workbench Management the largest
  - unverified: Nice Bill Tab's cached list, where a drag could bring a deleted bill back
  - unverified: loading a save after removing the mod, the reason its state avoids a GameComponent
session:      local_3527e6d8-def4-4e54-97ff-a6430f1dc569
updated:      2026-09-13, workflow audit
---

# Bill Autopilot — status

## Translation audit — 2026-09-13

Applied the shared `PUBLISHING.md` / `TRANSLATIONS.md` gate to revision `275ff25`
plus this working-tree update. The ordered workflow audit below supersedes the
historical `done` stage; `tested_on` remains unchanged. These three complete fields
certify static readiness only.

- Inventory: all C# string literals and their UI/helper call paths, settings and
  integration statuses, bench profiles, category headers, counters, repeat-mode menus,
  activation confirmations, gizmos, automatic bill markers, recipe letters and messages,
  including the Dubs Mint Menus template notification. `Mod/` has one assembly and
  English/French Keyed resources; no LoadFolders, version directories, Defs, XML patches,
  custom translatable XML fields or grammar resources require DefInjected validation.
- Fixed integration statuses to translate complete parameterized messages. Moved the
  settings title and count/group display formats into keys. Both languages now contain
  54 nonempty keys. Reviewed their meaning, parameter arguments, paragraph breaks and
  formatting; no dynamically constructed translation keys or rich-text tags are used.
  The unchanged title, `(auto)` marker and structural formats are intentional in both languages.
- Bench, recipe, category and foreign repeat-mode names come from the existing Def's
  `LabelCap`, never from a hardcoded English replacement. Their translations belong to
  the game or supplying mod; optional mod coverage still needs the runtime pass below.
  No dependency Keyed keys are explicitly resolved by this mod. Standard confirmation
  and close controls are drawn by the game's own dialogs.
- Exclusions: integration product names, numeric step buttons and disclosure symbols,
  user-entered bill names, serialization identifiers, reflection names, texture paths,
  English technical logs and About metadata. The automatic marker is a translated
  suffix on the existing bill label; recipe letter lines use a parameterized key.
- Validation: `./Tests/ValidateXml.ps1` passed **397 checks** over all three XML files,
  including source-key coverage, duplicate/empty entries and EN/FR placeholder parity.
  `dotnet build Source/BillAutopilot.csproj -c Release --no-restore --nologo` succeeded
  with zero warnings/errors and rebuilt `Mod/Assemblies/BillAutopilot.dll`.
- No game session was run. English/French rendering, fallback detection, dependency
  labels and clipping remain explicitly unverified in `remaining` and `TESTING.md`.
  Reaudit affected fields after any UI, text, Def, patch or language-resource change.

## Ordered workflow audit — 2026-09-13

This audit supersedes earlier completion decisions without deleting their historical evidence.
Audited base HEAD: `275ff25d866078d6d04bb058a6f9239b477a135c`, plus the local changes listed below; repository:
`C:/Users/nelim/Documents/rimworld/BillAutopilot`; distribution: its `Mod/` directory.
Before this audit only STATUS.md was modified: three unchecked localization fields and a
correction recording the previous preview push. Both changes are preserved or explicitly
updated by this audit. No code, distributed asset, licence or historical test result was changed.
Build outputs were written under ignored `.build/`; nothing was published or pushed.


Concurrent local changes appeared after the initial status snapshot and were reviewed at audit
close: `Source/BillAutopilotMod.cs`, `Source/UI/Dialog_BenchProfile.cs`, both language XMLs,
`Mod/Assemblies/BillAutopilot.dll` and `TESTING.md`. They add three translation keys, parameterize
integration labels/counts, translate SettingsCategory and add the runtime language checklist.
They were not made by this audit. The source/resource reads and successful builds/tests above
already include these changes; the delivered DLL hash was rechecked and still equals the audit
build. Thus these results apply to HEAD plus these local changes, not pristine HEAD. The increase
from 376 to 397 XML checks follows the expanded localization coverage. `git diff --check` passed.
The concurrent STATUS runtime-language reminder was preserved. The audit only edits STATUS.md.
**Previous stage: done. Retained stage: dansMonoRepo (workflow baseline).** The stage uses
literal names from the requested chain, not the older `port`/`showcase` codes. No completed
transition can be certified cumulatively because the first gate lacks ATTRIBUTION.md.
This baseline is a workflow rank only: the repository really IS detached, so `detached: yes`
and its GitHub remote remain correct. It must not be physically returned to a monorepo.

The user's interpretation takes precedence over the protocols: game interaction belongs to
`done -> tested`, not to `preOptions -> options`. No missing historical image-generation report
or side-by-side game-camera comparison is treated as a blocker.

| Transition | Audit result |
| --- | --- |
| dansMonoRepo -> horsMonoRepo | **Defect:** required ATTRIBUTION.md is absent. README describes specific internals of BWM, Dubs Mint Menus, Nice Bill Tab/Expansion and other integrations; PUBLISHING requires attribution distinguishing studied and reused code. This is missing documentation, not proof of illicit copying. Git root, public GitHub repository, origin and pushed main commit were verified live. README, CHANGELOG and both identical MIT notices exist in English. Names Bill Autopilot / BillAutopilot / Rimworld-Bill-Autopilot / nelim.billautopilot are coherent; no Renew, unofficial or prohibited suffix is justified by the recorded original provenance. `original`/MIT/public is retained provisionally; attribution must make the provenance and rights basis explicit. |
| horsMonoRepo -> ModIcon generated | Build and delivered DLL freshness **validated independently**. Icon PNG inspected directly: 128 x 128, 21,033 bytes, orange winking mascot, ponytail, clipboard and gear on a near-black background. No concrete visual defect identified. Development completion cannot be certified while the settings defects below remain. |
| ModIcon generated -> Preview generated | **Validated independently:** delivered PNG directly inspected, 896 x 504, 557,964 bytes. High oblique workshop view, tiled floor, one warm light pool, a small rear-facing worker without a readable face, restrained slate/orange families. No concrete camera defect identified. Full-size and existing 268-pixel thumbnail inspected; readable title/version and no clipping. |
| Preview generated -> preOptions | Palette and naming **validated independently**: orange accent clearly distinct from blue secondary; secondary unused because there is no tag/prefix/suffix. Both identity-bearing title words stay full size. Description is English. **Defect against PUBLISHING:** its final raw URL is not `[url=https://github.com/vbardales/Rimworld-Bill-Autopilot]Source code on GitHub[/url]`. |
| preOptions -> options | **Partial:** useful primary settings exist; hidden discoverable MainButtons shortcut is absent from all sources and shipped XML. A selected-bench gizmo is not that shortcut. A custom-mode editing defect is detailed below; the existing suite does not cover the complete applicable settings contract. No in-game run is demanded to pass this gate. |
| options -> l10n | Existing text inventory and EN/FR resources **validated independently**, as detailed below. This does not clear the preceding settings gate or certify future shortcut text. |
| l10n -> preTest | Source/declaration contract **validated independently**: Harmony is the only hard mod dependency; game 1.6 is declared. Six optional mod loadAfter entries match the documented integrations; reflection avoids compile-time references to their assemblies. No shipped LoadFolders, Defs or conditional XML patches require further path checks. Actual integration-version compatibility remains unverified in game. |
| preTest -> done | Existing test deliverables **validated independently**: 19 functional scenarios with setup/actions/expected outcomes in TESTING.md; both builds, 44 decision checks and 397 XML checks passed now. Full readiness is not established because earlier defects and settings coverage remain. |
| done -> tested | **Unverified:** no game launched, no current-build Player.log or FR/EN UI checked, no new-game/existing-save run or integration pass performed. The historical 2026-09-01 result is preserved and does not certify this DLL. |

### Commands and observed results

- `git rev-parse --show-toplevel`, `git status --short`, `git diff -- STATUS.md`,
  `git remote -v`, `git log -1`: autonomous root and starting local modifications verified.
- `gh repo view vbardales/Rimworld-Bill-Autopilot --json nameWithOwner,visibility,defaultBranchRef,url`:
  PUBLIC, main. `git ls-remote origin refs/heads/main`: exactly the audited HEAD above.
- `dotnet build Source/BillAutopilot.csproj -c Release --no-restore -p:OutputPath=../.build/audit-build/`:
  success, zero warnings/errors. Separate output preserves the shipped DLL.
- SHA-256 of rebuilt and shipped DLLs both:
  `7C05CDA616042B9CF1D90D94207402511D48CE5437292A9775DABB0B7A99D775`.
- `dotnet build Tests/BillAutopilot.Tests.csproj -c Release --no-restore`: success,
  zero warnings/errors. `.build/bin/tests/Release/BillAutopilot.Tests.exe`: **44 CHECKS PASSED**,
  exercising the copied shipped assembly against the installed RimWorld Managed assemblies.
- `Tests/ValidateXml.ps1`: **397 XML CHECKS PASSED (3 files)**. This is today's output;
  the historical count of 376 is not rewritten. The test checks URL presence, not the required
  Steam link markup, so its success does not contradict the description defect.
- Initial sandbox attempts could not read SDK/GitHub CLI configuration or reach GitHub;
  the approved rerun succeeded. These are resolved environment restrictions, not mod defects.
- `LICENSE` and `Mod/LICENSE` SHA-256 both:
  `1A24DB0B016BF77E6D15FDA1BF27B20C76F9B14F1A21458BA36F2BB3A7D2C908`.
- Existing Art/preview.html, preview-layout.json, preview-palette.json and preview-qa.json
  inspected. Historical font/contrast measurements are preserved, not represented as a fresh
  browser measurement. Direct visual checks of the actual PNGs were performed in this audit.

### Settings audit

Useful global configuration is implemented through SettingsCategory/DoSettingsWindowContents:
new-recipe notifications (true), automatic-bill marker (true), automatic-bill cap (8), and
per-workbench-type profiles (disabled, Maintain, target 50, floor 25, uncountable Excluded).
Profiles expose modes, quantities and per-recipe overrides, with clear-overrides and list filters.
`syncIntervalTicks` is an internal scheduling parameter (600, runtime minimum 60); no extra
option is required merely because that field is serialized.

Sources connect marking to the bill label, notification to the letter queue, and profiles/cap
to AutoBillSync. WriteSettings marks state dirty; profile edits write settings immediately.
Configuration is global, while bill stamps/memory are per game. The cap limits new creation;
it does not immediately remove already-running bills when lowered. The UI slider is 1..gameMax;
normal target is at least 1 and floor clamped to 0..target; custom bench target permits 0.
Text-entry empty/invalid-string tests are not applicable to pointer-only count buttons/slider.
Arithmetic/default/fallback checks pass, but they are not UI or Scribe round-trip tests.

**Defect:** `Source/UI/Dialog_BenchProfile.cs`, DrawRecipeRow, count button handler sets
`written.mode = AutoMode.Maintain` whenever the rule inherits. For a profile whose default is
Custom, changing an inherited recipe quantity therefore switches its mode to Maintain instead
of retaining the foreign mode. This follows directly from the handler and ModeFor resolution;
it was not reproduced interactively and is not covered by the 44 checks. Fix and add a focused
regression when development is authorized. The comment claiming both counters are shown is also
not borne out by DrawRecipeRow: only the target is editable there; per-recipe floor editing is absent.

**Defect:** no MainButtonDef/MainTabWindow or programmatic equivalent exists. Required work at
this later gate is a hidden-by-default, discoverable shortcut opening the same global settings,
plus applicable technical checks of its default visibility/routing and the corrected settings.
Settings serialization round-trip/older values are not covered by the suite and remain unverified.
Actual effects, reopening, restart/save persistence, FR/EN layout, logs and RIMMSQOL reveal/hide
interaction belong to the final game pass. No RIMMSQOL or other customization integration was
actually tested here. Synthetic foreign-mode tests do not constitute an Everybody Gets One test.

### Translation audit

All shipped XML and Source UI/message/gizmo/compatibility paths were inspected, including
ModeLabel switches, parameterized labels, confirmation branches and letter assembly.
54 nonempty keys per language; parity, duplicate checks, literal references and numbered
placeholder parity passed. English and French wording were read. No owned untranslated phrase,
missing key or unresolved owned DefInjected path was found. Proper names of companion mods,
numeric +/- controls, disclosure symbols, technical logs, metadata and serialized identifiers
are not untranslated prose. Workbench/recipe/category/foreign-mode labels use their owning Def's
LabelCap; the mod does not introduce those Defs. No redundant EN DefInjected file is required.
The existing keyed newline escapes and actual XML line breaks use the native resource mechanism.

`localization`, `translation_en`, `translation_fr: complete` certify the current static inventory
and resources only, retained independently as explicitly permitted by the audit request. They do
not finalize the blocked sequential options gate or claim runtime language validation. New or
changed shortcut/settings text must trigger the relevant resource recheck. In-game formatting,
clipping and language switching remain unverified.

### Next transition and optional recommendations

To pass the immediate `dansMonoRepo -> horsMonoRepo` gate, add the required English ATTRIBUTION.md
with sources/credits, distinguish study from reuse, and document the rights basis without
inventing third-party permissions. Include a distributed copy where third-party notices require
it. Recheck consistency with the original/MIT/public decision. No repository recreation, remote
repair, image generation or publication is needed. Later settings/description defects do not
need to be fixed merely to pass this first transition.

Optional: retain a durable per-run test log and clarify the creation-only cap behavior in the
player documentation. Neither recommendation is an additional workflow blocker.

## Preview recomposition — 2026-09-12

- Revised overlay brief checked and composition rendered again: `Bill` and `Autopilot`
  are both identity-bearing words, so both remain at 100% (46 px, weight 600).
  The exact name has no prefix, suffix or linking word requiring a 65% span.
  Orange accent from the lit subject contrasts by hue and saturation with the dominant
  cool blue-slate family and its light-blue secondary ink; it is not a brighter version
  of that secondary ink. Full-size and 268-pixel previews were visually rechecked.
  Existing illustration, title, summary and palette are retained for this revised brief;
  no further source replacement was needed. The font and contrast checks below passed again.

- Final asset: `Mod/About/Preview.png`, 896 x 504, 557,964 bytes (below 900 KB).
- New text-free illustration: `Art/Preview.png`. The available `Art/Preview-source.png`
  already contained engraved words and a bill interface, so it could not serve as a clean
  background. It remains untouched and was archived before generation as
  `Art/Preview-previous-engraved.png`. The replacement was visually inspected before composition.
- Composition: `Art/preview.html`; parameters and preserved title/summary:
  `Art/preview-layout.json`; sole colour reference: `Art/preview-palette.json`.
  Reproduce using `Art/render-preview.cjs` with Node.js, Playwright, Sharp and Chrome
  (`NODE_PATH` may point to the bundled Node modules). The script serves the files locally,
  waits for `document.fonts.ready` and image decoding, then captures at native size.
- Palette rationale: the broad blue-slate floor supplies the veil and the blue family of
  the lightened secondary ink. The characteristic orange task-lamp light and worker's clothing
  supply the saturated accent for the rule and badge. Title and summary share the same ivory ink.
  Secondary ink is saved for consistency but unused: this original public mod has no status tag.
- Fonts verified through Chrome's actual platform-font report: Segoe UI Semibold for the
  title, Segoe UI regular for the summary, Segoe UI Bold for the version. No fallback font.
- Version `1.6` checked against the highest stable `supportedVersions` entry in delivered
  `Mod/About/About.xml`; triangle and rotated digits use the guide's coordinates.
- QA: `Art/preview-qa.json` records actual fonts, geometry and contrast. The text-free rendered
  background is `Art/preview-background.png`; thumbnail is `Art/preview-268.png`.
  Minimum contrast over every pixel in each text rectangle (including all four corners):
  title 11.70:1, summary 8.74:1; badge digits against the opaque accent 8.82:1.
  Tag contrast is not applicable because no tag is displayed. Visual inspection at both
  896 x 504 and 268 pixels wide found no overlap or clipping; title/version identifiable,
  rule visible. The summary is intended for the full-size view, as specified by the guide.
- Preview revision committed as `275ff25` and pushed to `origin/main` at the owner's request.
  No Workshop publication performed.

Marked `done` at the owner's request on 2026-09-12 after the repository audit.
The unverified in-game scenarios remain recorded above; this status change does not
claim a new manual test pass or a Workshop release. `licence` remains `original`.

## Repository audit — 2026-09-12

- **Title:** keep `Bill Autopilot`. The repository records an original, unpublished mod;
  no continuation or port suffix is justified by the available provenance. Version 1.6 is
  already declared in `supportedVersions`.
- **Licence:** MIT, copyright (c) 2026 nelim. `LICENSE` and `Mod/LICENSE` are byte-identical.
  The `original` field above describes provenance; the actual licence is MIT.
- **Manual tests:** 19 functional scenarios in `TESTING.md`, including the added hidden-recipe
  integration scenario. Their presence is verified; no in-game test was run in this audit.
  The historical `tested_on` date remains unchanged. The owner subsequently requested `done`.
- **Automated tests:** both Release builds succeeded with zero warnings and errors;
  all 44 decision-layer checks passed against the shipped DLL and installed game assemblies.
  This does not validate runtime patches, save round-trips, or optional integrations.
- **XML:** `Tests/ValidateXml.ps1` passes 376 checks across the three shipped XML files:
  parsing, metadata/dependency contract, GitHub description link, duplicate/empty language keys,
  English/French key and placeholder parity, and literal translation keys referenced in C#.
  There are no shipped Defs or XML patches. Save serialization remains a manual scenario (12).
- **Description:** added the repository link directly in `About.xml`'s description; it was
  previously present only in the separate `url` field.

## Previous status and in-game evidence

Status sheet, read by a pass over every mod rather than by asking each session in turn.
It lives at the root, never in `Mod/`, so Steam never receives it.

The fields derived from disk on 2026-09-12 were checked one by one and hold. The four the sweep
cannot fill are settled here.

- **Previous `stage`** — `preTest` before the owner's completion request. Nothing has ever been published: no Workshop item, and
  `CHANGELOG.md` still marks 1.0.0 unreleased. The code and the showcase are finished, which is
  what separates this from `port` or `showcase`; what it waits on is a test pass, which is what
  separates it from `tested`.
- **`dependencies`** — `declared`, but only after checking, because the About lists **six**
  non-vanilla `loadAfter` entries that are not in `modDependencies`: Better Workbench Management,
  Nice Bill Tab, its Expansion, Choose Your Recipe, Dubs Mint Menus and Everybody Gets One. None
  of them is a dependency. Every one is reached by reflection, the only assemblies referenced at
  build time are the game's and Harmony's, and each accessor returns a neutral value when its mod
  is absent. `loadAfter` is there because we hook onto them when they happen to be present, and
  scenario 17 of `TESTING.md` exists to prove the mod runs with all six gone. An undeclared
  dependency is not cosmetic — on 2026-09-11 Reequilibrage animaux took 47 vanilla animals down
  with it, Muffalo included, because the class it injects belongs to a mod that was not declared
  and not loaded — which is why this one is written out rather than waved through.
- **`tested_on`** — 2026-09-01, when the base loop was confirmed by hand in a real save: bills put
  up and taken down on their own, a recipe unlocked by research arriving suspended, and deleting an
  automatic bill excluding its recipe. The line the sweep puts there by default, "never seen
  running", was wrong. But read the date against the seven lines above it: the build that ran that
  day is not this one, and everything written since has only ever been compiled.
- **`remaining`** — seven scenarios of `TESTING.md` that have never run. No known defect left
  unfixed and no feature missing from the first pass: what remains is unverified, not broken. The
  compatibility bridges come high because they fail silently by construction — a lost feature, never
  a crash — so nothing but a run will tell.

`TESTING.md` stays the source: it says for each scenario what it proves and what its failure looks
like. This sheet keeps only the balance.

One layer no longer waits on a run at all. `Tests/` holds a program that exercises the shipped
assembly against the real game assemblies with no game running, 44 checks over the decision layer,
non-zero exit on failure. It does not shorten the list above, because what it proves is arithmetic
and what the list wants is wiring.

`licence` vocabulary: `open` an explicit licence, `silent` no licence and a dead source,
`alive` no licence but a living source, `forbidden` a written refusal, `original` owing nothing
to anyone — not a name, not an idea traceable to one mod, not a value derived from its assets.
