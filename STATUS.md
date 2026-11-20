---
localization: complete
translation_en: complete
translation_fr: complete
mod:          Bill Autopilot
packageId:    nelim.billautopilot
repo:         Rimworld-Bill-Autopilot
visibility:   public
detached:     yes
workflow_stage: shootGallery[1.0.0]
licence:      original
licence_at:   original work
upstream_mod_remotes: N/A
licence_name: MIT
licence_file: LICENSE (identical copy in Mod/LICENSE)
dependencies: declared
workshop_required_items: n/a, not released yet
showcase:     partial
settings_audit: passed
ecosystem_line: 1.0
tested_on:    2026-11-12 pickle 4afaa49
workshop:      3806709456
publication_changelog_review_sha: 66acf05179c23fa2e50ec48d22c044da6729dcb5
code_review_sha: e70a6e39dea9cfba5019870b3a02288c55dca18b
remaining:
  - open 2026-11-20, owner order: no stage move until the gallery is ready and she validates it. Image 4: take 2 (ticket 1d15, zoom 4) scored REDO 2026-11-20 (Nelim head hidden by potato pile, letter absent, decor sparse; run folder deleted), potatoes moved to (180, 117), ticket c592 green but REDO again (Nelim stood west under the bills tab, hidden), potatoes moved to (184, 116), ticket 3ed1 green but REDO (Nelim whole carrying potatoes, but decor under the tab, letter absent), furniture moved east of the stove (v5), image 4 kept 2026-11-20 (run 8e13, crop left 1200 px, subtitle burned, a candidate with known weak points: Nelim face half hidden by the potatoes she carries, armchair cut by the crop, letter outside the frame). The six images exist; owner validation of the whole gallery (gallery_validated_sha) still owed before mountPreview. Asked of Audit: replay-audit must require her gallery validation (again after any gallery change). Open for mountPreview: 10.a bis keep or redo of Preview-source, 10.b to 10.d, 10.f
  - 2026-11-20, owner verdict: images 1, 4, 6 refused (Nelim bare-bottomed, wrong hairstyle). Cause found: `Tests/Pickle/config/galerie/` held no seed, so AB Visible Pants drew nothing and hat hair was unset; seeds copied from SanctuaryBacklot `config/sanctuary`, step `hairstyle is Roo_HD_Hair_Highpony` added before the face step. Art files of 1, 4, 6 deleted; refiled in one ticket (filter image 1, 4, 6). Image 4 still has the potato pile over her face (adequacy 3.5): judge it again on the new capture.
  - open 2026-11-19, Audit rules read at resume: session accepts or refuses each image alone (no Parked for gallery takes, a REDO take is deleted with its run folder); STATUS.md migration done (Docs/PROTOCOLS-READ.md and Docs/runs/status-history.md deleted, pickle-runs.md renamed pickle.md, old dated sections dropped). Check-Status WARN npt-sync: NPT changed 2026-11-19 after Steps read 2026-11-18, run sync-nsb-npt before the next scenario edit
  - open 2026-11-20, protocols read (f9313f96): 11.j gallery-credits (ATTRIBUTION.md section "Gallery set" for every third-party mod the gallery scenes use, or a "Not used in the captures" line; WARN at writeReleaseDocs, ERROR at publish) - ATTRIBUTION.md has no Gallery set section yet; stage moves 12.h now precede the dry-run
session:      local_3a8a05c7-7219-41d0-8306-35bbfe80986d
echo_review_sha: edeb39b1b134c64e175827bf6a44ea7104d79997
updated: 2026-11-20, stage back to shootGallery by owner order, gallery not validated by her
protocols_read_sha: 553fe0b2a14e0e48dcdd1031741a3d21fecbc68e
---
# Bill Autopilot: status

Version aimed: **1.0.0**, unreleased. Workshop item 3806709456 exists, private (`0.1.0`, item creation, 2026-09-23); nothing published as 1.0.0. `Mod/About/PublishedFileId.txt` committed (`13ac5ed`).

## Translation audit

- French review by owner, fifth round: **done 2026-10-19**, relayed in chat ("relu et validé"), revision `6f72bff`: `Confirm.BodyCustom` (fr) reworded after the seventh ChatGPT review (applied as the owner said "applique ce qui te semble pertinent"; the eighth returned no correction). `translation_fr` back to `complete`. The Steam description change (RIMMSQOL line) is not covered: `publication_changelog_review_sha` stays stale.

- Fourth ChatGPT review (2026-10-17), owner said "applique ce qui te semble cohérent": applied three of five: `RecipeExcluded` (English "A bill for {0} will no longer be put up on a {1}.", French "{0} ne donnera plus lieu à un travail sur un établi « {1} »."), `NewRecipeLetterBody` French ("Réactiver les recettes retenues, supprimer les autres"), `PUBLICATION.md` line 6 wording. Left: double space in `Profile.GroupCount` (layout), "keeps bills alive" in `Settings.Intro`. `translation_fr` and `translation_en` back to `unchecked` until the owner reads the changed rows in `FRENCH_REVIEW.md` (two French, one English); `publication_changelog_review_sha` stale for `PUBLICATION.md`.
- French and English review by owner, fourth round: **done 2026-10-17**, relayed in chat ("relu, validé"), revision `be642a4`: `RecipeExcluded` (en, fr), `NewRecipeLetterBody` (fr), `Confirm.BodyCustom` (fr) and `PUBLICATION.md` line 6, after the fourth to sixth ChatGPT reviews (the sixth returned no correction). `translation_fr` and `translation_en` back to `complete`; `publication_changelog_review_sha` moved to that revision.
- Fifth ChatGPT review (2026-10-17), owner said "applique ce qui est cohérent": applied one: `Confirm.BodyCustom` French ("c’est ce mod qui décide seul s’il y a du travail à faire"). Left: two style points in the Steam description of `PUBLICATION.md` ("The point of the mod.", "Smelting a weapon, cremation, surgery and anything else"): cosmetic, and they would reopen the description review. Rows to read since the third round: `RecipeExcluded` (en, fr), `NewRecipeLetterBody` (fr), `Confirm.BodyCustom` (fr).
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
  dependency; declared in `Mod/About/OptionalCompat.xml` (kind synergy, 2026-11-13); thanked in `THANKS`, named in Better Workbench Management bullet of description, exercised by
  pass `avec-nomaxbills`.
- RIMMSQOL: shortcut only, tested by pass `avec-rimmsqol`, not declared.
- DLC: Royalty, Ideology, Biotech, Anomaly, Odyssey in `loadAfter`; no `LoadFolders`, no conditional patch ships.
  `supportedVersions`: 1.6.

## Evidence

Evidence is kept on disk only (`Tests/Pickle/Inbox/` arrivals, `Evidence/` proofs, `Parked/` held failures; untracked or git-ignored); tracked is one-line-per-run summary
`Docs/runs/pickle.md`. Older greens had no deletion condition and were deleted in the 2026-10-19 triage. Now kept: `Evidence/foreign-mode-14` (minified proof, RELEASE.md); Inbox and Parked empty. Gallery image 3 never green (`remaining`).

## Gallery

Six images kept (2026-11-19), shot in the hearth hall of the Sanctuary of Nelim (fixture `Nelims-tribe`, scenario `23-gallery-vanilla.feature`, window 5 from feature 24; plan in the header of feature 23, shotlist v4). `Art/Gallery/` holds `0-preview.png` (byte copy of `Mod/About/Preview.png`) and the cropped captures below; subtitles of 1, 4, 6 burned 2026-11-19 (clean originals in `Art/Gallery-clean/`, copies in `Art/Gallery/`), score-gallery verdict candidate on all three (image, adequacy, understanding, decoration 4 or more; j ok on shapes at 268 px); the owner validates only "ready to publish" (`showcase: partial`).

| Index | File | Source run | Crop |
| --- | --- | --- | --- |
| 1 | `1-tailor-1-hall.png` | refused 2026-11-20, REDO | recompose, crop left 1100 px |
| 2 | `2-tailor-2-profile.png` | deleted 2026-11-20 (height was cut), refiled `6a72` | window, sides only, full height |
| 3 | `3-tailor-3-confirm.png` | deleted 2026-11-20 (height was cut), refiled `6a72` | window, sides only, full height |
| 4 | `4-tailor-4-stove.png` | refused 2026-11-20, REDO | recompose, crop left 1200 px |
| 5 | `5-tailor-5-neighbours.png` | deleted 2026-11-20 (height was cut), refiled `a04c` | window, sides only, full height |
| 6 | `6-tailor-6-answered.png` | refused 2026-11-20, REDO | recompose, crop left 1150 px |

## Shotlist (write-shotlist, 2026-11-13, v2)

Written in the header of `Tests/Pickle/Mod/Pickle/Features/23-gallery-vanilla.feature`; no capture of the new plan yet, so `score-gallery` rated the plan. Six images: 1 `tailor-1-hall` (wide), 2 `tailor-2-profile` (window), 3 `tailor-3-confirm` (window), 4 `tailor-4-stove` (tight), 5 `tailor-5-neighbours` (window, feature 24), 6 `tailor-6-answered` (medium, new closing scene). Story: midday of one day in the hearth hall, Nelim (canon hair, teal shirt, smile) stops rewriting her bench's list. Director pass (Opus sub-agent): kept grouped decor, Atticus asleep facing camera (animal criterion), camera off the tab, no "just finished a pair" claim, letter kept in frame, closing scene 6 and a medium shot, UI-scale request; refused moving the tailoring story off the stove (stove scene played green, run 2c7e; Nelim feeds the Sanctuary).

| Scene | a time | b decor | c living | d pawn | e action | f composition | g subject | j mod at 268 px | Plan score |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 hall, wide, 12:00 | hall, clear, +60 ticks | bench, Brazier lit, 2 PlantPot daylily, 4 TorchLamp lit, 4 Column, parquet | Atticus asleep facing camera, daylilies | teal rgb (40,110,120), canon ponytail, smile | forced job at bench, gaze to bench | zoom 3, centre (183,114), fore bench and brazier, back columns | auto mark vs hand bill | ok: UI scale step 175 percent (exists, NPT 2026-11-13), mark width checked at 268 px | 5 |
| 4 stove, medium-tight (zoom 4, REDO 2026-11-20), 12:15 | same hall, +625 ticks | FueledStove at (181,115), Brazier lit, 2 pots daylily, 1 torch, parquet | daylilies | same look, smile | forced job at stove, looks at tab | zoom 5 on (176,108), fore Nelim and stove | suspended pemmican row plus letter | ok: "SUSPENDED" overlay is large | 5 |
| 6 answered, medium, 12:45 | same hall, +1,875 ticks | FueledStove at (181,115), Brazier lit, 2 pots daylily, 2 torches, 4 columns | daylilies | same look, smile | unsuspended pemmican bill taken up, forced job | zoom 4 on (181,115) | the accepted recipe no longer suspended | ok: UI scale step 175 percent (exists, NPT 2026-11-13), mark width checked at 268 px | 5 |

Series k: ok (one story with place, day, rhythm; wide 1, medium 6, tight 4; opener 1, closer 6). Main risks: scene 1 and 6 j (marks at 268 px, fallback zoom 4 and crop on tab and bench), a state-check step "colonist at the bench and working" not verified (fallback: wait N ticks after the forced job), Atticus lies via the step "the animal lies at ... facing". The 0.5 malus on j in scenes 1 and 6 is assumed until NPT answers the UI-scale request. Not yet done: scenarios 23 and 24 rewritten to the new header, `Check-ShotlistVsScenario`, PUBLICATION.md gallery table for six images, old captures deleted with the rewrite (`4-candidate-stove.png` is stale: old plan, orange top, sparse decor).

## Ecosystem

Report `docs/inventory/ecosystem-steam/BillAutopilot.md` (PiI, 2026-11-13, second pass 2026-11-14; delta 2026-11-19: nothing new since 2026-11-14, neighbours unchanged). Original work: no parent, translation, retexture or patch of ours. No fork, no replacement. Closest ideas: Bill Presets 3752731886 (manual templates), AdaptiveMealBill 3668761367 (meals only, kept). Others: Auto Bill Restore, Periodic Bills, Nutrition Based Bill Repeat Modes, [B98] Random Bills, [B98] Clear Bills, No Max Bills (Redux, KV), Nice Bill Tab and Expansion, Categorized Bill Dropdown, Bill Manager (dead). To check in play tests: bills-tab overhauls (B98 Clear Bills, Nice Bill Tab) against our gizmo, Bill Presets with our saved state. Verdict: no duplicate, mod stays.

## NSB

Sync 2026-11-13 (sync-nsb-npt, first one for this mod, at `shootGallery` before the shotlist). Rules said back: everything in NSB is free; a colonist or domestic animal outside NSB, and any change of a colonist's properties, need the owner's express validation quoted here; every outfit is free; every animal the mod includes must be in the gallery (this mod adds none). Changed in NSB since the plan of 2026-10-08: the hearth hall and its four corners are decorated and validated (lot 5h, ticket 1140, 2026-11-08); the final fixture is `Nelims-tribe-decor-mods-z3.rws`, `Nelims-tribe.rws` (undecorated) stays beside it; the folder `docs/` is now `Docs/`; NPT gained the step "the main bar, the speed controls and the mouse-over readout are hidden". Left as is: scenarios 23 and 24 (shotlist not rewritten yet, `write-shotlist` is next; they still load `Nelims-tribe` and empty the hall). One finding: the plan sets Nelim's hair colour rgb (150, 75, 35) and hairstyle "Ponytail" while the validated canon is hair "brun presque noir" (`Hair_DarkSaturatedReddish`) and `Roo_HD_Hair_Highpony` (VALIDE.md, NELIM-PROFIL.md): a property change with no quoted validation, PENDING (owner question).

Sync 2026-11-17 (second): NSB commits of 2026-11-17 read (VALIDE.md: Soren Chaleur, Ada, Lyra validated; VTW body-type notes; neutral heads): none touches Nelim, the hearth hall, the animals catalogue or a place doc; NPT docs unchanged since 2026-11-16. Nothing updated, nothing left to rewrite. Table rows below unchanged (Nelim played with canon hair, no property change).

Sync 2026-11-18 (third, NPT part): PickleTools commit ace6816b (2026-11-17, `face-parts.md`: vanilla face photographed without Facial Animation) read: no step or catalogue entry used by the gallery, our scenarios keep the Nals face stack. NSB commit 2372794 (2026-11-18, `RECONSTRUCTION-SAUVEGARDE.md`: plan to rebuild the Sanctuary save as `Nelims-tribe-v2`, nothing built, gates open) read: no effect now, the gallery keeps `Nelims-tribe`. If v2 lands before our captures are accepted, scenarios 23 and 24 would be repointed by a lot. Nothing updated.

Sync 2026-11-19 (fourth): NSB log since 2026-11-17 holds nothing new for this mod (last commit 2372794, 2026-11-18, already read; Iori, Miel, Quo, Quaren, BiB gene renames and VALIDE rows touch no Nelim, hearth hall or animal). NPT: `docs/steps.md` and `ColonistRace/docs/catalogues` have no commit since 2026-11-18 (the Check-Status warning came from PickleTools commits on feature files: vanilla-face, metadata batches 1 to 5, none used here). Nothing used by the gallery changed; no scenario rewritten. Rules said back: all NSB free; colonist or domestic animal outside NSB and any colonist property change need her express validation; outfits free; every included animal in the gallery (this mod adds none).
Sync 2026-11-20 (fifth): NSB log since 2026-11-19 holds one commit (6db18f8, docs migration to pickle.md, status journals dropped): nothing for this mod. PickleTools log since 2026-11-19 on docs/steps.md and ColonistRace/docs/catalogues: no commit. Nothing updated in the shotlist or scenarios by this sync (image 4 zoom 4 recomposition is the REDO, not an NSB change). Rules said back: everything in NSB is free; a colonist or domestic animal outside NSB and any change of a colonist property need her express validation; every outfit is free; every animal the mod includes must be shown (the mod includes none).

Places read: 2026-11-13 Tribe read: 2026-11-17 Steps read: 2026-11-20

| Place | Shots that use it | Changes made for the gallery | Last read |
| --- | --- | --- | --- |
| `hearth-hall` (181, 115), now decorated and validated | 1 and 4 (plan of 2026-10-08, to rewrite) | emptied of furniture and animals, four columns and four torches added, midday, paused; whether to keep NSB's validated decor instead is the shotlist's call | 2026-11-13 |
| settings page and windows (no place) | 2, 3, 5 | none | n/a |

| Colonist | Shots | Outfit per shot | Last read of its profile | Exceptions |
| --- | --- | --- | --- | --- |
| Nelim | 1, 4 | basic shirt dyed teal (rgb 40, 110, 120), to set her apart from the orange bench; garment free | 2026-11-13 (NELIM-PROFIL.md, VALIDE.md, NSB-ROSTER.md lines) | PENDING: hair colour rgb (150, 75, 35) and hairstyle "Ponytail" differ from canon; no owner words yet; without validation she is played with canon hair and `Roo_HD_Hair_Highpony` |

| Animal | Shots | Outfit | Last read of its sheet | Exceptions |
| --- | --- | --- | --- | --- |
| none (Atticus and Rain sleep in the hall and are removed by the scenario) | none | none | n/a | none |

| Included animal (rule 5) | Shots that show it |
| --- | --- |
| the mod includes no animal | n/a |

