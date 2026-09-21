# TESTING.md scenario 16.
#
# Making a bench template photographs every bill on the bench. Without the postfix, a template taken
# from an autopiloted bench would capture whatever the autopilot happened to have up at that moment,
# and re-applying it later would turn those recipes into hand-placed bills for good - retiring the
# autopilot from them without a word, and with nothing on screen to say so.
#
# Only a game can show it: the template is built by Dubs Mint Menus' own method against a real bill
# stack, and this feature calls that real method so the mod's postfix runs. Building a template by
# hand here would test nothing.
#
# The second scenario is the one that keeps the fix honest. Removing the autopilot's bills must not
# take the player's with them, and a postfix that emptied the list would pass the first scenario.
@requires:dubwise.dubsmintmenus
Feature: a bench template holds the player's bills and not the autopilot's

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is on for "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile at (134, 150) holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: the mod found it
    Then Bill Autopilot found Dubs Mint Menus

  Scenario: the autopilot's bills are kept out of the template
    Given Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When a Dubs Mint Menus bench template is made from the "HandTailoringBench" at (140, 155)
    Then the Dubs Mint Menus template holds no bill for "Make_Patchleather"

  Scenario: the player's own bills stay in it
    Given Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (140, 155)
    And a Dubs Mint Menus bench template is made from the "HandTailoringBench" at (140, 155)
    Then the Dubs Mint Menus template holds a bill for "Make_Apparel_Pants"
    And the Dubs Mint Menus template holds no bill for "Make_Patchleather"
    And no errors were logged
