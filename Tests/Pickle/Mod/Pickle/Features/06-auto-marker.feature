# TESTING.md scenario 6. Never seen on screen before this suite.
#
# The mark is a postfix on Bill_Production.LabelCap, and that spot was chosen because it is the one
# place every bill interface reads: the vanilla tab, Nice Bill Tab, Dubs Mint Menus and Better
# Workbench Management all build their row from the label, so none of them has to be patched. That
# claim cannot be checked out of game - it needs a real bill whose property really goes through the
# patched getter.
#
# The marker itself is never spelled out here. The step resolves the mod's own translation key, so
# the French pass checks a French marker instead of failing on a correct translation.
#
# Whether the mark actually SHOWS in each of those tabs is a picture, not a property: the screenshot
# below is attached for a person to look at, and it proves nothing on its own.
Feature: an automatic bill is marked in its label

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)

  Scenario: the autopilot's own bill carries the mark
    Then Bill Autopilot marks its bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

  # The control. A marker appended to every bill would pass the scenario above and be worse than no
  # marker at all, since the gesture it warns about - delete to refuse - applies to one of the two.
  Scenario: a bill the player placed carries nothing
    When I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot does not mark the hand-placed bill for "Make_Apparel_Pants" on the "HandTailoringBench" at (140, 155)

  Scenario: turning marking off removes it and changes nothing else
    Given Bill Autopilot does not mark automatic bills
    Then Bill Autopilot does not mark its bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And no errors were logged

  # @review: this scenario asserts nothing about what the image contains. It attaches a picture of
  # the bills tab with one automatic and one hand-placed bill side by side, for a person to read.
  # Its green says the trip happened, not that the mark was legible - and in a pass that stages the
  # tab-replacing mods, this is the only place their rows can be seen at all.
  @review
  Scenario: the tab, with one bill of each kind, for a person to look at
    When I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (140, 155)
    And the bills tab of the Bill Autopilot bench "HandTailoringBench" at (140, 155) is opened
    And I take a screenshot "bills tab: one automatic bill and one placed by hand"
