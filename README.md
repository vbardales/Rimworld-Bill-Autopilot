# Bill Autopilot

A RimWorld 1.6 mod. Say once what a workbench is for, and stop rewriting its bill list every time
something is unlocked.

## What it does

A workbench type put on autopilot takes on **every** recipe it knows how to make. The mod puts a
bill up when there is something to do and takes it down once the stock is full, so the tab shows
what is left to produce rather than forty lines of configuration to scroll through.

When research unlocks a new recipe, it joins its workbench on its own, **as a suspended bill**,
with a letter naming it. Unsuspending it accepts the recipe, deleting it refuses it — and the
autopilot will not offer it again. Nothing is ever spent behind your back.

## Settings

Everything is set **per workbench type**: a bench built later is already configured. Reachable from
the mod settings, or from the "Autopilot profile" gizmo on the selected bench.

- **Default mode**: *Keep a stock of N*, or *Always*.
- **Target** and **restart threshold**: the bill appears when stock falls to the threshold and
  disappears when the target is reached. The gap between the two keeps it from flickering on every
  unit produced.
- **Recipes with an uncountable product** (butchering, smelting, cremation, surgery): the game
  cannot count these, so "keep a stock" is impossible for them. They get their own setting,
  *Never* by default.
- **Per-recipe override**: inherit, keep a different stock, always, or never.
- **Cap on automatic bills per bench** (8 by default): the game accepts only 15 bills per bench and
  hides the "Add" button beyond that. The cap leaves room for your own.

**Adjusting a bill in the tab adjusts the profile.** Change the target of an automatic bill and the
mod records it as an override for that recipe, instead of losing it the next time the bill comes
down.

## What it does not do

- No "x1" as a standing order: an instruction saying "make one" would be put back as soon as it
  finished, endlessly. *Keep a stock of 1* gives the intended effect and stops on its own.
- Nothing is redrawn in the bills tab, so mods that replace it keep working — Nice Bill Tab, Dubs
  Mint Menus and Categorized Bill Dropdown all own parts of it and are left alone. What the autopilot
  actively picks up from its neighbours is listed further down.
- A hand-placed bill always wins: the autopilot stands back from that recipe for as long as it
  exists.

## What it picks up from other mods

All detected on its own, none required. Everything goes through reflection: mod absent, behaviour
unchanged.

**Better Workbench Management** (`falconne.BWM`, assembly `ImprovedWorkbenches`) attaches an
`ExtendedBillData` to every `Bill_Production` — name, `CountAway`, additional product filter — kept
in a `WorldComponent`, and puts a prefix on `BillStack.Delete` that erases it along with the bill.
Since the autopilot removes and re-places bills constantly, all of that would be lost every time a
stock filled up. Hence four measures:

- **Read before removal, restored after placement.** The reading lives in `BillMemory`, keyed by
  "BenchDef/RecipeDef" — the granularity of the profile.
- **Linked bills preserved.** The `loadID`s of the companion bills are remembered; on re-placement
  the first still-living one is picked up again, and `LinkBills` reattaches to the existing group.
- **Workbench restriction applied.** BWM's hook on `BillUtility.MakeNewBill` reads
  `Find.Selector.SingleSelectedThing` to know which bench is meant, which means nothing when the
  call comes from a tick. So the mod sets it itself, for the right table.
- **Counting aligned.** Its postfix on `RecipeWorkerCounter.CountProducts` bails out if the bill has
  no extended data — which is exactly the case for the probe bill. The same reading is therefore
  grafted on before measuring; otherwise the threshold that triggers and the number the bill shows
  would not be talking about the same thing. The bill cap comes from its `GetMaxBills()` too: 15, or
  125 with No Max Bills.

**Nice Bill Tab - Expansion** (`HICON.NiceBillTabExpansion`): its `HiddenRecipeStore.IsHidden` only
filters its own add menu. A hidden recipe is treated as excluded.

**Dubs Mint Menus** (`dubwise.dubsmintmenus`): its bill menu does not take the tab over — its
`BillStack.DoListing` patch is a `void` prefix that only adjusts the rect — so nothing needs
reconciling there. Its **bench templates** do. `MakeBenchTemplate` photographs *every* bill on the
bench, autopilot bills included: a template made from an autopiloted bench would capture whatever
the autopilot happened to have up, and re-applying it later would put those recipes back as manual
bills, retiring the autopilot from them for good. A postfix takes our bills back out of the template
the moment it is created. Applying a template needs nothing: `ApplyTemplateToBench` clones with
`InitializeAfterClone()`, so the placed bills carry fresh ids that are absent from our stamps, and
the autopilot correctly reads them as placed by hand.

**Choose Your Recipe** (`zal.chooseyourrecipe`): nothing to do. It removes disabled recipes from
`def.allRecipesCached`, so they are already gone from the `AllRecipes` being walked.

On the **first activation** of a workbench type, every already-unlocked recipe is accepted at
once — which is what "I want all the recipes" means, but on a machining table that starts a great
deal of production. So it asks first, naming how many recipes it is about to take and at what
target. Switching the same bench back on later does not ask again: its opening stock of recipes has
already been absorbed, and nothing more is taken silently.

## How it works

- `AutoBillSync` — the engine. One pass per bench decides, recipe by recipe, whether to put a bill
  up or take it down.
- `RecipeProbe` — counts the stock of a product without leaving a trace: `RecipeWorkerCounter`
  demands a `Bill_Production` and reaches the map through `billStack.billGiver`, so one probe bill
  per recipe is kept in a detached stack.
- `BillAutopilotState` — the per-game state: which bills are ours (by `loadID`) and which recipes
  have already been seen. The configuration itself lives in the mod settings.
  **This is deliberately not a `GameComponent`**: the game writes a component as
  `<li Class="...">`, and removing the mod would make every load fail on
  `Can't load abstract class Verse.GameComponent`. The state is therefore grafted into the `<game>`
  node by a postfix on `Game.ExposeSmallComponents` — the only point common to both paths, since
  `ExposeData` refuses `LoadingVars`. Named nodes with no `Class` attribute are read by nobody once
  the mod is gone: the game ignores them silently. The heartbeat comes from a postfix on
  `TickManager.DoSingleTick`.
- `BillAutopilotSettings` — read in the `Mod` constructor, therefore **before** defs are loaded: no
  `Scribe_Defs` is possible there, hence the `AutoMode` enum rather than a `BillRepeatModeDef`.

## Build

    dotnet build Source/BillAutopilot.csproj -c Release

The assembly lands in `Mod/Assemblies/`. Reference assemblies come from NuGet
(`Krafs.Rimworld.Ref`), so no RimWorld installation is needed to compile.

## Licence

MIT. See `LICENSE`.
