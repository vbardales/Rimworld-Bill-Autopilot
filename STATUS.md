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
workflow_stage: done
licence:      original
licence_at:   original work
upstream_mod_remotes: N/A
licence_name: MIT
licence_file: LICENSE (identical copy in Mod/LICENSE)
dependencies: declared
showcase:     complete
settings_audit: partial
tested_on:    2026-09-01
workshop:      3806709456
remaining:
  - unverified: gate to tested, condition 2 - every @requires scenario has RUN: all ran green at least once (2026-10-02 reading of the kept reports: features 13-19 and 22 in French 23/23, feature 20 3/3, feature 26 2/2, gallery image 5 1/1); the one left is gallery image 3 (the switch-on confirmation), red in 1-4c on a test-isolation leak fixed in ProfileSteps, replay 24d queued, no report on disk yet
  - unverified: gate to tested, condition 3 - manual tests: the save-with-the-mod-removed case (removal chain), RIMMSQOL, Nice Bill Tab drag, BWM details and Choose Your Recipe are now automated and green; what is left is opening every @review capture not yet read (fr1 bills tab, five in fr3 - see docs/runs/pickle-runs.md) and the gallery images with the owner
  - unverified: full pass on the current build (DLL BA624725, sources modified since the passes of 4B89FBA7): suite 01-26 without the optional mods, with them, in English and in French; only the features touched by each fix have been replayed (v2: 13/13)
  - unverified: settings_audit is partial because settings-page and profile-window UI code changed on 2026-09-24; technical tests pass (74 checks), the full final pass and the French pass are still owed
  - unverified: English and French runtime translation checks on the final build (raw keys, fallback, clipping)
session:      local_25f4fbd1-6efc-46eb-a0a6-5a9f0eb26e21
updated:      2026-10-02, audit against AUDIT.md of 2026-09-25: stage kept at done, evidence pruned, nothing in Mod/ or Source/ touched
---

# Bill Autopilot — status

## Translation audit

- French review by the owner: **done 2026-10-02**, revision `bdfd660` (was b010196 before the rebase of 2026-10-02, same content), relayed in chat by the owner: five corrections
  (`Settings.Integrations`, `Settings.IntegrationsDesc`, `Settings.NotifyDesc`, `Profile.UncountableDesc`,
  `Profile.CustomFloor`) applied, report linked to that commit, no colon gender agreement required. `translation_fr`
  is `complete`. Any later change to a French file sets it back to `unchecked`. This line is the owner's; a session does not edit it.
- French lives in two files: `Mod/Languages/French/Keyed/BillAutopilot.xml` (68 keys) and
  `Mod/Languages/French/DefInjected/MainButtonDef/BillAutopilot.xml` (2 keys, the hidden
  RIMMSQOL settings shortcut).
- `FRENCH_REVIEW.md` (mod root) generated 2026-09-30 by `_tools/Generate-FrenchReview.ps1`
  (adapted from FoodCourt's own script) from the working tree after the 2026-09-30
  gender-agreement rule. Every row read: none flagged, no `?` cells — no text in this mod
  agrees in gender with a pawn (everything addresses the player or names a workbench
  type/recipe), so the `{PAWN_gender ? ...}` switch does not apply anywhere here.
- Original column equals English throughout: Bill Autopilot is original work (`licence: original`),
  not a port, so there is no separate source-language text.

## Audit of 2026-10-02 (AUDIT.md of 2026-09-25, protocols repository)

**Retained state: `done` (`stage: done`, `workflow_stage: done`).** Previous state `done`, unchanged. This replaces
no earlier decision paragraph: the 2026-09-21 workflow audit below stays as written, and this one is added on top.

Revision audited: `f269fe9` plus the uncommitted working tree (`Source/`, `Mod/` incl. `BillAutopilot.dll`,
`Tests/Pickle/`, `.github/`, `CHANGELOG`/`STATUS`/`TESTING`/`PUBLICATION` edits, new features 23-26 and `_tools/`).
Nothing under `Mod/`, `Source/` or `Tests/Pickle/` was changed by this audit, so queued Pickle requests keep their tree.

Checked now, outside the game (no RimWorld started, no lock taken):

| Check | Result |
| --- | --- |
| `Tests/ValidateXml.ps1` | **476 XML CHECKS PASSED (5 files)**. It first failed on a stale rule: it demanded the hand-written BBCode link, while `About.xml` now carries the plain text generated from the Markdown source (`Source code on GitHub (URL)`, PUBLISHING.md 2026-09-25). The check now accepts both forms |
| `dotnet run --project Tests/BillAutopilot.Tests.csproj -c Release` | **74 CHECKS PASSED** against the shipped DLL |
| `dotnet build Source/BillAutopilot.csproj -c Release` to a scratch folder | 0 warnings, 0 errors; SHA-256 begins `ba624725f320`, identical to `Mod/Assemblies/BillAutopilot.dll` |
| `node --test .github/tests/*.test.mjs` | 71 of 71 passed |
| Plural keys | 15 `.One`/`.Many`/`.Zero` keys, identical in English and French. `NewRecipeLetterTitle` has `.One` and the bare key as its many-form, no `.Many`: accepted by the XML check, not the literal `<key>.Many` shape TRANSLATIONS.md names (reserve, not a defect) |
| `@wip` | no scenario tagged `@wip` in `Tests/Pickle/Mod/Pickle/Features/` (gate to `tested`, condition 1: met) |
| `.dds` | `Autopilot.dds` exists on disk, was never tracked (`git log --all -- '*.dds'` empty) and is ignored by `*.dds` |
| Upstream | original work (`licence: original`): no source mod, no origin repository, nothing to fork or send a PR to. Neighbour mods are not upstreams (see BACKLOG.md) |
| Workshop item | `Mod/About/PublishedFileId.txt` = 3806709456, committed in `13ac5ed`; `CHANGELOG.md` opens with `1.0.0 - unreleased` above `0.1.0` (creation of the publishIdFile, 2026-09-23). A working-tree edit that dated `1.0.0` was reverted: nothing has been published as 1.0.0 |

Not rerun: the Pickle suite (taking the machine for a full pass is a `done -> tested` act, queued small requests only),
the in-game French and English reading, and any `@review` capture.

Evidence pruned on 2026-10-02 (the rule of AGENTS.md, "Test evidence": the latest report per scenario, an older one
only as sole proof). 306 MB became 6.6 MB. Kept, in `docs/runs/evidence/`: `2026-09-25-fr1-01-06`, `fr2-07-12`,
`fr3-13-22` (French passes of features 01-22 on build 4B89FBA7), `2026-09-25-removal-chain`, `2026-09-25-rimmsqol-20`,
`2026-09-24-skipcheck-13-17` (only proof of the pass without the optional mods for the skip), `2026-09-27-replay-fixes-v2`
(13 of 13, the later build), `2026-09-27-nomaxbills-26-retry`, `2026-09-28-galerie-studio-1-4c` (images 1, 2, 4) and
`2026-09-28-galerie-studio-5b` (image 5). Deleted: `2026-09-25-bwm-22` (replayed in fr3), `2026-09-26-gallery-1-4`, `-gallery-5`,
`2026-09-27-gallery-1-4-retry`, `-retry2`, `-retry3` (old colony, owner judged unusable), `2026-09-27-nomaxbills-26` and
`replay-fixes` (0 scenarios), `2026-09-28-galerie-studio-1-4`, `-1-4b`, `-5`. In the kept folders `report.html` and
`messages.ndjson` were removed and captures converted to JPEG (quality 88). One failure capture of image 3 in `1-4c` was
lost in the conversion (the file could not be opened); its cause is documented in "24b2 red" below and the replay `24d`
supersedes it. Sections below that cite a deleted folder (`gallery-1-4`, `gallery-5`, `retry3`, `1-4b`) are historical;
no front-matter field points to any of them.

**Next transition (`done -> tested`):** replay gallery image 3 on the fixed `ProfileSteps` (`24d`), run the whole suite on
the current build without the optional mods, with them, and once per language; open every `@review` capture; the
owner validates the gallery and the French. Optional, not a blocker: give `NewRecipeLetterTitle` a `.Many` key.

## 24b2 red: test-isolation leak in ProfileSteps, not the mod — 2026-09-29

`24b2` (galerie-studio-1-4c): 3/4 passed, image 3 failed — "no confirmation is open" for
`Bill Autopilot's toggle is used to switch "HandTailoringBench" on`. Verified directly against
`docs/runs/evidence/2026-09-28-galerie-studio-1-4c/summary.json`, not from the peer relay alone.

Root cause: `ProfileSteps.ResetToDefaults` ("Bill Autopilot settings are at their defaults")
cleared `BillAutopilotSettings.profiles` but never touched `BillAutopilotState.seeded/known/pending`.
Images 1 and 2 (same feature file, same save, no reload between scenarios) enable
`HandTailoringBench` and sync it, which seeds that bench type for the rest of the session. By
image 3, `BenchActivation.Toggle` correctly sees `state.IsSeeded(bench) == true` and skips the
confirmation on purpose — the same short-circuit that is right for "switching back on after a
pause". Test-isolation gap, not a mod defect: `Source/UI/BenchActivation.cs` is unchanged.

Fixed in `Tests/Pickle/Source/ProfileSteps.cs`: `ResetToDefaults` now also clears
`seeded`/`known`/`pending` via reflection, same pattern already used for `profiles`. Rebuilt clean.
Resubmitted as `24d` (label `galerie-studio-1-4d`, evidence
`docs/runs/evidence/2026-09-29-galerie-studio-1-4d`), images 1/3/4 only.


## 5da4 red: Pickle ran off the main thread, not our code — 2026-09-28

`5da4` (images 1-4, 8 torches): 1 of 4 passed (image 2). Light check passed everywhere (the 8-torch fix
holds); the three failures trace to one shared cause, verified in `Player.log` and `junit.xml`, not
guessed from the relay: `RunSession.InvokeMethodBinding` ran off Unity's main thread
(`System.Threading.ThreadPoolWorkQueue.Dispatch` in the stack). Image 1 ("0 bills, not 3"): `RecipeProbe`
caught the resulting exception and returned `false` for every recipe, correctly (`WarningOnce`, no
crash), which pushed every recipe to `uncountableMode` = `Excluded`. Image 3 ("no confirmation is
open"): same zero count, and `Find.WindowStack.Add` off-thread did not register. Image 4: a direct
engine crash (`MapPawns.AssertMainThread`), no BillAutopilot frame in the stack. Nothing to fix in this
mod. **`6513` (image 5 retry) is green, 1 of 1**, checked directly in its `summary.json` — the
dispatcher's relay named the stale ticket `d8bf` (the first, 4-torch attempt, already reported red
earlier), not `6513`. Resubmitting images 1, 3, 4 once. Evidence `docs/runs/evidence/2026-09-28-galerie-studio-1-4b`
and `-5b`.

## Gallery redone in the zen studio colony, with a roof and lights — 2026-09-28

The owner judged the `464d` captures **not usable as showcase images**: wrong colony (Pickle's played
`test-colony`, with a fallen monolith and debris), and the interiors too dark. Her direction: use `zenNelim` in
NPT, which is Nelim's Pickle Tools' zen meadow studio (fixture `nelim-zen-meadow-studio`,
`PickleTools/ScreenshotStudio`), and put lights in the interiors. **The `464d` and `0c9e` captures are superseded**
(kept on disk until the new ones are judged, then to be deleted per the evidence rule).

What changed, all in `Tests/Pickle/`, nothing in `Mod/` or `Source/`: `23-gallery-vanilla.feature` and
`24-gallery-neighbours.feature` now load `nelim-zen-meadow-studio` (paused) and build everything in the studio's
"display" pavilion (cells 116-134 by 89-101, its empty interior meant for mod demonstrations, bench at (125, 96)).
The studio leaves every roof off on purpose, which reads as "outdoors" for a bench and is dark once roofed, so the
new step `the studio's display pavilion is closed, roofed and lit` (`GallerySteps.ClosePavilion`, replacing
`MakeRoom`) puts a door and wall segments where the pavilion is open, roofs it, and places four standing torches
(`TorchLamp`, filled: no power grid needed, and the warm light the studio's own lanterns already use), with the same
region-updater guard that fixed `88b3`. Two new pass maps: `wsl-deps.galerie.map` (the studio only, so images 1 to 4
stay "without the optional mods") and `wsl-deps.galerie-voisins.map` (the neighbours plus the studio, image 5).
Not yet played; the torch positions, how bright they read and the camera framing are guesses until a capture is
opened.

## 464d green: gallery images 1-4, all four — 2026-09-27

`464d` (gallery 23 retry, after the `MakeRoom` region-updater fix): **`exitReason: passed`, 4 of 4**,
evidence `docs/runs/evidence/2026-09-27-gallery-1-4-retry3`. Read from `summary.json` directly. The
region corruption seen in `88b3` does not recur. **All four gallery images now exist as real captures**
and are ready to be opened and judged: images 1, 3 and 4 for the first time on the fixed room-building
step, image 2 for the first time with the real per-recipe override setter (fixed after `5d66`). Image 5
(`24-gallery-neighbours.feature`) was already green, `0c9e`, 2026-09-26.

**Still owed before they go into `Art/Gallery`:** open all five and judge them with the
owner (bench indoors, Learning helper off, image 4 zoomed enough), then convert to JPEG as `1-`… to
`5-` (`0-preview.png` already there, a copy of the Preview). **The dispatcher's queue is empty for this mod** (`-List`, checked 2026-09-27): `7c5d` was
cancelled earlier and superseded by `b2c7`, which is done; the relay's own line still naming it as
queued is stale.

## 8240 green: No Max Bills alone, the ceiling fix proven in game — 2026-09-27

`8240` (feature 26 alone, pass `avec-nomaxbills`, No Max Bills: Redux staged without Better Workbench
Management): **`exitReason: passed`, 2 of 2**, evidence `docs/runs/evidence/2026-09-27-nomaxbills-26-retry`.
Read from `summary.json` and `Player.log` directly. `Integrations: ... No Max Bills found. Bill cap per
workbench: 2147483647` — `NoMaxBillsCompat`'s fallback fired, exactly as intended. "a cap above
fifteen is honoured" passed with 16 automatic bills genuinely standing, so the game's own 15-bill
interface limit really was lifted.

**The open question from 2026-09-27 is answered.** The Harmony crash (`Undefined target method` in
`RaiseBillCountLimitForImprovedWorkbenches`) is still in this log too, unchanged. But
`RaiseBillCountLimitForVanillaBillStackMethods` — the patch that actually lifts the interface's 15-bill
limit — ran anyway: `Harmony.PatchAll()`'s type-enumeration order happens to reach it before the one
that throws. So, in this build at least, **No Max Bills: Redux's own interface limit works even when it
crashes on its Better Workbench Management bridge.** Still worth a report to its author
(`justharry.nomaxbillsredux`, Workshop 3526216885) — the crash itself is real and logged as an ERROR on
every load without BWM — but it is not the defect that was feared: nothing here needs No Max Bills: Redux
to change before this mod can rely on the ceiling it reports.

Both No Max Bills corrections now have in-game proof (`8240` for the ceiling, `b2c7` for the Choose Your
Recipe pending-trap). Still queued: the gallery retry, resubmitted after fixing a real defect in
`GallerySteps.MakeRoom` (see below).

## 88b3 red: a real defect in the gallery's own room-building step — 2026-09-27

`88b3` (gallery 23 retry): **`exitReason: failed`, 1 of 4 passed** (image 2 only), worse than the
earlier `5d66` (3 of 4). Read from `junit.xml` directly. Images 1 and 3 failed on the same engine
exception: `Exception while rebuilding dirty regions: System.InvalidOperationException: Collection was
modified; enumeration operation may not execute`, in
`Verse.RegionAndRoomUpdater.RegenerateNewRegionsFromDirtyCells`. Image 4 then timed out entirely at
`Given the save "test-colony" is loaded` (180s) — consistent with the region graph left inconsistent by
the earlier crash, in the same game process. Image 2 uses the identical room-building step and passed:
not deterministic, which fits a re-entrancy bug rather than a bad value.

**Cause, and it is this session's own code, in `GallerySteps.MakeRoom`** (the "closed roofed room" step
added 2026-09-26): it destroyed things, spawned a wall or set terrain, and set the roof, cell by cell,
interleaved. `GenSpawn.Spawn` of a wall and `RoofGrid.SetRoof` each notify RimWorld's region system,
which keeps its dirty cells in a `HashSet` with no re-entrancy guard; interleaving destroys, spawns and
roof-sets across many cells in one call risks one notification's rebuild running while another's is
still populating that set. **Fixed 2026-09-27**: `MakeRoom` now disables `map.regionAndRoomUpdater`
for the whole build (destroy pass, then wall/floor pass, then roof pass, each over the full rect rather
than interleaved), and calls `RebuildAllRegionsAndRooms()` once at the end — the pattern RimWorld's own
bulk map edits use. Compiles, Check-Steps run again. Requeued as a fresh ticket once Check-Steps
confirms.

## b2c7 green: 13 of 13, the Choose Your Recipe fix proven in game — 2026-09-27

`b2c7` (features 04, 05, 10, 25, corrected filter, build `BA624725...`): **`exitReason: passed`, 13 of
13**, evidence `docs/runs/evidence/2026-09-27-replay-fixes-v2`. Read from `summary.json` directly, not
from the dispatcher's summary alone. Features 04, 05 and 10 (8 scenarios) replay what was already green
on the old build, now proven again after the removal-path change. **Feature 25's one scenario is the
first in-game proof of the Choose Your Recipe pending-trap fix**: "the announced recipe leaves and
returns, and the question is asked again" — green. Still queued: `88b3` (gallery 23 retry), `8240` (No
Max Bills alone). `7c5d` (the earlier, wrongly-filtered duplicate of this request) is confirmed gone
from the queue, not merely reported gone.

## Gallery run 5d66: 3 of 4 good, image 2 was never taken — 2026-09-27

`5d66` came back red: `exitReason: failed`, 3 of 4 passed. Images 1, 3 and 4 are real captures, kept in
`docs/runs/evidence/2026-09-26-gallery-1-4`. Image 2 failed on its own setup step before the profile window ever
opened, so nothing of it exists to judge. **Not a defect of the mod, a mistake in the feature file**: the override
line, `And Bill Autopilot keeps 200 of "Make_Patchleather" on "HandTailoringBench", restarting at 100`, matched by
text alone to `ProfileSteps.AssertRecipeCounts`, an assertion (`[Then]`), not a setter — Pickle binds a step by its
text regardless of the Given/When/Then word written in the feature. Nothing was ever set, so the assertion read the
profile's own default (`BenchProfile.DefaultTargetCount = 50`) and failed with "kept at 50, not 200".
Fixed: a new `[Given("Bill Autopilot overrides {string} on {string} to keep {int}, restarting at {int}")]` step
(`ProfileSteps.SetRecipeOverride`) that actually writes the override; `23-gallery-vanilla.feature` now uses it.
Check-Steps: 105 patterns, none ambiguous, every step line resolves. Queued: `ba6d` (feature 23 alone). `0c9e`
(image 5) is green, `docs/runs/evidence/2026-09-26-gallery-5`.

**Correction, 2026-09-27: `9ae9` and `92fe` were NOT a shared-tooling outage.** The dispatcher read both as
`exitReason: infrastructure-error` and grouped them with failures on other mods; the owner asked whether that was
really checked, and it was not — read the `Player.log` of each instead of trusting the label. Two different, real
causes, both this session's:

- **`9ae9`** (features 04, 05, 10, 25): a filter-syntax mistake of my own. `Submit-PickleRun.ps1 -Filter` passes its
  value straight to Pickle's `-pickle-run`, and mine, `features 04,05,10,25`, is not a term Pickle understands
  (`InvalidOperationException: filter 'features 04,05,10,25' matched no scenarios`; the log names the valid forms —
  `@tag`, mod name, feature path, `path::name`, `path:line`, `::name`). The form that has worked before is a
  comma-separated list of full feature file names, no spaces, no "features" word:
  `13-better-workbenches,14-foreign-repeat-mode,...` (seen in `2026-09-25-fr3-13-22/Player.log`). `7c5d` carried the
  same mistake and was cancelled unrun; resubmitted correctly as `b2c7`, filter
  `04-new-recipe,05-delete-refuses,10-bill-cap,25-recipe-taken-off-the-bench`.
- **`92fe`** (feature 26, pass `avec-nomaxbills`): **the same filter mistake as `9ae9`**, corrected a second time
  here. `-Filter "feature 26"` is not a valid term either (`InvalidOperationException: filter 'feature 26' matched
  no scenarios`); a single feature also wants its bare file name, no "feature" word: `26-no-max-bills` (confirmed
  against `2026-09-25-rimmsqol-20/Player.log`: `-pickle-run=20-rimmsqol-shortcut`). **First correction of this
  section was itself wrong**: reading the log, a real Harmony crash from No Max Bills: Redux stood out and was
  named as the cause, but it is not — RimWorld tolerates a mod class whose constructor throws and continues
  loading (our own startup line still printed afterwards), and the run's actual `exitReason: infrastructure-error`
  came from the filter, later, same as `9ae9`. `ba6d` (the gallery retry) carried the identical `feature 23`
  mistake and failed the same way. All three refiled with the corrected syntax: `88b3` (gallery 23,
  `23-gallery-vanilla`), `8240` (No Max Bills alone, `26-no-max-bills`, same pass map as before).

**The Harmony crash is still real and worth watching for.** `RaiseBillCountLimitForImprovedWorkbenches` finds its
Harmony target by reflecting into `ImprovedWorkbenches.Main`; when Better Workbench Management is absent that
reflection yields no method, and Harmony refuses a patch with an empty `[HarmonyTargetMethods]` result
(`HarmonyException: ... Undefined target method`), logged as `[ERROR] Error while instantiating a mod of type
NoMaxBillsRedux.NoMaxBillsReduxMod`. The game survives it, but `Harmony.PatchAll()` aborts the whole assembly's
patch pass on the first exception, in type-enumeration order that is not under this mod's control: whether the
OTHER patch in the same class (`RaiseBillCountLimitForVanillaBillStackMethods`, the one that actually lifts the
game's 15-bill interface limit) ran before or after the one that throws is unknown until `8240`'s result is read.
If it did not run, No Max Bills: Redux may not even raise its own interface limit when Better Workbench Management
is absent — a real defect in that mod, still to be read from `8240`, and still a matter for its author regardless
of how this pass fares.

Queued: `88b3`, `8240`, `b2c7`.

## French pass done, gallery captures redone — 2026-09-26

Stage stays **done**. The three French passes are green on build `4B89FBA7`: `baba` (01-06) 27 of 27, `dd78` (07-12)
20 of 20, `7018` (13-19 and 22) 23 of 23, all `exitReason: passed` (rows 11 to 13 of `docs/runs/pickle-runs.md`).
Superseded evidence folders (run 4, final 1 to 3, run5-fixcheck, fixcheck 2 to 4) were deleted on 2026-09-26 to free
disk; their results stay as text lines in `pickle-runs.md`.

The first gallery requests (`9f44`, `6434`) were cancelled: the background was wrong (bench outdoors, Learning helper
open, no zoom). `GallerySteps.cs` now has a closed roofed room step, a Learning-helper-off step and a camera framing
step (101 patterns, all compile, every step line resolves); features 23 and 24 use them. Refiled, waiting for disk
space (the worker starts nothing under 2 GB free): `5d66` (images 1 to 4, without the optional mods) and `0c9e`
(image 5, with the neighbours). Room, tutor and camera steps are unproven in game until these run.

The gallery images are opened and looked at one by one before they go into `Art/WorkshopScreenshots` as `01-`… to
`05-` (converted to JPEG). The `@review` captures of the earlier runs (features 06, 15, 18, 19, the French ones) are
still to be opened with the owner.

**Two corrections made in the code, 2026-09-26 (owner's rule: a correction goes into 1.0.0, a pure check into 1.0.1).**
The build `4B89FBA7` is superseded: the DLL is now `BA624725...` and the passes above were played on the old one.
(1) No Max Bills alone: the cap read the game's 15 unless Better Workbench Management reported more; now
`NoMaxBillsCompat` (type `NoMaxBills.Patch_BillStack_DoListing`) is asked when BWM does not answer, the settings
slider spans 1 to 100 and the sentence says "the game sets no limit" when there is none (new keys, EN and FR).
(2) Choose Your Recipe trap: a recipe announced and then taken off the bench kept its "pending" mark once the bill
was taken down, and was never offered again; `AutoBillSync.Remove` now forgets the open question
(`BillAutopilotState.Unannounce`). New scenarios, not played: `25-recipe-taken-off-the-bench.feature` (every pass),
`26-no-max-bills.feature` with the new pass map `wsl-deps.avec-nomaxbills.map`; new steps `RecipeListSteps.cs`
and one in `BillSteps.cs` (Check-Steps: 104 patterns, every step line resolves). **Queued 2026-09-27, once the disk cleared (~30 GB free):** `9ae9` (features 04, 05, 10, 25, default pass) and
`92fe` (feature 26 alone, `wsl-deps.avec-nomaxbills.map`), both on the current tree (DLL `BA624725...`), alongside
the gallery tickets `5d66` and `0c9e` already queued (25 requests ahead in the dispatcher). None has run yet.
Verification-only items stay in `BACKLOG.md` for 1.0.1.

**Publication prepared, not committed (2026-09-26).** The description is now one Markdown source, `## Steam
description` of `PUBLICATION.md`; `Mod/About/About.xml` was regenerated from it (`sync-about-description.mjs
--write`, the description is already the plain text of the source); `Mod/README.template.md` and `Mod/.steamignore`
are deleted. The `### 1.0.0` Steam change note starts with `[b]1.0.0[/b]`; `CHANGELOG.md` says `[1.0.0] - 2026-09-26`.
The manual publish workflow was generated with `generate-publish-workflow.sh` (`.github/publish-tag.yml`,
`script-tests.yml`, `publish.config.json`, scripts and tests; `--build-project Source/BillAutopilot.csproj`, gallery
dir `Art/WorkshopScreenshots`); its 68 generated tests pass locally. **Still to do before the `publish`:** commit and
push, the dry-run of the exact SHA (with `update_description`), the gallery, her manual validations. Run ID and SHA
of the dry-run go here once done.


## RIMMSQOL green; removal chain and BWM details written — 2026-09-25

Stage stays **done**. Request `dafa` (feature 20, pass `avec-rimmsqol`): **`exitReason: passed`, 3 of 3**, tree
unchanged since the request (checked by file dates), evidence `2026-09-25-rimmsqol-20`. The three `@review` captures
were opened and read: RIMMSQOL's own list offers "Bill Autopilot"; its edit page shows Visible ticked; the "Bill
Autopilot" button then sits at the right of the main bar and opens this mod's settings page. The settings page in
that capture reads "Brewery: 1 recipe", so the plural fix is now seen in game (English). The manual RIMMSQOL test is
**closed by automation**; its persistence across a restart stays not applicable (RIMMSQOL's own behaviour).

**The save loaded without the mod: green.** Request `cf3a` (`21-removal-write.feature`, the companion
`nelim.billautopilot.pickleremoval`, chain `-Then` / `-ThenWithout`): both launches **`exitReason: passed`**. Launch 1
wrote a game with a bill up, checked the save holds the mod's state as plain nodes (`billAutopilot...`, a positive
control) and no `Class="BillAutopilot` anywhere outside the header, and handed it to the companion. Launch 2's log
lists `nelim.billautopilot.pickleremoval` as the only mod of the game it loads, so the mod really was out; the game
loaded, ran 250 ticks, kept the bill as an ordinary bill (`the "HandTailoringBench" has 1 bills`), saved and reloaded,
with no error logged. This is the first time `-ThenWithout` has been seen running end to end. The manual "save loaded
with the mod removed" test is **closed by automation**. Evidence `2026-09-25-removal-chain/seq1` and `seq2`.

**The two Better Workbench Management details: green.** Request `932e` (`22-bwm-restriction-and-count.feature`, pass
with the neighbours): **`exitReason: passed`, 2 of 2**. A bill created from a tick carries the restriction set on its
bench (non-mechs, skill range 5 to 15, values that are not a bill's defaults, so the check cannot pass by accident),
and the count the autopilot decides with equals the bill's own with an additional product counted, at least 40.
Both manual entries are **closed by automation**. Caveat: neither scenario has been seen red (no mutation of the
bridge was tried), so they prove agreement today, not that they would catch a regression. Evidence
`2026-09-25-bwm-22`.

**Manual list, now:** every former manual test is automated and green or not applicable with its reason (table in
`TESTING.md`). What remains for a person: open the `@review` captures not yet opened (the French pass ones included),
and the owner's own validations before a `publish`.

## Red scenarios all replayed green; RIMMSQOL automated; description template — 2026-09-25

Stage stays **done**. Feature 14 replay `115b`: `exitReason: passed`, 3 of 3. With `c5dd` (feature 13) and `5a32`
(feature 15) every scenario that went red has been replayed green on build `4B89FBA7...`, which is what fail fast
requires of the tests before a `publish`; the full 69-scenario replay on this build is the regression pass that may
follow. **Still required before a `publish`:** the gallery (five captures), the owner's manual validations, and the
CI setup.

**Manual tests, classified as `AUDIT.md` asks** (table in `TESTING.md`, condition 3): RIMMSQOL is now automated by
`20-rimmsqol-shortcut.feature` (pass `wsl-deps.avec-rimmsqol.map`, request `dafa`, not yet played); RIMMSQOL's
persistence across a restart, the Nice Bill Tab drag and Choose Your Recipe are **not applicable**, each with its
reason; the save loaded without the mod (`-Then` with `-ThenWithout`) and the two Better Workbench Management details
are **to automate**; the `@review` captures are to be opened one by one.

**Description by template.** `OPERATIONS.md` documents `--description-markdown FILE` for the manual publish workflow.
`Mod/README.template.md` (6956 bytes, limit 8000) carries the description of `About.xml` as Markdown, with the
butchering correction, and `Mod/.steamignore` keeps it and `README.md` out of the upload. `About.xml` stays as it is
and the two texts must be kept the same. `ATTRIBUTION.md` (both copies, identical) now lists the mods and tools only
the tests use.

## Thanks brought to the current rules, CI path asked — 2026-09-25

Stage stays **done**. Reading PUBLISHING.md showed that every integration named, claimed or exercised must be
thanked, the test-only ones included. Done in the repository, not on the Steam page (which stays frozen until
hand-edited, see PUBLICATION.md): `Mod/About/About.xml` now links every neighbour's Workshop page at each
mention, thanks its author (Falconne, Andromeda, HICON, Dubwise, Uuugggg, Zaljerem, Just Harry), thanks TD Find
Lib and TDS Bug Fixes (mounted by the test pass for Everybody Gets One, never dependencies), names Pickle and
RimLogging as development-only, and no longer repeats Claude Code, already named under `AI-GENERATED`.
`PUBLICATION.md` carries nine drafted comments in copyable blocks (six neighbours plus No Max Bills, TD Find Lib,
TDS Bug Fixes), all under 1000 characters; the collection register `WORKSHOP_COMMENTS.md` has nine `drafted`
rows and Bill Autopilot added to the `Covers` of Harmony, Pickle, RimLogging and RIMMSQOL. Nothing is posted:
the item is private. The description grew by these lines; the text was validated (463 XML checks).

The CI path (manual workflow or semantic-release, tag and rollback target, gallery folder) was put to the CI/CD
session, which answered: the manual workflow, generated by them in this repository, tag and release created by the CI
after the first upload, gallery folder committed with the five captures and numbered `01-`… (`Art/WorkshopScreenshots`),
first-publication rollback = the owner sets the item back to private.

**One source for the description (CI/CD session, 2026-09-25, `Rimworld-Release-Admin` f196148, `Rimworld-protocols`
16f3c59; nothing forced).** The description is written once, in Markdown, in a fenced block under
`## Steam description` of `PUBLICATION.md` (no code fence inside, last line `[Source code on GitHub](URL)`); the CI
converts it to BBCode and **generates the `<description>` of `About.xml` from it**, and every dry-run and publish
stops if the two differ. This **supersedes** what was done on 2026-09-25 (`Mod/README.template.md` and
`Mod/.steamignore`): when the workflow is generated (on this standard from the start), move the text into
`PUBLICATION.md`, delete both files, and let `sync-about-description.mjs --write` rewrite `About.xml` (read the diff:
it will lose its BBCode links). The CI also refuses a change note whose first line lacks the version (`[b]1.0.0[/b]`).
Do not edit `.github/` by hand.

## First play of the optional-mod scenarios — 2026-09-25

Stage stays **done**. The features that needed Better Workbench Management, Nice Bill Tab, Dubs Mint Menus,
Nice Bill Tab - Expansion and Everybody Gets One had never run. Request `479c` (features 13 to 19, pass
`avec-facultatifs`): **21 played, 17 passed, 4 failed**. Features 16, 17, 18 and 19 are green. The skip check
`c6a4` is green: without the mods the 14 `@requires` scenarios are skipped, not failed. The four failures have
three causes, and two of them are defects of the mod:

1. **Better Workbench Management: the bridge lost everything on an ordinary bill (mod defect).** In
   `BetterWorkbenchesCompat.CaptureLinks`, `GetBillSetContaining` returns null for any bill that is not linked,
   and the next line asked that null set for its `Bills` through reflection. The exception was swallowed by the
   `catch` in `Capture`, which returned null and discarded the custom name and the count-away flag already read:
   `named 'nothing' in Better Workbench Management's own store, not 'caravan stock'`. Only a bill in a link
   group survived. Fixed with a null check; the warning now names the inner exception (it read "Exception has
   been thrown by the target of an invocation", which says nothing). The log line "Could not read Better
   Workbench Management data" is what gave it away.
2. **A repeat mode that throws took the tick down (mod defect).** `AutoBillSync.ShouldRetire` called a foreign
   mode's `ShouldDoNow` unguarded, so the exception went up through the tick as "Root level exception in
   Update()". `RecipeProbe` already guarded the same call. `ShouldRetire` now guards it, leaves the bill as it
   is and names the failure once.
3. **Everybody Gets One: the test environment was incomplete (not the mod), and that was not the whole story.**
   Its 1.6 version declares TD Find Library, which declares TDS Bug Fixes, and the staging mounts only what the map
   lists. The assembly loaded half a class ("Could not resolve type ... TD_Find_Lib.SearchEditorRevertableWindow"),
   the repeat mode threw from `ShouldDoNow`, and two scenarios of feature 14 went red: `TargetCount` instead of
   `TD_PersonCount`, and an `InvalidOperationException`. Both libraries are now in
   `wsl-deps.avec-facultatifs.map`, and the replay `c5dd` shows no loading error any more. The guard of point 2 is
   what the exception exposed, so it stays. The two scenarios were nevertheless still red: see the replay below.
4. **Nice Bill Tab, "putting a bill up tells the list to rebuild".** The take-down scenario passes on the same
   bridge, so the bridge works, and Nice Bill Tab lowers the flag only inside its own tab drawing (decompiled).
   The first guess (a race between two steps) was wrong: reordering them changed nothing. The cause is in the
   scenario: the Background sets no stock, so the bill is wanted at once and the autopilot's tick put it up before
   the cache was marked up to date. The scenario now starts from a bench with no bill.

**Replay `c5dd` (features 13 to 15, same build `4B89FBA7...`): `exitReason: failed`, 10 played, 7 passed, 3 failed.**
Feature 13, all three scenarios: **passed**, so the Better Workbench Management fix (point 1) is proven in game:
the custom name, the count-away flag and the link group all come back. The `@review` capture of feature 15 was
opened: "Make patchleather (auto)" carries the mark inside Nice Bill Tab's redrawn tab, the hand-placed bill does
not. The three failures were errors of the scenarios, not of the mod:

- **Feature 14, first scenario: a bill already standing (my first diagnosis of "wrong mode names" was wrong).** I
  read `Defs/BillRepeatModeDef.xml` at the root of Everybody Gets One - Continued, saw `TD_ColonistCount`, and
  renamed the modes in the feature, the README and a comment. The replay `5a32` then failed with `no
  BillRepeatModeDef named 'TD_ColonistCount'. This pass loaded: ... TD_PersonCount, TD_XPerPerson,
  TD_WithSurplusIng`: the mod's `LoadFolders` decides which of its `Defs` folders is read, and under 1.6 the game
  loads the original names. **Reverted** (README, feature, comment). The real cause of `TargetCount` instead of
  `TD_PersonCount` is the Background: it switches the bench on with nothing in stock, so the autopilot's tick put
  the bill up in `TargetCount` before the scenario set the bench default, and a sync never rewrites the mode of a
  bill that stands. Same family as feature 15. The scenario now starts from a bench with no bill (stock raised,
  synced, mode set, stock lowered); replay queued as request `115b`.
- **Feature 14, second scenario:** the bench took every recipe, and the 8 automatic bills of the default cap were
  used up by the first hats, so `Make_Patchleather` never got a slot. The Background now restricts the bench to
  that one recipe, as feature 13 does.
- **Feature 15, "putting a bill up":** as in point 4, now rewritten.

The replay of the corrected features 14 and 15 is request `5a32` (queued, same build; the DLL did not change).

Also seen: the shipped assembly changed again, SHA-256
`4B89FBA7D61C828A13663A74D002CA9508DC0AAD1FFD11FB64CE2F25F2670B83`. The requests already run (`6e5f`, `1f5e`)
describe `8E3C5CC7...` and are to be replayed on this one, after `c5dd`.

## Plural "1 recipes" fixed, DLL changed again — 2026-09-24

Stage stays **done**. Found on a capture of the fix check: the settings page wrote "Brewery — 1 recipes". The
count was inside the sentences as "{0} recipes", and a plural cannot be built by adding an "s" (French says
"0 recette" and "1 recette"). It is now a noun phrase the translator owns, three keys
`BillAutopilot.Recipes.Zero / One / Many` behind `RecipeCount.Phrase`, passed to the sentences as one argument;
`SummaryOff` is gone (the phrase is the line), the letter title has a `.One` form. The same fault was then fixed
wherever a count meets a noun, through one helper, `CountForm.Text(keyBase, count, ...)`, which picks
`keyBase.One`, `.Many` or (when defined) `.Zero`: the overridden count on the settings line (French read "1
surchargées", now "1 surchargée"), the clear button of the profile window ("Clear the override", "Effacer la
surcharge"), the cap slider label ("At most 1 automatic bill"), and the Dubs Mint Menus message ("1 autopilot bill
was left out of the template"). Both languages have 63 keys; 463 XML checks and 74 unit checks pass;
`Tests/ValidateXml.ps1` now accepts a source key whose `.One` and `.Many` exist. New scenarios: feature 02, the
brewery, which has exactly one recipe, must read the singular on the settings line; feature 19, the scenario with
one overridden recipe asserts the same for the override count.

**The shipped assembly changed again:** SHA-256 `8E3C5CC749BC698D267A1CCD91A373B3B177D0472178AA9682D1DEEBA3A7D5EE`
(it was `436E7179...` when run 5b passed). Run 5b therefore describes the build before this change: it stays as the
proof for the countability fix and the two captures, and the four queued requests, which have not started, will
replay features 01 to 19 on this build. Nothing here has been played in game yet. Not covered by any scenario: the
button, the cap label and the Dubs message read from a screen; the `@review` capture of feature 19 shows the button.

## Fix check replayed in game — 2026-09-24

Stage stays **done**. Run 5 (fix-check request `20260924-163910-321-41c5`, features 09, 18 and 19, English, no
optional mod, build `436E7179...`): **`exitReason: passed`, 12 of 12 scenarios played and passed**, one attempt.
That covers the two new scenarios of feature 09 (butchering kept in stock; the confirmation announces exactly
the bills the engine puts up) and the ones that failed in run 4 for the same reasons.

The four `@review` captures were opened: on the settings page the cross now sits left of the count with a
clear gap (defect 3 gone); on the electric smelter's profile window "Smelt metal from slag" reads *Keep in
stock* and the five uncountable recipes read *Never*. **Not covered by this run:** the Steam description still
says butchering and has to be edited by hand (PUBLICATION.md), and the other 57 scenarios are not yet replayed
on this build. An earlier attempt at the full suite, killed by the Pickle watchdog after 4 scenarios while the
machine was loaded (the save took 78 s to load), counts for nothing and is one line in `docs/runs/pickle-runs.md`.

Process, per the maintainer: **small requests, submitted through the TicketDispatcher** (`Submit-PickleRun.ps1`,
no watcher, no heartbeat: it messages the session at START, END and RUN_DONE). A fix check plays the fewest
scenarios; only the initial and final passes play everything.

## Three defects fixed, DLL changed — 2026-09-24

Stage stays **done**. The shipped assembly changed: SHA-256 is now
`436E71797FFF8241BF6C20C9B4881AB467E2B866F74B56F984443D2F1B8E0F8A` (was `D13D4225...DB599E`). Every
in-game result before this date describes the old build.

1. **Countability, one answer.** `RecipeProbe.CanCount` is now the only place that decides it. The profile
   window (`Dialog_BenchProfile.cs`) and the switch-on confirmation (`BenchActivation.cs`) used to derive it
   from the recipe's products, which is only what the *base* `RecipeWorkerCounter` does; subclasses decide
   otherwise, and butchering does (`CanCountProducts` returns true). With no game loaded (the settings page
   from the main menu) there is no bill to probe with, so it asks the recipe's own counter with none, falling
   back to the base rule if that counter needs its bill.
2. **Shipped text.** Butchering is no longer named among the uncountable recipes: `About.xml` and
   `BillAutopilot.Profile.UncountableDesc` in English and French now say "smelting a weapon, cremation,
   surgery". **The Steam page is frozen at the description sent at creation and still says butchering: that
   has to be edited by hand on the page** (noted in `PUBLICATION.md`).
3. **Settings page overlap.** The checkbox is drawn at the right edge of its rect, which ended exactly where
   the summary text starts; the rect is 16 px narrower so the cross no longer covers the first digit.

**New coverage:** two scenarios in feature 09 — butchering resolves to *keep in stock*, and the confirmation
announces exactly the bills the engine then puts up (before the fix: one announced, two taken on a butcher
spot).

**Found on the way, and mine:** `Tests/BillAutopilot.Tests.csproj` had stopped building the day
`Tests/Pickle/Source` was created, because the SDK's default globbing compiled the Pickle sources into it.
It now excludes `Pickle\**`. The last unit-test result before today had been obtained from an executable built
before that; the suite is green again on the new build (**74 checks passed**), XML **407 checks passed**.

**Rules applied.** `settings_audit` goes back to `partial`: the protocol resets it after a change to settings
UI, and the in-game re-check has not been done. `localization` and both language fields stay `complete`: the
one changed text was revalidated statically (407 checks, key and placeholder parity); the runtime check in
each language is still listed in `remaining`.

## First verdict: Pickle run 4, and what it found — 2026-09-23

`exitReason: failed`, **65 scenarios run of 65 selected**: 40 passed, 11 failed, 14 skipped. Stage stays
**done**. The 14 skipped are the `@requires` scenarios (features 13 to 17), expected in a pass without the
optional mods and **not yet played**. The full breakdown is in `docs/runs/pickle-runs.md`.

- **The 11 failures are in the suite, except one question that turned out to be about the documentation.**
  The unexplained `Object reference` failure of runs 2 to 4 is explained: an assertion message read
  `rule.mode` before the assertion ran, when the rule was null. Also a stockpile too small for a later,
  larger request, a hysteresis scenario written against the wrong starting state, and butchering, which
  the game **does** count (`RecipeWorkerCounter_ButcherAnimals.CanCountProducts` returns true). All fixed in
  the suite, feature 09 now uses the electric smelter; **none replayed yet**.
- **Three defects in the shipped mod were exposed** and are in `remaining`: countability decided by hand in
  two places against the engine's own answer; shipped text (frozen Steam description, in-game tooltip in
  English and French) still calling butchering uncountable; a layout overlap on the settings page. They
  change shipped content, so none was touched without a decision. Repository documents that only described
  the claim (README, CHANGELOG, TESTING.md) were corrected.
- **Two `@review` captures were opened**: the profile window reads cleanly, and the settings page shows the
  overlap above. The marker capture of feature 06 also reads ("Make patchleather (auto)" beside an unmarked
  hand-placed bill). The other `@review` captures are still to be opened.

## Where the run evidence lives

Raw evidence (reports, launcher logs, captures) is **on disk only**, in `docs/runs/evidence/`, ignored by
git; what is tracked is the text summary `docs/runs/pickle-runs.md`, one table line per run. Under the "Test
evidence" rule of the root AGENTS.md, only the latest report per scenario on build `4B89FBA7` is kept (the French
passes `fr1`, `fr2`, `fr3`, plus `bwm-22`, `rimmsqol-20`, `removal-chain` and `skipcheck-13-17`, about 5 MB).
Run 4, the earlier final passes and the fix checks were deleted on 2026-09-26, replaced by those passes; runs 2
and 3 were deleted the day run 4 replayed all of them, and run 1's report was lost to the shared script's
five-report rotation before it was copied. Captures are kept as reduced JPEG.

Queued through the TicketDispatcher on 2026-09-24 and 25, all English, all `-pickle-no-http`. **Historical:
except `skipcheck-13-17`, the evidence folders named below were deleted on 2026-09-26**; the French passes replayed
every scenario on the current build (`pickle-runs.md` has one line per run):

| Request | Filter | Mods | Purpose |
| --- | --- | --- | --- |
| `6e5f` | features 01 to 06 | 7 optional | final pass 1 of 3: **done, `exitReason: passed`, 27 of 27**, evidence `2026-09-24-final1-01-06` (the marker capture of feature 06 was opened and reads: "(auto)" on the autopilot's bill, nothing on the hand-placed one, inside Better Workbench Management's tab) |
| `1f5e` | features 07 to 12 | 7 optional | final pass 2 of 3: **done, `exitReason: passed`, 20 of 20**, evidence `2026-09-24-final2-07-12`; feature 09, with its two new scenarios, passes on the build that fixes the countability defect |
| `479c` | features 13 to 19 | 7 optional | final pass 3 of 3, the `@requires` scenarios finally played: **`exitReason: failed`, 21 played, 17 passed, 4 failed**, evidence `2026-09-24-final3-13-19`; the failures and their causes are in "First play of the optional-mod scenarios" |
| `c6a4` | features 13 to 17 | none | done: **`exitReason: passed`, 14 skipped by requirement, 1 passed** (the one scenario that needs no mod), as expected; evidence `2026-09-24-skipcheck-13-17` |
| `c5dd` | features 13 to 15 | 7 optional + TD Find Lib + TDS Bug Fixes | fix check on build `4B89FBA7...`: **`exitReason: failed`, 10 played, 7 passed, 3 failed**; feature 13 fully green (the BWM fix works), the 3 failures were scenario errors; evidence `2026-09-25-fixcheck2-13-15` |
| `5a32` | features 14 and 15 | same | replay of the corrected scenarios: **`exitReason: failed`, 7 played, 5 passed, 2 failed**. Feature 15 **green** (all four scenarios, including the row-cache ones; the `@review` capture of the redrawn tab was opened earlier). Feature 14: the two Everybody Gets One scenarios failed on the mode names I had wrongly renamed, the third (owner gone) passed; evidence `2026-09-25-fixcheck3-14-15` |
| `115b` | feature 14 | same | replay after reverting the names and rewriting the first scenario: **`exitReason: passed`, 3 of 3**, evidence `2026-09-25-fixcheck4-14`. **Every scenario that has been red is now replayed green** (13 in `c5dd`, 15 in `5a32`, 14 here), on build `4B89FBA7...` |
| `dafa` | feature 20 (new) | RIMMSQOL + PickleTools `RimmsqolSteps`, pass `avec-rimmsqol` | first play of the automated RIMMSQOL test, queued 2026-09-25 (tree = HEAD `a12568a` plus uncommitted feature 20, its map, `Mod/README.template.md`, `Mod/.steamignore`, `ATTRIBUTION.md` and `TESTING.md`; DLL unchanged) |

Requests `6e5f` and `1f5e` ran on the build before the fixes below (`8E3C5CC7...`), so the whole suite still has
to be replayed on the last build (`4B89FBA7...`) as three small requests plus the skip check. **What blocks the
publication and what does not** (owner, 2026-09-25): every scenario that has been red must have been replayed
green after its fix before the `publish`, which is `5a32` (features 14 and 15) and nothing else; the full
regression replay of the 69 scenarios may follow the publication, in small tickets, and a red there is a defect of
the published version. The gallery and the owner's manual validations are still required before the `publish`.

The French pass has since run, green (see the 2026-09-26 section above).

## Workshop item created, and the gate to tested restated — 2026-09-23

Stage stays **done**. Nothing in the shipped mod changed on 2026-09-23; the DLL was still D13D4225...DB599E (it changed on 2026-09-24, see above).

- **0.1.0 prepublished by the maintainer.** The Workshop item exists, private, id **3806709456**;
  `About/PublishedFileId.txt` was committed at once (13ac5ed), as PUBLICATION.md requires.
  `CHANGELOG.md` now opens with `## [0.1.0]`: the creation of that item. The feature list stays
  under `[1.0.0] - unreleased`, since no functional release exists yet. The 0.1.0 upload also left
  `Mod/Textures/BillAutopilot/Autopilot.dds` beside the PNG; RimWorld generates it, so it is
  ignored in `.gitignore` rather than committed.
- **Three new conditions to reach tested** (maintainer, 2026-09-23), written into TESTING.md and
  `remaining`: no `@wip` scenario, every `@requires` scenario actually run, and every manual
  test validated green. State today: condition 1 holds (none tagged); condition 2 does not (four
  features and two scenarios have never been played, only skipped or not reached); condition 3 does
  not (no manual test has been done).

## Prepublication work done off-game — 2026-09-21

The in-game gate is handed to the Pickle side, so this pass advances everything `tested ->
prepublished` asks for that needs no game. **The stage does not move: it stays `done`.** Nothing was
uploaded, no tag was posted, no release was created.

`PUBLICATION.md` is new, and it holds what the Workshop page asks for and the repository had nowhere
else: the one-way parts, the screenshot order, the dependencies and DLC, the adult-content answers,
six thank-you messages and the release notes.

**One defect found and fixed in the description.** It ended on "This mod is MIT licensed." with no
pointer to `ATTRIBUTION.md`, where the workflow requires a line sending the reader to both the
attribution and the licence before the GitHub link. It now reads "Credits, and the rights they rest
on, are in ATTRIBUTION.md in the repository. This mod is MIT licensed and the notice ships with it."
Worth catching now rather than later: `SetItemDescription` is called only at creation, so after the
first upload this text is corrected by hand on the Steam page and never from `About.xml`.

**Dependencies and DLC settled from the sources, not from intent.** Harmony is the only hard
dependency. **No DLC at all** — `supportedVersions` is 1.6, there is no `LoadFolders.xml`, no
`Patches/`, no `MayRequire` and no Def or C# path conditional on any expansion; the five DLC in
`loadAfter` are load order only and must not be declared as requirements, since a hard dependency
forces a download on someone who does not want it. The six optional integrations stay `loadAfter`.

**Adult-content boxes answered no, with both images opened rather than judged by their file names:**
`Preview.png` is a title over an overhead workshop scene with a small rear-facing colonist;
`ModIcon.png` is a stylised winking face with a clipboard and a gear.

**Six thank-you messages written**, one per mod this one reaches into, each naming what it actually
does with that mod. Measured: 882, 770, 691, 539, 732 and 454 characters against Steam's limit of
1000. To be posted only after the item is made public, a link to a private item opening for nobody.

### What `tested -> prepublished` still lacks

- The **five page captures do not exist**. Their order and what each must show are settled in
  `PUBLICATION.md`; producing them needs a game, and each one has to be opened and looked at.
- **No version tag and no GitHub release.** Deliberately left: tagging 1.0.0 would put a version
  number on a build that has not passed the in-game gate.
- `done -> tested` itself.


## Third Pickle run: three features green, still no verdict — 2026-09-21

`exitReason: watchdog-timeout`, **26 scenarios of 65**, 15 passed, 11 failed. Not a verdict: the
game stopped writing after feature 06 and the watchdog killed it. Report kept outside the repository
(the shared script's own stall-archive step is broken, see below); the machine was left clean —
lock released, no RimWorld and no Xvfb surviving, checked by `pgrep`.

**Three features fully green, and two of them had never been seen working at all:**

| Feature | |
| --- | --- |
| 01 loading | 4/4 — patches applied, integration report matches the mods loaded |
| 02 activation | 4/4 — the confirmation, its count, the silent intake, the second toggle, the teardown |
| 06 the marker | 4/4 — the postfix on `LabelCap`, absent on a hand-placed bill, removed by its setting |

The marker is the one TESTING.md said had never been on screen. It is now, `@review` screenshot
included — which says the trip happened, not that the image is legible.

The 11 failures are two causes, both in the suite and both already corrected after this run:
the stock step used `GenPlace.TryPlaceThing`, which **refuses** a cell and returns false, and that
answer was ignored — nothing was spawned, the stock stayed at zero, and it surfaced three steps
later as "the autopilot counts 0, not 30", which reads as a broken threshold in the mod. It now
spawns with `GenSpawn` and re-reads the count through the game's own counter before handing back.
And the stove filled its eight-bill allowance with meals before reaching the recipe under test, so
feature 04 narrows it the way the tailoring benches already were.

One `Object reference` failure is still unexplained. Pickle attaches no stack, and it sat behind a
Background that failed first; the next run sees it alone.

### Two defects in the shared tooling, reported and not touched

Both are in `scripts/`, which belongs to the monorepo and had eighteen tickets queued on it.

1. **A minimal pass is not minimal for any mod owning `wsl-deps.map`** — `PICKLE_DEPMAP=none`
   arrives empty through `WSLENV`'s `/p` translation. Worked around here by naming the set.
2. **The stall archive never runs.** `Run-PickleWsl.ps1:367` builds its path from
   `"pickle-reports-archive\bloque-{0}-{1}"` where `\b` is a literal **backspace byte (0x08)**, not
   two characters. Windows rejects it, `New-Item` fails with "Caractères non conformes dans le
   chemin", and the Player.log the comment above it exists to preserve is lost every time a run
   stalls. Seen on two runs in a row.


## First Pickle run: no verdict, two defects in the suite — 2026-09-21

The first WSL pass, minimal set, English. **`exitReason: infrastructure-error`, 0 scenarios played
out of 19 features discovered.** Not a result, and not read as one. Report kept at
`pickle-reports-archive/0921-1508`; the live `pickle-reports/` was overwritten by the next session's
run within the minute, which is why the archived copy is the one cited.

**Stage is unchanged at `done`.** Both defects are in the test suite, none in the shipped mod, whose
DLL is untouched at `D13D4225...DB599E`. Running these passes is a criterion of `done -> tested`, and
it has still not been met.

### What broke, and what it cost

**1. Parentheses in a step pattern are not parentheses.** In a Cucumber Expression they mean
OPTIONAL TEXT, so `at ({int}, {int})` is an optional group containing parameters, which is illegal.
Pickle builds its whole step table before it runs anything, so one bad pattern out of 87 meant not
one scenario of the nineteen ran:

```
Invalid step pattern: Bill Autopilot syncs the {string} at ({int}, {int})
An optional may not contain a parameter type.
```

36 patterns carried it. Fixed by escaping — `at \({int}, {int}\)`, written `\\(` and `\\)` in a C#
literal. Verified against Pickle's own expression compiler, not by reading.

**2. Three step texts were declared twice.** A `Given` setter and a `Then` assertion spelled
identically: `Bill Autopilot is on for {string}`, `is off for {string}`, and `... is named {string}`.
Pickle matches on the expression text alone and the keyword is decoration, so these are Ambiguous
steps — they would have failed healthy scenarios on the next run. The setters are now `is switched
on for`, `is switched off for` and `is renamed`. One step nothing called was deleted rather than kept.

The earlier text-level cross-check passed on both of these: it normalised patterns and sorted them
unique, which hid the duplicates, and it never compiled anything, which hid the parentheses.

### The guard that replaces that check

`Tests/Pickle/Check-Steps.ps1`, a few seconds, no game. It loads Pickle's own
`CucumberExpressions` and `PickleParameterTypeRegistry`, compiles every declared pattern, reports any
declared twice, matches every step line against the compiled regexes, and names any pattern no
feature uses.

**Since 2026-09-21 it also checks ambiguity**, which came back from `PickleTools`, whose own
`Check-Steps` grew out of an earlier copy of this file. That is the check a text comparison cannot
do: two DIFFERENT expressions can both match one line, and Pickle matches on the text alone across
every suite installed in a run. This suite's expressions are now compared against Pickle's own
vocabulary, read out of its assemblies with Mono.Cecil rather than from its documentation, and
against every other step source in the collection.

Current state: **87 patterns declared, 87 compile, none declared twice, none ambiguous against 568
others (205 from Pickle, 363 from 20 step sources), every one of the 480 step lines resolves**.

A lost run is not only this mod's forty minutes: the machine is shared and there were eleven tickets
in the queue behind this one.

### And the minimal pass was not minimal

The archived Player.log shows Better Workbench Management, Nice Bill Tab, Nice Bill Tab - Expansion
and Dubs Mint Menus all detected, with the bill ceiling at 2147483647 — in the pass meant to prove
the mod stands alone.

`Run-PickleWsl.ps1` exports `PICKLE_DEPMAP=none` for a pass with no `-DepMap`, but it exports it
through `WSLENV=...:PICKLE_DEPMAP/p`, and `/p` path translation turns a value that is not a path into
an empty string. Measured directly:

```
PICKLE_DEPMAP=none WSLENV=PICKLE_DEPMAP/p wsl.exe -- bash -lc 'echo "[$PICKLE_DEPMAP]"'   ->   []
```

`stage-pickle-wsl.sh` then falls back to `<mod>/Tests/Pickle/wsl-deps.map` — the very fallback its
own comment warns about. Adding that file earlier today is what turned this mod's minimal pass into
an optional one, silently.

Fixed here by naming the set: `wsl-deps.avec-facultatifs.map`, passed explicitly with `-DepMap`, so
nothing is picked up by default. That is also the naming the workflow asks for. **The underlying
defect is in the shared tooling and is left untouched**: `scripts/` is the monorepo's, eleven
sessions were queued on it at the time, and a change there lands under runs already in flight. It
affects every mod that owns a `wsl-deps.map`.


## Pickle suite written — 2026-09-21

This closes the one gate the audit below left open. **Stage: preTest -> done.** Written on top of
`878ac02`, the audited revision; no shipped source, asset or resource changed, so every validation
recorded in that audit still holds and the delivered DLL is still SHA-256 `D13D4225...DB599E`.

`Tests/Pickle/` now holds 19 feature files, a companion test mod (`Bill Autopilot - Pickle tests`,
`nelim.billautopilot.pickletests`, never distributed) and a step assembly of 86 steps.

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
mods, with them (`Tests/Pickle/wsl-deps.avec-facultatifs.map`), and one per language. **No incompatibility pass**, and
the reason is recorded: `About.xml` declares no `incompatibleWith`.

`wsl-deps.avec-facultatifs.map` mounts all six optional integrations, every packageId read from that mod's own
About.xml on 2026-09-21 rather than from its Workshop title — three of them are continuations whose
title and packageId disagree. No Max Bills: Redux (3526216885) is staged too although the mod
neither declares nor reaches it: it raises the per-bench bill ceiling from 15 to 125, a number the
mod is documented to honour, and scenario 10 reads that ceiling live instead of hard-coding it.

One set covers the six rather than several. A named set is owed per exclusive combination, and the
question is a fair one — Nice Bill Tab redraws the whole bills tab, Better Workbench Management adds
to it, Dubs Mint Menus keeps its own menu beside it. Virginie, who plays with these mods, reports
seeing no incompatibility between them (2026-09-21). Recorded as what it is: a player's experience,
not a measurement. A scenario going red in that pass because two neighbours fight would be a real
result, and the answer would be to split the set per combination.

In the minimal pass the integration scenarios carry `@requires:` and are **skipped**, so that pass
is green with about a dozen scenarios never played. Its report has to be read for skips as well as
for failures.

### Checks performed on the suite itself

- `dotnet build Tests/Pickle/Source/BillAutopilot.PickleSteps.csproj -c Release`: success, 0 warnings.
- Every step text used in the 19 features resolves against the ones defined here plus Pickle's 202
  built-ins, **0 undefined**. An undefined step costs a whole run, so this was checked mechanically
  rather than by reading. *(This text-level check missed two real defects; see the run below.)*
- **0 collisions** with Pickle's own vocabulary, and **0 collisions** with the SkillIcons,
  WorkStudio, ArchitectStudio and QuietNewFactions suites on this machine. Two suites sharing a step
  text produce "Ambiguous step" and fail healthy scenarios.
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


## Preview source migration — 2026-10-02

Copy, typography, layout and palette are consolidated in `Art/Preview.config.json`. The canonical inputs are `Art/Preview-source.png`, `Art/echo.png` and `Art/ModIcon-source.png`; the shared renderer writes temporary diagnostics under ignored `Art/.render/`. Existing distributed Preview, gallery and ICO outputs were preserved because they were present and coherent; no render was run for this migration. Superseded JSON files and generated QA intermediates were removed. Nothing published.
