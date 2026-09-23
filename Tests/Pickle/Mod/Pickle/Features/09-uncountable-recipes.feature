# TESTING.md scenario 9.
#
# "Keep 50 in stock" is impossible for a recipe whose product the game cannot count - smelting a
# weapon, cremating a corpse, an operation - so such a recipe gets a setting of its own. The arithmetic
# of that fallback is proven out of game; what is NOT, and what this feature is for, is the
# countability test itself. It runs through RimWorld's RecipeWorkerCounter on a probe bill attached to
# a real workbench on a real map, and there is no such thing outside a running game.
#
# BUTCHERING IS NOT IN THAT LIST, and this feature used to say it was. The first full run of this suite
# (2026-09-23) failed three scenarios because RecipeWorkerCounter_ButcherAnimals.CanCountProducts
# returns true: the game counts raw meat for the butchery bill. The mod asks the game, so it was right
# and the scenario, TESTING.md and the mod's own description were wrong. The electric smelter is the
# bench that really carries one recipe of each kind: SmeltWeapon has specialProducts and no products,
# so it cannot be counted; ExtractMetalFromSlag makes fifteen steel, so it can. A setting that hit both
# would look correct on a bench that only had uncountable recipes.
Feature: recipes the game cannot count have a setting of their own

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "ElectricSmelter" is built at (146, 155)
    And Bill Autopilot is switched on for "ElectricSmelter"

  # The guard. If the game ever changes which of these it counts, everything below stops meaning what it
  # says, and this is where that should be noticed. The butchery line is the one that moved the
  # documentation: it is countable, and a scenario saying otherwise would be wrong again.
  Scenario: the game cannot count a smelted weapon, and can count slag steel and butchering
    Then Bill Autopilot cannot count "SmeltWeapon"
    And Bill Autopilot can count "ExtractMetalFromSlag"
    And Bill Autopilot can count "ButcherCorpseFlesh"

  # The documented default. A smelter on autopilot puts up nothing for an uncountable recipe, and that
  # is correct: "keep a stock" has no meaning here, so the mod refuses to guess rather than standing up
  # a bill that would smelt every weapon on the map forever.
  Scenario: with the default, an uncountable recipe gets no bill
    Then Bill Autopilot would run "SmeltWeapon" on "ElectricSmelter" as "never"
    When Bill Autopilot syncs the "ElectricSmelter" at (146, 155)
    Then Bill Autopilot has no bill up for "SmeltWeapon" on the "ElectricSmelter" at (146, 155)

  Scenario: setting them to always puts up a standing bill
    Given Bill Autopilot uncountable recipes on "ElectricSmelter" are "always"
    Then Bill Autopilot would run "SmeltWeapon" on "ElectricSmelter" as "always"
    When Bill Autopilot syncs the "ElectricSmelter" at (146, 155)
    Then Bill Autopilot has a bill up for "SmeltWeapon" on the "ElectricSmelter" at (146, 155)
    And Bill Autopilot's bill for "SmeltWeapon" on the "ElectricSmelter" at (146, 155) has the repeat mode "Forever"

  # The setting reaches the uncountable recipe and nothing else. Slag steel stays on the bench default,
  # under its own stock threshold, whichever way the uncountable setting is turned.
  Scenario: the countable recipe on the same bench is left on the bench default
    Given Bill Autopilot uncountable recipes on "ElectricSmelter" are "always"
    Then Bill Autopilot would run "ExtractMetalFromSlag" on "ElectricSmelter" as "keep in stock"
    Given Bill Autopilot uncountable recipes on "ElectricSmelter" are "never"
    Then Bill Autopilot would run "ExtractMetalFromSlag" on "ElectricSmelter" as "keep in stock"
    And no errors were logged
