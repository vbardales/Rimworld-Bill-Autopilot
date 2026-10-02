# Publishing Bill Autopilot

What the Workshop page asks for and the repository holds nowhere else. Written before the first upload,
and kept for whoever picks this mod up later.

**The item exists, private, and only the item.** The maintainer created it on 2026-09-23 as 0.1.0, id
**3806709456**, and `About/PublishedFileId.txt` is committed. That upload was the creation, so the game sent
the description once, the one committed in `1842dec`. **From 1.0.0 the description is sent by the CI**, from
the single Markdown source under `## Steam description` below (decision of 2026-09-25), which also generates the
`<description>` of `About.xml`. The item stays private until Virginie switches it to public by hand.

## The one-way parts

Three things do not get a second chance, and two of them are silent when they go wrong.

- **The description.** `Verse.Steam.Workshop.SetWorkshopItemDataFrom` calls `SetItemDescription` only when
  `creating` is true, so the game's own upload never corrects it. The CI does: a `publish` with
  `update_description` **overwrites the page** with the block below. Compare the text the dry-run prints with
  this block before approving, not the hash.
- **The packageId.** `nelim.billautopilot`. It is written into every subscriber's `ModsConfig.xml` and into
  other mods' `loadAfter`. Changing it after publication disables the mod for everyone.
- **`About/PublishedFileId.txt`.** Steam writes it into the mod folder at creation. **Commit it immediately.**
  Lost, the next upload creates a second item, and the first one stays up with nobody able to update it.

And one that surprises people: **Steam creates every item private.** RimWorld never calls
`SetItemVisibility`, so the item has to be switched to public by hand once it has been checked.

## Screenshots, in upload order

Steam shows the first one large. That slot goes to the most demonstrative image, not the prettiest.

**None of these exist yet.** They are not the Preview — that is the header image, already made
(`Mod/About/Preview.png`, 896x504). These are the page's own captures, and the intent is to produce them
from a dedicated Pickle scenario rather than by hand, so they can be remade after any interface change.

| # | What it must show | Why this slot |
| --- | --- | --- |
| 1 | A bills tab holding two or three automatic bills, each marked, on a bench with far more recipes than that | The whole pitch in one image: the tab shows what is left to make, not a wall of configuration |
| 2 | The profile window, groups collapsed except one, with an overridden recipe visible | Where the "say it once" happens, and the proof it is reachable with a pointer alone |
| 3 | The confirmation dialog when a workbench type is switched on, naming its count | The one moment a lot of production can start at once, and the mod asks first |
| 4 | A suspended bill for a newly researched recipe, with the letter that named it | The point of the mod: nothing is spent without an answer |
| 5 | The settings page, with the "found around it" line naming the integrations detected | Answers "does it work with X" before anyone asks |

Rules for every one of them: no developer tools, no debug overlay, no other mod's overlay, no Pickle launcher
panel in a corner, no column sitting on its prompt text. A window with nothing in it sells nothing. The scene
is built by the scenario — a group created and filled under a readable name, never one borrowed from another
mod whose raw defName shows.

**Each image has to be opened and looked at before it is uploaded.** A capture scenario passing green says the
trip happened, not that the picture shows anything.

## Dependencies and DLC to declare

Read from the sources on 2026-09-21, not from intent.

**Hard dependency: Harmony, and only Harmony.** `brrainz.harmony`, declared in `modDependencies` with both its
Workshop URL and its GitHub release URL. The mod installs its patches through it at construction; without it
nothing loads.

**No DLC, none, not even optionally.** `supportedVersions` is `1.6`. There is no `LoadFolders.xml`, no
`Patches/`, no Def or C# path conditional on Royalty, Ideology, Biotech, Anomaly or Odyssey, and no
`MayRequire` anywhere. The five DLC in `loadAfter` are load-order only and must **not** be declared as
requirements on the page: a hard dependency forces a download on someone who does not want it.

**Six optional integrations, declared as `loadAfter` and nothing more.** They are detected at runtime by
reflection, every call into them is wrapped, and the worst case is a lost feature rather than a broken game.
The settings page names the ones it found. On the Workshop page they belong in the text as "works with",
never in the required items.

| Mod | packageId | What this mod does with it |
| --- | --- | --- |
| Better Workbench Management | `falconne.BWM` | Keeps its extended bill data across the autopilot's remove-and-replace cycle, applies its workbench restriction to a bill created from a tick, honours its raised bill ceiling |
| Dubs Mint Menus | `dubwise.dubsmintmenus` | Keeps automatic bills out of a bench template |
| Nice Bill Tab | `Andromeda.NiceBillTab` | Tells its cached row list to rebuild whenever a bill goes up or comes down |
| Nice Bill Tab - Expansion | `HICON.NiceBillTabExpansion` | Treats a recipe hidden there as one the player does not want |
| Everybody Gets One | `Memegoddess.EverybodyGetsOne` | Keeps a repeat mode it owns, and asks it whether there is work rather than guessing |
| Choose Your Recipe | `zal.chooseyourrecipe` | Nothing to do: it removes disabled recipes from the workbench itself, so the autopilot never sees them |

No Max Bills is not an integration, and is not declared: it raises the per-bench bill ceiling that Better Workbench Management
reports, and the cap honours whatever that number is. It is named in the text, and thanked, because the test pass mounts it.
So are the two libraries that pass mounts for Everybody Gets One (TD Find Lib, TDS Bug Fixes): none is a dependency of this mod.

## Adult content boxes

**No to all of them.** Both images were opened and looked at on 2026-09-21, not judged by their file names:

- `Mod/About/Preview.png` — a title, a tagline, and an overhead workshop scene: a machining table, a lamp, a
  crate of components, and a small rear-facing colonist in orange with no readable face. A `1.6` corner badge.
- `Mod/About/ModIcon.png` — a stylised orange face, winking, with a ponytail, a clipboard and a gear.

Nothing sexual, nothing graphic, no nudity, no gore. The mod adds no art beyond one gizmo icon, no text
beyond its own interface strings, and touches no body, health or social system.

## Messages to post, one per recipient

Written for the nine recipients this mod reaches: the six it works beside, and three that only the test pass
mounts (No Max Bills, and the two libraries Everybody Gets One needs). They follow the method of
`WORKSHOP_COMMENTS.md` ("Writing a comment", 2026-09-26): the owner's own plain voice, one true detail of the
recipient's mod, one thanks, 150 to 350 characters, one link to this mod hidden behind `[url=]`, never a bare
URL. **Posted after the item is public**: a link to a private item opens for nobody. Before posting any of
them, look the recipient's Workshop ID up in the register (every row is `drafted`), read the last comments of
the page, and put the drafts side by side: no shared opening, ending or joke. At most three a day and not in a
row; wait for an author's reaction before the next batch. Each block is BBCode ready to copy, one line, under
the limit of 1000 characters. Only what was played in the game is said to be tested: the Choose Your Recipe
draft says plainly that it was read, not run.

### Better Workbench Management (`falconne.BWM`, 935982361)

```text
Your extended bill data was the tricky part of my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url]: it takes bills down and puts them back, and BWM wipes its data when a bill is deleted, so I copy the name, the linked set and the rest first and restore them after. Works in my tests. Thanks for the mod :)
```

### Dubs Mint Menus (`dubwise.dubsmintmenus`, 1446523594)

```text
Your bench templates made me stop and think: they photograph every bill on the bench, so a template made on a bench run by my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] would keep whatever it had queued that minute. It now leaves its own bills out, tested in game. Thanks for the menus xD
```

### Nice Bill Tab (`Andromeda.NiceBillTab`, 3520130671)

```text
My [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] adds and removes bills on its own, and a cached row list has to be told when that happens, so I set your refresh flag every time. Checked in game with your tab open, no ghost rows. Thanks for the tab
```

### Nice Bill Tab - Expansion (`HICON.NiceBillTabExpansion`, 3721023311)

```text
Your hidden recipes turned out to be a handy signal: hide one on a bench and my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] reads it as "not wanted here" and never puts a bill up for it. Unhide it and it comes back on the next pass. Tested in game, thank you ✨
```

### Everybody Gets One (`Memegoddess.EverybodyGetsOne`, 3530806680)

```text
Your repeat modes are why my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] leaves any mode it doesn't own alone: a bill set to one of yours keeps it when I take it down and put it back, and I ask your mode whether there is work instead of comparing numbers. Tested in game. Thank you!
```

### Choose Your Recipe (`zal.chooseyourrecipe`, 3263007587)

```text
Yours needed no code from me, which is the best kind: you remove disabled recipes from the bench itself, so my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] never sees them. That is from reading your mod, I haven't run it in game yet. Thanks :)
```

### No Max Bills: Redux (`justharry.nomaxbillsredux`, 3526216885) — test pass only

```text
One of my test passes runs with your Redux on, to see how my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] copes when the 15-bill limit is gone (with BWM it reads as no limit at all). It copes. Thanks for bringing the mod back.
```

### TD Find Lib (`Memegoddess.TDFindLib`, 3529443295) — test pass only

```text
I found out how much Everybody Gets One leans on you the hard way: my first test run loaded it without TD Find Lib and a repeat mode threw in the middle of a tick, lol. It is mounted now for my [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] tests only. Thanks for the plumbing
```

### TDS Bug Fixes (`Memegoddess.TDSBugFixes`, 3529433984) — test pass only

```text
You sit at the bottom of a chain in my tests: Everybody Gets One needs TD Find Lib, which needs TDS Bug Fixes, so you get loaded in every [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456]Bill Autopilot[/url] run. Not a dependency of mine, just a thank-you for fixing what the game wouldn't :)
```

Andreas Pardeike (Harmony), Pickle and RimLogging are thanked in the description, and their registry rows are
already `posted` by other mods of the collection, so this mod is added to their `Covers` and nothing is posted.
Claude Code (Anthropic) is named under `AI-GENERATED` in the description and is not repeated in the thanks.

## Steam description

The single source (Virginie, 2026-09-25): Markdown, written once. The CI converts it to Steam BBCode and
generates the plain-text `<description>` of `Mod/About/About.xml` from it (`node
.github/scripts/sync-about-description.mjs --write`); every run stops if the two differ. No code fence inside
the block. The last line is the `Source code on GitHub` link.

```markdown
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
```

## Steam change notes

One `### <version>` fenced block per version, sent as written (BBCode, under 8000 bytes). **The first line
carries the exact version**, or the run stops. Unlike the description these can be corrected afterwards, and
they start again at every update.

### 1.0.0

```text
[b]1.0.0[/b]

First release.

Put a workbench type on autopilot and it takes every recipe it can do: a bill goes up when there is work and comes down once the shelf is full, so the tab shows what is left to make rather than a wall of configuration. A recipe unlocked later arrives suspended, with a letter naming it: unsuspend to accept, delete to refuse for good.

Per workbench type, with a default of "keep N in stock" or "always", a separate setting for recipes the game cannot count, and a per-recipe override that is also recorded when you adjust an automatic bill in the tab. A cap keeps room for bills of your own.

Works with Better Workbench Management, Dubs Mint Menus, Nice Bill Tab and its Expansion, Everybody Gets One and Choose Your Recipe. None required. Interface in English and French, reachable with a pointer alone.

Can be added to a game in progress and taken back out of one.
```

## Right after the upload, in this order

1. ~~Commit and push `Mod/About/PublishedFileId.txt`.~~ Committed (`13ac5ed`); pushing is what remains.
2. Subscribe to your own item and load it, as a subscriber sees it.
3. Switch the item to public by hand.
4. Post the six messages above. Their links already carry the item id.
5. ~~Write the Workshop id into `STATUS.md` under `workshop:`.~~ Done.

## What the first CI publish changes on the page

The page still carries the wording of 2026-09-23. The block above differs from it in one place: **butchering is
not an uncountable recipe** (the game counts raw meat for it), so the "What you set" item now starts with
"Smelting a weapon, cremation, surgery". Everything else reads the same. The publish is done with
`update_description` (and `update_preview` if the header image is to be resent) at both the dry-run and the
`publish`; the gallery of `Art/Gallery/` stays a manual upload.
