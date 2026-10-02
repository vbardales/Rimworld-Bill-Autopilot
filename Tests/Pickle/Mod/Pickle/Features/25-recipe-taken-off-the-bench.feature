# A recipe that leaves a bench's list while its question is still open, and comes back.
#
# Choose Your Recipe removes a disabled recipe from the workbench's own list, and any mod may do the same. The
# autopilot then takes down the suspended bill it had put up (the recipe "has left the workbench"). What it used
# to leave behind was the memory that the question was open: put back, the recipe was "waiting for an answer" with
# no bill to answer, and no bench of that type ever started it again. The correction is that a bill taken down by
# the autopilot itself, rather than answered by the player, forgets the question, so the recipe is announced
# afresh when it returns.
#
# The list is edited directly (RecipeListSteps), the way that mod edits it, so this runs in every pass and asserts
# this mod's reaction rather than another mod's window. Not covered here, and left for a later version because it
# only verifies behaviour that was already right: a running bill taken down when its recipe leaves, a recipe
# disabled before the first pass, and a hand-placed bill on a recipe that leaves.
Feature: a recipe taken off the bench while its question is open is asked again when it returns

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "FueledStove" is built at (140, 155)
    And Bill Autopilot only takes "Make_Pemmican" on "FueledStove"
    And Bill Autopilot is switched on for "FueledStove"
    And Bill Autopilot syncs the "FueledStove" at (140, 155)

  Scenario: the announced recipe leaves and returns, and the question is asked again
    Given research "Pemmican" is finished
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is suspended
    And Bill Autopilot is still waiting for an answer about "Make_Pemmican" on "FueledStove"
    When the recipe "Make_Pemmican" is taken off the "FueledStove" the way Choose Your Recipe does
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Pemmican" on the "FueledStove" at (140, 155)
    And Bill Autopilot has never met "Make_Pemmican" on "FueledStove"
    When the recipe "Make_Pemmican" is put back on the "FueledStove"
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Pemmican" on the "FueledStove" at (140, 155)
    And Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is suspended
    And Bill Autopilot is still waiting for an answer about "Make_Pemmican" on "FueledStove"
    And no errors were logged
