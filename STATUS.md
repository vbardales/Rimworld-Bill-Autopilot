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
remaining:
  - defect: Art/ModIcon-source.png (2026-10-04) and Art/Preview-source.png (2026-10-10), both modified and not committed, are newer than their derivatives (Mod/About/ModIcon.png 2026-10-02, Art/ModIcon.ico, Mod/About/Preview.png and Art/Gallery/0-preview.png 2026-10-03, Art/Preview.ico): stale (AUDIT.md 2.b); regenerate once the owner confirms the new sources are final
  - unverified: Art/Preview-source.png of 2026-10-10 measures 896 x 503 (the renderer expects 504) and a mean value of 0.196 (AUDIT.md 2.e asks at least 0.30, Measure-Palette.sh), one hue family, dominant 91%; the owner's call
  - unverified: playTests 8.c, gallery image 3 (the switch-on confirmation) has never been green: red in 1-4c on a test-isolation leak fixed in ProfileSteps (no mod defect), replay 24d was never run (queue empty for this mod on 2026-10-10)
  - unverified: playTests 8.d and 8.k, the @review captures not yet opened (fr1 bills tab, five in fr3, see docs/runs/pickle-runs.md) and the five gallery images, with the owner
  - unverified: playTests 8.c, 8.f and 8.h, full pass on the current build (DLL BA624725): suite 01-26 without the optional mods, with them, in English and in French; only the features touched by each fix have been replayed (v2: 13/13). It is a non-regression pass: file it after the commit, in small tickets
  - unverified: settings_audit stays partial (settings page and profile window UI changed 2026-09-24): in-game checks, then English and French replay
  - unverified: English and French runtime translation checks on the final build (raw keys, fallback, clipping)
  - unverified: code review (AUDIT.md 8.m), code_review_sha absent: review 0.1.0..HEAD once the code is stable and played
session:      local_af4f68d3-3ae2-45fb-852a-eed8ef2bace4
updated:      2026-10-10, audit against AUDIT.md of 2026-10-10: playTests[1.0.0] (old done); mod root cleaned (.build to build/, _tools removed, .editorconfig moved); STATUS.md folded into docs/runs/status-history.md
protocols_read_sha: 1405c22e11d72011390df2af5013f748f6ed374b
---

# Bill Autopilot — status

Version aimed: **1.0.0**, unreleased. Workshop item 3806709456 exists, private (`0.1.0`, the creation of the item,
2026-09-23); nothing was published as 1.0.0. `Mod/About/PublishedFileId.txt` is committed (`13ac5ed`).

## Audit of 2026-10-10 (AUDIT.md of 2026-10-10, protocols repository)

**Retained state: `playTests[1.0.0]`** (old `workflow_stage: done`, mapped by the table of section 16; the retired `stage`
field is removed). This replaces the audit paragraph of 2026-10-02, kept as a line in `docs/runs/status-history.md`.

Revision audited: `84b1787` plus the uncommitted working tree. Under `Mod/`, `Source/` and `Tests/Pickle/` only
build-output paths changed (below); the shipped DLL is untouched, and no Pickle request of this mod was in the queue.

Transitions, replayed against the current protocols:

| Transition | Result |
| --- | --- |
| `setupRepo` to `makeSources` | Validated. Standalone public repository, MIT, original work, English docs, `LICENSE` and `ATTRIBUTION.md` identical in `Mod/` |
| `makeSources` to `code` | Sources present. **Two Art findings, both on uncommitted files of the owner** (first two `remaining` entries): derivatives older than their sources (2.b, mechanical, owed) and a Preview source under the 0.30 value floor (2.e). Not regenerated: a source in the middle of an edit is not mine to render from. The committed state (`84b1787`) is coherent |
| `code` to `declareDependencies` | Validated: sources of `1.0.0` complete, `CHANGELOG.md` `[1.0.0] - unreleased` |
| `declareDependencies` to `auditSettings` | Validated, see "Dependencies" |
| `auditSettings` to `localize` | Validated at `partial`, see "Settings audit" |
| `localize` to `writeTests` | Validated: 68 + 2 French keys, owner's French review done (below) |
| `writeTests` to `playTests` | Validated: automated 74 and XML 476 replayed now, green; 26 Pickle features, 82 scenarios, no `@wip`; scenarios and passes declared in `TESTING.md` |
| `playTests` | **Open**, see `remaining` |

Run now, outside the game (no RimWorld started, no lock taken, nothing submitted):

| Check | Result |
| --- | --- |
| `Tests/ValidateXml.ps1` | 476 XML CHECKS PASSED (5 files) |
| `Tests/BillAutopilot.Tests.csproj` Release, run | 74 CHECKS PASSED against the shipped DLL |
| `Source/BillAutopilot.csproj` Release to a scratch folder | 0 warnings, 0 errors; SHA-256 begins `ba624725f320`, identical to `Mod/Assemblies/BillAutopilot.dll` |
| `Check-Status.ps1 -Mod BillAutopilot` | before: `stage` retired, old vocabulary, no `protocols_read_sha`, size 100 KB (ERROR) |

**Mod root cleaned** (closing pass, step 3): `.build/` is now `build/` (three `Directory.Build.props`, `.gitignore`,
`TESTING.md` path, `Tests/Pickle/Check-Steps.ps1`), `_tools/Generate-FrenchReview.ps1` removed (the shared
`scripts/Make-FrenchReview.ps1` replaces it), `.editorconfig` moved to `Source/` with a copy in `Tests/`. Root now:
`.git`, `.gitignore`, `.gitattributes`, `.github`, plus plain folders and documents. Left for the owner's list:
`Art/Preview-original.png` (outside the minimal set of PUBLISHING.md, tracked).

**Next transition (`playTests` to `shootGallery`):** replay gallery image 3 (`24d`, one small ticket), open the `@review`
captures, the code review (8.m), then commit and file the non-regression passes (after the commit: AUDIT.md "Passes
Pickle"); the owner validates the French and English texts she has not seen and the gallery.

## Translation audit

- French review by the owner: **done 2026-10-02**, revision `bdfd660`, relayed in chat by the owner: five corrections
  (`Settings.Integrations`, `Settings.IntegrationsDesc`, `Settings.NotifyDesc`, `Profile.UncountableDesc`,
  `Profile.CustomFloor`) applied, report linked to that commit, no colon gender agreement required. `translation_fr`
  is `complete`. Any later change to a French file sets it back to `unchecked`. This line is the owner's; a session does not edit it.
  No file under `Mod/Languages/` changed since `bdfd660` (checked 2026-10-10).
- French lives in two files: `Mod/Languages/French/Keyed/BillAutopilot.xml` (68 keys) and
  `Mod/Languages/French/DefInjected/MainButtonDef/BillAutopilot.xml` (2 keys, the hidden RIMMSQOL settings shortcut).
- `FRENCH_REVIEW.md` (mod root) generated by `scripts/Make-FrenchReview.ps1 -ModRoot`. Every row read: none flagged;
  no text in this mod agrees in gender with a pawn (everything addresses the player or names a workbench type or
  recipe), so the `{PAWN_gender ? ...}` switch applies nowhere.
- Original column equals English throughout: original work, no source language.
- Plural keys: 15 `.One`/`.Many`/`.Zero` keys, identical in English and French. `NewRecipeLetterTitle` has `.One` and the
  bare key as its many-form (accepted by the XML check; reserve, not a defect).

## Settings audit

- Inventory: per-bench-type profile (on/off, default mode and target, restart threshold, per-recipe override, exclusion),
  uncountable-recipe mode, cap on automatic bills per bench, label marker, new-recipe letter. Each controls a behaviour.
- Access: Options de mod, Bill Autopilot (native `Dialog_ModSettings`); hidden `BillAutopilot_Settings` MainButtonDef
  (`buttonVisible=false`, native visibility, same dialog), no customization mod required.
- Automated: 18 persistence checks on the real `ExposeData`/Scribe paths (`Tests/SettingsPersistence.cs`) inside the 74.
- In game (Pickle): RIMMSQOL list, Visible ticked, button opens this page (feature 20, `dafa`, 3 of 3, 2026-09-25).
- `partial` because settings-page and profile-window UI code changed on 2026-09-24; the in-game completion and the
  English and French replay are owed (`remaining`).

## Dependencies (AUDIT.md 4.g, rechecked in `Mod/About/About.xml` 2026-10-10 on `84b1787`)

- Hard: `brrainz.harmony`, used. Nothing else is referenced at build time.
- `loadAfter` only, reached by reflection, every call wrapped, a neutral value when absent: Better Workbench Management
  (`falconne.BWM`), Nice Bill Tab (`Andromeda.NiceBillTab`) and its Expansion (`HICON.NiceBillTabExpansion`), Choose Your
  Recipe (`zal.chooseyourrecipe`), Dubs Mint Menus (`dubwise.dubsmintmenus`), Everybody Gets One (`Memegoddess.EverybodyGetsOne`).
- No Max Bills (KiameV and Redux): found at run time by its marker type (`NoMaxBillsCompat`), so no `loadAfter` and no
  dependency; thanked in `THANKS`, exercised by pass `avec-nomaxbills`. It is not in the "Works with" list of the
  description (reserve for the owner's reading of `PUBLICATION.md`).
- RIMMSQOL: a shortcut only, tested by pass `avec-rimmsqol`, not declared.
- DLC: Royalty, Ideology, Biotech, Anomaly, Odyssey in `loadAfter`; no `LoadFolders`, no conditional patch ships.
  `supportedVersions`: 1.6.

## Evidence

Raw evidence is on disk only, `docs/runs/evidence/` (ignored by git); tracked is the one-line-per-run summary
`docs/runs/pickle-runs.md`. Kept: the French passes `fr1`, `fr2`, `fr3` (features 01 to 22, build `4B89FBA7`),
`rimmsqol-20`, `removal-chain`, `skipcheck-13-17`, `replay-fixes-v2` (13 of 13, build `BA624725`),
`nomaxbills-26-retry`, `galerie-studio-1-4c` (images 1, 2, 4) and `-5b` (image 5). Every scenario that was red has been
replayed green, except gallery image 3 (never green, `remaining`). History of the folded sections: `docs/runs/status-history.md`.

## Gallery

Five images planned (`PUBLICATION.md`), shot in the zen studio colony (`nelim-zen-meadow-studio`) with a roofed, lit
pavilion. `Art/Gallery/` holds only `0-preview.png` (a byte copy of `Mod/About/Preview.png` of 2026-10-03). Images 1, 2, 4
and 5 have captures in evidence; image 3 has none yet. None has been accepted by the owner: `showcase: partial`.
