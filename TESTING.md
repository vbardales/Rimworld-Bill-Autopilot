# Testing Bill Autopilot in game

Nothing has been released. Version 1.0.0 is still unpublished, so every line below describes a mod
that has been compiled far more often than it has been played.

Already seen working, by hand, in a real save, on 1 September 2026: bills put up and taken down on
their own, a recipe unlocked by research arriving as a suspended bill, and deleting an automatic bill
excluding its recipe. That is the base loop, and it is the only part with a witness.

Never run once: everything written since. The mark in the bill label, the capture of a change made in
the tab, the memory held per workbench rather than per workbench type, repeat modes belonging to
other mods, and all five compatibility layers. Better Workbench Management is the largest of them and
has never had a single line of it executed in a running game.

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
the decision reaches the bench: that a butcher table really does stay idle, and that a foreign mode
really is set on a real bill. **The program is the arithmetic, the scenario is the wiring.**

It was itself checked by breaking the clamp on purpose and confirming that those two checks, and only
those two, turned red. A suite never seen to fail proves nothing.

## The Pickle suite, and how many passes a verdict needs

Nineteen feature files live in `Tests/Pickle/`, written on 21 September 2026 and **not yet run**.
They hold only what a running game can show. The decision layer — which mode applies, which target,
which floor, the clamps, the overflow guard, the fallback for a repeat mode whose owner has gone,
and the settings round trip through Scribe — is proven by the executable suite above and is
deliberately not restated there: a Pickle run takes the whole machine for tens of minutes, and a
scenario repeating a unit test would cost that every time and add nothing.

`Tests/Pickle/README.md` says, per feature, why a game is needed, and keeps the list of what stays
manual. The numbered scenarios below are what the features were written from.

**A verdict needs three passes, and they are not interchangeable.**

1. **Without the optional mods** — the default staging: Core, the DLC, Harmony, RimLogging, Pickle,
   Harmony as this mod's only hard dependency, and the suite. It covers scenarios 1 to 12 and 18,
   and proves the mod stands alone, which is what the mod page claims of all five bridges. The
   scenarios for absent mods carry `@requires:` and are **skipped**, so this pass is green with a
   dozen scenarios never played: read the skips, not only the failures.

   ```powershell
   powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot
   ```

2. **With the optional mods** — `Tests/Pickle/wsl-deps.avec-facultatifs.map`. It mounts all six integrations this mod
   declares in `loadAfter`, plus No Max Bills: Redux for the raised bill ceiling that scenario 10
   reads live rather than hard-coding. It covers scenarios 13 to 16 and 19 on top of the first pass.
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
   features 13, 15, 16 and 17 and two scenarios of 14: none has run yet. The pass with the optional
   mods (`wsl-deps.avec-facultatifs.map`) has to play every one of them, and the report has to show
   them played, not skipped.
3. **No manual test is left to validate: all are green.** The manual list of `Tests/Pickle/README.md`
   (a save loaded with the mod removed, RIMMSQOL's own interface, the Nice Bill Tab drag, the two
   Better Workbench Management details, Choose Your Recipe, every `@review` screenshot) is closed by
   a person, each entry recorded green in STATUS.md. An entry not yet done is pending, not passed.

**Two things no pass here can do**, both kept in `Tests/Pickle/README.md`: loading a save with the
mod removed, which the mod list makes impossible from inside a run and which is the whole reason the
state avoids a GameComponent; and anything inside RIMMSQOL's own interface.

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

**Proves** the Harmony patches and the five detection probes. Every other scenario depends on this
one.

Start the game, reach the main menu, quit. Search `Player.log` for `[Bill Autopilot]`.

**One line is expected, not silence**: `Integrations: … Bill cap per workbench: 15.` Each of the four
names is followed by `found` or `not found`, and the cap reads 125 instead of 15 when Better
Workbench Management sees No Max Bills.

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
asks.

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

**Proves** the counting through the probe bill, and the two thresholds. Confirmed once, in September;
worth replaying because the counting path has been rewritten since to agree with Better Workbench
Management.

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
an answer. Confirmed once.

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

**Proves** the hook on the deletion, which is what gives the gesture a meaning. Confirmed once.

Delete a running automatic bill. A message names the recipe, the autopilot stops offering it, and the
*Configure* window shows that recipe set to *Never*.

Set it back to *Default* in that window: it must come back on the next pass.

Deleting a bill **you** placed must do none of this.

## 6 — The mark in the label

**Proves** the postfix on `Bill_Production.LabelCap`, chosen so that every interface picks it up
without any of them being patched. Never seen on screen.

Automatic bills carry `(auto)` at the end of their label.

- Look in the **vanilla tab**, and in every other bill interface you have: Nice Bill Tab, Dubs Mint
  Menus, Better Workbench Management. The mark must show in all of them, since they all read the same
  label.
- Bills you placed by hand carry nothing.
- Turn *Mark automatic bills* off in the settings: the mark goes, everything else stays.

A mark that shows in the vanilla tab but not in a replacement tab means that mod builds its rows from
something other than the label, and the approach needs revisiting.

## 7 — Adjusting a bill in the tab is adjusting the profile

**Proves** the drift capture. Never run.

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

**Proves** the rule that keeps the mod out of your way.

On an autopiloted bench, place a bill yourself for a recipe the autopilot also handles.

- **The autopilot's bill for that recipe goes**, and does not come back while yours stands.
- Delete yours: the autopilot may take the recipe again.

## 9 — Recipes the game cannot count

**Proves** the countability test, and the separate setting that exists because *keep a stock* is
impossible for them.

Put a **butcher table** on autopilot, or a smelter, a crematorium, an electric smithy doing surgery.

- In *Autopilot profile*, those recipes are marked as uncountable and their tooltip says so.
- With the default, *Recipes with no countable product* set to *Never*, **the butcher table produces
  nothing at all**. That is correct, not a bug.
- Set it to *Always* instead: standing bills appear and never stop.

## 10 — The cap, and room left for your own bills

**Proves** the arithmetic that keeps the *Add* button alive. The game accepts 15 bills per bench and
hides the button beyond that.

On a workbench with many recipes, with the cap at its default of 8:

- **At most 8 automatic bills** stand at once, however much work there is.
- Place several bills yourself. The automatic ones give way: the cap counts yours against the game's
  ceiling, so the *Add* button must never disappear.
- Raise the cap in the settings and check that the slider stops at 15, or at 125 with Better
  Workbench Management and No Max Bills.

## 11 — Two benches of the same kind, set differently

**Proves** the memory keyed per workbench. It used to be keyed per workbench type, and the two were
indistinguishable until a second bench existed.

Build **two** benches of the same kind, both on autopilot, and give the same recipe a different
custom name on each, through Better Workbench Management or vanilla renaming.

Let both bills come down and go back up. **Each bench must get its own name back.** One name landing
on both benches, or on the wrong one, is the old per-type key returning.

Then **deconstruct one bench**. Its entry is dropped on the next pass; nothing visible should happen
to the other.

## 12 — Save, reload, and remove the mod

**Proves** the one decision that a single session cannot check: the state is grafted into the save's
`<game>` node instead of living in a `GameComponent`, precisely so the mod can be removed.

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
lost, nothing crashes.

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
is not a cosmetic problem: dragging can put a deleted bill back.

With Nice Bill Tab active, on an autopiloted bench, **keep the tab open** and let a bill come down on
its own as its stock fills.

- **The row must disappear from its list.** A row that stays is the cache not being told.
- Then **drag** the remaining rows around. No deleted bill may reappear. This is the failure the
  integration exists to prevent, and it has never been replayed.
- With **Nice Bill Tab - Expansion**, hide a recipe on that bench: the autopilot must treat it as
  refused and never put it up.

## 16 — Dubs Mint Menus bench templates

**Proves** the postfix on `MakeBenchTemplate`. Without it a template photographs the autopilot's
passing queue, and re-applying it later turns those recipes into hand-placed bills for good, retiring
the autopilot from them without a word.

With Dubs Mint Menus active, on a bench running several automatic bills, **make a bench template**.

- A message names how many autopilot bills were left out.
- Open the template: it holds **only the bills you placed**.
- Apply it to another bench. Those bills read as hand-placed, which is correct, and the autopilot
  stands back from their recipes.

## 17 — Without any of the optional mods

**Proves** that the five bridges are soft, as the mod page claims.

Turn off Better Workbench Management, Nice Bill Tab, Nice Bill Tab - Expansion, Dubs Mint Menus,
Everybody Gets One and Choose Your Recipe, keeping Harmony. The mod must load, the settings and profile windows must open,
the base loop must work, and the log must stay silent apart from the startup line, which now reads
`not found` throughout.

## 18 — The profile window on a Steam Deck

**Proves** the one constraint that shaped the interface: it is played with a pointer, and a text
field would summon the virtual keyboard.

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

**Proves** the Nice Bill Tab - Expansion hidden-recipe bridge and the recipe list supplied by
Choose Your Recipe. Never run during the repository audit.

Use a disposable save with an enabled bench, a countable recipe below its restart threshold,
and no manual bill for that recipe. Test each integration separately with its required dependencies.

- With Nice Bill Tab - Expansion, hide the recipe and synchronise the bench by opening its bills
  tab. No automatic bill for it should remain or reappear on subsequent passes.
- Unhide it: its automatic bill should return while stock is still below the threshold.
- With Choose Your Recipe, disable the recipe using that mod's configuration and reload if
  required by that mod. It should be absent from the bench's available recipes and should not
  acquire an automatic bill. Re-enable it and verify it becomes eligible again.
- Repeat without either integration: the recipe should follow the normal profile rules.

A hidden recipe being queued is an exclusion failure; one that stays excluded after being
restored is a stale-state failure. Preserve Player.log and the active mod list with the result.

## 20 — Hidden settings shortcut and custom quantity regression

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

Not executed in game during the implementation pass. The executable tests cover
quantity decisions, overflow and the shortcut worker's native visibility contract;
the XML tests cover the shipped hidden default and bilingual definition fields.

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
