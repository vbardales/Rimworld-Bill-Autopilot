---
localization: complete
translation_en: complete
translation_fr: complete
mod:          Bill Autopilot
packageId:    nelim.billautopilot
repo:         Rimworld-Bill-Autopilot
visibility:   public
detached:     yes
workflow_stage: playTests[1.0.0]
licence:      original
licence_at:   original work
upstream_mod_remotes: N/A
licence_name: MIT
licence_file: LICENSE (identical copy in Mod/LICENSE)
dependencies: declared
showcase:     partial
settings_audit: partial
tested_on:    2026-09-01
workshop:      3806709456
publication_changelog_review_sha: 5683bee16012423faa0326f86e28ecf219e5d116
code_review_sha: 84156f4483e7f3512d6f96413dab844da9c6d987
remaining:
  - unverified: Art/Preview-source.png of 2026-10-10 measures 896 x 503 (the renderer expects 504) and a mean value of 0.196 (AUDIT.md 2.e asks at least 0.30), one hue family, dominant 91%; the owner declared both sources final on 2026-10-13 and asked for the regeneration, so these two measures are accepted by her
  - unverified: Preview.png regenerated 2026-10-13 and opened by this session: the echo (lathe line-art) overlaps the letters O and T of "AUTOPILOT"; the owner validated the overlap as it is (2026-10-13), Preview.config.json unchanged. The corner badge reads 1.6 (follows supportedVersions); Audit ruled (9a33932, STYLE_RIMWORLD.md "Badge de version", AUDIT.md 1.7): the badge follows supportedVersions, so it stays 1.6 until a green 1.7 run and the owner confirms the tag; then add <li>1.7</li>, set tested_1_7, redo the Preview in the same commit, release a patch (update_tags=true)
  - unverified: gallery at the Sanctuary (hearth hall, midday, four torches), run 8f81 and e2b7 of 2026-10-10, captures opened: images 1, 2 and 3 read well (bills tab with three marked bills and Nelim at her bench; profile window with one group open; confirmation dialog); image 4 (bdf9, 30 ticks, filth cleaned) shows the suspended bill and its letter but still a "Roof collapse" letter, dust haze and "Unroofed": emptying the hall removes what holds its roof, so the scenario must not empty the hall (open); image 5 replayed with the new texts and reads well (7874, "Detected integrations", four neighbours named); the beige carpet at the top left shows in every frame; the owner accepts or refuses all five | UPDATE 2026-10-15: image 4 fixed (area emptying that keeps the roof holders, Nelim added at the stove, run 2c7e green, capture opened: roof defect gone); candidate Art/Gallery/4-candidate-stove.png awaits the owner. Owed after acceptance and reindex (GALLERY.md, "Sous-titres", rule of 2026-10-15): the one-line English subtitle on scene images 1 and 4 (originals in Art/Gallery-clean/, lines in a PUBLICATION.md table "Gallery subtitles"); windows 2, 3, 5 and 0- carry none
  - unverified: ticket `20261017-163425-516-be34` in the queue (features 14 and 19, zero-target fix `b8d2ace`, evidence `Tests/Pickle/Evidence/zero-target-fix-14-19`, owner session `local_3a8a05c7`): at `RUN_DONE` read `summary.json`, replay any red scenario alone; no scenario covers the zero target itself (a target of 0 set under a foreign repeat mode, then Maintain): write one if wanted. After it: `code_review_sha` (`84156f4`) is stale since that code commit, re-confirm it; review points (b) to (e) stay open
  - unverified: playTests 8.d and 8.k, the @review captures not yet opened (fr1 bills tab, five in fr3, see docs/runs/pickle-runs.md) and the five gallery images, with the owner
  - unverified: playTests 8.c, 8.f and 8.h, full pass on the current build (DLL 7E329F9FDE9A since the 2026-10-15 review fixes; BA624725 before): suite 01-26 without the optional mods, with them, in English and in French; only the features touched by each fix have been replayed (v2: 13/13). It is a non-regression pass: file it after the commit, in small tickets
  - unverified: settings_audit stays partial (settings page and profile window UI changed 2026-09-24): in-game checks, then English and French replay
  - unverified: English and French runtime translation checks on the final build (raw keys, fallback, clipping)
  - done 2026-10-17, code review (AUDIT.md 8.m) complete, code_review_sha set: every file of Source/ read (2026-10-15: AutoBillSync, BillAutopilotState, BillAutopilotPatches, RecipeProbe, BillAutopilotSettings, BenchActivation, BetterWorkbenchesCompat, BillAutopilotMod; 2026-10-17: Dialog_BenchProfile, DubsMintMenusCompat, NiceBillTabCompat, HiddenRecipesCompat, NoMaxBillsCompat, BillMemory, BillAutopilotStartup, RecipeCount, CountForm). Second round found no crash and no data loss. Open: (a) FIXED 2026-10-17 in Dialog_BenchProfile (LiftZeroTargets, and a recipe rule going to Maintain with target 0 gets 1): a target of 0 set under a foreign repeat mode no longer survives the switch to Maintain, where `count < target` never held and the bench made nothing; the fix changes the DLL (new build to replay: feature 19 profile window and 14 foreign repeat mode, small ticket); (b) DubsMintMenusCompat strips from a template every bill whose recipe has an auto bill on the bench, also a hand-placed one of the same recipe (rare: autopilot retires from hand-placed recipes); (c) HiddenRecipesCompat: assembly found but `IsHidden` signature missing logs nothing and Startup says "not found"; (d) assembly-walk loop copied three times (DubsMintMenusCompat, NiceBillTabCompat, HiddenRecipesCompat); (e) CountForm concatenates two strings per call, called every frame by the profile window. First round: three defects fixed (a finished "Do X times" auto bill excluded its recipe for good; Create() failure left a recipe pending; Refill restarted the queue before it drained, starving benches). Left open, judged minor: counter edits under a foreign repeat mode not captured (CaptureDrift), memory of a minified bench pruned, LabelCap postfix allocates per call, two small simplifications
  - unverified: PUBLICATION.md checked against PUBLISHING.md on 2026-10-10 (structure, dependencies, adult boxes with the current Preview and ModIcon reopened, nine thanks drafts all in the register, description equals About.xml); the author of Choose Your Recipe ("Zaljerem") is not verified on its Workshop page, and the owner reads PUBLICATION.md and CHANGELOG.md before prepareRelease
  - unverified: writeDocs 11.b (protocols of 2026-10-10): THANKS must also name the author of every integration exercised by a test pass: RIMMSQOL (pass avec-rimmsqol) and Pickle Tools (the gallery maps now stage ScreenshotStudio, CameraZoom, StageDecor, ClearScreen, ScreenshotMode, ColonistRace; Workshop 3806142401, development only) are not in the description; look up their Workshop ids and authors on their pages, add the rows to WORKSHOP_COMMENTS.md, then the owner re-reads the changed Steam description (11.j)
session:      local_3a8a05c7-7219-41d0-8306-35bbfe80986d
updated:      2026-10-13, owner declared ModIcon-source.png and Preview-source.png final; ModIcon.png, ModIcon.ico, Preview.png, 0-preview.png and Preview.ico regenerated (AUDIT.md 2.b closed)
protocols_read_sha: 416d9051ad19d11c6352665809adbb5e5cb0ecaa
---
# Bill Autopilot: status

Version aimed: **1.0.0**, unreleased. Workshop item 3806709456 exists, private (`0.1.0`, item creation, 2026-09-23); nothing published as 1.0.0. `Mod/About/PublishedFileId.txt` committed (`13ac5ed`).

## Audit of 2026-10-10 (AUDIT.md of 2026-10-10, protocols repository)

**Retained state: `playTests[1.0.0]`** (old `workflow_stage: done`, mapped by table of section 16; retired `stage` field removed). Replaces audit paragraph of 2026-10-02, kept as line in `docs/runs/status-history.md`.

Revision audited: `84b1787` plus uncommitted working tree. Under `Mod/`, `Source/`, `Tests/Pickle/` only build-output paths changed (below); shipped DLL untouched, no Pickle request of this mod in queue.

Transitions, replayed against current protocols:

| Transition | Result |
| --- | --- |
| `setupRepo` to `makeSources` | Validated. Standalone public repository, MIT, original work, English docs, `LICENSE` and `ATTRIBUTION.md` identical in `Mod/` |
| `makeSources` to `code` | Sources present. **Two Art findings, both on uncommitted owner files** (first two `remaining` entries): derivatives older than sources (2.b, mechanical, owed) and Preview source under 0.30 value floor (2.e). Not regenerated: source mid-edit not mine to render from. Committed state (`84b1787`) coherent |
| `code` to `declareDependencies` | Validated: sources of `1.0.0` complete, `CHANGELOG.md` `[1.0.0] - unreleased` |
| `declareDependencies` to `auditSettings` | Validated, see "Dependencies" |
| `auditSettings` to `localize` | Validated at `partial`, see "Settings audit" |
| `localize` to `writeTests` | Validated after owner's second review of 2026-10-11 (see "Translation audit"): 65 Keyed keys identical in both languages, plus 2 DefInjected keys |
| `writeTests` to `playTests` | Validated: automated 74 and XML 476 replayed now, green; 26 Pickle features, 82 scenarios, no `@wip`; scenarios and passes declared in `TESTING.md` |
| `playTests` | **Open**, see `remaining` |

Run now, outside game (no RimWorld started, no lock taken, nothing submitted):

| Check | Result |
| --- | --- |
| `Tests/ValidateXml.ps1` | 476 XML CHECKS PASSED (5 files) |
| `Tests/BillAutopilot.Tests.csproj` Release, run | 74 CHECKS PASSED against shipped DLL |
| `Source/BillAutopilot.csproj` Release to scratch folder | 0 warnings, 0 errors; SHA-256 begins `ba624725f320`, identical to `Mod/Assemblies/BillAutopilot.dll` |
| `Check-Status.ps1 -Mod BillAutopilot` | before: `stage` retired, old vocabulary, no `protocols_read_sha`, size 100 KB (ERROR) |

**Mod root cleaned** (closing pass, step 3): `.build/` now `build/` (three `Directory.Build.props`, `.gitignore`, `TESTING.md` path, `Tests/Pickle/Check-Steps.ps1`), `_tools/Generate-FrenchReview.ps1` removed (shared `scripts/Make-FrenchReview.ps1` replaces it), `.editorconfig` moved to `Source/` with copy in `Tests/`. Root now: `.git`, `.gitignore`, `.gitattributes`, `.github`, plain folders and documents. Left for owner's list: `Art/Preview-original.png` (outside PUBLISHING.md minimal set, tracked).

**Next transition (`playTests` to `shootGallery`):** replay gallery image 3 (`24d`, one small ticket), open `@review` captures, code review (8.m), then commit and file non-regression passes (after commit: AUDIT.md "Passes Pickle"); owner validates French and English texts she has not seen, and gallery.

## Translation audit

- French review by owner, third round: **done 2026-10-17**, relayed in chat ("je valide"), revision `5683bee`: rewrite without vous, tu or imperative (nine rows, TRANSLATIONS.md of 2026-10-17) and `Profile.CustomCountDesc` ("demande à ce mod"), `FRENCH_REVIEW.md` regenerated; she also validated `CHANGELOG.md` (uncountable-recipe wording). Three ChatGPT reviews read: `.Zero` forms stay singular (project rule), last one returned no recommendation. `translation_fr` back to `complete`; `publication_changelog_review_sha` moved to that revision.
- French and English review by owner, second round: **done 2026-10-11**, relayed in chat by owner: two English texts added after first review (`Settings.MaxBillsFree.One` and `.Many`) read correct in both languages, no pawn gender agreement anywhere; six corrections she dictated applied verbatim: `Settings.Intro` (FR), `Settings.NotifyDesc` (FR and EN), `Settings.Integrations` (EN: "Detected integrations"), `Profile.Uncountable` and `Profile.UncountableRecipe` (FR: "dénombrable"), `Confirm.BodyMaintain` (FR first line); `FRENCH_REVIEW.md` regenerated (revision stamp reads `84b1787`, generator quirk: texts come from working tree). `translation_en` and `translation_fr` are `complete`.
- French review by owner, first round: **done 2026-10-02**, revision `bdfd660`, relayed in chat by owner: five corrections
  (`Settings.Integrations`, `Settings.IntegrationsDesc`, `Settings.NotifyDesc`, `Profile.UncountableDesc`,
  `Profile.CustomFloor`) applied, report linked to that commit, no colon gender agreement required. `translation_fr`
  is `complete`. Any later French file change sets it back to `unchecked`. This line is owner's; session does not edit it.
  French files unchanged since `bdfd660`; English file gained two keys after it (see audit table), so owner reads those two English texts in `FRENCH_REVIEW.md` (English column).
- French lives in two files: `Mod/Languages/French/Keyed/BillAutopilot.xml` (65 keys) and
  `Mod/Languages/French/DefInjected/MainButtonDef/BillAutopilot.xml` (2 keys, hidden RIMMSQOL settings shortcut).
- `FRENCH_REVIEW.md` (mod root) generated by `scripts/Make-FrenchReview.ps1 -ModRoot`. Every row read: none flagged;
  no text in this mod agrees in gender with pawn (all addresses player or names workbench type or
  recipe), so `{PAWN_gender ? ...}` switch applies nowhere.
- Original column equals English throughout: original work, no source language.
- Plural keys: 15 `.One`/`.Many`/`.Zero` keys, identical in English and French. `NewRecipeLetterTitle` has `.One` and
  bare key as many-form (accepted by XML check; reserve, not defect).

## Settings audit

- Inventory: per-bench-type profile (on/off, default mode and target, restart threshold, per-recipe override, exclusion),
  uncountable-recipe mode, cap on automatic bills per bench, label marker, new-recipe letter. Each controls a behaviour.
- Access: Options de mod, Bill Autopilot (native `Dialog_ModSettings`); hidden `BillAutopilot_Settings` MainButtonDef
  (`buttonVisible=false`, native visibility, same dialog), no customization mod required.
- Automated: 18 persistence checks on real `ExposeData`/Scribe paths (`Tests/SettingsPersistence.cs`) inside the 74.
- In game (Pickle): RIMMSQOL list, Visible ticked, button opens this page (feature 20, `dafa`, 3 of 3, 2026-09-25).
- `partial` because settings-page and profile-window UI code changed 2026-09-24; in-game completion and English and French replay owed (`remaining`).

## Dependencies (AUDIT.md 4.g, rechecked in `Mod/About/About.xml` 2026-10-10 on `84b1787`)

- Hard: `brrainz.harmony`, used. Nothing else referenced at build time.
- `loadAfter` only, reached by reflection, every call wrapped, neutral value when absent: Better Workbench Management
  (`falconne.BWM`), Nice Bill Tab (`Andromeda.NiceBillTab`) and its Expansion (`HICON.NiceBillTabExpansion`), Choose Your
  Recipe (`zal.chooseyourrecipe`), Dubs Mint Menus (`dubwise.dubsmintmenus`), Everybody Gets One (`Memegoddess.EverybodyGetsOne`).
- No Max Bills (KiameV and Redux): found at run time by marker type (`NoMaxBillsCompat`), so no `loadAfter`, no
  dependency; thanked in `THANKS`, named in Better Workbench Management bullet of description, exercised by
  pass `avec-nomaxbills`.
- RIMMSQOL: shortcut only, tested by pass `avec-rimmsqol`, not declared.
- DLC: Royalty, Ideology, Biotech, Anomaly, Odyssey in `loadAfter`; no `LoadFolders`, no conditional patch ships.
  `supportedVersions`: 1.6.

## Evidence

Raw evidence on disk only, `docs/runs/evidence/` (git-ignored); tracked is one-line-per-run summary
`docs/runs/pickle-runs.md`. Kept: French passes `fr1`, `fr2`, `fr3` (features 01 to 22, build `4B89FBA7`),
`rimmsqol-20`, `removal-chain`, `skipcheck-13-17`, `replay-fixes-v2` (13 of 13, build `BA624725`),
`nomaxbills-26-retry`, `galerie-studio-1-4c` (images 1, 2, 4) and `-5b` (image 5). Every red scenario replayed green, except gallery image 3 (never green, `remaining`). History of folded sections: `docs/runs/status-history.md`.

## Gallery

Five images planned (`PUBLICATION.md`), shot in hearth hall of Sanctuary of Nelim (fixture `Nelims-tribe`, midday, torches, Nelim at her bench; plan in header of `23-gallery-vanilla.feature`); zen studio no longer exists
`Art/Gallery/` holds only `0-preview.png` (byte copy of `Mod/About/Preview.png` of 2026-10-03). Images 1, 2, 4
and 5 have older captures in evidence from deleted zen studio (superseded); Sanctuary captures of 2026-10-10 in `docs/runs/evidence/2026-10-10-galerie-*-sanctuaire*`. None accepted by owner: `showcase: partial`.

## Complement of the audit, 2026-10-11

2026-10-10 audit leaned on 2026-10-02 one for dependencies, settings, translation. Rechecked now on `8117372`:
`Check-DefInjected.ps1` 2 keys, 0 errors; 65 Keyed keys identical in English and French, none empty; `ATTRIBUTION.md` and
`LICENSE` identical to `Mod/` copies (translation defect found then, resolved same day by owner's review); `.gitignore` carries `build/`, `*.dds` (none tracked) and `Art/.render/`;
no `LoadFolders.xml`, no `Patches/` ship; no em dash left in repository documents (removed from `PUBLICATION.md`,
`STATUS.md`, `TESTING.md`, `BACKLOG.md`; `About.xml` had none). One defect found (translation, above). Still open and
outside this recheck: Art findings, other gallery captures, code review, thanks owed to RIMMSQOL and
Pickle Tools. `protocols_read_sha` now `a5c7cf48` (changes after `eedd5b0` reached me through Ticket Manager relay: CHANGELOG form, Use This Instead, private-mod comments; none applies before `prepareRelease`).