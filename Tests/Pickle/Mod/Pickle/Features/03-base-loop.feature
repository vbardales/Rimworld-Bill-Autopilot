# TESTING.md scenario 3.
#
# The arithmetic of the two thresholds is proven out of game, in Tests/BillAutopilot.Tests.csproj.
# What is NOT provable there, and what this feature is for, is that the decision reaches the bench:
# the count comes from a probe bill dressed like the real one and read through the game's own
# RecipeWorkerCounter, and that machinery has no meaning outside a running map.
#
# The gap between the two thresholds is the point. A bill goes up at or below the restart number
# and comes down at the target; between them nothing happens, and that "nothing" is what stops the
# tab flickering a row in and out on every unit made. A scenario that only checked the two ends
# would pass just as well on a mod that had collapsed both onto one number.
Feature: the base loop, and the gap that stops it flickering

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25

  Scenario: the stock this scenario sets is the stock the autopilot reads
    Given the Bill Autopilot test stockpile holds 30 "Leather_Patch"
    Then Bill Autopilot counts 30 of "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  Scenario: a bill goes up at or below the restart number
    Given the Bill Autopilot test stockpile holds 20 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) keeps 50, restarting at 25

  Scenario: no bill between the restart number and the target
    Given the Bill Autopilot test stockpile holds 30 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  Scenario: the bill comes down once the stock reaches the target
    Given the Bill Autopilot test stockpile holds 20 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 50 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  # The failure this guards against: both thresholds collapsed onto one number. The stock is walked
  # down from the target into the band and synced at every step, which is what production past the
  # target looks like from the bench's point of view. A mod with one threshold puts the bill back at
  # 49 and the tab flickers a row on every unit made.
  Scenario: the bill does not come back while the stock is still inside the band
    Given the Bill Autopilot test stockpile holds 50 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 49 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 26 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 25 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And no errors were logged
