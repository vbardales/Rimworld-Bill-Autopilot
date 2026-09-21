# TESTING.md scenario 10.
#
# RimWorld accepts fifteen bills on a bench and hides the Add button past that, so an autopilot
# free to fill the stack would quietly take the player's ability to add a bill of their own. The
# cap exists for that, and it is counted against the live ceiling rather than against the number
# fifteen: Better Workbench Management reports a higher one when No Max Bills is present.
#
# Only a game can show this. The budget is computed against a real bill stack holding both kinds of
# bill, and how many recipes a bench really offers depends on what is loaded and researched.
Feature: the cap on automatic bills, and the room it leaves

  # The bench is switched on LAST, in each scenario rather than here, and that ordering is the whole
  # correction this feature needed. The game goes on ticking between two steps of a scenario, and the
  # autopilot's own pass runs on those ticks: switched on first, it had already filled its default
  # allowance of eight before the cap was lowered, and then - correctly, as the third scenario below
  # asserts - it did not take those bills down again. The run read that as a broken cap.
  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)

  # "Always" means every available recipe wants a bill, so the only thing that can stop the stack
  # filling is the cap. The guard above it is not a claim about the mod: it says this bench really
  # does offer more recipes than the cap, so that a fixture with fewer would fail by saying so
  # rather than passing a cap it never reached.
  Scenario: at most as many automatic bills as the cap allows
    Given Bill Autopilot allows 2 automatic bills per bench
    And Bill Autopilot default mode for "HandTailoringBench" is "always"
    And Bill Autopilot is switched on for "HandTailoringBench"
    Then Bill Autopilot has more than 2 recipes to take on "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 2 bills up on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 2 bills up on the "HandTailoringBench" at (140, 155)

  # The invariant that matters to a player: the Add button never disappears. Asserted against the
  # live ceiling, so a pass staging No Max Bills checks it against that mod's higher number.
  Scenario: room is always left for a bill of the player's own
    Given Bill Autopilot allows 8 automatic bills per bench
    And Bill Autopilot default mode for "HandTailoringBench" is "always"
    And Bill Autopilot is switched on for "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot leaves room for another bill on the "HandTailoringBench" at (140, 155)

  # The cap applies to creation, not to what is already standing: lowering it must not yank bills
  # out from under work in progress. Documented behaviour, easy to get wrong in the other direction,
  # and invisible without a second sync after the change.
  Scenario: lowering the cap does not take down bills already up
    Given Bill Autopilot allows 3 automatic bills per bench
    And Bill Autopilot default mode for "HandTailoringBench" is "always"
    And Bill Autopilot is switched on for "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 3 bills up on the "HandTailoringBench" at (140, 155)
    Given Bill Autopilot allows 1 automatic bills per bench
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 3 bills up on the "HandTailoringBench" at (140, 155)
    And no errors were logged
