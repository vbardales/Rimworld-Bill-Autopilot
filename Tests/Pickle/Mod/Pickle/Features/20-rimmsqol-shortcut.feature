# TESTING.md scenario 20, the part that needs RIMMSQOL. 18-settings-shortcut.feature tests THIS mod's side of the
# MOD_SETTINGS.md contract by moving the def's buttonVisible by hand. This drives RIMMSQOL itself, through the
# shared steps of PickleTools/RimmsqolSteps, so that "can RIMMSQOL list and reveal Bill Autopilot's shortcut, and
# does the revealed button open the same settings" no longer rests on reading its source:
#
#   - RIMMSQOL's own list of main buttons offers BillAutopilot_Settings, and the entry a player would click reads
#     hidden;
#   - RIMMSQOL reveals it (its own settings instance, its own write; the def's buttonVisible moves as a result),
#     the main bar then draws it, and the file RIMMSQOL wrote says so;
#   - the revealed button opens THIS mod's settings, the same dialog as Mod options;
#   - hiding it again empties the bar, and forgetting the choice leaves nothing in RIMMSQOL's file.
#
# What it does not do: click RIMMSQOL's checkbox (the steps call what the checkbox calls; that the checkbox is wired to
# it is read from RIMMSQOL's source), and check that RIMMSQOL keeps its choice across a restart. The second is
# RIMMSQOL's own behaviour, not this mod's; it is exercised by the restart chain of PickleTools' demonstration
# (FlavorTextExtendedFR, features 12 to 15, 2026-09-21) and not repeated here.
#
# Played only by the pass avec-rimmsqol (wsl-deps.avec-rimmsqol.map). The requirement tags make a missing staged tool a
# skip rather than a false validation. The screenshots are what shows pixels, and a green scenario says nothing about
# them.
@review @rimmsqol @requires:MalteSchulze.RIMMSqol @requires:nelim.pickletools.rimmsqol
Feature: RIMMSQOL reveals and hides the Bill Autopilot shortcut

  Background:
    Given the save "test-colony" is loaded
    And I close all dialogs
    Then mod "MalteSchulze.RIMMSqol" is loaded
    And RIMMSQOL is ready to be driven

  Scenario: RIMMSQOL's own list offers the shortcut, hidden, and the bar does not draw it
    Then RIMMSQOL's own list of main buttons offers "BillAutopilot_Settings"
    And RIMMSQOL shows the main button "BillAutopilot_Settings" as hidden
    And RIMMSQOL holds no choice for the main button "BillAutopilot_Settings"
    And the main bar does not draw the button "BillAutopilot_Settings"
    When RIMMSQOL's own window is opened on its list of main buttons
    Then RIMMSQOL's own window is open
    When I take a screenshot "rimmsqol, its list of main buttons, with the bill autopilot shortcut"
    And I close all dialogs

  Scenario: revealed in RIMMSQOL the shortcut is drawn, and it opens the same settings as Mod options
    When RIMMSQOL reveals the main button "BillAutopilot_Settings"
    Then RIMMSQOL shows the main button "BillAutopilot_Settings" as visible
    And RIMMSQOL's settings file records the main button "BillAutopilot_Settings" as visible
    And the main bar draws the button "BillAutopilot_Settings"
    When RIMMSQOL's own window is opened on the main button "BillAutopilot_Settings"
    Then RIMMSQOL's own window is open
    When I take a screenshot "rimmsqol, edit page of the bill autopilot shortcut, revealed"
    And I close all dialogs
    And the main bar's button "BillAutopilot_Settings" is activated
    Then a settings dialog is open for Bill Autopilot
    And no errors were logged
    When I take a screenshot "bill autopilot settings, opened by the shortcut RIMMSQOL revealed"
    And I close all dialogs

  Scenario: hidden again in RIMMSQOL the shortcut leaves the bar, and forgetting the choice leaves nothing behind
    Given RIMMSQOL reveals the main button "BillAutopilot_Settings"
    And the main bar draws the button "BillAutopilot_Settings"
    When RIMMSQOL hides the main button "BillAutopilot_Settings"
    Then RIMMSQOL shows the main button "BillAutopilot_Settings" as hidden
    And the main bar does not draw the button "BillAutopilot_Settings"
    And RIMMSQOL's settings file records the main button "BillAutopilot_Settings" as hidden
    When RIMMSQOL forgets its choice for the main button "BillAutopilot_Settings"
    Then RIMMSQOL holds no choice for the main button "BillAutopilot_Settings"
    And RIMMSQOL's settings file records no choice for the main button "BillAutopilot_Settings"
