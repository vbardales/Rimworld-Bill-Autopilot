# TESTING.md scenario 20, the part that does not need RIMMSQOL.
#
# The contract MOD_SETTINGS.md puts on this shortcut: available for a customization mod to reveal,
# hidden by default, neither visible nor greyed, and opening the SAME settings as Mod options. The
# def and its two language fields are checked by the XML validator; what only a game can answer is
# what the main bar's own worker does with them, and which mod the dialog that opens belongs to.
#
# What RIMMSQOL does when a player reveals the button is move MainButtonDef.buttonVisible. These
# scenarios move that same field, and then ask RimWorld's own worker what the bar would draw. What
# stays manual, and cannot be otherwise from here: revealing it inside RIMMSQOL's own interface,
# and whether ITS visibility choice survives a restart. Both are RIMMSQOL's behaviour, and staging
# it would mean mounting a mod to test code that is not ours.
Feature: the hidden MainButtons shortcut opens this mod's own settings

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And I close all dialogs

  # Both halves matter. MOD_SETTINGS.md forbids a greyed shortcut as firmly as a visible one, and a
  # def can be drawn and still be dead.
  Scenario: hidden on a clean configuration, drawn and live once revealed, gone again when hidden
    Then Bill Autopilot's settings shortcut is hidden on a clean configuration
    When Bill Autopilot's settings shortcut is revealed, as a customization mod would
    Then Bill Autopilot's settings shortcut is drawn in the bar
    When Bill Autopilot's settings shortcut is hidden again
    Then Bill Autopilot's settings shortcut is not drawn in the bar

  # The real claim is not "a settings window opened" but "the same settings opened". A dialog built
  # for another mod would be indistinguishable in a screenshot, so the window is asked which mod it
  # belongs to.
  Scenario: activating it opens the settings of this mod and no other
    When Bill Autopilot's settings shortcut is activated
    Then a settings dialog is open for Bill Autopilot
    And no errors were logged
    When I close all dialogs

  # @review: the settings page as a player sees it, for a person to read. Nothing about its contents
  # is asserted - raw keys, clipping, an integration line naming the wrong mods and the layout at
  # this resolution are all things only an eye catches, and in the French pass this is where a
  # missing key shows up as accented gibberish.
  @review
  Scenario: the settings page, for a person to look at
    When Bill Autopilot's settings shortcut is activated
    And I take a screenshot "settings page opened through the MainButtons shortcut"
    And I close all dialogs
