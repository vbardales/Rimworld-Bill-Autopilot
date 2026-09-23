# Publishing Bill Autopilot

What the Workshop page asks for and the repository holds nowhere else. Written before the first upload,
and kept for whoever picks this mod up later.

**The item exists, private, and only the item.** The maintainer created it on 2026-09-23 as 0.1.0, id
**3806709456**, and `About/PublishedFileId.txt` is committed. That upload was the creation, so
`SetItemDescription` ran once and **the description is now frozen**: it was the one committed in
`1842dec`, with the credit line, and any change to it is made by hand on the Steam page, never from
`About.xml`. The mod is at `done` in the workflow: the in-game gate of `done -> tested` has not been
passed, so no functional release is due yet, and the item stays private until it is.

## The one-way parts

Three things do not get a second chance, and two of them are silent when they go wrong.

- **The description.** `Verse.Steam.Workshop.SetWorkshopItemDataFrom` calls `SetItemDescription` only when
  `creating` is true. Every later update leaves the page's text alone, so a correction after the first upload
  is made by hand on Steam and never from `About.xml`. Read it once more in the file before clicking.
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

No Max Bills is not an integration. It raises the per-bench bill ceiling that Better Workbench Management
reports, and the cap honours whatever that number is; it is worth a line in the text, not a declaration.

## Adult content boxes

**No to all of them.** Both images were opened and looked at on 2026-09-21, not judged by their file names:

- `Mod/About/Preview.png` — a title, a tagline, and an overhead workshop scene: a machining table, a lamp, a
  crate of components, and a small rear-facing colonist in orange with no readable face. A `1.6` corner badge.
- `Mod/About/ModIcon.png` — a stylised orange face, winking, with a ponytail, a clipboard and a gear.

Nothing sexual, nothing graphic, no nudity, no gore. The mod adds no art beyond one gizmo icon, no text
beyond its own interface strings, and touches no body, health or social system.

## Messages to post, one per mod

Written for the six mods this one reaches into. **Posted after the item is public** — a link to a private item
opens for nobody. One per recipient and personalised: the same text pasted three times is visible from orbit.
BBCode works in Steam comments, and pasting a bare Workshop URL makes a thumbnail, so the link to this mod
goes on its own line. Steam's comment limit is 1000 characters; each of these is well under it.

### Better Workbench Management (`falconne.BWM`)

> I have published a mod that puts a workbench type on autopilot: it raises a bill when there is work and takes
> it down when the stock is full. That cycle is brutal for your extended bill data, since you clear it on
> BillStack.Delete, so the mod reads it before every removal and puts it back on the bill it creates — the
> custom name, counting away from the home map, the extra product filter, and membership of a linked set,
> which it rejoins rather than starting a new group. It also applies your workbench restriction to bills it
> creates from a tick, which your own hook cannot do since it reads the selected bench, and it uses your wider
> counting rules to decide whether a stock is full, so the threshold and the bill's own display agree. Thank
> you for a mod that has been the reference for bill management for years.
>
> https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456

### Dubs Mint Menus (`dubwise.dubsmintmenus`)

> I have published a mod that keeps a workbench's bills up to date on its own, and it needed care around your
> bench templates. Making one photographs every bill on the bench, so a template taken from an autopiloted
> bench would have captured whatever the autopilot happened to have up at that moment, and re-applying it
> later would have turned those recipes into hand-placed bills for good — retiring the autopilot from them
> without a word. The mod now removes its own bills from a template as it is made, and says how many it left
> out. Applying a template needs nothing: the bills it places read as placed by hand, which is exactly right.
> Thank you for the menus, and for keeping them out of the tab's way.
>
> https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456

### Nice Bill Tab (`Andromeda.NiceBillTab`)

> I have published a mod that puts bills up and takes them down on its own, which is exactly the case your
> cached row list does not expect: a bill can disappear while your tab is open. So the mod sets your refresh
> flag every time it changes a stack. Without it your list would go on drawing a bill that no longer exists,
> and dragging the rows could put the deleted one back — which is the single most dangerous interaction I
> found while writing this. Thank you for a tab that is genuinely nicer than the vanilla one; the mod marks
> its own bills in their label rather than patching any tab, so yours picks the mark up for free.
>
> https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456

### Nice Bill Tab - Expansion (`HICON.NiceBillTabExpansion`)

> I have published a mod that takes charge of a workbench's bills, and your hidden recipes turned out to be
> exactly the right signal for it: hiding a recipe on a bench says you do not want it there, so the mod treats
> it as excluded and never raises a bill for it. Unhide it and it comes back on the next pass. It reads your
> store through IsHidden and nothing else, so the worst case if it ever moves is a lost feature rather than a
> broken game. Thank you for the expansion.
>
> https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456

### Everybody Gets One (`Memegoddess.EverybodyGetsOne`)

> I have published a mod that maintains bills on a workbench by itself, and yours is the reason it never tries
> to understand a repeat mode it does not own. A bill set to one of your modes keeps it across the mod's own
> remove-and-replace cycle, it can be chosen as the default for a whole bench, and when the mod needs to know
> whether there is work to do under one of them it asks your mode rather than comparing thresholds that mean
> nothing in your terms. The two counters are labelled neutrally under a foreign mode, since you read them
> your own way. Any mod that adds a repeat mode gets the same treatment; yours is what showed me it had to
> work that way. Thank you.
>
> https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456

### Choose Your Recipe (`zal.chooseyourrecipe`)

> I have published a mod that puts a workbench type on autopilot and takes every recipe it can do — which
> makes yours a natural fit, and pleasantly so: nothing had to be written. You remove disabled recipes from
> the workbench itself, so the autopilot simply never sees them and follows your choice without knowing it is
> doing so. That is the best kind of compatibility. Thank you for it.
>
> https://steamcommunity.com/sharedfiles/filedetails/?id=3806709456

Andreas Pardeike is thanked for Harmony in the description rather than by comment, and Claude Code (Anthropic)
is named there under `AI-GENERATED`.

## Steam release notes

They are written in a tab of the upload form that nothing asks for until the form is open, which makes them
the easiest thing to forget. Unlike the description they can be corrected afterwards and they start again at
every update — which is no reason to arrive without one, since a first upload with no notes leaves a page
silent about what it contains.

For 1.0.0, from `CHANGELOG.md`:

> First release.
>
> Put a workbench type on autopilot and it takes every recipe it can do: a bill goes up when there is work and
> comes down once the shelf is full, so the tab shows what is left to make rather than a wall of
> configuration. A recipe unlocked later arrives suspended, with a letter naming it — unsuspend to accept,
> delete to refuse for good.
>
> Per workbench type, with a default of "keep N in stock" or "always", a separate setting for recipes the game
> cannot count, and a per-recipe override that is also recorded when you adjust an automatic bill in the tab.
> A cap keeps room for bills of your own.
>
> Works with Better Workbench Management, Dubs Mint Menus, Nice Bill Tab and its Expansion, Everybody Gets One
> and Choose Your Recipe. None required. Interface in English and French, reachable with a pointer alone.
>
> Can be added to a game in progress and taken back out of one.

## Right after the upload, in this order

1. ~~Commit and push `Mod/About/PublishedFileId.txt`.~~ Committed (`13ac5ed`); pushing is what remains.
2. Subscribe to your own item and load it, as a subscriber sees it.
3. Switch the item to public by hand.
4. Post the six messages above. Their links already carry the item id.
5. ~~Write the Workshop id into `STATUS.md` under `workshop:`.~~ Done.
