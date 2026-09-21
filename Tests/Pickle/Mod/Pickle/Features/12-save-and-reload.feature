# TESTING.md scenario 12, the half a single session can play.
#
# The design decision under test: the state is grafted into the save's <game> node by a postfix on
# ExposeSmallComponents, rather than living in a GameComponent. A component is written with a Class
# attribute, and removing the mod would then make every load of that save fail on "Can't load
# abstract class Verse.GameComponent". Named nodes are read by nobody once the mod is gone.
#
# This is the one decision that CANNOT be checked out of game: the round trip goes through
# RimWorld's own Scribe, on a real game, and the state it carries - which bills are the
# autopilot's, which recipes were refused, which were already met - is what decides whether a
# reloaded colony behaves as the player left it or announces everything again.
#
# THE OTHER HALF STAYS OUTSIDE THIS SUITE, and it is the more important one: loading the same save
# with the mod REMOVED. A Pickle run cannot do it, because the mod list is fixed when the game
# starts and removing this mod would remove the suite with it. It belongs to a pass of its own -
# see Tests/Pickle/README.md - and it is the first thing to report if it ever fails.
Feature: everything the autopilot knows survives a save and a reload

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  # Ownership is the load-bearing part. A bill whose stamp did not survive stops being the
  # autopilot's: it would never come down on its own again, it would lose its mark, and deleting it
  # would no longer refuse its recipe.
  Scenario: the autopilot still owns its own bills
    Given Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When I save and reload
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot marks its bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot keeps 50 of "Make_Patchleather" on "HandTailoringBench", restarting at 25

  Scenario: a refusal survives, and the recipe is not offered again
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is deleted
    And I save and reload
    Then Bill Autopilot has "Make_Patchleather" set to "never" on "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  # What has already been met must stay met. If that set were lost, every recipe on the bench would
  # read as new on the next pass: forty suspended bills and a letter naming them, on a colony that
  # had been running for years.
  Scenario: nothing already met is announced a second time
    Given Bill Autopilot has already met "Make_Patchleather" on "HandTailoringBench"
    When I save and reload
    Then Bill Autopilot has already met "Make_Patchleather" on "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And I wait 120 ticks
    Then Bill Autopilot has not announced "Make_Patchleather" on "HandTailoringBench"

  # The whole graft in one step: Pickle's own round trip fails if anything hits the error log during
  # the save and the load. A Class attribute written where none belongs, or a node the loader cannot
  # read back, surfaces here whatever it was.
  Scenario: the round trip itself logs nothing
    Then the save round trips
    And no errors were logged
