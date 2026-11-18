# Image 5 of the six Workshop page captures (PUBLICATION.md, shotlist v2): the settings page, with the "found around it" line naming the
# integrations it detected. It answers "does it work with X" before anyone asks, so it has to be taken in the pass with the
# neighbours; in the pass without them the same line says "not found" four times. Images 1, 2, 3, 4 and 6 are in
# 23-gallery-vanilla.feature, in the pass without the optional mods.
#
# Played with wsl-deps.galerie-voisins.map, which is the neighbours plus the Sanctuary's tools (see
# wsl-deps.galerie.map). SHOOTING PLAN: the bamboo backdrop of the Sanctuary of Nelim (window-backdrop-for-height, GALLERY.md "Relire les
# captures"), midday, clear sky, game paused, interface scale 125 percent, HUD hidden; the window is the subject, cropped sideways after the run. It says: the mod names the neighbours it found.
@review @gallery @requires:falconne.BWM @requires:nelim.pickletools.screenshotmode @requires:nelim.pickletools.camerazoom @requires:nelim.pickletools.interfacescale @requires:nelim.pickletools.screenshotstudio
Feature: the capture of the Workshop page that names the neighbours it found

  Scenario: image 5, the settings page with the integrations it found
    Given the save "Nelims-tribe" is loaded
    And game speed is paused
    And I set the hour to 12
    And I set the weather to "Clear"
    And the Learning helper is switched off
    And Nelim's Pickle Tools: the colonist bar is hidden
    And Nelim's Pickle Tools: the resource readout is hidden
    And Nelim's Pickle Tools: the main bar, the speed controls and the mouse-over readout are hidden
    And Nelim's Pickle Tools: the alerts are hidden
    And Nelim's Pickle Tools: the tooltips are hidden
    And Nelim's Pickle Tools: the selection brackets are hidden
    And Nelim's Pickle Tools: the item and name labels are hidden
    And Nelim's Pickle Tools: the interface scale is 125 percent
    And Nelim's Sanctuary: the animals are removed from the sanctuary "hearth-hall"
    And Bill Autopilot settings are at their defaults
    And I close all dialogs
    Then Bill Autopilot's integration report matches the mods this pass loaded
    When Nelim's Sanctuary: I am at the sanctuary "window-backdrop-for-height"
    And Bill Autopilot's settings shortcut is activated
    Then a settings dialog is open for Bill Autopilot
    When Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 5: the settings page, naming the integrations it found"
    And I close all dialogs
