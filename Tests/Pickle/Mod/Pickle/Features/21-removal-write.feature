# TESTING.md scenario 12, the half no other feature can play: a game saved WITH the mod, loaded WITHOUT it.
#
# The state of the mod lives in plain named nodes grafted into the save's game node, not in a GameComponent, so that
# removing the mod raises no load error. The mod list is fixed when the game starts, so this needs two launches under
# one hold of the lock (`-Then`), the second with the mod taken out of the list (`-ThenWithout`):
#
#   -Filter '21-removal-write' -Then 'removal-check' -ThenWithout 'nelim.billautopilot','nelim.billautopilot.pickletests'
#
# This feature is the first launch: it puts state in the game, saves it, checks the save holds that state as plain
# nodes with no class of the mod in it, and hands the file to the companion mod `nelim.billautopilot.pickleremoval`
# (Tests/Pickle/Removal/Mod), which does not depend on Bill Autopilot. The second launch is that mod's
# `removal-check.feature`. Pattern from Housebroken's TF-18; that chain had not been seen running end to end when this
# was written.
#
# Played only by the pass wsl-deps.removal.map: without the companion the tag makes it a skip.
@requires:nelim.billautopilot.pickleremoval
Feature: a game saved with Bill Autopilot, handed over to a launch without it

  Scenario: state written, save checked, hand over
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot saves the game as "billautopilot-with-mod"
    Then Bill Autopilot save "billautopilot-with-mod" keeps its state as plain nodes, with no class of the mod in it
    When Bill Autopilot hands the saved game "billautopilot-with-mod" to the mod "nelim.billautopilot.pickleremoval"
    Then no errors were logged
