# The captures of the Workshop page (PUBLICATION.md, "Screenshots, in upload order", images 1 to 4), produced by a
# scenario rather than by hand so that they can be remade after any interface change. Image 5 (the settings page naming
# the neighbours it found) needs the pass with the neighbours and is in 24-gallery-neighbours.feature.
#
# Played in the pass WITHOUT the optional mods (wsl-deps.galerie.map): Nice Bill Tab and Better Workbench Management
# replace or extend the bills tab, and image 1 must show the tab a player who has none of them sees. Nothing here asserts
# what a picture shows: a green scenario says the trip happened, and each image is opened and looked at before it goes
# into the gallery folder.
#
# The place is the Sanctuary of Nelim (fixture Nelims-tribe, mod Nelim's Sanctuary Backlot): since 2026-10-08 galleries
# are shot there and no longer in the zen meadow studio, which was deleted. The session stages the whole series
# (GALLERY.md, "La session est la metteuse en scene").
#
# SHOOTING PLAN (one line per image: place, moment, subject, composition, what it says)
#   Place and moment, all four: the hearth hall (hearth-hall, framed on (181, 115)), midday, clear sky, game paused. The
#     hall is emptied of its furniture and of its animals, and four standing torches are placed and lit close around the bench: it is a roofed
#     room, and an unlit roofed room is too dark to sell anything (GALLERY.md, "Eclairer un lieu sombre": torches,
#     never remove the roof). The bench is built by the scenario, with recipes of its own, never borrowed from another mod.
#   Story: a quiet tailoring workshop that runs itself while its tailor, Nelim (the only colonist of the fixture, the
#     owner herself), keeps her own hand in.
#   1. Nelim at her bench, close (zoom 5), the bills tab open: three marked automatic bills and one bill placed by hand
#      (no mark) on a bench with far more recipes than that. It says: the tab shows what is left to make.
#   2. The profile window, a screenshot of what it is (not staged): one group open, one overridden recipe in it. It
#      says: "say it once" happens here, reachable with a pointer alone.
#   3. The confirmation when a bench type is switched on, a window, not staged: the mod asks first and names the count.
#   4. A stove in the same hall, its bills tab open with a suspended bill and the letter that named it, in one frame. Only 30 ticks pass (5 were too few: run 6274 timed out waiting for the letter), then the filth is cleaned: with 120 ticks (run 8f81) the emptied hall lost its roof ("Roof collapse" letter, rubble everywhere, "Unroofed").
#      It says: nothing is spent without an answer.
#   Rhythm: every scenario reloads the save and sets midday again; no game time passes between images, the light stays
#     the same on purpose (a window and a tab do not need a moving sun).
# The shots are @review because that is what makes Pickle keep them as pictures a person has to open.
@review @gallery @requires:nelim.pickletools.screenshotmode
Feature: the captures of the Workshop page, from the pass without the optional mods

  # Image 1. The whole pitch in one picture.
  Scenario: image 1, a bills tab holding three automatic bills on a bench with many recipes
    Given the save "Nelims-tribe" is loaded
    And game speed is paused
    And I set the hour to 12
    And I set the weather to "Clear"
    And the Learning helper is switched off
    And Nelim's Sanctuary: the animals are removed from the sanctuary "hearth-hall"
    And Nelim's Sanctuary: the sanctuary "hearth-hall" is emptied
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (181, 115)
    Then the cell (181, 115) is lit at least 50 percent
    And Bill Autopilot allows 3 automatic bills per bench
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot syncs the "HandTailoringBench" at (181, 115)
    Then Bill Autopilot has 3 bills up on the "HandTailoringBench" at (181, 115)
    When I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (181, 115)
    And Bill Autopilot's gallery: the colonist "Nelim" stands at (181, 114) facing north
    And Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (181, 115) at zoom 5
    And the bills tab of the Bill Autopilot bench "HandTailoringBench" at (181, 115) is opened
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 1: the bills tab, three automatic bills on a bench with many recipes"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed

  # Image 2. Where "say it once" happens.
  Scenario: image 2, the profile window with one group open and an overridden recipe in it
    Given the save "Nelims-tribe" is loaded
    And game speed is paused
    And I set the hour to 12
    And I set the weather to "Clear"
    And the Learning helper is switched off
    And Nelim's Sanctuary: the animals are removed from the sanctuary "hearth-hall"
    And Nelim's Sanctuary: the sanctuary "hearth-hall" is emptied
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (181, 115)
    Then the cell (181, 115) is lit at least 50 percent
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot overrides "Make_Patchleather" on "HandTailoringBench" to keep 200, restarting at 100
    When Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (181, 115) at zoom 7
    And Bill Autopilot's profile window for "HandTailoringBench" is opened
    And Bill Autopilot's profile window keeps open only the group of "Make_Patchleather"
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 2: the profile window, one group open, an overridden recipe"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed

  # Image 3. The one moment a lot of production can start at once, and the mod asks first, naming its count.
  Scenario: image 3, the confirmation when a workbench type is switched on
    Given the save "Nelims-tribe" is loaded
    And game speed is paused
    And I set the hour to 12
    And I set the weather to "Clear"
    And the Learning helper is switched off
    And Nelim's Sanctuary: the animals are removed from the sanctuary "hearth-hall"
    And Nelim's Sanctuary: the sanctuary "hearth-hall" is emptied
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (181, 115)
    Then the cell (181, 115) is lit at least 50 percent
    And I close all dialogs
    When Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (181, 115) at zoom 7
    And Bill Autopilot's toggle is used to switch "HandTailoringBench" on
    Then Bill Autopilot asks before taking the recipes it would take on "HandTailoringBench"
    When Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 3: the confirmation naming how many recipes it is about to take"
    And the Bill Autopilot confirmation is refused
    And Nelim's Pickle Tools: the decor is removed

  # Image 4. The point of the mod: nothing is spent without an answer.
  Scenario: image 4, a suspended bill for a newly researched recipe, with its letter
    Given the save "Nelims-tribe" is loaded
    And game speed is paused
    And I set the hour to 12
    And I set the weather to "Clear"
    And the Learning helper is switched off
    And Nelim's Sanctuary: the animals are removed from the sanctuary "hearth-hall"
    And Nelim's Sanctuary: the sanctuary "hearth-hall" is emptied
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Bill Autopilot settings are at their defaults
    And a "FueledStove" is built at (181, 115)
    Then the cell (181, 115) is lit at least 50 percent
    And Bill Autopilot only takes "Make_Pemmican" on "FueledStove"
    And Bill Autopilot is switched on for "FueledStove"
    And Bill Autopilot syncs the "FueledStove" at (181, 115)
    And Bill Autopilot has never met "Make_Pemmican" on "FueledStove"
    When research "Pemmican" is finished
    And Bill Autopilot syncs the "FueledStove" at (181, 115)
    And Nelim's Pickle Tools: I let 30 ticks pass
    And Nelim's Pickle Tools: all filth is cleaned
    Then Bill Autopilot has announced "Make_Pemmican" on "FueledStove"
    When Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (181, 115) at zoom 5
    And the bills tab of the Bill Autopilot bench "FueledStove" at (181, 115) is opened
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 4: a suspended bill for a newly researched recipe, with the letter that named it"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed
