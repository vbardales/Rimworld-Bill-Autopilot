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
# The bench and the research are chosen against the fixture, not out of the air. The first run of
# this suite failed every scenario here on "Make_Apparel_Pants is already known" - the guard working
# exactly as intended, because test-colony has ComplexClothing researched. Its save file lists
# `Pemmican` at zero progress, and that project gates Make_Pemmican on a stove, whose product is a
# countable resource made from no stuff.
#
# The guard in the first step stays. The day the fixture is rebuilt with pemmican researched, this
# says so instead of passing on a recipe that was never new.
Feature: a recipe unlocked by research arrives suspended, and is answered once for the type

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "FueledStove" is built at (140, 155)
    And Bill Autopilot only takes "Make_Pemmican" on "FueledStove"
    And Bill Autopilot is switched on for "FueledStove"
    And Bill Autopilot syncs the "FueledStove" at (140, 155)

  Scenario: it arrives suspended, and a letter names it
    Then Bill Autopilot has never met "Make_Pemmican" on "FueledStove"
    When research "Pemmican" is finished
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Pemmican" on the "FueledStove" at (140, 155)
    And Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is suspended
    And Bill Autopilot is still waiting for an answer about "Make_Pemmican" on "FueledStove"
    When I wait 120 ticks
    Then Bill Autopilot has announced "Make_Pemmican" on "FueledStove"

  Scenario: unsuspending it is the acceptance
    Given research "Pemmican" is finished
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    When Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is unsuspended
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot is no longer waiting for an answer about "Make_Pemmican" on "FueledStove"
    And Bill Autopilot has a bill up for "Make_Pemmican" on the "FueledStove" at (140, 155)
    And Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is running

  # Deleting the suspended bill is the refusal, and it has to stick: the recipe is set to never on
  # that workbench type and no later pass may offer it again.
  Scenario: deleting it refuses the recipe for good
    Given research "Pemmican" is finished
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    When Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is deleted
    Then Bill Autopilot has "Make_Pemmican" set to "never" on "FueledStove"
    When Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Pemmican" on the "FueledStove" at (140, 155)
    When Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Pemmican" on the "FueledStove" at (140, 155)

  # The question is asked once for the workbench TYPE. The second bench is what makes the rule
  # visible: with one bench, an answered and an unanswered question look exactly the same.
  Scenario: a second bench of the same kind does not start it while the question stands
    Given a "FueledStove" is built at (144, 155)
    And research "Pemmican" is finished
    When Bill Autopilot syncs the "FueledStove" at (140, 155)
    Then Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is suspended
    When Bill Autopilot syncs the "FueledStove" at (144, 155)
    Then Bill Autopilot has no bill up for "Make_Pemmican" on the "FueledStove" at (144, 155)
    When Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (140, 155) is unsuspended
    And Bill Autopilot syncs the "FueledStove" at (140, 155)
    And Bill Autopilot syncs the "FueledStove" at (144, 155)
    Then Bill Autopilot has a bill up for "Make_Pemmican" on the "FueledStove" at (144, 155)
    And no errors were logged
