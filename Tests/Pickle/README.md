# The Pickle suite for Bill Autopilot

Nineteen feature files, written and **not yet run**. They hold what only a running game can show;
everything provable outside one is proven outside one, in `Tests/BillAutopilot.Tests.csproj`.

A run takes the whole machine — real rendering, real clicks, real screenshots — for tens of
minutes, against seconds for the executable suite. That is the reason for the line drawn below.

## What lives here and what does not

`Tests/BillAutopilot.Tests.csproj` runs the shipped assembly against the real RimWorld assemblies
with no game at all, and already answers the decision layer: which mode applies to a recipe, which
target, which floor, the clamp that keeps a floor under its target, the overflow guard on a quantity
edit, what becomes of a repeat mode whose owner has been removed, and the settings round trip
through RimWorld's own Scribe. **None of that is repeated here.** A scenario that restated it would
confiscate the machine on every run and add nothing.

What is left needs a game, and needs it for a concrete reason:

| Feature | Why a running game |
| --- | --- |
| 01 loading | The Harmony patches took, and the mod's report of its neighbours matches what is loaded |
| 02 activation | A real dialog, whose announced count has to equal a real intake |
| 03 base loop | The count comes from a probe bill read through the game's own RecipeWorkerCounter |
| 04 new recipe | ResearchManager.FinishProject, a real letter stack, and the pending gate across two benches |
| 05 delete refuses | Notify_BillDeleted, raised only by a real BillStack.Delete |
| 06 the marker | A postfix on Bill_Production.LabelCap, on a bill whose property really goes through it |
| 07 drift capture | A real bill edited, then a real down-and-up cycle: the bill that returns is a different object |
| 08 hand-placed wins | Two bills for one recipe on one real stack |
| 09 uncountable | RecipeWorkerCounter.CanCountProducts on a probe bill attached to a real bench on a real map |
| 10 the cap | A budget computed against a real stack and the live bill ceiling |
| 11 per-bench memory | Two benches of one kind: with one bench, the per-bench and per-type keys are identical |
| 12 save and reload | RimWorld's own Scribe, on a real game |
| 13, 15, 16, 17 | Another mod's live objects |
| 14 foreign repeat mode | A foreign mode on a real bill, surviving a real cycle |
| 18 shortcut | What the main bar's worker does, and which mod the dialog belongs to |
| 19 profile window | A layout, read by an eye |

## How many passes a verdict needs

Three families, and a verdict needs the first two. The mod declares no `incompatibleWith`, so there
is no incompatibility pass to write: if one is ever declared, it needs a pass of its own that
ASSERTS the documented symptom, since an expected red and an accidental red are the same colour.

**1. Without the optional mods** — the default staging: Core, the DLC, Harmony, RimLogging, Pickle,
this mod's only hard dependency and the suite.

```powershell
powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot
```

It proves the mod stands alone, which is what the mod page claims of all five bridges: soft, so the
worst case is a lost feature. The scenarios for absent mods carry `@requires:` and are skipped, so
**this pass is green with a dozen scenarios never played** — read the skips, not only the failures.

**2. With the optional mods** — `wsl-deps.avec-facultatifs.map`, which mounts all six integrations the mod declares,
plus No Max Bills: Redux for the raised bill ceiling that `10-bill-cap.feature` reads live.

```powershell
powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot -DepMap wsl-deps.avec-facultatifs.map
```

It proves the mod holds in the decor it will really be loaded in. Green on the first pass says
nothing about this one, and the reverse is just as true: a scenario can pass *only* because an
optional mod is there.

**One set, not several.** The doctrine asks for a named set per exclusive combination, and Nice Bill
Tab redraws the whole bills tab while Better Workbench Management adds to it and Dubs Mint Menus
keeps its own menu beside it — so the question was worth asking. Virginie, who plays with these, has
seen no incompatibility between them (2026-09-21). That is a player's experience rather than a
measurement, which is why this pass exists: a scenario going red because two neighbours fight is a
real result, and the answer is to split this set per combination, not to loosen the scenario.

**3. Each language, in its own pass.** The language is fixed at staging and never switched inside a
run: `SelectLanguage` reloads every piece of game data underneath the runner.

```powershell
powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod BillAutopilot -Language French
```

No scenario here spells an English string — every label, marker, dialog and letter is rebuilt from
the mod's own translation key — so the same suite is the French check. The `@review` screenshots are
where a missing key shows: in developer mode, which every Pickle run is in, a key absent from the
active language comes back as accented gibberish rather than as clean English.

## Which bench, and why

`HandTailoringBench` with `Make_Patchleather`, whose product is `Leather_Patch`: one product, no
stuff, no research, and countable, so a stock can be set to an exact number and the thresholds
placed on either side of it. `Make_Apparel_Pants` behind `ComplexClothing` is the recipe unlocked
later. `ButcherSpot` carries one recipe of each kind — butchering cannot be counted, kibble can —
which is what lets 09 show that the uncountable setting reaches one and not the other.

If the fixture ever ships with `ComplexClothing` already researched, 04 fails on its first step
saying the recipe is already known. That is the guard working, not a defect in the mod.

## What is still manual, and why

Nothing below is a defect. It is work no scenario here performs.

- **Loading the save with the mod removed.** The whole reason the state is grafted into the save's
  `<game>` node instead of a GameComponent. A Pickle run cannot do it: the mod list is fixed at
  startup, and removing this mod removes the suite with it. It needs a pass of its own, by hand, and
  it is the first thing to report if it ever fails.
- **RIMMSQOL.** Revealing the shortcut inside its interface, and whether its visibility choice
  survives a restart. That is RIMMSQOL's behaviour; 18 covers this mod's side of the contract.
- **The Nice Bill Tab drag.** 15 asserts the cause — the cache is told to rebuild every time the
  autopilot changes the stack — because the drag itself is a gesture inside another mod's window,
  and a Pickle click lands on whatever window owns the point.
- **Better Workbench Management's workbench restriction, and the agreement between its widened
  count and what a bill displays.** Both are reachable in principle; neither was written against an
  interface that could be read rather than guessed at.
- **Choose Your Recipe.** It removes disabled recipes from the workbench before the autopilot sees
  them, so there is nothing of this mod's to assert. Confirming that is still a look.
- **Every `@review` screenshot.** A green there says the trip happened. It does not say the image
  shows anything, and it is not a visual check performed.

## Building the steps

```powershell
dotnet build Tests/Pickle/Source/BillAutopilot.PickleSteps.csproj -c Release
```

The output lands in `Mod/Pickle/Assemblies/`, which is what the staging copies. Step DLLs are loaded
at game start, so a rebuild needs a new run: a report produced without one did not test the fix.

Every step text begins with "Bill Autopilot" or names the neighbour it drives. Pickle loads the
steps of every installed suite into one namespace, and two suites sharing a step text produce
"Ambiguous step" and fail scenarios that are perfectly healthy.

## Reading a report

`exitReason` first, before any number. A run killed in flight leaves a report that looks like a
result. Then the count of scenarios played against the count of features discovered, in the
`SuiteScanner` line: `exitReason: passed` says nothing about what was never selected.
