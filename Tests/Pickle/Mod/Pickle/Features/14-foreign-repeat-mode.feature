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
    # One recipe only, as in feature 13. The bench takes every recipe it has otherwise, and the bill cap (8 by
    # default) is reached by the first hats, so the recipe under test never got a slot (2026-09-25).
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"

  # The mode names are the ones the game LOADS from Everybody Gets One - Continued under 1.6: TD_PersonCount,
  # TD_XPerPerson, TD_WithSurplusIng (the run's own report lists them). Its root Defs folder declares another set,
  # TD_ColonistCount and TD_XPerColonist, and a change of names to that set was tried on 2026-09-25 and failed with
  # "no BillRepeatModeDef named 'TD_ColonistCount'": the mod's LoadFolders decides which folder is read.
  #
  # This scenario has to START from a bench with no bill, and that was the real cause of its first two reds: the
  # Background switches the bench on with nothing in stock, so the autopilot's own tick put the bill up in
  # TargetCount before this step set the bench default, and a sync never rewrites the mode of a bill that stands.
  # The stock is raised above the target first and the bench synced, so that whatever the tick put up comes down;
  # the mode is set on an empty bench, and the stock is lowered to bring a NEW bill up under it.
  @requires:Memegoddess.EverybodyGetsOne
  Scenario: the bench default reaches the bill
    Given the Bill Autopilot test stockpile holds 60 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot default mode for "HandTailoringBench" is the repeat mode "TD_PersonCount"
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
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
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to the repeat mode "TD_XPerPerson"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has "Make_Patchleather" set to "another mod" on "HandTailoringBench"

    Given the Bill Autopilot test stockpile holds 500 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 0 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) has the repeat mode "TD_XPerPerson"

  # The provider removed, which is what a player gets by disabling the mod and loading the save.
  # The profile still holds the name, the lookup comes back empty, and the mod must fall back to one
  # of its own modes rather than put up a bill carrying no mode at all.
  Scenario: a mode whose owner is gone falls back to keeping a stock
    Given Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And Bill Autopilot default mode for "HandTailoringBench" is a repeat mode no longer in this game
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) keeps 50, restarting at 25
    And no errors were logged
