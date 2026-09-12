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

## Before anything

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

Turn off Better Workbench Management, Nice Bill Tab, Nice Bill Tab - Expansion, Dubs Mint Menus and
Everybody Gets One, keeping Harmony. The mod must load, the settings and profile windows must open,
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
