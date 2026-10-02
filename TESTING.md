# Testing Bill Autopilot in game

Nothing has been released. Version 1.0.0 is still unpublished. Most of what is written below is no
longer untested: as of 26 September 2026 the whole numbered suite (scenarios 1 to 22) has been played
green at least once, most of it twice (an English pass and a French one), on build `4B89FBA7...`.
**That build is superseded.** Two corrections landed in the code on 26 and 27 September — the No Max
Bills ceiling when Better Workbench Management is absent, and a trap where a recipe taken off a bench
while its "new recipe" question was still open was never offered again — and the shipped assembly is
now `BA624725...`. The replay of features 04, 05, 10 and 25 on that build is green (`b2c7`, 2026-09-27,
13 of 13) — the Choose Your Recipe pending-trap fix now has an in-game witness. The No Max Bills ceiling
fix does not yet: its own isolated scenario (`8240`, feature 26) is still queued. `STATUS.md` is the
source of truth for what has actually run and when; this file states what each scenario proves and how
to read a failure, not the day-to-day verdicts.

Each scenario says what it proves. A test whose failure you cannot interpret is not worth running.

## What no longer needs a human

One layer is covered by a program instead, in `Tests/`. It runs the shipped assembly against the
real RimWorld assemblies with no game running, and answers for itself:

```bash
dotnet build Source/BillAutopilot.csproj -c Release && dotnet build Tests/BillAutopilot.Tests.csproj -c Release && .build/bin/tests/Release/BillAutopilot.Tests.exe
```

Forty-four checks, and it exits non-zero when one fails. What it covers is the decision layer: given
a profile and a recipe, which mode applies, how many to keep, when to start again. That includes the
countability fallback, the clamp that keeps a floor under its target, and what happens to a repeat
mode whose mod has been removed — answers that are invisible in game, because a wrong one still
looks like a working mod quietly making the wrong amount of the wrong thing.

So scenarios 9 and 14 below no longer have to be read as arithmetic. What they still prove is that
the decision reaches the bench: that a smelter really does put up nothing for a smelted weapon, and that a foreign mode
really is set on a real bill. **The program is the arithmetic, the scenario is the wiring.**

It was itself checked by breaking the clamp on purpose and confirming that those two checks, and only
those two, turned red. A suite never seen to fail proves nothing.

## The Pickle suite, and how many passes a verdict needs

Twenty-six feature files live in `Tests/Pickle/`, written from 21 September 2026 and played since 23 September
(`docs/runs/pickle-runs.md` has one line per run). They hold only what a running game can show. The decision layer — which mode applies, which target,
which floor, the clamps, the overflow guard, the fallback for a repeat mode whose owner has gone,
and the settings round trip through Scribe — is proven by the executable suite above and is
deliberately not restated there: a Pickle run takes the whole machine for tens of minutes, and a
scenario repeating a unit test would cost that every time and add nothing.

`Tests/Pickle/README.md` says, per feature, why a game is needed, and keeps the list of what stays
manual. The numbered scenarios below are what the features were written from.

**A verdict needs three passes, and they are not interchangeable.**

1. **Without the optional mods** — the default staging: Core, the DLC, Harmony, RimLogging, Pickle,
   Harmony as this mod's only hard dependency, and the suite. It covers scenarios 1 to 12, 17, 18,
   20 (its non-RIMMSQOL half) and the vanilla gallery (23), and proves the mod stands alone, which is
   what the mod page claims of all six bridges. The scenarios for absent mods carry `@requires:` and
   are **skipped**, so this pass is green with a dozen scenarios never played: read the skips, not
   only the failures.

   ```powershell
   powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot
   ```

2. **With the optional mods** — `Tests/Pickle/wsl-deps.avec-facultatifs.map`. It mounts all six integrations this mod
   declares in `loadAfter`, plus No Max Bills: Redux for the raised bill ceiling that scenario 10
   reads live rather than hard-coding, and the two libraries Everybody Gets One needs (TD Find Lib and
   TDS Bug Fixes: the staging mounts only what a map lists, and without them its assembly loads half a
   class). It covers scenarios 13 to 17, 19 and 22 on top of the first pass.
   Green on the first pass says nothing about this one, and the reverse is equally true: a scenario
   can pass *only* because an optional mod is present.

   ```powershell
   powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot -DepMap wsl-deps.avec-facultatifs.map
   ```

   **One set covers them, not several.** A named set is owed per exclusive combination, and the
   question is a fair one here — Nice Bill Tab redraws the whole bills tab, Better Workbench
   Management adds to it, Dubs Mint Menus keeps its own menu beside it. Virginie, who plays with
   these mods, has seen no incompatibility between them (2026-09-21). That is a player's experience,
   not a measurement, which is what this pass is for: a scenario going red because two neighbours
   fight is a real result, and the answer to it is to split the set per combination.

   **A fourth set, for RIMMSQOL alone:** `wsl-deps.avec-rimmsqol.map` mounts RIMMSQOL and
   PickleTools' `RimmsqolSteps`, and only `20-rimmsqol-shortcut.feature` runs in it. It stays out of the pass
   with the neighbours because it changes the main bar, which no bill feature looks at, and a red then names
   its own cause. What it does not test: that RIMMSQOL keeps its choice across a restart, which is RIMMSQOL's
   behaviour and was shown by PickleTools' own demonstration on 2026-09-21.

   **A fifth set, for No Max Bills alone:** `wsl-deps.avec-nomaxbills.map` mounts only No Max Bills: Redux,
   deliberately without Better Workbench Management, so that `26-no-max-bills.feature` exercises the
   fallback path added 2026-09-26 (`NoMaxBillsCompat`, asked only when BWM does not answer). Queued,
   not yet played, as `8240`. Watch for the Harmony crash logged as `[ERROR] Error while instantiating a
   mod of type NoMaxBillsRedux.NoMaxBillsReduxMod` when reading its result (`STATUS.md`, 2026-09-27): the
   game survives it, but whether the mod's *other* patch — the one that actually lifts the interface's
   15-bill limit — ran before or after the one that throws is unknown until this pass has a real result.

   **A sixth set, for the removal chain:** `wsl-deps.removal.map` with `-Then removal-check -ThenWithout
   nelim.billautopilot,nelim.billautopilot.pickletests` (see scenario 21 below). Not a `-DepMap` any other
   pass uses; it exists only to hand a save from one launch to a second launch without the mod.

3. **Each language, in its own pass.** `-Language French`. The language is fixed at staging and
   never switched inside a run. No scenario spells an English string — every label, marker, dialog
   and letter is rebuilt from the mod's own translation key — so the same suite is the French check,
   and the `@review` screenshots are where a missing key shows: in developer mode, which every
   Pickle run is in, a key absent from the active language comes back as accented gibberish.

**No incompatibility pass.** `About.xml` declares no `incompatibleWith` and the README claims no mod
is broken by this one, so there is nothing to go and look at. If an incompatibility is ever
declared, it needs a pass of its own that **asserts the documented symptom** rather than expecting a
red: an expected red and an accidental red are the same colour, and nobody can tell afterwards which
one they were looking at.

### The gate to `tested`, as stated on 2026-09-23

Three conditions, on top of the passes above being run and read (`exitReason` first, then scenarios
played against features discovered):

1. **No scenario tagged `@wip` is left.** None is tagged today; the condition is that none is added to
   get a pass green.
2. **Every conditional scenario has actually run.** A scenario carrying `@requires:` is skipped, not
   failed, in a pass that lacks its mod, so a pass can be green with it never played. Here that is
   features 13 to 17, 19 and 22 (feature 20 in its own pass, feature 26 in its own pass, feature 21 in
   the removal pass). **Met on build `4B89FBA7...`**: the pass with the optional mods ran every one of
   them (English, request `479c`, then the French run `7018`), after repairing features 13, 14 and 15
   (two defects of the mod, and errors of the scenarios) on 2026-09-25. Feature 26, new on
   2026-09-26, has never run — its own isolated pass is queued as `8240`.
3. **No manual test is left to validate.** `AUDIT.md`: what used to be ticked by hand is either
   automated and green, or listed as not applicable with its reason. The former manual list of
   `Tests/Pickle/README.md` stands as follows (2026-09-27):

   | Former manual test | Now |
   | --- | --- |
   | RIMMSQOL revealing the shortcut in its own interface | Automated and green: `20-rimmsqol-shortcut.feature`, pass `avec-rimmsqol`, 3 of 3 on 2026-09-25, captures opened |
   | That RIMMSQOL keeps its visibility choice across a restart | **Not applicable**: RIMMSQOL's own behaviour, shown by PickleTools' demonstration (2026-09-21) |
   | The Nice Bill Tab drag | **Not applicable**: a gesture inside another mod's window, where a click lands on whatever window owns the point. The cause (the cache told to rebuild) is asserted by feature 15, green |
   | Choose Your Recipe | **Revised 2026-09-26, no longer simply not applicable.** Reading its code (it removes disabled recipes from a bench's own list) turned up a real trap: a recipe announced and then taken off the bench kept its "waiting for an answer" mark once its bill came down, and was never offered again after coming back. Corrected in code (`BillAutopilotState.Unannounce`) and covered generically by `25-recipe-taken-off-the-bench.feature`, which edits the recipe list the way that mod does without needing it staged — **played and green**, `b2c7`, 2026-09-27. A pass that stages Choose Your Recipe itself and drives its own window is still not written; see `BACKLOG.md` for the scenarios owed there (1.0.1, verification only) |
   | A save loaded with the mod removed | Automated and green: `21-removal-write.feature` and the companion `nelim.billautopilot.pickleremoval`, chain `-Then` / `-ThenWithout` (pass `wsl-deps.removal.map`), both launches passed on 2026-09-25 |
   | BWM: the workbench restriction applied to a bill created from a tick, and the agreement between the widened count and what the bill displays | Automated and green: `22-bwm-restriction-and-count.feature`, pass with the neighbours, 2 of 2 on 2026-09-25, and again in the French run of 2026-09-26 (not yet seen red: no mutation of the bridge was tried) |
   | Every `@review` screenshot | Most opened and read, recorded in `STATUS.md`. Still owed: the French pass captures (`fr1`, `fr3`), the gallery images once `88b3` and `8240` come back |

   An entry not yet done is pending, not passed. **Condition 3 is otherwise met**; the gate is held open
   by the build change of 2026-09-26/27 (below), not by any of these three.

**The build has moved twice since the passes above ran.** `4B89FBA7...` is what conditions 2 and 3 were
proven against. Two corrections since (No Max Bills alone, the Choose Your Recipe pending-trap) changed
`AutoBillSync` and `BetterWorkbenchesCompat`, shipping `BA624725...`. The conditions above therefore need
a replay on the new build before `done -> tested` can be called met again, not just a first play of what
is new: at minimum features 04, 05, 10 and 25, since the removal path every one of them exercises is
what changed. **Done**: `b2c7`, 2026-09-27, 13 of 13. **Still owed**: feature 26 alone (queued `8240`),
and the wider suite (13 to 24) has not been replayed on `BA624725...` at all yet — only the four
features the removal-path change directly touches have been. `STATUS.md` has the day-to-day state.

**Two things no pass here can do**, both kept in `Tests/Pickle/README.md`: loading a save with the
mod removed, which the mod list makes impossible from inside a run and which is the whole reason the
state avoids a GameComponent; and anything inside RIMMSQOL's own interface.

## Evidence to keep when a test runs

The root `AGENTS.md` rule ("Test evidence") is: keep only the reports that still prove something, and the disk
is full. For this mod that means the following. Everything raw lives in `docs/runs/evidence/<date>-run<N>/`,
which git ignores; what git tracks is `docs/runs/<date>-pickle-runs.md`, one table line per run.

**Keep, per run**

| What | Why it is kept |
| --- | --- |
| `summary.json` | Its `exitReason`, read before any number, and the scenarios played against the features discovered. Without them a killed run reads as a result |
| `junit.xml` | Per scenario outcome and failure message, which is where a suite defect is told from a mod defect |
| `Player.log` and the launcher's own log | The `Command line arguments` line proves which pass it was (`-pickle-set-name`), and the integration lines prove which optional mods were really mounted. A "minimal" pass that shows Better Workbench Management detected is not minimal |
| The `@review` captures | Their only value is that a person opens them. Feature 06's marker capture, 18's settings page, 19's profile windows, 15's Nice Bill Tab tab. Keep at full size |
| The captures of a scenario **no later run repeated** | The sole proof of that check |

**Delete, as soon as a newer report replaces it**

- `report.html` and `messages.ndjson`: about 100 MB each, and nothing in them that `junit.xml` and `summary.json` do not say.
- Any capture whose scenario a later run played again.
- Every report about a build that is no longer the one in the repository. When `Mod/Assemblies/BillAutopilot.dll` or a
  step assembly changes, the reports of the old one prove nothing about the new one.

**How**

- Copy a run out of `pickle-reports/` the moment it ends: the next session's run overwrites it, and the archive keeps
  only five. Run 1 of 2026-09-21 was lost that way.
- Shrink captures to JPEG (1280 wide, quality 70; the `@review` ones full size, quality 88). 78 MB became under 3.
- Keep a file path under 260 characters, or the conversion fails: shorten the scenario's capture name.
- Before deleting a report, check that no `STATUS.md` field points to it; repoint it first.

**What proves what, for this mod (2026-10-02).** Keep one report per row, the latest on the build in the repository; delete
the earlier ones once the new one is read. Per row, only `summary.json`, `junit.xml`, `Player.log` and the opened captures
(JPEG); never `report.html` or `messages.ndjson`, never a whole `screenshots/` folder.

| Proof | Latest report kept | What it is the proof of |
| --- | --- | --- |
| Features 01-12, a pass with the optional mods, per language | `fr1-01-06`, `fr2-07-12` (French), and the English twin when played | the base loop, the marker, drift, the cap, save and reload |
| Features 13-19 and 22, with the optional mods | `fr3-13-22` | the five bridges, the settings page, the profile window, BWM details |
| The skip of conditional scenarios in a pass without them | `skipcheck-13-17` | `@requires` scenarios are skipped, not failed, without the optional mods |
| The mod removed from a game saved with it | `removal-chain` (`-Then` with `-ThenWithout`) | the state avoids a GameComponent |
| RIMMSQOL reveals and hides the shortcut | `rimmsqol-20` | the hidden shortcut, its visibility, and the same settings page |
| No Max Bills alone | `nomaxbills-26-retry` | the ceiling above 15 without BWM |
| Fixes of 2026-09-27 | `replay-fixes-v2` | the recipe taken off a bench and brought back, deletion semantics |
| Workshop gallery, images 1-5 | `galerie-studio-1-4c` (images 1, 2, 4), `5b` (image 5), then the replay of image 3 | the five captures the owner judges; a green says the trip ran, not that the image is good |

A failure capture is kept only if its scenario has not been replayed green yet. Converting captures needs a path under 260
characters: a name that long lost one failure capture on 2026-10-02.

**Manual tests need their own evidence**, and it is small: the date, the game and mod versions, the list of active
mods, the language and UI scale, the `Player.log` for that session, and one line per case saying what was seen. A
manual entry recorded green without those is not recorded. They go under `docs/runs/`, in the same text summary.

## Before anything

### XML checks (no game required)

Run from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/ValidateXml.ps1
```

This checks every shipped XML file for parsing errors, the About metadata and Harmony dependency,
the GitHub link inside the description, duplicate or empty translation entries, English/French
key and placeholder parity, and literal translation keys referenced in C#. It exits non-zero
on failure. On 12 September 2026, all 376 XML checks and all 44 C# checks passed; both Release
builds completed without warnings. There are no Defs or XML patches in this mod. XML written into
saves still needs scenario 12 in a running game.

### Translation gate and runtime pass (added 13 September 2026)

Before `preTest`, follow the shared `PUBLISHING.md` and `TRANSLATIONS.md` protocol:
inventory player-facing text and its helper paths, check both languages against that
inventory, run the XML validator, and record evidence in `STATUS.md`. Reset affected
translation fields to `unchecked` whenever UI code or language resources change.
The 13 September static audit passed 397 XML checks with 54 keys per language;
the rebuilt Release assembly compiled without warnings or errors.

**Not yet run:** launch in English, then in French, and repeat this pass in each:

1. Open settings: title, introduction, integration presence/absence, tooltips, bill cap
   and bench summaries. Check at the intended resolution and UI scale.
2. Open a bench profile: counters, category counts, collapse/expand, filters, overrides,
   every repeat-mode menu and the uncountable-recipe tooltip.
3. Trigger each activation confirmation (maintain, always and an optional custom mode),
   inspect both gizmos and automatic bill labels, then trigger a new-recipe letter,
   recipe exclusion and captured-override message. Verify paragraph breaks and arguments.
4. With the optional integrations installed, repeat their displays and trigger the
   Dubs Mint Menus template notification. Verify the supplying mods' Def labels too.

Fail on raw keys, unintended English fallback in French, missing arguments, visible
escape sequences, clipping or overlap. Product names and the `(auto)` marker may match.
Record language, game/mod versions, UI scale, exercised cases and results in `STATUS.md`;
keep each untested case in `remaining`. Static completion does not prove this pass.

### Game setup

1. **Harmony must be active, and this mod after it.** It is the only hard dependency, declared in
   `About.xml`; the mod list says so if it is missing.
2. **The packageId is `nelim.billautopilot`.** It has never been published under another one.
3. **Keep `Player.log`.** Everything this mod says is prefixed `[Bill Autopilot]`. The file is
   overwritten at the next launch and moved to `Player-prev.log`, so copy it out before relaunching.
4. **Know how often it looks.** One workbench is synchronised per tick, and the queue is refilled
   every 600 ticks by default. On a base with twenty workbenches, a given bench is therefore visited
   about every ten seconds of game time. **Opening the bills tab synchronises that bench at once**,
   throttled to twice a second and paced in real time, so it works with the game paused too. If a
   scenario seems to do nothing, open the tab before concluding anything.
5. **Note which of the five optional mods are active.** Better Workbench Management, Nice Bill Tab,
   Nice Bill Tab - Expansion, Dubs Mint Menus, Everybody Gets One. The settings screen names the ones
   it found, under *Found around it*, and that line is the fastest way to learn that a feature is
   missing because a mod is, not because the code is wrong.
6. **Start from a save you can throw away.** Scenario 2 begins real production immediately, by
   design.

There are three ways in, and they do the same thing: the mod settings, the *Bill autopilot* toggle on
a selected workbench, and *Autopilot profile* next to it.

## 1 — The mod loads and says what it found

**Proves** the Harmony patches and the six detection probes. Every other scenario depends on this
one. Automated as `01-loading.feature`, green on build `4B89FBA7...` in both English and French
(runs 11 and, before it, the standalone run of 2026-09-21).

Start the game, reach the main menu, quit. Search `Player.log` for `[Bill Autopilot]`.

**One line is expected, not silence**: `Integrations: … Bill cap per workbench: 15.` Each of the five
names (Better Workbench Management, hidden recipes, Dubs Mint Menus, Nice Bill Tab, No Max Bills) is
followed by `found` or `not found`. The cap reads 125 instead of 15 when Better Workbench Management
sees No Max Bills, or "no limit at all" (2147483647 internally, never shown as a number since
2026-09-26) when No Max Bills is present without Better Workbench Management — that fallback is new
and unplayed, see scenario 26.

What a failure looks like:

- Any unhandled exception naming `BillAutopilot`. The stack trace names the step.
- *"… found, but its bench templates could not be reached"*, or the same shape for Nice Bill Tab.
  The mod is there but has renamed what this code reaches by reflection. Everything else keeps
  working; that one integration is dead until it is repaired. This is the warning most likely to
  arrive with someone else's update.
- A name reading `not found` for a mod you have active. That is the detection failing, not the
  integration.

## 2 — Switching a workbench type on, and what it takes at once

**Proves** the confirmation, its count, and the silent absorption of everything already unlocked.
This is the one moment where a great deal of production can start in a single click, which is why it
asks. Automated as `02-activation.feature`, green on build `4B89FBA7...`, English and French.

Select a stocked workbench, a machining table or a tailor bench. Use the *Bill autopilot* toggle.

- **A confirmation appears**, naming how many recipes are about to be taken and the target, 50 by
  default. Refuse it: nothing must change.
- Accept it. Within a few seconds, or at once if you open the bills tab, bills appear for what you
  are short of. **They arrive unsuspended and running** — already-unlocked recipes are absorbed in
  silence, and only what is unlocked later announces itself.
- Turn it off and on again. **It must not ask a second time**, in this game. The question is about
  the opening intake, which has already happened.
- Turning it off takes every automatic bill down and **leaves hand-placed bills alone**.

## 3 — The base loop, and the gap that stops it flickering

**Proves** the counting through the probe bill, and the two thresholds. Automated as
`03-base-loop.feature`, green on build `4B89FBA7...`, English and French.

With a bench on autopilot, default mode *Keep in stock*, target 50 and restart at 25:

- A bill appears while the stock is **at or below 25**, and not between 26 and 49.
- It comes down once the stock **reaches 50**.
- Let production run past the target and watch the tab: **the bill must not appear and disappear on
  every unit made**. That is what the gap between 25 and 50 exists for. A flicker means the two
  thresholds have collapsed onto one number.
- A colonist actually working at the bench holds his bill: **a bill being worked is never taken
  down**, even if the stock crosses the target mid-job.

## 4 — A recipe unlocked by research

**Proves** the announcement path, the suspended arrival, and the rule that nothing is spent without
an answer. Automated as `04-new-recipe.feature`, green on build `4B89FBA7...`, English and French.
**Replayed green on `BA624725...`** (`b2c7`, 2026-09-27): the recipe-forgotten trap fixed 2026-09-26
touches the same removal path this scenario exercises when a bill it put up comes down.

With a bench on autopilot, finish a research project that unlocks a recipe on it.

- A **suspended** bill appears, and a letter lists it. The letter can be several seconds behind the
  bill: it is sent once the sync queue empties.
- **Unsuspend it**: it is accepted and behaves like any other from then on.
- Or **delete it**: the recipe is refused for good, with a message saying so, and it must never come
  back on that workbench type.
- With two benches of the same kind, check that **the second does not start the recipe** while the
  first still shows it suspended. The question is asked once for the type, and until it is answered
  no bench of that type may act on it.

## 5 — Deleting an automatic bill means refusing the recipe

**Proves** the hook on the deletion, which is what gives the gesture a meaning. Automated as
`05-delete-refuses.feature`, green on build `4B89FBA7...`, English and French. **Replayed green on
`BA624725...`** (`b2c7`, 2026-09-27), same reason as scenario 4.

Delete a running automatic bill. A message names the recipe, the autopilot stops offering it, and the
*Configure* window shows that recipe set to *Never*.

Set it back to *Default* in that window: it must come back on the next pass.

Deleting a bill **you** placed must do none of this.

## 6 — The mark in the label

**Proves** the postfix on `Bill_Production.LabelCap`, chosen so that every interface picks it up
without any of them being patched. Automated as `06-auto-marker.feature`, green on build
`4B89FBA7...`, English and French; the `@review` capture has been opened and read: "Make patchleather
(auto)" beside an unmarked hand-placed bill.

Automatic bills carry `(auto)` at the end of their label.

- Look in the **vanilla tab**, and in every other bill interface you have: Nice Bill Tab, Dubs Mint
  Menus, Better Workbench Management. The mark must show in all of them, since they all read the same
  label.
- Bills you placed by hand carry nothing.
- Turn *Mark automatic bills* off in the settings: the mark goes, everything else stays.

A mark that shows in the vanilla tab but not in a replacement tab means that mod builds its rows from
something other than the label, and the approach needs revisiting.

## 7 — Adjusting a bill in the tab is adjusting the profile

**Proves** the drift capture. Automated as `07-drift-capture.feature`, green on build `4B89FBA7...`,
English and French.

On a running automatic bill, change the target in the tab, from 50 to 200 say.

- A message says the recipe has been kept as an override on that workbench type's profile.
- Open *Autopilot profile*: the recipe now shows its own numbers rather than the default.
- Let the stock fill so the bill comes down, then fall so it comes back. **It returns with 200, not
  50.** Without the capture, the change would be lost on the first cycle.

Switch a bill to *Forever* in the tab: the same capture happens, recorded as *Always*.

Switch one to **"do 1 time"** instead. **Nothing must be recorded and no message must appear.** That
mode makes no sense as a standing order and is deliberately ignored; the bill is left exactly as you
set it.

## 8 — A bill placed by hand always wins

**Proves** the rule that keeps the mod out of your way. Automated as `08-hand-placed-wins.feature`,
green on build `4B89FBA7...`, English and French.

On an autopiloted bench, place a bill yourself for a recipe the autopilot also handles.

- **The autopilot's bill for that recipe goes**, and does not come back while yours stands.
- Delete yours: the autopilot may take the recipe again.

## 9 — Recipes the game cannot count

**Proves** the countability test, and the separate setting that exists because *keep a stock* is
impossible for them. Automated as `09-uncountable-recipes.feature`, green on build `4B89FBA7...`,
English and French.

Put an **electric smelter** on autopilot, or a crematorium, or an electric smithy doing surgery. A **butcher
table is not a case for this scenario**: `RecipeWorkerCounter_ButcherAnimals.CanCountProducts` returns true and
the game counts raw meat for it. This scenario said otherwise until the first full Pickle run of 2026-09-23.

- In *Autopilot profile*, the recipes with no countable product (a smelted weapon) are marked as
  uncountable and their tooltip says so.
- With the default, *Recipes with no countable product* set to *Never*, **the smelter puts up no bill
  for a smelted weapon**. That is correct, not a bug.
- Set it to *Always* instead: standing bills appear and never stop.

## 10 — The cap, and room left for your own bills

**Proves** the arithmetic that keeps the *Add* button alive. The game accepts 15 bills per bench and
hides the button beyond that. Automated as `10-bill-cap.feature`, green on build `4B89FBA7...`,
English and French. **Replayed green on `BA624725...`** (`b2c7`, 2026-09-27), and its slider text has a
new case to check since 2026-09-26, still unproven: see scenario 26.

On a workbench with many recipes, with the cap at its default of 8:

- **At most 8 automatic bills** stand at once, however much work there is.
- Place several bills yourself. The automatic ones give way: the cap counts yours against the game's
  ceiling, so the *Add* button must never disappear.
- Raise the cap in the settings and check that the slider stops at 15, or at 125 with Better
  Workbench Management and No Max Bills, or at 100 with No Max Bills alone (the slider's own ceiling
  when the game sets none — the sentence beside it reads "the game sets no limit" instead of a number).

## 11 — Two benches of the same kind, set differently

**Proves** the memory keyed per workbench. It used to be keyed per workbench type, and the two were
indistinguishable until a second bench existed. Automated as `11-per-bench-memory.feature`, green on
build `4B89FBA7...`, English and French.

Build **two** benches of the same kind, both on autopilot, and give the same recipe a different
custom name on each, through Better Workbench Management or vanilla renaming.

Let both bills come down and go back up. **Each bench must get its own name back.** One name landing
on both benches, or on the wrong one, is the old per-type key returning.

Then **deconstruct one bench**. Its entry is dropped on the next pass; nothing visible should happen
to the other.

## 12 — Save, reload, and remove the mod

**Proves** the one decision that a single session cannot check: the state is grafted into the save's
`<game>` node instead of living in a `GameComponent`, precisely so the mod can be removed. The
save-and-reload half is automated as `12-save-and-reload.feature`, green on build `4B89FBA7...`,
English and French. **The mod-removed half is scenario 21**, its own feature and its own companion
mod, because no single Pickle run can change its own mod list mid-run; see below.

With several benches on autopilot, bills up and at least one recipe refused:

- Save, quit the game entirely, reload. **Everything is as you left it**: the same bills, the same
  refusals, no recipe re-announced as new, and the log silent.
- Then **remove the mod from the mod list** and load that same save. It must load, with **no**
  *"Can't load abstract class Verse.GameComponent"* and no error naming the mod. The bills it had put
  up stay behind as ordinary bills, yours to keep or delete.
- Put the mod back and load again: it picks up where it was.

The middle step is the whole point of the design. If it fails, say so before anything else.

## 13 — Better Workbench Management

**Proves** the largest compatibility layer, and the one with no witness at all. Everything here goes
through reflection into `ImprovedWorkbenches`, so a failure is silent by construction: the feature is
lost, nothing crashes. Automated as `13-better-workbenches.feature` (the down-and-up cycle) plus
`22-bwm-restriction-and-count.feature` (the workbench restriction on a bill created from a tick, and
the agreement between the widened count and what the bill displays — the two details written before an
interface could be read, and now covered): both green on build `4B89FBA7...`, English and French. Two
real defects of the mod were found and fixed this way on 2026-09-25 — see `STATUS.md`, "First play of
the optional-mod scenarios". Neither scenario has been seen red on the restriction/count details (no
mutation of the bridge was tried), so they prove agreement today, not that they would catch a
regression.

With that mod active, on one automatic bill, set as many of these as you can:

- a **custom name**,
- **count away from the home map**,
- an **additional product filter**,
- a **link** with another bill,
- and a **workbench restriction** on the bench itself.

Now let the stock fill so the bill comes down, then fall so it comes back.

- **All five must survive** the cycle. The bill returns with its name, its widened counting, its
  filter, and rejoins its link group rather than starting a new one.
- The **restriction** is the one thing this mod applies that Better Workbench Management cannot do by
  itself for a bill created from a tick, since its own hook reads the selected workbench. Check the
  new bill carries it.
- The threshold and the bill's own displayed count must **agree**. They are measured through the same
  widened rules; if the bill says 40 and the autopilot behaves as though it were 12, the probe is
  counting the vanilla way.

Then repeat the whole scenario **without** the mod. Nothing must break and the log must stay silent.

## 14 — A repeat mode from another mod

**Proves** that a mode the autopilot does not understand is set, kept, and asked rather than guessed
at. Everybody Gets One is the test case; any mod adding a repeat mode should behave the same.
Automated as `14-foreign-repeat-mode.feature`, green on build `4B89FBA7...`, English and French, after
two rounds of scenario repair on 2026-09-25 (a bill already standing before the mode was set; the
bench cap absorbed by unrelated recipes first) — see `STATUS.md`.

With Everybody Gets One active:

- In *Autopilot profile*, its three modes appear in the default menu and in any recipe's menu, under
  *Another mod*. Choose one as the **bench default**, "one per colonist" say.
- The bills that go up carry that mode. Whether there is work to do is decided by **that mod**, not
  by a target of ours: add or lose a colonist and the queue must follow.
- The two counters are labelled neutrally under such a mode, *Count* and *Second count*, because the
  owning mod reads them its own way.
- Set a mode by hand on an automatic bill, in the tab. It is recorded as an override, and the bill
  **returns with it** after a down-and-up cycle rather than being flattened to one of ours.

Then **disable Everybody Gets One** and load the save. A profile pointing at a mode that no longer
exists must fall back to *Keep in stock*, not put up a bill with no mode at all.

## 15 — Nice Bill Tab, and the drag that must not resurrect a bill

**Proves** the single most dangerous interaction in the mod. Nice Bill Tab keeps its own cached list
of the bills it draws, and reorders from that list before writing back into the stack. A stale entry
is not a cosmetic problem: dragging can put a deleted bill back. Automated as `15-nice-bill-tab.feature`
for the cause (the cache flag), green on build `4B89FBA7...`, English and French; its `@review` capture
has been opened and read. **The drag itself stays manual** (see below): it is a gesture inside another
mod's window, where a Pickle click would land on whatever window owns the point rather than testing
this mod.

With Nice Bill Tab active, on an autopiloted bench, **keep the tab open** and let a bill come down on
its own as its stock fills.

- **The row must disappear from its list.** A row that stays is the cache not being told. Automated.
- Then **drag** the remaining rows around. No deleted bill may reappear. This is the failure the
  integration exists to prevent, and the drag itself has never been played — only its cause, above.
- With **Nice Bill Tab - Expansion**, hide a recipe on that bench: the autopilot must treat it as
  refused and never put it up. This half is scenario 19, automated and green (`17-hidden-recipes.feature`).

## 16 — Dubs Mint Menus bench templates

**Proves** the postfix on `MakeBenchTemplate`. Without it a template photographs the autopilot's
passing queue, and re-applying it later turns those recipes into hand-placed bills for good, retiring
the autopilot from them without a word. Automated as `16-dubs-mint-menus.feature`, green on build
`4B89FBA7...`, English and French.

With Dubs Mint Menus active, on a bench running several automatic bills, **make a bench template**.

- A message names how many autopilot bills were left out.
- Open the template: it holds **only the bills you placed**.
- Apply it to another bench. Those bills read as hand-placed, which is correct, and the autopilot
  stands back from their recipes.

## 17 — Without any of the optional mods

**Proves** that the six bridges are soft, as the mod page claims. **No feature file of its own**: this
is what the first pass (without the optional mods, `sans-facultatifs`) proves by construction, since
every `@requires:` scenario is skipped rather than run in it and everything else must still be green.
Confirmed every time that pass has run, most recently the French run of 2026-09-25 (27+20+... scenarios,
`exitReason: passed`).

Turn off Better Workbench Management, Nice Bill Tab, Nice Bill Tab - Expansion, Dubs Mint Menus,
Everybody Gets One and Choose Your Recipe, keeping Harmony. The mod must load, the settings and profile windows must open,
the base loop must work, and the log must stay silent apart from the startup line, which now reads
`not found` throughout.

## 18 — The profile window on a Steam Deck

**Proves** the one constraint that shaped the interface: it is played with a pointer, and a text
field would summon the virtual keyboard. Automated as `19-profile-window.feature` — its own file
number and this scenario's number have drifted apart since scenario 17 above stopped being a feature
file; go by the `@review` tag, not the number, when in doubt. Green on build `4B89FBA7...`, English and
French; its three captures (the grouped window, one override, the uncountable group) have been opened
and read.

Open *Autopilot profile* on a workbench with many recipes.

- Recipes are **grouped by product category**, each group collapsible, with the uncountable ones
  gathered last under *Other*.
- *Collapse all* and *Expand all* work, and the collapsed state survives closing and reopening the
  window.
- *Overridden recipes only* filters to what you have changed, and *Clear the N overrides* empties
  them.
- **Every control is reachable with a pointer alone.** There is no search field, deliberately. If any
  step here needs the keyboard, that is the failure.

## 19 — Hidden recipes from optional mods

**Proves** the Nice Bill Tab - Expansion hidden-recipe bridge. Automated as `17-hidden-recipes.feature`
(again, its file number and this scenario's number disagree), green on build `4B89FBA7...`, English and
French, for the Nice Bill Tab - Expansion half.

**The Choose Your Recipe half was revised 2026-09-26.** It was marked below as "not applicable" because
that mod removes disabled recipes from a workbench's own list before the autopilot ever sees them, so
nothing of this mod's seemed left to assert. Reading its code (it has no public source; decompiled from
`1.6/Assemblies/ChooseYourRecipe.dll`) turned up a real trap that claim missed: a recipe announced —
suspended bill, letter, "waiting for an answer" — and then taken off the bench before the player
answered had its bill removed by the "recipe left the workbench" path, but the "waiting for an answer"
mark was not cleared with it; once the recipe came back, the mod believed the question was still open
with no bill to answer it, and never offered the recipe again, on any bench of that type. Fixed in code
(`BillAutopilotState.Unannounce`, called from `AutoBillSync.Remove`) and covered generically by
`25-recipe-taken-off-the-bench.feature` — see scenario 25 below — without needing Choose Your Recipe
staged, since any mod that edits `ThingDef.AllRecipes` hits the same path. **Played and green** (`b2c7`,
2026-09-27, on the current build). A pass
that stages Choose Your Recipe itself, sets its disabled list and drives its own window is still not
written (`BACKLOG.md`, for 1.0.1): what follows is that mod's own manual procedure, still owed.

Use a disposable save with an enabled bench, a countable recipe below its restart threshold,
and no manual bill for that recipe. Test each integration separately with its required dependencies.

- With Nice Bill Tab - Expansion, hide the recipe and synchronise the bench by opening its bills
  tab. No automatic bill for it should remain or reappear on subsequent passes. **Automated, green.**
- Unhide it: its automatic bill should return while stock is still below the threshold. **Automated, green.**
- With Choose Your Recipe, disable the recipe using that mod's configuration and reload if
  required by that mod. It should be absent from the bench's available recipes and should not
  acquire an automatic bill. Re-enable it and verify it becomes eligible again — **and if a "new
  recipe" letter was pending for it when it was disabled, check that the letter and the suspended bill
  come back too**, rather than the recipe staying silently excluded (the 2026-09-26 trap).
- Repeat without either integration: the recipe should follow the normal profile rules.

A hidden recipe being queued is an exclusion failure; one that stays excluded after being
restored is a stale-state failure. Preserve Player.log and the active mod list with the result.

## 20 — Hidden settings shortcut and custom quantity regression

Automated as two files: `18-settings-shortcut.feature` (everything below except the RIMMSQOL steps),
green on build `4B89FBA7...`, English and French; and `20-rimmsqol-shortcut.feature` (RIMMSQOL actually
revealing and using the shortcut), its own pass `avec-rimmsqol`, green in English (request `dafa`, 3 of
3, its captures opened and read). RIMMSQOL keeping its visibility choice across a restart is RIMMSQOL's
own behaviour, shown by PickleTools' own demonstration rather than repeated here.

Preconditions: RimWorld 1.6, Harmony and Bill Autopilot; a disposable save. Repeat in
English and French. Record exact game and optional-mod versions with Player.log.

1. With clean settings and no customization mod, verify no Bill Autopilot main-bar
   button is visible or greyed out. Open Mod options -> Bill Autopilot, change the
   marking option, close/reopen and verify the value and actual bill-label effect.
2. Install RIMMSQOL (or a tool supporting MainButtonDef visibility). Reveal
   `BillAutopilot_Settings`. Activate it: the native Bill Autopilot settings dialog
   must open. Change the cap; close and reopen through Mod options. Both routes must
   show the same value. Restart/reload and check persistence. Hide the shortcut again;
   verify the customization tool retains that visibility choice. Check logs throughout.
3. With Everybody Gets One, select a custom default mode on a bench profile. Leave
   a recipe inheriting that mode, then change its quantity. It must remain in that
   foreign mode with the new quantity. Test zero and an explicit custom override too.
   Save/restart/reload and verify the profile and actual bill use the retained mode.
4. Repeat normal Maintain quantity editing: minimum 1, valid restart threshold,
   unchanged inheritance. Remove the optional mode provider: safe Maintain fallback;
   restore it and verify the inherited mode identity was retained.

**Points 1 and 2 are automated and green** (features 18 and 20 above). **Point 3 is automated and
green** (feature 14 above: a custom default mode, a recipe inheriting it, a quantity change, the
provider removed and restored). **Point 4** (ordinary Maintain quantity editing, minimum 1, restart
threshold, fallback identity) is covered out of game by the executable tests
(`Tests/BillAutopilot.Tests.csproj`) rather than by a Pickle scenario; the shortcut worker's native
visibility contract and the shipped hidden default are covered by the XML tests. None of the four
points is left wholly unexecuted, but this section's own framing as one undivided manual procedure is
stale: read it as already split across scenarios 14, 18 and 20, plus the executable suite.

## 21 — A game saved with the mod, loaded without it

**Proves** the one decision no other scenario can: the mod's state lives in plain named nodes grafted
into the save's `<game>` node rather than a `GameComponent`, precisely so removing the mod raises no
load error. No single Pickle run can change its own mod list mid-run, so this needs two launches under
one hold of the dispatcher's lock: `-Filter 21-removal-write -Then removal-check -ThenWithout
nelim.billautopilot,nelim.billautopilot.pickletests`. Automated as `21-removal-write.feature` (first
launch: writes state, saves, checks the save holds `billAutopilot...` nodes and no `Class="BillAutopilot`
anywhere, hands the file to the companion mod) and `removal-check.feature` in the companion mod
`Tests/Pickle/Removal/Mod` (`nelim.billautopilot.pickleremoval`, never distributed, does not depend on
this mod; second launch: loads that save with Bill Autopilot genuinely absent from the mod list, runs
250 ticks, keeps the bill as an ordinary one, saves and reloads). Both launches green, English, request
`cf3a`, 2026-09-25. This closes the one manual entry `Tests/Pickle/README.md` used to keep as
unreachable from inside a run.

## 23 to 26 — the Workshop gallery, and the two 2026-09-26 corrections

Four features added after the numbered scenarios above, none of them proving a new mechanic on its own:

- **23 and 24 — the Workshop page's own captures**, produced by a scenario rather than by hand so they
  can be remade after any interface change (`PUBLICATION.md`, "Screenshots, in upload order").
  `23-gallery-vanilla.feature` (images 1 to 4, `-DepMap wsl-deps.galerie.map`: the studio and nothing
  else, so still "without the optional mods") and `24-gallery-neighbours.feature` (image 5,
  `-DepMap wsl-deps.galerie-voisins.map`: the neighbours plus the studio) are taken in **Nelim's Pickle
  Tools' zen meadow studio** (fixture `nelim-zen-meadow-studio`, `PickleTools/ScreenshotStudio`, chosen by
  the owner on 2026-09-27 after the first captures, taken in Pickle's played `test-colony`, were judged
  unfit for a Workshop page). Each scenario works in the studio's "display" pavilion, the empty interior
  meant for mod demonstrations: it is closed (a door and wall where it is open), roofed, so that the bench
  does not read "outdoors", and lit by four filled standing torches; the studio leaves its roofs off on
  purpose, and a roofed room with no light is too dark to sell anything. **The light is asserted, not left
  to the eye**: `the cell (125, 96) is lit at least 50 percent` reads the game's own glow grid on the bench
  cell (the game's own "lit" threshold is 30). Earlier history, for the record: the first tries of images
  1 to 4 failed once on a mistake in the feature (a setter line that silently matched an assertion step,
  `5d66`) and once on a region-updater re-entrancy in the room-building step (`88b3`), both fixed; the
  studio-based version is **not yet played**. **Every accepted image still has to be opened and looked at**
  before it becomes `Art/WorkshopScreenshots/01-`… to `05-`; a green scenario says the trip happened, not
  that the picture shows anything worth publishing.
- **25 — the Choose Your Recipe pending-trap, generically.** `25-recipe-taken-off-the-bench.feature`,
  added 2026-09-26 with the correction it proves (see scenario 19 above): a recipe announced and then
  taken off a bench's own list has its bill removed, and must be announced afresh, not silently
  forgotten, when it returns. Edits `ThingDef.AllRecipes` the way Choose Your Recipe does, without
  needing that mod staged, so it runs in every pass. **Played and green**, `b2c7`, 2026-09-27, 13 of 13
  (this is the scenario that proved it).
- **26 — No Max Bills, alone, without Better Workbench Management.** `26-no-max-bills.feature`, its own
  pass `wsl-deps.avec-nomaxbills.map` (deliberately without BWM, so the fallback added 2026-09-26 in
  `NoMaxBillsCompat` is the thing actually asked). **Not yet played** (queued `8240`); watch for the
  Harmony crash in that mod's own code described under pass 2 above when reading the result.

## Native settings persistence tests — 13 September 2026

`Tests/SettingsPersistence.cs` runs as part of the existing executable. It exercises
BillAutopilotSettings.ExposeData through RimWorld's Scribe saver, loader, cross-reference
resolution and post-load initialization. It does not substitute a custom XML serializer.
The full suite now has 74 checks, including 18 native persistence checks. The earlier
44/56-check results above remain historical snapshots.

Coverage: global toggles/cap/interval; enabled profiles and their counts, modes and
uncountable policy; per-recipe overrides; deletion and resaving; restored defaults;
absent fields in a synthetic legacy-shaped file; null collections; safe fallback when
an old custom mode has no identity; edited quantities retaining inheritance after reload.
These legacy fixtures test absent-field compatibility, not an authenticated old user save.

Harness setup is confined to the console test process. The .NET Framework CLR cannot
inspect some game types containing default interface methods. The test supplies the
real loadable types to GenTypes and registers the three real serialized model types,
which the game normally discovers through its mod loader. It disables DeepProfiler,
whose preferences are not initialized here, and uses a managed log sink that fails on
serializer errors. No Scribe method or mod serialization method is patched or replaced.
Generated XML fixtures are under `.build/bin/tests/Release/persistence-results/`.

This proves native file/object persistence for the settings schema. It does not test
actual Mod.WriteSettings file selection, process restart, save-game bill memory, Unity
widgets or RIMMSQOL. Those remain in scenarios 12 and 20 and the final FR/EN game pass.
The console setup is not shipped. The runtime DLL was unchanged by this test addition.
