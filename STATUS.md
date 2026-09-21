---
localization: complete
translation_en: complete
translation_fr: complete
mod:          Bill Autopilot
packageId:    nelim.billautopilot
repo:         Rimworld-Bill-Autopilot
visibility:   public
detached:     yes
stage:        done
licence:      original
licence_at:   original work
licence_name: MIT
licence_file: LICENSE (identical copy in Mod/LICENSE)
dependencies: declared
showcase:     complete
settings_audit: complete
tested_on:    2026-09-01
workshop:
remaining:
  - unverified: the Pickle suite has never been run; done -> tested needs the pass without the optional mods, the pass with them, and one per language, with exitReason and the scenarios-played against features-discovered counts read before the numbers
  - unverified: every @review screenshot the suite attaches; a green there says the trip happened, not that the image shows anything
  - unverified: loading a save with the mod removed, which no Pickle run can do since the mod list is fixed at startup
  - unverified: RIMMSQOL revealing the shortcut in its own interface, and that visibility choice surviving a restart
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
updated:      2026-09-21, Pickle suite written
---

# Bill Autopilot — status

## Pickle suite written — 2026-09-21

This closes the one gate the audit below left open. **Stage: preTest -> done.** Written on top of
`878ac02`, the audited revision; no shipped source, asset or resource changed, so every validation
recorded in that audit still holds and the delivered DLL is still SHA-256 `D13D4225...DB599E`.

`Tests/Pickle/` now holds 19 feature files, a companion test mod (`Bill Autopilot - Pickle tests`,
`nelim.billautopilot.pickletests`, never distributed) and a step assembly of 84 steps.

**Written, not run.** The audit's own rule applies: running them is a criterion of `done -> tested`,
not of this transition. Nothing here claims a game was launched; none was.

### What the suite holds, and what it deliberately does not

Only what a running game can show. The decision layer — which mode applies, which target, which
floor, the clamps, the overflow guard, the fallback for a repeat mode whose owner has gone, and the
settings round trip through Scribe — stays in `Tests/BillAutopilot.Tests.csproj` and is not
restated: a Pickle run takes the machine for tens of minutes, and a scenario repeating a unit test
would cost that on every run and add nothing.

The 19 features cover TESTING.md scenarios 1-16 and 18-20. Each one's reason for needing a game is
recorded per feature in `Tests/Pickle/README.md`: a real dialog whose announced count must equal a
real intake, a count read through the game's own RecipeWorkerCounter, Notify_BillDeleted raised by a
real BillStack.Delete, a postfix on a real bill's LabelCap, a drift capture proven by a bill that is
a different object from the one edited, two benches of one kind, and RimWorld's own Scribe.

Scenario 17 became the minimal pass itself rather than a feature. Scenario 19's Choose Your Recipe
half has no scenario: that mod removes disabled recipes from the workbench before the autopilot sees
them, so there is nothing of this mod's to assert.

### Passes declared

`TESTING.md` now states how many passes a verdict needs and what each covers: without the optional
mods, with them (`Tests/Pickle/wsl-deps.map`), and one per language. **No incompatibility pass**, and
the reason is recorded: `About.xml` declares no `incompatibleWith`.

`wsl-deps.map` mounts Better Workbench Management (935982361) and Dubs Mint Menus (1446523594), the
two of the six optional mods installed on this machine, both packageIds read from their own
About.xml. Nice Bill Tab, Nice Bill Tab - Expansion, Everybody Gets One and Choose Your Recipe are
not subscribed here, so no line was written for them rather than guessing a Workshop id: their
scenarios carry `@requires:` and are skipped. **A pass is therefore green with scenarios never
played, and its report has to be read for skips as well as failures.**

### Checks performed on the suite itself

- `dotnet build Tests/Pickle/Source/BillAutopilot.PickleSteps.csproj -c Release`: success, 0 warnings.
- Every step text used in the 19 features resolves: 98 distinct texts against the 84 defined here
  plus Pickle's 202 built-ins, **0 undefined**. An undefined step costs a whole run, so this was
  checked mechanically rather than by reading.
- **0 duplicate** step texts here, **0 collisions** with Pickle's own vocabulary, and **0 collisions**
  with the SkillIcons, WorkStudio, ArchitectStudio and QuietNewFactions suites on this machine. Two
  suites sharing a step text produce "Ambiguous step" and fail healthy scenarios.
- **0 steps defined and unused**; two written and unreached were deleted rather than kept.
- No scenario spells an English label: every marker, dialog, letter and message is rebuilt from the
  mod's own translation key, so the same suite is the French pass.
- `Tests/ValidateXml.ps1`: **407 XML CHECKS PASSED**, unchanged — the test mod lives outside `Mod/`.
- `.build/bin/tests/Release/BillAutopilot.Tests.exe`: **74 CHECKS PASSED**, unchanged.
- Nothing was added to the distributed `Mod/` folder, and no build output sits inside the test mod.

### Next transition

`done -> tested` needs the passes above actually run, their `exitReason` read before their numbers,
the scenarios played compared with the features discovered, and every `@review` screenshot opened
and looked at. `Tests/Pickle/README.md` keeps what no pass can reach: loading a save with the mod
removed, RIMMSQOL's own interface, the Nice Bill Tab drag, and two Better Workbench Management
details written against an interface this session could not read rather than guess at.


## Workflow audit — 2026-09-21

Audited revision `772abc8cb784021cf44fc4740de65f0686b8803c` (main, equal to origin/main after
`git fetch`), working tree clean at start; only STATUS.md is modified by this audit. Repository:
`C:/Users/nelim/Documents/rimworld/BillAutopilot`, distributed folder `Mod/`.
`stage` uses the literal names of the workflow chain, no codes.

**Previous stage: done. Retained stage: preTest.** `l10n -> preTest` is established;
`preTest -> done` is not, because one mandatory deliverable is missing (below). This is a
missing deliverable, not a defect of the shipped mod.

| Transition | Result |
| --- | --- |
| dansMonoRepo -> horsMonoRepo | Validated: standalone repo, GitHub origin, main pushed and in sync, STATUS.md, README/CHANGELOG/ATTRIBUTION/LICENSE in English; LICENSE and ATTRIBUTION.md identical in `Mod/` (SHA-256 compared); public + original + MIT coherent; names coherent. |
| -> ModIcon generated | Validated: mod build succeeds (0 warnings/errors); rebuilt DLL SHA-256 equals the shipped one (`D13D4225...DB599E`); ModIcon is a 128x128 PNG. |
| -> Preview generated | Validated: Preview 896x504 PNG, 557,964 bytes (< 1 MB). Not re-inspected visually in this pass; the 2026-09-13 inspection is kept, no new doubt. |
| -> preOptions | Validated: English description ending with `[url=https://github.com/vbardales/Rimworld-Bill-Autopilot]Source code on GitHub[/url]`; no prefix/suffix required. |
| -> options | Validated on code and automated tests (`settings_audit: complete`): 74 checks pass; hidden MainButton shortcut defined with `buttonVisible=false`. In-game and RIMMSQOL checks belong to done -> tested. |
| -> l10n | Validated: 54 keys identical in EN and FR, all defined for every literal key used in the sources, no empty value; FR DefInjected for the shortcut resolves (`Check-DefInjected.ps1`: 2 keys, 0 errors). Runtime accent/clipping check stays unverified. |
| -> preTest | Validated: Harmony is the only hard dependency and is used; six optional mods are `loadAfter` only, matching the reflection bridges in `Source/Compat/`; no LoadFolders or conditional patches ship. |
| preTest -> done | **Not met.** Met: 20 written scenarios in TESTING.md, automated suite green (74 checks), XML checks green (407). **Missing: Pickle (Gherkin) tests are not written** (no `Tests/Pickle`, no `.feature`), and neither TESTING.md nor this file justifies their scope or non-applicability. Scenarios such as the bills tab redrawn by other mods, the Nice Bill Tab drag, save/reload and the Steam Deck layout are what only a running game shows, so a justified scope is unlikely to be empty. |
| done -> tested | Unverified: no game run. The 2026-09-01 result is historical and does not cover this DLL. |

### Commands and results

- `git fetch origin`, `git status -sb`: in sync, clean.
- `dotnet build Source/BillAutopilot.csproj -c Release -p:OutputPath=../.build/audit-build/`: success, 0 warnings/errors, DLL hash identical to the shipped one.
- `dotnet build Tests/BillAutopilot.Tests.csproj -c Release`, then `.build/bin/tests/Release/BillAutopilot.Tests.exe`: **74 CHECKS PASSED**.
- `Tests/ValidateXml.ps1`: **407 XML CHECKS PASSED (5 files)**.
- `scripts/Check-DefInjected.ps1 -TransMod Mod`: 2 keys checked, 0 errors.
- Key cross-check: 54 EN = 54 FR; every literal `BillAutopilot.*` key used in the sources is defined.
- No RimWorld instance was launched (Windows or WSL), no Pickle run, no lock taken, nothing published.

### Next transition

Write the Pickle tests, or state in TESTING.md, with reasons, which part of scenarios 1-20 only a running game can show and write that part as Gherkin. Declare the passes in TESTING.md (without optional mods, with optional mods, one per exclusive combination). Running them is not required for `done`.

### Optional

Keep `@review` scenarios for captures only; prefer the existing unit suite for anything provable outside the game.


## Technical settings gate completed — 2026-09-13

This result supersedes earlier pending settings-serialization entries. Base revision
`c1df4a8ef1970473384489beee0580a9241697ec` plus the existing local implementation,
attribution, description and documentation edits. This pass adds tests/documentation;
no runtime source or distributed DLL change was needed. Existing history is preserved.

`Tests/SettingsPersistence.cs` adds 18 checks using the actual shipped ExposeData and
native Scribe save/load/finalize paths. Nondefault values survive, cleared overrides
stay cleared, omitted fields restore defaults, null collections become writable,
legacy-shaped custom modes without identity fall back safely, and a custom quantity
edit retains inheritance after reload. Full executable output is retained in
`Tests/Results/settings-persistence-2026-09-13.txt`: **74 checks passed**, exit 0.
Test Release build: zero warnings/errors. The tested shipped DLL is still SHA-256
`D13D4225B3667CA08F857FB0771F507A2CB472F04C82ACA232442F1B44DB599E`.

The harness uses real loadable types in the native discovery cache and explicitly
registers the three serialized model types because it has no mod loader. Some unrelated
game interfaces cannot be enumerated on the desktop .NET Framework CLR. DeepProfiler
is disabled and logging uses a managed fail-on-error sink. These are test-process-only
fixtures; neither Scribe nor ExposeData is mocked or patched. Initial attempts exposed
those harness dependencies and failed; the final successful run does not claim the
unmodified console environment can initialize the entire game. Details in TESTING.md.

Settings source paths were rechecked: notification gates the letter queue; marking
gates the bill-label postfix; cap and profiles feed AutoBillSync; both settings access
routes use the same native dialog/instance and WriteSettings persistence. The prior
quantity, default, mode-fallback and shortcut-definition checks remain covered by the
suite. Empty/invalid text input is not applicable to the pointer-only quantity controls.
Absent/older stored fields and relevant model persistence are now tested. Actual
production, UI rendering, process restart and customization interactions remain game
scenarios rather than claims of console coverage.

**settings_audit: complete under the user's technical-gate interpretation.** This does
not claim the interactive verification required by the unamended MOD_SETTINGS protocol;
the user's explicit override places those checks in `done -> tested`.

**Stage: preOptions -> options -> l10n -> preTest -> done.** Current static EN/FR resource
and dependency validations remain valid; no runtime text/Def/dependency changed in this
pass. The written functional scenarios, passing executable suite and XML validation
establish readiness for the final game pass. `done` is not `tested`. No game was launched,
no real integration was exercised, and the historical tested_on date is not renewed.
The remaining entries track current-build FR/EN UI, logs, new/existing game, persistence,
shortcut/RIMMSQOL and optional integrations. Next required transition: execute those
scenarios in game, retain logs/version details and fix any failures actually found.


## Description link fix — 2026-09-13

Replaced the final raw repository URL in `Mod/About/About.xml` with the required
Steam markup: `[url=https://github.com/vbardales/Rimworld-Bill-Autopilot]Source code on GitHub[/url]`.
The destination is unchanged from the repository verified during the audit.
`Tests/ValidateXml.ps1` now checks the exact final link rather than URL presence alone.
All **407 XML checks over five files passed**; `git diff --check` passed.
No compiled source or image changed in this fix, so their independent validations remain valid.
No Workshop publication or remote description update was performed.

**Stage: Preview generated -> preOptions.** This supersedes the earlier description-link
blocker, whose historical audit entries remain below. The next transition, `preOptions -> options`,
requires completion of the applicable technical settings checks, including serialization
round-trips and older stored values; `settings_audit` remains `partial`. Those missing results
are unverified, not established defects. Interactive game and RIMMSQOL checks remain separately
pending for the final game-validation gate under the user's workflow interpretation.


## Settings fixes — 2026-09-13

Implemented against base HEAD `c1df4a8ef1970473384489beee0580a9241697ec` plus local
changes. Existing attribution, README and STATUS edits preserved. This section
supersedes the earlier missing-shortcut and custom-mode-edit defects.

- `BenchProfile.AdjustTarget` preserves explicit/inherited mode identity, permits
  zero for a resolved custom mode, protects integer addition from overflow and
  leaves the custom second-count override unchanged. The recipe UI calls this
  method; normal Maintain target/floor constraints remain enforced.
- New `BillAutopilot_Settings` MainButtonDef uses `buttonVisible=false` and the
  native worker visibility mechanism; nothing forcibly hides it every frame.
  `MainButtonWorker_Settings.Activate` opens RimWorld's `Dialog_ModSettings` with
  the existing BillAutopilotMod instance. The native dialog renders the same settings
  and calls WriteSettings on close. No customization dependency was added.
- Native MainButtonDef/MainButtonWorker/Dialog_ModSettings implementations inspected
  in the installed game assembly using ilspycmd with DOTNET_ROLL_FORWARD=Major.
  No third-party source was copied. English label/description are native Def values;
  both French DefInjected fields were added and validated.
- Both Release builds succeeded with zero warnings/errors. Shipped DLL SHA-256:
  `D13D4225B3667CA08F857FB0771F507A2CB472F04C82ACA232442F1B44DB599E`.
  `Tests/ProfileTests.cs`: **56 checks passed**, including ten quantity regressions
  and two worker contract checks. `Tests/ValidateXml.ps1`: **407 checks passed over
  five XML files**. Shared `Check-DefInjected.ps1 -TransMod <repo>/Mod`: **2 keys,
  0 errors**. `git diff --check` passed.
- An initial executable attempt to call native Worker.Visible failed because
  ModsConfig initialization reaches Unity ECalls unavailable outside the game.
  That is not reported as a passing runtime test. The final automated check verifies
  the worker retains the native visibility getter; actual reveal/hide behavior is
  explicitly left to scenario 20 in TESTING.md. No RIMMSQOL run was performed.
- Current EN/FR static localization remains complete after inspecting the new Def
  and successful injection checks. Previous claims that no Defs exist are historical.
  New native settings route and changed quantity behavior need their game regressions.

**Stage: horsMonoRepo -> Preview générée**, using literal workflow state names.
The two identified implementation defects are fixed; the DLL is rebuilt and existing
independent icon/preview inspections remain applicable. The next preOptions gate is
still blocked by the previously recorded raw GitHub URL in About.xml's description.
`settings_audit: partial` remains honest: the broader settings serialization/upgrade
coverage has not been established. No game pass or settings-complete status is claimed.


## Attribution fix — 2026-09-13

Added English `ATTRIBUTION.md` and an identical distributed copy in
`Mod/ATTRIBUTION.md`. The document credits the six optional integrations and
explains the specific interfaces studied, distinguishes original adapters from
copied implementations, identifies external runtime/build tools and records
AI-assisted code/art provenance. No third-party licence or permission was invented.
No copied companion-mod source, assembly or artwork was identified in the reviewed
files; this is a scoped repository finding, not a claim about unexamined history.
The existing original/MIT/public decision is consistent with that recorded scope.

**Stage: dansMonoRepo -> horsMonoRepo.** This resolves the first-gate attribution
defect from the audit below. The autonomous repository, public remote and pushed
commit were verified during that audit. Its findings remain below as historical
evidence; this section supersedes its retained baseline and immediate next-step text.
The next transition remains blocked by the recorded development/settings defects;
the already-validated builds and images do not establish development completion.
No game validation is claimed. Existing unrelated working-tree edits are preserved.

Validation: root/distributed attribution and licence copies compared byte-for-byte;
`git diff --check` passed. This documentation-only fix does not change the shipped
DLL, settings, translations or images, so those independent checks were not rerun.


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
- **Licence:** MIT, copyright (c) 2026 Nelim. `LICENSE` and `Mod/LICENSE` are byte-identical.
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
