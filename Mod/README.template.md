Say once what a workbench is for, and stop rewriting its bill list.

Put a workbench type on autopilot and it takes every recipe it can do. A bill goes up when there is work to do, and comes down once the shelf is full, so the tab shows what is left to make, not a wall of configuration you have to scroll past.

## When a recipe is unlocked

The point of the mod. Finish the research, and the recipe joins its workbench on its own, as a suspended bill plus a letter naming it. Unsuspend it to accept, delete it to refuse, and the autopilot will not offer it again. Nothing is ever spent behind your back.

## What you set

- Per workbench type, so a bench you build later is already configured.
- A default for every recipe: keep a stock of N, or always.
- Or a repeat mode from another mod, when one is installed: "one per colonist" and the like can be set as the default for a whole bench.
- Smelting a weapon, cremation, surgery and anything else the game cannot count get their own setting, since "keep N in stock" is impossible for them.
- Any recipe can override the default, or be left out entirely.
- A cap on how many automatic bills may stand on one bench at a time, so there is always room for bills of your own.

The recipe list is grouped by product category, each group collapsible, with a filter for the ones you have overridden. No search field: every control is meant to be reachable with a pointer alone.

Switching a workbench type on asks first, naming how many recipes it is about to take and at what target: the one moment where a lot of production can start at once. Switching it back on later does not ask again.

Bills the autopilot put up are marked in their label, so you can tell them from the ones you placed yourself. That matters, because deleting an automatic bill is what excludes its recipe.

Change a bill's target in the bills tab and the autopilot keeps it: adjusting the bill IS adjusting the profile.

## Why not "do 1 time"

A standing order that says "make one" would be remade the moment it finished, forever. "Keep 1 in stock" gives you what people usually want from it, and stops on its own.

## Works with

Nothing is replaced in the bills tab, so mods that redraw it keep working. Bills placed by hand always win over the autopilot: put one up yourself and it steps aside for that recipe.

Detected automatically, none required; the settings screen names the ones it found. Every call into a neighbour is wrapped, so the worst case is a lost feature, never a broken game.

- [Better Workbench Management](https://steamcommunity.com/sharedfiles/filedetails/?id=935982361): what it adds to a bill survives the autopilot taking that bill down and putting it back up (custom name, counting away from the home map, extra products counted toward the target, and membership of a linked bill set, which is rejoined rather than lost). Its workbench restriction is applied to bills the autopilot creates, which its own hook cannot do since that hook reads the selected workbench. Its wider counting rules are used when deciding whether a stock is full, so the threshold and the bill's own display agree. Its raised bill ceiling is honoured too, when [No Max Bills](https://steamcommunity.com/sharedfiles/filedetails/?id=3526216885) is present.
- [Everybody Gets One](https://steamcommunity.com/sharedfiles/filedetails/?id=3530806680): a bill set to one of its repeat modes keeps it. The autopilot never flattens a repeat mode it does not own, and asks the mod that owns it whether there is work to do, instead of guessing at thresholds that are not its own. Any other mod adding a repeat mode gets the same treatment.
- [Nice Bill Tab](https://steamcommunity.com/sharedfiles/filedetails/?id=3520130671): it redraws the whole tab and keeps its own cached list of the bills it shows. That list is told to rebuild whenever the autopilot puts a bill up or takes one down, without which it would go on drawing bills that no longer exist, and dragging one could bring a deleted bill back.
- [Nice Bill Tab - Expansion](https://steamcommunity.com/sharedfiles/filedetails/?id=3721023311): a recipe you hide on a workbench is treated as one you do not want, and the autopilot leaves it alone.
- [Dubs Mint Menus](https://steamcommunity.com/sharedfiles/filedetails/?id=1446523594): its bill menu leaves the tab alone, so nothing collides. Its bench templates do need care: making one photographs every bill on the bench, so a template taken from an autopiloted bench would capture whatever the autopilot had up at that moment, and re-applying it later would turn those recipes into hand-placed bills for good. Autopilot bills are kept out of the template. Applying one needs nothing: the bills it places read as placed by hand, which is exactly right.
- [Choose Your Recipe](https://steamcommunity.com/sharedfiles/filedetails/?id=3263007587): nothing needed. It removes disabled recipes from the workbench itself, so the autopilot never sees them.

Can be added to a game in progress, and taken back out of one. Nothing it stores is written to your save as a class of its own, so removing it raises no load error the way a mod with its own game component does; the bills it had put up simply stay behind as ordinary bills, yours to keep or delete.

Interface in English and French. Every control is reachable with a pointer, no keyboard needed, with the Steam Deck in mind.

## If I go quiet

If I do not answer within a reasonable time after being contacted, anyone may freely update this or any other of my mods, including publishing a continuation of it. All credit must be preserved.

## AI-generated

This mod's code was written with Claude Code (Anthropic), under human direction, review and testing. Stated openly: designing with these tools is my job.

## Thanks

Andreas Pardeike for [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077).

The authors of the mods this one works beside, against whose interfaces it was written and with which it was tested: Falconne for Better Workbench Management, Andromeda for Nice Bill Tab, HICON for its Expansion, Dubwise for Dubs Mint Menus, Uuugggg for Everybody Gets One, Zaljerem for Choose Your Recipe and Just Harry for No Max Bills. Uuugggg again for [TD Find Lib](https://steamcommunity.com/sharedfiles/filedetails/?id=3529443295) and [TDS Bug Fixes](https://steamcommunity.com/sharedfiles/filedetails/?id=3529433984), which Everybody Gets One needs and which the test pass mounts for that reason only.

Used for development and testing only, never a dependency of this mod: [Pickle](https://steamcommunity.com/sharedfiles/filedetails/?id=3791648678) and [RimLogging](https://steamcommunity.com/sharedfiles/filedetails/?id=3733484696).

Credits, and the rights they rest on, are in ATTRIBUTION.md in the repository. This mod is MIT licensed and the notice ships with it.

[Source code on GitHub](https://github.com/vbardales/Rimworld-Bill-Autopilot)
