# TESTING.md scenario 9.
#
# "Keep 50 in stock" is impossible for butchering, smelting or cremation: the game cannot count
# what they produce, so the recipe gets a setting of its own. The arithmetic of that fallback is
# proven out of game; what is NOT, and what this feature is for, is the countability test itself.
# It runs through RimWorld's RecipeWorkerCounter on a probe bill attached to a real workbench on a
# real map, and there is no such thing outside a running game.
#
# The butcher spot is the right bench for it because it carries one recipe of each kind: butchering
# cannot be counted, kibble can. A setting that hit both would look correct on a bench that only
# had uncountable recipes.
Feature: recipes the game cannot count have a setting of their own

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "ButcherSpot" is built at (146, 155)
    And Bill Autopilot is on for "ButcherSpot"

  # The guard. If the game ever starts counting butchery products, everything below stops meaning
  # what it says, and this is where that should be noticed.
  Scenario: the game cannot count butchering, and can count kibble
    Then Bill Autopilot cannot count "ButcherCorpseFlesh"
    And Bill Autopilot can count "Make_Kibble"

  # The documented default. A butcher spot on autopilot produces nothing at all, and that is
  # correct: "keep a stock" has no meaning here, so the mod refuses to guess rather than standing up
  # a bill that would butcher every corpse on the map forever.
  Scenario: with the default, an uncountable recipe gets no bill
    Then Bill Autopilot would run "ButcherCorpseFlesh" on "ButcherSpot" as "never"
    When Bill Autopilot syncs the "ButcherSpot" at (146, 155)
    Then Bill Autopilot has no bill up for "ButcherCorpseFlesh" on the "ButcherSpot" at (146, 155)

  Scenario: setting them to always puts up a standing bill
    Given Bill Autopilot uncountable recipes on "ButcherSpot" are "always"
    Then Bill Autopilot would run "ButcherCorpseFlesh" on "ButcherSpot" as "always"
    When Bill Autopilot syncs the "ButcherSpot" at (146, 155)
    Then Bill Autopilot has a bill up for "ButcherCorpseFlesh" on the "ButcherSpot" at (146, 155)
    And Bill Autopilot's bill for "ButcherCorpseFlesh" on the "ButcherSpot" at (146, 155) has the repeat mode "Forever"

  # The setting reaches the uncountable recipe and nothing else. Kibble stays on the bench default,
  # under its own stock threshold, whichever way the uncountable setting is turned.
  Scenario: the countable recipe on the same bench is left on the bench default
    Given Bill Autopilot uncountable recipes on "ButcherSpot" are "always"
    Then Bill Autopilot would run "Make_Kibble" on "ButcherSpot" as "keep in stock"
    Given Bill Autopilot uncountable recipes on "ButcherSpot" are "never"
    Then Bill Autopilot would run "Make_Kibble" on "ButcherSpot" as "keep in stock"
    And no errors were logged
