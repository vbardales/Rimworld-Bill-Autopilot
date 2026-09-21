# TESTING.md scenario 19.
#
# Hiding a recipe on a bench says you do not want it there, so the autopilot treats it as refused
# and leaves it alone. Unhiding it must bring it back: a recipe that stays excluded after being
# restored is a stale-state failure, which is a different defect from one that was never excluded
# at all, and only a pair of scenarios can tell them apart.
#
# The hidden-recipe store belongs to Nice Bill Tab - Expansion, which loads on top of Nice Bill Tab,
# so this needs both. Choose Your Recipe is NOT covered here: it removes disabled recipes from the
# workbench itself, before the autopilot ever sees them, so there is nothing of this mod's to
# assert - the mod's own README says as much. Verifying that claim still means loading it and
# looking, which Tests/Pickle/README.md keeps on the manual list rather than faking here.
@requires:HICON.NiceBillTabExpansion
Feature: a recipe hidden by another mod is one the autopilot leaves alone

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"

  Scenario: the mod found the hidden-recipe store
    Then Bill Autopilot found the hidden-recipe store

  # Written as a pair on purpose. The first half alone would also pass on a mod that never took the
  # recipe at all, for any reason; the second is what says the exclusion was the hiding.
  Scenario: hiding a recipe takes its bill down, and unhiding brings it back
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    When "Make_Patchleather" is hidden on the Bill Autopilot bench "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    # Hiding is not refusing: the profile must be untouched, or unhiding would bring back a recipe
    # the autopilot now believes was set to never.
    And Bill Autopilot has no override for "Make_Patchleather" on "HandTailoringBench"

    When "Make_Patchleather" is unhidden on the Bill Autopilot bench "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And no errors were logged
