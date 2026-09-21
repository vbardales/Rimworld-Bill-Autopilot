# TESTING.md scenario 13. The largest compatibility layer, and the one with no witness at all.
#
# Everything here goes through reflection into ImprovedWorkbenches, so a failure is silent by
# construction: the feature is lost and nothing crashes. That is precisely why it needs a test -
# there is no error message to notice, and a player would only ever see a name quietly gone.
#
# The whole scenario is a down-and-up cycle. Better Workbench Management prefixes BillStack.Delete
# to erase what it attached along with the bill, and the autopilot takes bills down every time a
# stock fills up, so without the bridge everything set through BWM would vanish on the first cycle.
# The bill that comes back is a different object from the one that was set, which is what makes
# this impossible to fake and impossible to check out of game.
#
# Values are written and read through BWM's own store, never through this mod's copy of it: a check
# reading back the mod's own memory would agree with itself whether or not anything ever reached
# BWM.
#
# NOT covered here, and still manual: the workbench restriction the mod applies to a bill it
# creates, and the agreement between the widened count and what the bill displays. Both are
# reachable in principle; neither was written against an interface this session could read rather
# than guess at. Tests/Pickle/README.md keeps them on the manual list rather than pretending.
@requires:falconne.BWM
Feature: what Better Workbench Management adds to a bill survives the autopilot's cycle

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: the mod found it
    Then Bill Autopilot found Better Workbench Management

  Scenario: a custom name and a widened count both come back
    Given Better Workbench Management names Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) "caravan stock"
    And Better Workbench Management counts Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) away from the home map

    Given the Bill Autopilot test stockpile holds 60 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    Given the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Better Workbench Management still names Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) "caravan stock"
    And Better Workbench Management still counts Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) away from the home map
    And no errors were logged

  # Membership in a link group, which is the one that cannot be faked by re-setting a field: the
  # returning bill has to rejoin the group its predecessor was in rather than start a new one, and
  # on a single bill those two look exactly alike.
  # A second recipe is let back in for this one: the Background narrows the bench to one, and a link
  # group of one bill is not a link group. Make_Apparel_Pants is available from the start in this
  # fixture, so it needs no research and arrives running rather than suspended.
  Scenario: a linked bill rejoins its group rather than starting a new one
    Given Bill Autopilot's rule for "Make_Apparel_Pants" on "HandTailoringBench" is set back to the default
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)
    Given Better Workbench Management links Bill Autopilot's bills for "Make_Patchleather" and "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)

    Given the Bill Autopilot test stockpile holds 60 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    Given the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Better Workbench Management still links Bill Autopilot's bills for "Make_Patchleather" and "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)
