# TESTING.md scenario 11.
#
# What another mod attaches to a bill - a name, a widened count, a link group - is held from the
# moment the autopilot takes that bill down to the moment it puts it back. The entry is keyed by
# the workbench itself, not by its type, and the difference was invisible until a second bench of
# the same kind existed: with one bench, both keys behave identically.
#
# A custom name is used here because vanilla renaming writes the same field Better Workbench
# Management writes, so this holds in the minimal pass, with no optional mod staged at all. What
# BWM adds on top of it - counting away from the home map, the extra product filter, the link
# group - is 13.
Feature: the memory is held per workbench, not per workbench type

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And a "HandTailoringBench" is built at (144, 155)
    And Bill Autopilot is on for "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (144, 155)

  # One name landing on both benches, or on the wrong one, is the old per-type key coming back. The
  # two names differ by more than a digit on purpose: a failure message quoting the wrong one should
  # be unmistakable.
  Scenario: each bench gets its own name back after a down-and-up cycle
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is named "north bench"
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (144, 155) is named "south bench"

    Given the Bill Autopilot test stockpile at (134, 150) holds 60 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (144, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (144, 155)
    And Bill Autopilot remembers the name "north bench" for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot remembers the name "south bench" for "Make_Patchleather" on the "HandTailoringBench" at (144, 155)

    Given the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (144, 155)
    Then Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is named "north bench"
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (144, 155) is named "south bench"
    And no errors were logged

  # A bench that no longer exists keeps no memory. thingIDNumber is never reused, so without the
  # pruning its entry would sit in the save for the rest of the game with nothing able to reach it.
  #
  # The wait is not padding: the pruning runs from the periodic refill, not from a sync, and the
  # refill is on the mod's own interval of 600 ticks. The runner drives ticks far faster than real
  # time, so this costs under a second.
  Scenario: deconstructing a bench drops its entry and leaves the other alone
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is named "north bench"
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (144, 155) is named "south bench"
    Given the Bill Autopilot test stockpile at (134, 150) holds 60 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (144, 155)
    And Bill Autopilot's memory is counted

    When I destroy the "HandTailoringBench" at (144, 155)
    And I wait 700 ticks
    Then Bill Autopilot is holding something for one bill fewer than before
    And Bill Autopilot remembers the name "north bench" for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
