# Changelog

Format inspired by [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
This file serves the repository and the writing of Steam patch notes; RimWorld does not display it in game.

## [1.0.0] - unreleased

### Added

- Autopilot per workbench type, so a bench built later is already configured. The mod puts a bill up
  when there is something to do and takes it down once the target is reached, keeping the tab short.
- *Keep a stock* and *Always* modes, with a target and a restart threshold. The gap between the two
  stops a bill from flickering on every unit produced.
- A separate setting for recipes with an uncountable product (butchering, smelting, cremation,
  surgery) for which the game cannot count and "keep a stock" is impossible.
- Per-recipe override, also recorded when you adjust an automatic bill directly in the tab.
- Deleting an automatic bill excludes its recipe from the autopilot, so the gesture means something.
- A recipe unlocked after a bench went on autopilot arrives as a suspended bill, with a letter
  naming it. Nothing is ever spent without your say-so.
- Switching a workbench type on for the first time asks for confirmation, naming how many recipes
  the autopilot is about to take and at what target. Switching it back on later does not ask again.
- A cap on how many automatic bills may stand on one bench, leaving room for bills of your own.
- Recipe lists grouped by product category, each group collapsible, with a collapse-all button and a
  filter for overridden recipes. No search field: it would summon the virtual keyboard on a
  Steam Deck.
- Interface in English and French, every control reachable with a pointer alone.
- Nothing is written to the save as a class belonging to the mod, so removing it from an ongoing
  game produces no load error.
- A startup line in the log naming the companion mods found and the bill cap in force; the settings
  screen names them too.

### Works with

Detected on its own, none required. Every call into a neighbour is wrapped: the worst case is a lost
feature, never a broken game.

- **Better Workbench Management**: what it attaches to a bill survives the remove/replace cycle
  (name, off-map counting, additional products, and membership of a linked bill group, which is
  rejoined rather than lost). Its workbench restriction is applied to autopilot bills, which its own
  hook cannot do. Its counting rules feed the thresholds, so they agree with what the bill displays.
  Its raised bill ceiling replaces the game's 15 when No Max Bills is present.
- **Dubs Mint Menus**: autopilot bills are kept out of a bench template, which would otherwise
  capture the autopilot's passing queue and, once re-applied, retire the autopilot from those
  recipes for good.
- **Everybody Gets One**: its repeat modes (one per person, X per person, with surplus) survive the
  remove/replace cycle, and it is asked whether there is work rather than having its thresholds
  guessed at. The same holds for any other mod that adds a repeat mode.
- A repeat mode from another mod can be chosen as a profile default or a per-recipe override, so a
  whole bench can be set to "one per colonist" at once. Under such a mode the two counters are
  labelled neutrally, since the owning mod reads them its own way.
- **Nice Bill Tab**: its cached bill list is told to rebuild whenever the autopilot puts a bill up or
  takes one down, without which it would keep drawing bills that no longer exist and its
  drag-and-drop would reinsert deleted ones.
- **Nice Bill Tab - Expansion**: a recipe hidden on a bench is treated as excluded.
- **Choose Your Recipe**: respected with no work needed, since it removes disabled recipes from the
  bench itself.
