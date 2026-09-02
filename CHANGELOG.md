# Changelog

## 0.1.0 — unreleased

First version.

- Autopilot per workbench type: the mod puts a bill up when there is something to do and takes it
  down once the target is reached.
- *Keep a stock* and *Always* modes, with a target and a restart threshold.
- A separate setting for recipes with an uncountable product (butchering, smelting, cremation,
  surgery), which the game cannot count.
- Per-recipe override, also written when you adjust an automatic bill in the tab.
- Deleting an automatic bill excludes its recipe from the autopilot.
- Recipe unlocked later: suspended bill plus a summary letter.
- A cap on automatic bills per bench, to leave room under the game's limit of 15.
- Interface in English and French, fully pointer-driven (Steam Deck).
- Nothing is written to the save as a class belonging to the mod: removing it from an ongoing game
  produces no load error.
- Better Workbench Management: what it attaches to a bill survives the remove/replace cycle — name,
  off-map counting, additional products, membership of a linked bill group. Its workbench
  restriction applies to autopilot bills, its counting rules feed the thresholds, and its bill cap
  replaces the game's 15.
- Nice Bill Tab - Expansion: a recipe hidden on a bench is treated as excluded.
- Choose Your Recipe: already respected with no work, since it removes disabled recipes from the
  bench.
