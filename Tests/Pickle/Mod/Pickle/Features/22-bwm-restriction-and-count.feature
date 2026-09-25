# TESTING.md scenario 13, the two details that stayed manual because they were written before an interface could be
# read: the workbench restriction the mod applies to a bill it creates, and the agreement between the widened count and
# what the bill reports. 13-better-workbenches.feature covers the down-and-up cycle; these are the rest of the bridge.
#
# Both read Better Workbench Management's own objects, not the mod's copy of them (see BwmDetailSteps).
#
# Each scenario has to START from a bench with no bill, for the same reason as 14 and 15: the Background sets no stock,
# so the autopilot's own tick puts the bill up before the scenario has set anything. The stock is raised above the
# target and the bench synced first, so that whatever the tick put up comes down.
@requires:falconne.BWM
Feature: what Better Workbench Management adds to a bench and to a count reaches the bills the autopilot creates

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25

  # BWM's hook applies a bench's restriction from the SELECTED bench, which reads nothing when a tick creates the bill.
  Scenario: a bill created from a tick carries the restriction set on its bench
    Given the Bill Autopilot test stockpile holds 60 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Better Workbench Management restricts the "HandTailoringBench" at (140, 155) to non-mechs with a skill range of 5 to 15
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) carries the restriction of the bench: non-mechs with a skill range of 5 to 15
    And no errors were logged

  # The probe bill has no extended data and BWM's postfix does nothing for such a bill, so the mod grafts the reading of the
  # real bill on before measuring. Without it the threshold that fires and the bill's own figure drift apart.
  Scenario: the count the autopilot decides with is the count the bill reports
    Given the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Better Workbench Management also counts "Leather_Plain" toward Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And the Bill Autopilot test stockpile holds 30 "Leather_Plain"
    Then Bill Autopilot counts the same as the bill does for "Make_Patchleather" on the "HandTailoringBench" at (140, 155), and at least 40
    And no errors were logged
