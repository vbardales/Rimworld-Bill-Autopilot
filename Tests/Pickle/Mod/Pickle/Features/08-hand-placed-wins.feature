# TESTING.md scenario 8. The rule that keeps the mod out of the player's way.
#
# A bill placed by hand always wins: the autopilot stands back from that recipe entirely, and takes
# it again only once the player's bill is gone. Both bills are for the same recipe on the same
# bench, which is the only arrangement in which the rule is visible at all.
Feature: a bill placed by hand always wins

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: the autopilot steps aside, and comes back when the player's bill goes
    Given Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When I add bill "Make_Patchleather" to the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot left the hand-placed bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    # Standing back is not refusing: the profile must be untouched, or the recipe would be retired
    # by a gesture that said nothing of the kind.
    And Bill Autopilot has no override for "Make_Patchleather" on "HandTailoringBench"

    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    When the hand-placed bill for "Make_Patchleather" on the Bill Autopilot bench "HandTailoringBench" at (140, 155) is deleted
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And no errors were logged
