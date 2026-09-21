# TESTING.md scenario 7. Never run before this suite.
#
# Adjusting a bill in the tab IS adjusting the profile. Without that capture, a target changed by
# hand would be lost the first time the stock filled and the bill came down - the mod would put the
# recipe back at its old number and look as though it had ignored the player.
#
# Only a running game shows it. The capture reads a real bill against the stamp taken when that
# bill was created, and the proof is a real down-and-up cycle: the number has to come back on a
# bill that did not exist when the change was made.
#
# The last scenario is the deliberate exception. "Do it N times" makes no sense as a standing
# order - the bill would be recreated the moment it finished, forever - so it is left exactly as
# the player set it and nothing is written.
Feature: a change made in the bills tab is kept as a profile override

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: a new target set in the tab is written to the profile
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to keep 200, restarting at 100
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot keeps 200 of "Make_Patchleather" on "HandTailoringBench", restarting at 100

  # The real proof. The bill that comes back is a different object from the one that was edited, so
  # a mod that only mutated the live bill would pass the scenario above and fail this one - which is
  # exactly the failure the capture exists to prevent.
  Scenario: the bill returns with the new target, not the old one
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to keep 200, restarting at 100
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 200 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    Given the Bill Autopilot test stockpile holds 50 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) keeps 200, restarting at 100

  Scenario: switching a bill to forever is recorded as always
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to repeat forever
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has "Make_Patchleather" set to "always" on "HandTailoringBench"
    Given the Bill Autopilot test stockpile holds 500 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  # The exception, and it is deliberate. Nothing must be recorded and the bill must be left as it
  # is: a standing order that says "make one" would be remade the moment it finished.
  Scenario: do it N times records nothing at all
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to do it 1 times
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no override for "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And no errors were logged
