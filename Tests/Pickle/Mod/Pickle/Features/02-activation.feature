# TESTING.md scenario 2.
#
# The one moment where a great deal of production can start in a single click, which is why the mod
# asks first. Only a running game can show it: the dialog is a real window, the count it announces
# is computed against the recipes really available on that bench right now, and "nothing changed"
# after a refusal is a claim about a real bill stack.
#
# The count is never spelled out in a scenario. How many recipes a workbench offers depends on the
# fixture's research and on which DLC are mounted, so the step rebuilds the whole sentence from the
# mod's own keys and its own intake calculation, and compares. What that really checks is the
# promise the dialog makes: the number on screen IS the number about to be taken.
Feature: switching a workbench type on asks first, and takes everything already unlocked

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And I close all dialogs

  Scenario: refusing the question changes nothing
    When Bill Autopilot's toggle is used to switch "HandTailoringBench" on
    Then Bill Autopilot asks before taking the recipes it would take on "HandTailoringBench"
    When the Bill Autopilot confirmation is refused
    Then Bill Autopilot is off for "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 0 bills up on the "HandTailoringBench" at (140, 155)

  # Found on a capture of the settings page (2026-09-24): the brewery read "1 recipes". The count now goes
  # through a noun phrase the translator owns, so the singular is a key, not a trailing "s".
  Scenario: a workbench type with a single recipe is counted in the singular
    Then Bill Autopilot's settings line for "Brewery" counts its recipes in the singular

  # Accepting absorbs everything already unlocked in silence. "I want all the recipes" means today's
  # ones, not forty suspended lines and a letter naming them; only what is unlocked LATER announces
  # itself, and that is scenario 04.
  Scenario: accepting takes the already-unlocked recipes at once, and without announcing them
    When Bill Autopilot's toggle is used to switch "HandTailoringBench" on
    And the Bill Autopilot confirmation is accepted
    Then Bill Autopilot is on for "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is running
    And Bill Autopilot has not announced "Make_Patchleather" on "HandTailoringBench"
    And no errors were logged

  # The question is about the opening intake, and that has already happened in this game. Asking it
  # again every time a player toggles a bench back on would train them to click through it, which is
  # the failure mode a confirmation dialog has.
  Scenario: switching it off and on again does not ask a second time
    When Bill Autopilot's toggle is used to switch "HandTailoringBench" on
    And the Bill Autopilot confirmation is accepted
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's toggle is used to switch "HandTailoringBench" off
    Then Bill Autopilot is off for "HandTailoringBench"
    When Bill Autopilot's toggle is used to switch "HandTailoringBench" on
    Then Bill Autopilot asks nothing
    And Bill Autopilot is switched on for "HandTailoringBench"

  # Switching off takes down what the autopilot put up and leaves everything else exactly where it
  # is. The hand-placed bill is the control: a DropAll that took the whole stack would look like a
  # working teardown on a bench carrying nothing else.
  Scenario: switching off takes down the automatic bills and leaves hand-placed ones alone
    Given Bill Autopilot is switched on for "HandTailoringBench"
    When I add bill "Make_Patchleather" to the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot left the hand-placed bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot's toggle is used to switch "HandTailoringBench" off
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 0 bills up on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot left the hand-placed bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
