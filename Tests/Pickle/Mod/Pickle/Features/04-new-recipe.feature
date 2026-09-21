# TESTING.md scenario 4. The point of the mod.
#
# A recipe unlocked after the bench was switched on must arrive SUSPENDED and announce itself:
# nothing is ever spent behind the player's back. Unsuspending accepts it, deleting refuses it for
# good. None of that is reachable outside a game - it needs a real research manager, the patch on
# FinishProject, a real letter stack and a real bill.
#
# The last scenario is the one no single bench can show. While a recipe is announced and
# unanswered, NO bench of that type may act on it: without that rule the first bench would show the
# suspended bill while a second quietly started producing, and the question would have been
# answered by the mod rather than by the player.
#
# If the fixture ever ships with ComplexClothing already researched, the guard in the first step
# fails and says so, rather than this feature passing on a recipe that was never new.
Feature: a recipe unlocked by research arrives suspended, and is answered once for the type

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: it arrives suspended, and a letter names it
    Then Bill Autopilot has never met "Make_Apparel_Pants" on "HandTailoringBench"
    When research "ComplexClothing" is finished
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155) is suspended
    And Bill Autopilot is still waiting for an answer about "Make_Apparel_Pants" on "HandTailoringBench"
    When I wait 120 ticks
    Then Bill Autopilot has announced "Make_Apparel_Pants" on "HandTailoringBench"

  Scenario: unsuspending it is the acceptance
    Given research "ComplexClothing" is finished
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    When Bill Autopilot's bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155) is unsuspended
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot is no longer waiting for an answer about "Make_Apparel_Pants" on "HandTailoringBench"
    And Bill Autopilot has a bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot's bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155) is running

  # Deleting the suspended bill is the refusal, and it has to stick: the recipe is set to never on
  # that workbench type and no later pass may offer it again.
  Scenario: deleting it refuses the recipe for good
    Given research "ComplexClothing" is finished
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    When Bill Autopilot's bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155) is deleted
    Then Bill Autopilot has "Make_Apparel_Pants" set to "never" on "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)

  # The question is asked once for the workbench TYPE. The second bench is what makes the rule
  # visible: with one bench, an answered and an unanswered question look exactly the same.
  Scenario: a second bench of the same kind does not start it while the question stands
    Given a "HandTailoringBench" is built at (144, 155)
    And research "ComplexClothing" is finished
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot's bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155) is suspended
    When Bill Autopilot syncs the "HandTailoringBench" at (144, 155)
    Then Bill Autopilot has no bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (144, 155)
    When Bill Autopilot's bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155) is unsuspended
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot syncs the "HandTailoringBench" at (144, 155)
    Then Bill Autopilot has a bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (144, 155)
    And no errors were logged
