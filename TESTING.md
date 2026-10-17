# Testing Bill Autopilot in game

Nothing released. Version 1.0.0 still unpublished. Most below no longer untested: as of 26 September 2026 whole numbered suite (scenarios 1 to 22) played green at least once, most twice (English pass, French pass), on build `4B89FBA7...`.
**That build superseded.** Two corrections landed in code 26 and 27 September: No Max Bills ceiling when Better Workbench Management absent, and trap where recipe taken off bench while "new recipe" question still open never offered again. Shipped assembly now `BA624725...`. Replay of features 04, 05, 10 and 25 on that build green (`b2c7`, 2026-09-27, 13 of 13): Choose Your Recipe pending-trap fix now has in-game witness. No Max Bills ceiling fix does not yet: its isolated scenario (`8240`, feature 26) still queued. `STATUS.md` is source of truth for what actually ran and when; this file states what each scenario proves and how to read failure, not day-to-day verdicts.

Each scenario says what it proves. Test whose failure you cannot interpret not worth running.

## What no longer needs a human

One layer covered by program, in `Tests/`. Runs shipped assembly against real RimWorld assemblies, no game running, answers for itself:

```bash
dotnet build Source/BillAutopilot.csproj -c Release && dotnet build Tests/BillAutopilot.Tests.csproj -c Release && build/bin/tests/Release/BillAutopilot.Tests.exe
```

Forty-four checks, exits non-zero when one fails. Covers decision layer: given profile and recipe, which mode applies, how many to keep, when to start again. Includes countability fallback, clamp keeping floor under target, and what happens to repeat mode whose mod removed: answers invisible in game, because wrong one still looks like working mod quietly making wrong amount of wrong thing.

So scenarios 9 and 14 below no longer read as arithmetic. They still prove decision reaches bench: smelter really puts up nothing for smelted weapon, foreign mode really set on real bill. **Program is arithmetic, scenario is wiring.**

Itself checked by breaking clamp on purpose and confirming those two checks, only those two, turned red. Suite never seen to fail proves nothing.

## The Pickle suite, and how many passes a verdict needs

Twenty-six feature files in `Tests/Pickle/`, written from 21 September 2026, played since 23 September (`docs/runs/pickle-runs.md` has one line per run). Hold only what running game can show. Decision layer: which mode applies, which target, which floor, clamps, overflow guard, fallback for repeat mode whose owner gone, settings round trip through Scribe: proven by executable suite above, deliberately not restated there: Pickle run takes whole machine for tens of minutes, scenario repeating unit test would cost that every time and add nothing.

`Tests/Pickle/README.md` says per feature why game needed, keeps list of what stays manual. Numbered scenarios below are what features were written from.

**Verdict needs three passes, not interchangeable.**

1. **Without optional mods**: default staging: Core, DLC, Harmony, RimLogging, Pickle, Harmony as this mod's only hard dependency, and suite. Covers scenarios 1 to 12, 17, 18, 20 (non-RIMMSQOL half) and vanilla gallery (23), proves mod stands alone, which mod page claims of all six bridges. Scenarios for absent mods carry `@requires:` and **skipped**, so pass green with dozen scenarios never played: read skips, not only failures.

   ```powershell
   powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot
   ```

2. **With optional mods**: `Tests/Pickle/wsl-deps.avec-facultatifs.map`. Mounts all six integrations this mod declares in `loadAfter`, plus No Max Bills: Redux for raised bill ceiling scenario 10 reads live rather than hard-coding, and two libraries Everybody Gets One needs (TD Find Lib and TDS Bug Fixes: staging mounts only what map lists, without them its assembly loads half a class). Covers scenarios 13 to 17, 19 and 22 on top of first pass.
   Green first pass says nothing about this one, reverse equally true: scenario can pass *only* because optional mod present.

   ```powershell
   powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot -DepMap wsl-deps.avec-facultatifs.map
   ```

   **One set covers them, not several.** Named set owed per exclusive combination, fair question here: Nice Bill Tab redraws whole bills tab, Better Workbench Management adds to it, Dubs Mint Menus keeps own menu beside it. Virginie, who plays with these mods, saw no incompatibility between them (2026-09-21). Player's experience, not measurement, which is what this pass is for: scenario going red because two neighbours fight is real result, answer is split set per combination.

   **Fourth set, RIMMSQOL alone:** `wsl-deps.avec-rimmsqol.map` mounts RIMMSQOL and PickleTools' `RimmsqolSteps`, only `20-rimmsqol-shortcut.feature` runs in it. Stays out of pass with neighbours because it changes main bar, which no bill feature looks at, so red names own cause. Not tested: RIMMSQOL keeping its choice across restart, which is RIMMSQOL's behaviour, shown by PickleTools' own demonstration 2026-09-21.

   **Fifth set, No Max Bills alone:** `wsl-deps.avec-nomaxbills.map` mounts only No Max Bills: Redux, deliberately without Better Workbench Management, so `26-no-max-bills.feature` exercises fallback path added 2026-09-26 (`NoMaxBillsCompat`, asked only when BWM does not answer). Played and green 2026-09-27, `8240`, 2 of 2. Harmony crash logged as `[ERROR] Error while instantiating a
   mod of type NoMaxBillsRedux.NoMaxBillsReduxMod` when reading its result (`STATUS.md`, 2026-09-27): game survives it, but whether mod's *other* patch: one actually lifting interface's 15-bill limit: ran before or after one that throws unknown until this pass has real result.

   **Sixth set, removal chain:** `wsl-deps.removal.map` with `-Then removal-check -ThenWithout
   nelim.billautopilot,nelim.billautopilot.pickletests` (see scenario 21 below). Not a `-DepMap` any other pass uses; exists only to hand save from one launch to second launch without mod.

3. **Each language, own pass.** `-Language French`. Language fixed at staging, never switched inside run. No scenario spells English string: every label, marker, dialog and letter rebuilt from mod's own translation key: so same suite is French check, and `@review` screenshots are where missing key shows: in developer mode, which every Pickle run is in, key absent from active language comes back as accented gibberish.

**No incompatibility pass.** `About.xml` declares no `incompatibleWith`, README claims no mod broken by this one, so nothing to look at. If incompatibility ever declared, needs own pass that **asserts documented symptom** rather than expecting red: expected red and accidental red same colour, nobody can tell afterwards which one they looked at.

### The gate to `tested`, as stated on 2026-09-23

Three conditions, on top of passes above being run and read (`exitReason` first, then scenarios played against features discovered):

1. **No scenario tagged `@wip` left.** None tagged today; condition is none added to get pass green.
2. **Every conditional scenario actually run.** Scenario carrying `@requires:` skipped, not failed, in pass lacking its mod, so pass can be green with it never played. Here that is features 13 to 17, 19 and 22 (feature 20 in own pass, feature 26 in own pass, feature 21 in removal pass). **Met on build `4B89FBA7...`**: pass with optional mods ran every one (English, request `479c`, then French run `7018`), after repairing features 13, 14 and 15 (two defects of mod, and errors of scenarios) on 2026-09-25. Feature 26, new 2026-09-26, ran green 2026-09-27 (`8240`, 2 of 2), in own isolated pass.
3. **No manual test left to validate.** `AUDIT.md`: what used to be ticked by hand either automated and green, or listed as not applicable with reason. Former manual list of `Tests/Pickle/README.md` stands as follows (2026-09-27):

   | Former manual test | Now |
   | --- | --- |
   | RIMMSQOL revealing shortcut in own interface | Automated and green: `20-rimmsqol-shortcut.feature`, pass `avec-rimmsqol`, 3 of 3 on 2026-09-25, captures opened |
   | RIMMSQOL keeps visibility choice across restart | **Not applicable**: RIMMSQOL's own behaviour, shown by PickleTools' demonstration (2026-09-21) |
   | Nice Bill Tab drag | **Not applicable**: gesture inside another mod's window, where click lands on whatever window owns the point. Cause (cache told to rebuild) asserted by feature 15, green |
   | Choose Your Recipe | **Revised 2026-09-26, no longer simply not applicable.** Reading its code (removes disabled recipes from bench's own list) turned up real trap: recipe announced then taken off bench kept "waiting for an answer" mark once bill came down, never offered again after coming back. Corrected in code (`BillAutopilotState.Unannounce`), covered generically by `25-recipe-taken-off-the-bench.feature`, which edits recipe list the way that mod does without needing it staged: **played and green**, `b2c7`, 2026-09-27. Pass staging Choose Your Recipe itself and driving its own window still not written; see `BACKLOG.md` for scenarios owed (1.0.1, verification only) |
   | Save loaded with mod removed | Automated and green: `21-removal-write.feature` and companion `nelim.billautopilot.pickleremoval`, chain `-Then` / `-ThenWithout` (pass `wsl-deps.removal.map`), both launches passed 2026-09-25 |
   | BWM: workbench restriction applied to bill created from tick, agreement between widened count and what bill displays | Automated and green: `22-bwm-restriction-and-count.feature`, pass with neighbours, 2 of 2 on 2026-09-25, again in French run of 2026-09-26 (not yet seen red: no mutation of bridge tried) |
   | Every `@review` screenshot | Most opened and read, recorded in `STATUS.md`. Still owed: French pass captures (`fr1`, `fr3`), gallery images once `88b3` and `8240` come back |

   Entry not yet done is pending, not passed. **Condition 3 otherwise met**; gate held open by build change of 2026-09-26/27 (below), not by any of these three.

**Build has moved twice since passes above ran.** `4B89FBA7...` is what conditions 2 and 3 proven against. Two corrections since (No Max Bills alone, Choose Your Recipe pending-trap) changed `AutoBillSync` and `BetterWorkbenchesCompat`, shipping `BA624725...`. Conditions above therefore need replay on new build before `done -> tested` can be called met again, not just first play of what is new: at minimum features 04, 05, 10 and 25, since removal path every one exercises is what changed. **Done**: `b2c7`, 2026-09-27, 13 of 13. **Still owed**: feature 26 alone (queued `8240`), and wider suite (13 to 24) not replayed on `BA624725...` at all yet: only four features removal-path change directly touches. `STATUS.md` has day-to-day state.

**Two things no pass here can do**, both kept in `Tests/Pickle/README.md`: loading save with mod removed, which mod list makes impossible from inside run and is whole reason state avoids GameComponent; and anything inside RIMMSQOL's own interface.

## Evidence to keep when a test runs

Root `AGENTS.md` rule ("Test evidence"): keep only reports that still prove something, disk is full. For this mod that means following. Everything raw lives in `docs/runs/evidence/<date>-run<N>/`, which git ignores; git tracks `docs/runs/<date>-pickle-runs.md`, one table line per run.

**Keep, per run**

| What | Why kept |
| --- | --- |
| `summary.json` | Its `exitReason`, read before any number, and scenarios played against features discovered. Without them killed run reads as result |
| `junit.xml` | Per scenario outcome and failure message, where suite defect told from mod defect |
| `Player.log` and launcher's own log | `Command line arguments` line proves which pass (`-pickle-set-name`), integration lines prove which optional mods really mounted. "Minimal" pass showing Better Workbench Management detected is not minimal |
| `@review` captures | Only value is person opens them. Feature 06's marker capture, 18's settings page, 19's profile windows, 15's Nice Bill Tab tab. Keep full size |
| Captures of scenario **no later run repeated** | Sole proof of that check |

**Delete, as soon as newer report replaces it**

- `report.html` and `messages.ndjson`: about 100 MB each, nothing in them `junit.xml` and `summary.json` do not say.
- Any capture whose scenario later run played again.
- Every report about build no longer the one in repository. When `Mod/Assemblies/BillAutopilot.dll` or step assembly changes, reports of old one prove nothing about new one.

**How**

- Copy run out of `pickle-reports/` the moment it ends: next session's run overwrites it, archive keeps only five. Run 1 of 2026-09-21 lost that way.
- Shrink captures to JPEG (1280 wide, quality 70; `@review` ones full size, quality 88). 78 MB became under 3.
- Keep file path under 260 characters, or conversion fails: shorten scenario's capture name.
- Before deleting report, check no `STATUS.md` field points to it; repoint first.

**What proves what, for this mod (2026-10-02).** Keep one report per row, latest on build in repository; delete earlier ones once new one read. Per row, only `summary.json`, `junit.xml`, `Player.log` and opened captures (JPEG); never `report.html` or `messages.ndjson`, never whole `screenshots/` folder.

| Proof | Latest report kept | What it is proof of |
| --- | --- | --- |
| Features 01-12, pass with optional mods, per language | `fr1-01-06`, `fr2-07-12` (French), English twin when played | base loop, marker, drift, cap, save and reload |
| Features 13-19 and 22, with optional mods | `fr3-13-22` | five bridges, settings page, profile window, BWM details |
| Skip of conditional scenarios in pass without them | `skipcheck-13-17` | `@requires` scenarios skipped, not failed, without optional mods |
| Mod removed from game saved with it | `removal-chain` (`-Then` with `-ThenWithout`) | state avoids GameComponent |
| RIMMSQOL reveals and hides shortcut | `rimmsqol-20` | hidden shortcut, its visibility, same settings page |
| No Max Bills alone | `nomaxbills-26-retry` | ceiling above 15 without BWM |
| Fixes of 2026-09-27 | `replay-fixes-v2` | recipe taken off bench and brought back, deletion semantics |
| Workshop gallery, images 1-5 | `galerie-studio-1-4c` (images 1, 2, 4), `5b` (image 5), then replay of image 3 | five captures owner judges; green says trip ran, not image good |

Failure capture kept only if its scenario not replayed green yet. Converting captures needs path under 260 characters: name that long lost one failure capture on 2026-10-02.

**Manual tests need own evidence**, small: date, game and mod versions, list of active mods, language and UI scale, `Player.log` for that session, one line per case saying what seen. Manual entry recorded green without those not recorded. Go under `docs/runs/`, in same text summary.

## Before anything

### XML checks (no game required)

Run from repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/ValidateXml.ps1
```

Checks every shipped XML file for parsing errors, About metadata and Harmony dependency, GitHub link inside description, duplicate or empty translation entries, English/French key and placeholder parity, literal translation keys referenced in C#. Exits non-zero on failure. On 12 September 2026, all 376 XML checks and all 44 C# checks passed; both Release builds completed without warnings. No Defs or XML patches in this mod. XML written into saves still needs scenario 12 in running game.

### Translation gate and runtime pass (added 13 September 2026)

Before `preTest`, follow shared `PUBLISHING.md` and `TRANSLATIONS.md` protocol: inventory player-facing text and helper paths, check both languages against inventory, run XML validator, record evidence in `STATUS.md`. Reset affected translation fields to `unchecked` whenever UI code or language resources change. 13 September static audit passed 397 XML checks with 54 keys per language; rebuilt Release assembly compiled without warnings or errors.

**Not yet run:** launch in English, then French, repeat this pass in each:

1. Open settings: title, introduction, integration presence/absence, tooltips, bill cap and bench summaries. Check at intended resolution and UI scale.
2. Open bench profile: counters, category counts, collapse/expand, filters, overrides, every repeat-mode menu and uncountable-recipe tooltip.
3. Trigger each activation confirmation (maintain, always, optional custom mode), inspect both gizmos and automatic bill labels, then trigger new-recipe letter, recipe exclusion and captured-override message. Verify paragraph breaks and arguments.
4. With optional integrations installed, repeat their displays and trigger Dubs Mint Menus template notification. Verify supplying mods' Def labels too.

Fail on raw keys, unintended English fallback in French, missing arguments, visible escape sequences, clipping or overlap. Product names and `(auto)` marker may match. Record language, game/mod versions, UI scale, exercised cases and results in `STATUS.md`; keep each untested case in `remaining`. Static completion does not prove this pass.

### Game setup

1. **Harmony must be active, this mod after it.** Only hard dependency, declared in `About.xml`; mod list says so if missing.
2. **packageId is `nelim.billautopilot`.** Never published under another.
3. **Keep `Player.log`.** Everything this mod says prefixed `[Bill Autopilot]`. File overwritten at next launch and moved to `Player-prev.log`, so copy out before relaunching.
4. **Know how often it looks.** One workbench synchronised per tick, queue refilled every 600 ticks by default. On base with twenty workbenches, given bench visited about every ten seconds of game time. **Opening bills tab synchronises that bench at once**, throttled to twice a second and paced in real time, so works with game paused too. If scenario seems to do nothing, open tab before concluding anything.
5. **Note which of five optional mods active.** Better Workbench Management, Nice Bill Tab, Nice Bill Tab - Expansion, Dubs Mint Menus, Everybody Gets One. Settings screen names ones it found, under *Detected integrations*, fastest way to learn feature missing because mod is, not because code wrong.
6. **Start from save you can throw away.** Scenario 2 begins real production immediately, by design.

Three ways in, same thing: mod settings, *Bill autopilot* toggle on selected workbench, and *Autopilot profile* next to it.

## 1: The mod loads and says what it found

**Proves** Harmony patches and six detection probes. Every other scenario depends on this one. Automated as `01-loading.feature`, green on build `4B89FBA7...` in English and French (runs 11 and, before it, standalone run of 2026-09-21).

Start game, reach main menu, quit. Search `Player.log` for `[Bill Autopilot]`.

**One line expected, not silence**: `Integrations: … Bill cap per workbench: 15.` Each of five names (Better Workbench Management, hidden recipes, Dubs Mint Menus, Nice Bill Tab, No Max Bills) followed by `found` or `not found`. Cap reads 125 instead of 15 when Better Workbench Management sees No Max Bills, or "no limit at all" (2147483647 internally, never shown as number since 2026-09-26) when No Max Bills present without Better Workbench Management: fallback new and unplayed, see scenario 26.

What failure looks like:

- Any unhandled exception naming `BillAutopilot`. Stack trace names step.
- *"… found, but its bench templates could not be reached"*, or same shape for Nice Bill Tab. Mod there but renamed what this code reaches by reflection. Everything else keeps working; that one integration dead until repaired. Warning most likely to arrive with someone else's update.
- Name reading `not found` for mod you have active. Detection failing, not integration.

## 2: Switching a workbench type on, and what it takes at once

**Proves** confirmation, its count, silent absorption of everything already unlocked. Only moment where great deal of production can start in single click, which is why it asks. Automated as `02-activation.feature`, green on build `4B89FBA7...`, English and French.

Select stocked workbench, machining table or tailor bench. Use *Bill autopilot* toggle.

- **Confirmation appears**, naming how many recipes about to be taken and target, 50 by default. Refuse it: nothing must change.
- Accept. Within few seconds, or at once if you open bills tab, bills appear for what you are short of. **They arrive unsuspended and running**: already-unlocked recipes absorbed in silence, only what unlocked later announces itself.
- Turn off and on again. **Must not ask second time**, in this game. Question is about opening intake, already happened.
- Turning off takes every automatic bill down and **leaves hand-placed bills alone**.

## 3: The base loop, and the gap that stops it flickering

**Proves** counting through probe bill, and two thresholds. Automated as `03-base-loop.feature`, green on build `4B89FBA7...`, English and French.

With bench on autopilot, default mode *Keep in stock*, target 50, restart at 25:

- Bill appears while stock **at or below 25**, not between 26 and 49.
- Comes down once stock **reaches 50**.
- Let production run past target and watch tab: **bill must not appear and disappear on every unit made**. That is what gap between 25 and 50 exists for. Flicker means two thresholds collapsed onto one number.
- Colonist actually working at bench holds his bill: **bill being worked never taken down**, even if stock crosses target mid-job.

## 4: A recipe unlocked by research

**Proves** announcement path, suspended arrival, rule that nothing spent without answer. Automated as `04-new-recipe.feature`, green on build `4B89FBA7...`, English and French. **Replayed green on `BA624725...`** (`b2c7`, 2026-09-27): recipe-forgotten trap fixed 2026-09-26 touches same removal path this scenario exercises when bill it put up comes down.

With bench on autopilot, finish research project unlocking recipe on it.

- **Suspended** bill appears, letter lists it. Letter can be several seconds behind bill: sent once sync queue empties.
- **Unsuspend it**: accepted, behaves like any other from then on.
- Or **delete it**: recipe refused for good, message saying so, must never come back on that workbench type.
- With two benches of same kind, check **second does not start recipe** while first still shows it suspended. Question asked once for type, until answered no bench of that type may act on it.

## 5: Deleting an automatic bill means refusing the recipe

**Proves** hook on deletion, which gives gesture meaning. Automated as `05-delete-refuses.feature`, green on build `4B89FBA7...`, English and French. **Replayed green on `BA624725...`** (`b2c7`, 2026-09-27), same reason as scenario 4.

Delete running automatic bill. Message names recipe, autopilot stops offering it, *Configure* window shows that recipe set to *Never*.

Set back to *Default* in that window: must come back on next pass.

Deleting bill **you** placed must do none of this.

## 6: The mark in the label

**Proves** postfix on `Bill_Production.LabelCap`, chosen so every interface picks it up without any being patched. Automated as `06-auto-marker.feature`, green on build `4B89FBA7...`, English and French; `@review` capture opened and read: "Make patchleather (auto)" beside unmarked hand-placed bill.

Automatic bills carry `(auto)` at end of label.

- Look in **vanilla tab**, and every other bill interface you have: Nice Bill Tab, Dubs Mint Menus, Better Workbench Management. Mark must show in all, since all read same label.
- Bills placed by hand carry nothing.
- Turn *Mark automatic bills* off in settings: mark goes, everything else stays.

Mark showing in vanilla tab but not replacement tab means that mod builds rows from something other than label, approach needs revisiting.

## 7: Adjusting a bill in the tab is adjusting the profile

**Proves** drift capture. Automated as `07-drift-capture.feature`, green on build `4B89FBA7...`, English and French.

On running automatic bill, change target in tab, from 50 to 200 say.

- Message says recipe kept as override on that workbench type's profile.
- Open *Autopilot profile*: recipe now shows own numbers rather than default.
- Let stock fill so bill comes down, then fall so it comes back. **Returns with 200, not 50.** Without capture, change lost on first cycle.

Switch bill to *Forever* in tab: same capture happens, recorded as *Always*.

Switch one to **"do 1 time"** instead. **Nothing must be recorded, no message must appear.** That mode makes no sense as standing order, deliberately ignored; bill left exactly as you set it.

## 8: A bill placed by hand always wins

**Proves** rule keeping mod out of your way. Automated as `08-hand-placed-wins.feature`, green on build `4B89FBA7...`, English and French.

On autopiloted bench, place bill yourself for recipe autopilot also handles.

- **Autopilot's bill for that recipe goes**, does not come back while yours stands.
- Delete yours: autopilot may take recipe again.

## 9: Recipes the game cannot count

**Proves** countability test, and separate setting existing because *keep a stock* impossible for them. Automated as `09-uncountable-recipes.feature`, green on build `4B89FBA7...`, English and French.

Put **electric smelter** on autopilot, or crematorium, or electric smithy doing surgery. A **butcher table is not a case for this scenario**: `RecipeWorkerCounter_ButcherAnimals.CanCountProducts` returns true and game counts raw meat for it. This scenario said otherwise until first full Pickle run of 2026-09-23.

- In *Autopilot profile*, recipes with no countable product (smelted weapon) marked uncountable, tooltip says so.
- With default, *Recipes with no countable product* set to *Never*, **smelter puts up no bill for smelted weapon**. Correct, not bug.
- Set to *Always* instead: standing bills appear and never stop.

## 10: The cap, and room left for your own bills

**Proves** arithmetic keeping *Add* button alive. Game accepts 15 bills per bench and hides button beyond that. Automated as `10-bill-cap.feature`, green on build `4B89FBA7...`, English and French. **Replayed green on `BA624725...`** (`b2c7`, 2026-09-27), its slider text has new case to check since 2026-09-26, still unproven: see scenario 26.

On workbench with many recipes, cap at default 8:

- **At most 8 automatic bills** stand at once, however much work.
- Place several bills yourself. Automatic ones give way: cap counts yours against game's ceiling, so *Add* button must never disappear.
- Raise cap in settings, check slider stops at 15, or 125 with Better Workbench Management and No Max Bills, or 100 with No Max Bills alone (slider's own ceiling when game sets none: sentence beside it reads "the game sets no limit" instead of number).

## 11: Two benches of the same kind, set differently

**Proves** memory keyed per workbench. Used to be keyed per workbench type, the two indistinguishable until second bench existed. Automated as `11-per-bench-memory.feature`, green on build `4B89FBA7...`, English and French.

Build **two** benches of same kind, both on autopilot, give same recipe different custom name on each, through Better Workbench Management or vanilla renaming.

Let both bills come down and go back up. **Each bench must get own name back.** One name landing on both benches, or wrong one, is old per-type key returning.

Then **deconstruct one bench**. Its entry dropped on next pass; nothing visible should happen to other.

## 12: Save, reload, and remove the mod

**Proves** one decision single session cannot check: state grafted into save's `<game>` node instead of living in `GameComponent`, precisely so mod can be removed. Save-and-reload half automated as `12-save-and-reload.feature`, green on build `4B89FBA7...`, English and French. **Mod-removed half is scenario 21**, own feature and own companion mod, because no single Pickle run can change own mod list mid-run; see below.

With several benches on autopilot, bills up and at least one recipe refused:

- Save, quit game entirely, reload. **Everything as you left it**: same bills, same refusals, no recipe re-announced as new, log silent.
- Then **remove mod from mod list** and load same save. Must load, with **no** *"Can't load abstract class Verse.GameComponent"* and no error naming mod. Bills it put up stay behind as ordinary bills, yours to keep or delete.
- Put mod back and load again: picks up where it was.

Middle step is whole point of design. If it fails, say so before anything else.

## 13: Better Workbench Management

**Proves** largest compatibility layer, one with no witness at all. Everything goes through reflection into `ImprovedWorkbenches`, so failure silent by construction: feature lost, nothing crashes. Automated as `13-better-workbenches.feature` (down-and-up cycle) plus `22-bwm-restriction-and-count.feature` (workbench restriction on bill created from tick, agreement between widened count and what bill displays: two details written before interface could be read, now covered): both green on build `4B89FBA7...`, English and French. Two real defects of mod found and fixed this way 2026-09-25: see `STATUS.md`, "First play of the optional-mod scenarios". Neither scenario seen red on restriction/count details (no mutation of bridge tried), so prove agreement today, not that they would catch regression.

With that mod active, on one automatic bill, set as many of these as you can:

- **custom name**,
- **count away from home map**,
- **additional product filter**,
- **link** with another bill,
- **workbench restriction** on bench itself.

Now let stock fill so bill comes down, then fall so it comes back.

- **All five must survive** cycle. Bill returns with name, widened counting, filter, and rejoins link group rather than starting new one.
- **Restriction** is the one thing this mod applies that Better Workbench Management cannot do by itself for bill created from tick, since its own hook reads selected workbench. Check new bill carries it.
- Threshold and bill's own displayed count must **agree**. Measured through same widened rules; if bill says 40 and autopilot behaves as though 12, probe counting vanilla way.

Then repeat whole scenario **without** mod. Nothing must break, log must stay silent.

## 14: A repeat mode from another mod

**Proves** mode autopilot does not understand is set, kept, asked rather than guessed at. Everybody Gets One is test case; any mod adding repeat mode should behave same. Automated as `14-foreign-repeat-mode.feature`, green on build `4B89FBA7...`, English and French, after two rounds of scenario repair 2026-09-25 (bill already standing before mode set; bench cap absorbed by unrelated recipes first): see `STATUS.md`.

With Everybody Gets One active:

- In *Autopilot profile*, its three modes appear in default menu and any recipe's menu, under *Another mod*. Choose one as **bench default**, "one per colonist" say.
- Bills that go up carry that mode. Whether work to do decided by **that mod**, not by target of ours: add or lose colonist and queue must follow.
- Two counters labelled neutrally under such mode, *Count* and *Second count*, because owning mod reads them its own way.
- Set mode by hand on automatic bill, in tab. Recorded as override, bill **returns with it** after down-and-up cycle rather than flattened to one of ours.

Then **disable Everybody Gets One** and load save. Profile pointing at mode no longer existing must fall back to *Keep in stock*, not put up bill with no mode at all.

## 15: Nice Bill Tab, and the drag that must not resurrect a bill

**Proves** single most dangerous interaction in mod. Nice Bill Tab keeps own cached list of bills it draws, reorders from that list before writing back into stack. Stale entry not cosmetic problem: dragging can put deleted bill back. Automated as `15-nice-bill-tab.feature` for cause (cache flag), green on build `4B89FBA7...`, English and French; `@review` capture opened and read. **Drag itself stays manual** (see below): gesture inside another mod's window, where Pickle click would land on whatever window owns the point rather than testing this mod.

With Nice Bill Tab active, on autopiloted bench, **keep tab open** and let bill come down on its own as stock fills.

- **Row must disappear from its list.** Row that stays is cache not being told. Automated.
- Then **drag** remaining rows around. No deleted bill may reappear. Failure integration exists to prevent, drag itself never played: only its cause, above.
- With **Nice Bill Tab - Expansion**, hide recipe on that bench: autopilot must treat it as refused and never put it up. This half is scenario 19, automated and green (`17-hidden-recipes.feature`).

## 16: Dubs Mint Menus bench templates

**Proves** postfix on `MakeBenchTemplate`. Without it template photographs autopilot's passing queue, re-applying later turns those recipes into hand-placed bills for good, retiring autopilot from them without word. Automated as `16-dubs-mint-menus.feature`, green on build `4B89FBA7...`, English and French.

With Dubs Mint Menus active, on bench running several automatic bills, **make bench template**.

- Message names how many autopilot bills left out.
- Open template: holds **only bills you placed**.
- Apply to another bench. Those bills read as hand-placed, correct, autopilot stands back from their recipes.

## 17: Without any of the optional mods

**Proves** six bridges soft, as mod page claims. **No feature file of its own**: what first pass (without optional mods, `sans-facultatifs`) proves by construction, since every `@requires:` scenario skipped rather than run in it and everything else must still be green. Confirmed every time that pass ran, most recently French run of 2026-09-25 (27+20+... scenarios, `exitReason: passed`).

Turn off Better Workbench Management, Nice Bill Tab, Nice Bill Tab - Expansion, Dubs Mint Menus, Everybody Gets One and Choose Your Recipe, keeping Harmony. Mod must load, settings and profile windows must open, base loop must work, log must stay silent apart from startup line, which now reads `not found` throughout.

## 18: The profile window on a Steam Deck

**Proves** one constraint that shaped interface: played with pointer, text field would summon virtual keyboard. Automated as `19-profile-window.feature`: its file number and this scenario's number drifted apart since scenario 17 above stopped being feature file; go by `@review` tag, not number, when in doubt. Green on build `4B89FBA7...`, English and French; its three captures (grouped window, one override, uncountable group) opened and read.

Open *Autopilot profile* on workbench with many recipes.

- Recipes **grouped by product category**, each group collapsible, uncountable ones gathered last under *Other*.
- *Collapse all* and *Expand all* work, collapsed state survives closing and reopening window.
- *Overridden recipes only* filters to what you changed, *Clear the N overrides* empties them.
- **Every control reachable with pointer alone.** No search field, deliberately. If any step needs keyboard, that is failure.

## 19: Hidden recipes from optional mods

**Proves** Nice Bill Tab - Expansion hidden-recipe bridge. Automated as `17-hidden-recipes.feature` (again, file number and this scenario's number disagree), green on build `4B89FBA7...`, English and French, for Nice Bill Tab - Expansion half.

**Choose Your Recipe half revised 2026-09-26.** Was marked below "not applicable" because that mod removes disabled recipes from workbench's own list before autopilot ever sees them, so nothing of this mod's seemed left to assert. Reading its code (no public source; decompiled from `1.6/Assemblies/ChooseYourRecipe.dll`) turned up real trap that claim missed: recipe announced, suspended bill, letter, "waiting for an answer": then taken off bench before player answered had bill removed by "recipe left the workbench" path, but "waiting for an answer" mark not cleared with it; once recipe came back, mod believed question still open with no bill to answer it, never offered recipe again, on any bench of that type. Fixed in code (`BillAutopilotState.Unannounce`, called from `AutoBillSync.Remove`) and covered generically by `25-recipe-taken-off-the-bench.feature`: see scenario 25 below: without needing Choose Your Recipe staged, since any mod editing `ThingDef.AllRecipes` hits same path. **Played and green** (`b2c7`, 2026-09-27, on current build). Pass staging Choose Your Recipe itself, setting its disabled list and driving its own window still not written (`BACKLOG.md`, for 1.0.1): what follows is that mod's own manual procedure, still owed.

Use disposable save with enabled bench, countable recipe below restart threshold, no manual bill for that recipe. Test each integration separately with required dependencies.

- With Nice Bill Tab - Expansion, hide recipe and synchronise bench by opening its bills tab. No automatic bill for it should remain or reappear on subsequent passes. **Automated, green.**
- Unhide it: automatic bill should return while stock still below threshold. **Automated, green.**
- With Choose Your Recipe, disable recipe using that mod's configuration and reload if that mod requires. Should be absent from bench's available recipes and not acquire automatic bill. Re-enable, verify becomes eligible again: **and if "new recipe" letter pending for it when disabled, check letter and suspended bill come back too**, rather than recipe staying silently excluded (2026-09-26 trap).
- Repeat without either integration: recipe should follow normal profile rules.

Hidden recipe being queued is exclusion failure; one staying excluded after restored is stale-state failure. Preserve Player.log and active mod list with result.

## 20: Hidden settings shortcut and custom quantity regression

Automated as two files: `18-settings-shortcut.feature` (everything below except RIMMSQOL steps), green on build `4B89FBA7...`, English and French; and `20-rimmsqol-shortcut.feature` (RIMMSQOL actually revealing and using shortcut), own pass `avec-rimmsqol`, green in English (request `dafa`, 3 of 3, captures opened and read). RIMMSQOL keeping visibility choice across restart is RIMMSQOL's own behaviour, shown by PickleTools' own demonstration rather than repeated here.

Preconditions: RimWorld 1.6, Harmony and Bill Autopilot; disposable save. Repeat in English and French. Record exact game and optional-mod versions with Player.log.

1. With clean settings and no customization mod, verify no Bill Autopilot main-bar
   button visible or greyed out. Open Mod options -> Bill Autopilot, change
   marking option, close/reopen and verify value and actual bill-label effect.
2. Install RIMMSQOL (or tool supporting MainButtonDef visibility). Reveal
   `BillAutopilot_Settings`. Activate it: native Bill Autopilot settings dialog
   must open. Change cap; close and reopen through Mod options. Both routes must
   show same value. Restart/reload and check persistence. Hide shortcut again;
   verify customization tool retains that visibility choice. Check logs throughout.
3. With Everybody Gets One, select custom default mode on bench profile. Leave
   recipe inheriting that mode, then change its quantity. Must remain in that
   foreign mode with new quantity. Test zero and explicit custom override too.
   Save/restart/reload and verify profile and actual bill use retained mode.
4. Repeat normal Maintain quantity editing: minimum 1, valid restart threshold,
   unchanged inheritance. Remove optional mode provider: safe Maintain fallback;
   restore it and verify inherited mode identity retained.

**Points 1 and 2 automated and green** (features 18 and 20 above). **Point 3 automated and green** (feature 14 above: custom default mode, recipe inheriting it, quantity change, provider removed and restored). **Point 4** (ordinary Maintain quantity editing, minimum 1, restart threshold, fallback identity) covered out of game by executable tests (`Tests/BillAutopilot.Tests.csproj`) rather than Pickle scenario; shortcut worker's native visibility contract and shipped hidden default covered by XML tests. None of four points left wholly unexecuted, but this section's own framing as one undivided manual procedure stale: read as already split across scenarios 14, 18 and 20, plus executable suite.

## 21: A game saved with the mod, loaded without it

**Proves** one decision no other scenario can: mod's state lives in plain named nodes grafted into save's `<game>` node rather than `GameComponent`, precisely so removing mod raises no load error. No single Pickle run can change own mod list mid-run, so needs two launches under one hold of dispatcher's lock: `-Filter 21-removal-write -Then removal-check -ThenWithout
nelim.billautopilot,nelim.billautopilot.pickletests`. Automated as `21-removal-write.feature` (first launch: writes state, saves, checks save holds `billAutopilot...` nodes and no `Class="BillAutopilot` anywhere, hands file to companion mod) and `removal-check.feature` in companion mod `Tests/Pickle/Removal/Mod` (`nelim.billautopilot.pickleremoval`, never distributed, does not depend on this mod; second launch: loads that save with Bill Autopilot genuinely absent from mod list, runs 250 ticks, keeps bill as ordinary one, saves and reloads). Both launches green, English, request `cf3a`, 2026-09-25. Closes one manual entry `Tests/Pickle/README.md` used to keep as unreachable from inside run.

## 23 to 26: the Workshop gallery, and the two 2026-09-26 corrections

Four features added after numbered scenarios above, none proving new mechanic on own:

- **23 and 24: Workshop page's own captures**, produced by scenario rather than by hand so they **Update 2026-10-10: the zen meadow studio was deleted by PickleTools on 2026-10-08 (galleries are shot at the Sanctuary of Nelim, fixture `Nelims-tribe`). Features 23 and 24 and both gallery maps were rewritten the same day for the hearth hall of the Sanctuary, midday, eight lit torches, Nelim at her bench (plan in the header of `23-gallery-vanilla.feature`); the paragraph below describes the studio version and is historical. `GallerySteps.ClosePavilion` is no longer used by any scenario.**
  can be remade after any interface change (`PUBLICATION.md`, "Screenshots, in upload order").
  `23-gallery-vanilla.feature` (images 1 to 4, `-DepMap wsl-deps.galerie.map`: studio and nothing
  else, so still "without the optional mods") and `24-gallery-neighbours.feature` (image 5,
  `-DepMap wsl-deps.galerie-voisins.map`: neighbours plus studio) taken in **Nelim's Pickle
  Tools' zen meadow studio** (fixture `nelim-zen-meadow-studio`, `PickleTools/ScreenshotStudio`, chosen by
  owner 2026-09-27 after first captures, taken in Pickle's played `test-colony`, judged
  unfit for Workshop page). Each scenario works in studio's "display" pavilion, empty interior
  meant for mod demonstrations: closed (door and wall where open), roofed, so bench
  does not read "outdoors", lit by four filled standing torches; studio leaves roofs off on
  purpose, roofed room with no light too dark to sell anything. **Light asserted, not left
  to eye**: `the cell (125, 96) is lit at least 50 percent` reads game's own glow grid on bench
  cell (game's own "lit" threshold is 30). Earlier history, for record: first tries of images
  1 to 4 failed once on mistake in feature (setter line silently matching assertion step,
  `5d66`) and once on region-updater re-entrancy in room-building step (`88b3`), both fixed; studio-based version ran 2026-10-02 (`a496`: images 1, 2 and 4 green, image 3 red on test-isolation bug since fixed; images 1 and 3 replayed in queued requests `31ca` and `e8a5`). **Every accepted image still has to be opened and looked at**
  before becoming `Art/Gallery/1-`… to `5-` (`0-preview.png` is Preview, PUBLISHING.md); green scenario says trip happened, not
  that picture shows anything worth publishing.
- **25: Choose Your Recipe pending-trap, generically.** `25-recipe-taken-off-the-bench.feature`,
  added 2026-09-26 with correction it proves (see scenario 19 above): recipe announced then
  taken off bench's own list has bill removed, must be announced afresh, not silently
  forgotten, when it returns. Edits `ThingDef.AllRecipes` the way Choose Your Recipe does, without
  needing that mod staged, so runs in every pass. **Played and green**, `b2c7`, 2026-09-27, 13 of 13
  (scenario that proved it).
- **26: No Max Bills, alone, without Better Workbench Management.** `26-no-max-bills.feature`, own
  pass `wsl-deps.avec-nomaxbills.map` (deliberately without BWM, so fallback added 2026-09-26 in
  `NoMaxBillsCompat` is thing actually asked). **Played and green** (`8240`, 2026-09-27, 2 of 2); Harmony crash
  in that mod's own code described under pass 2 logged on every load, does not stop its interface limit.

## Native settings persistence tests: 13 September 2026

`Tests/SettingsPersistence.cs` runs as part of existing executable. Exercises BillAutopilotSettings.ExposeData through RimWorld's Scribe saver, loader, cross-reference resolution and post-load initialization. Does not substitute custom XML serializer. Full suite now 74 checks, including 18 native persistence checks. Earlier 44/56-check results above remain historical snapshots.

Coverage: global toggles/cap/interval; enabled profiles and their counts, modes and uncountable policy; per-recipe overrides; deletion and resaving; restored defaults; absent fields in synthetic legacy-shaped file; null collections; safe fallback when old custom mode has no identity; edited quantities retaining inheritance after reload. Legacy fixtures test absent-field compatibility, not authenticated old user save.

Harness setup confined to console test process. .NET Framework CLR cannot inspect some game types containing default interface methods. Test supplies real loadable types to GenTypes and registers three real serialized model types, which game normally discovers through mod loader. Disables DeepProfiler, whose preferences not initialized here, uses managed log sink failing on serializer errors. No Scribe method or mod serialization method patched or replaced. Generated XML fixtures under `build/bin/tests/Release/persistence-results/`.

Proves native file/object persistence for settings schema. Does not test actual Mod.WriteSettings file selection, process restart, save-game bill memory, Unity widgets or RIMMSQOL. Those remain in scenarios 12 and 20 and final FR/EN game pass. Console setup not shipped. Runtime DLL unchanged by this test addition.