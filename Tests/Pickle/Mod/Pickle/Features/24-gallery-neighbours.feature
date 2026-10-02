# Image 5 of the Workshop page's captures (PUBLICATION.md): the settings page, with the "found around it" line naming the
# integrations it detected. It answers "does it work with X" before anyone asks, so it has to be taken in the pass with the
# neighbours; in the pass without them the same line says "not found" four times. Images 1 to 4 are in
# 23-gallery-vanilla.feature, in the pass without the optional mods.
#
# Played with wsl-deps.galerie-voisins.map, which is the neighbours plus Nelim's Pickle Tools' zen meadow studio: the same
# colony as the other four images (chosen by the owner on 2026-09-27), seen behind the settings window.
@review @gallery @requires:falconne.BWM
Feature: the capture of the Workshop page that names the neighbours it found

  Scenario: image 5, the settings page with the integrations it found
    Given the save "nelim-zen-meadow-studio" is loaded
    And game speed is paused
    And the Learning helper is switched off
    And the studio's display pavilion is closed, roofed and lit
    Then the cell (125, 96) is lit at least 50 percent
    And Bill Autopilot settings are at their defaults
    And I close all dialogs
    Then Bill Autopilot's integration report matches the mods this pass loaded
    When the camera looks at (125, 96) from a distance of 12
    And Bill Autopilot's settings shortcut is activated
    Then a settings dialog is open for Bill Autopilot
    When I take a screenshot "gallery 5: the settings page, naming the integrations it found"
    And I close all dialogs
