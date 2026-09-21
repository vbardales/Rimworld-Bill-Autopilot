# TESTING.md scenario 5.
#
# Deleting an automatic bill is what refuses its recipe. That gesture is the whole reason the mod
# marks its bills in their label - without the mark, a player deleting what looks like an ordinary
# bill would silently retire a recipe.
#
# The hook hangs off Building_WorkTable.Notify_BillDeleted, which nothing outside a running game
# raises: BillStack.Delete is the call the tab's X button makes, and it is the call these scenarios
# make. The mod's own removals pass through the same method, which is why a flag exists to tell
# them apart - the last scenario here is what proves that flag still works.
Feature: deleting an automatic bill refuses its recipe, and only an automatic one

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: deleting a running automatic bill sets its recipe to never
    Given Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is deleted
    Then Bill Autopilot has "Make_Patchleather" set to "never" on "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  # An explicit refusal leaves nothing to restore. What the bill carried - a name, a widened count,
  # a link - is held only for the down-and-up cycle the autopilot itself performs.
  Scenario: an explicit refusal drops what the bill carried
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is renamed "hand cut"
    And Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is deleted
    Then Bill Autopilot remembers nothing for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  Scenario: setting the recipe back to default brings it back on the next pass
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is deleted
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot's rule for "Make_Patchleather" on "HandTailoringBench" is set back to the default
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has no override for "Make_Patchleather" on "HandTailoringBench"

  # Deleting a bill the player placed must do none of this. Both bills are for the same recipe on
  # the same bench on purpose: that is the case where a hook that did not check ownership would
  # retire the recipe on a gesture that meant nothing of the kind.
  Scenario: deleting a hand-placed bill refuses nothing
    When I add bill "Make_Patchleather" to the "HandTailoringBench" at (140, 155)
    And the hand-placed bill for "Make_Patchleather" on the Bill Autopilot bench "HandTailoringBench" at (140, 155) is deleted
    Then Bill Autopilot has no override for "Make_Patchleather" on "HandTailoringBench"
    And no errors were logged

  # The autopilot takes its own bills down through the same BillStack.Delete the tab uses, so
  # without the guard flag every stock that filled up would read as a refusal and the recipe would
  # retire itself. This is that flag, seen from outside: a bill removed because the stock reached
  # the target leaves the profile untouched, and comes back when the stock falls.
  Scenario: the autopilot taking its own bill down is not a refusal
    Given the Bill Autopilot test stockpile holds 50 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has no override for "Make_Patchleather" on "HandTailoringBench"
    Given the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
