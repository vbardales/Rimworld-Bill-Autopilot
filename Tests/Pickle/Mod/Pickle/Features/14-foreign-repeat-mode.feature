# TESTING.md scenario 14.
#
# A repeat mode the autopilot does not understand is SET, KEPT, and ASKED rather than guessed at.
# Everybody Gets One is the test case; any mod adding a repeat mode should behave the same, which is
# why nothing below names that mod except the tags and the mode defNames.
#
# The arithmetic is proven out of game. What is not, and what this feature is for: a foreign mode
# reaching a real bill, and surviving a real down-and-up cycle rather than being flattened to one of
# the autopilot's own.
#
# The tags are on the scenarios, not on the feature. The last one needs no mod at all - it stands
# for the case where the provider has been removed - so tagging the whole file would have skipped
# it in the very pass where it is cheapest to run.
Feature: a repeat mode belonging to another mod is set, kept and asked

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"

  @requires:Memegoddess.EverybodyGetsOne
  Scenario: the bench default reaches the bill
    Given Bill Autopilot default mode for "HandTailoringBench" is the repeat mode "TD_PersonCount"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) has the repeat mode "TD_PersonCount"
    And no errors were logged

  # A mode set by hand on an automatic bill is a choice nothing else would catch. It is recorded as
  # an override and the bill returns with it, rather than being flattened. The cycle matters: the
  # returning bill is a different object, so a mod that only left the live bill alone would pass the
  # first half and fail the second.
  @requires:Memegoddess.EverybodyGetsOne
  Scenario: a mode set by hand in the tab is recorded and comes back
    Given Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to the repeat mode "TD_XPerPerson"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has "Make_Patchleather" set to "another mod" on "HandTailoringBench"

    Given the Bill Autopilot test stockpile at (134, 150) holds 500 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile at (134, 150) holds 0 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) has the repeat mode "TD_XPerPerson"

  # The provider removed, which is what a player gets by disabling the mod and loading the save.
  # The profile still holds the name, the lookup comes back empty, and the mod must fall back to one
  # of its own modes rather than put up a bill carrying no mode at all.
  Scenario: a mode whose owner is gone falls back to keeping a stock
    Given Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And Bill Autopilot default mode for "HandTailoringBench" is a repeat mode no longer in this game
    And the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) keeps 50, restarting at 25
    And no errors were logged
