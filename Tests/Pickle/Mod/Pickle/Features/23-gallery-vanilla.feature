# The captures of the Workshop page (PUBLICATION.md, "Screenshots, in upload order", images 1 to 4), produced by a
# scenario rather than by hand so that they can be remade after any interface change. Image 5 (the settings page naming
# the neighbours it found) needs the pass with the neighbours and is in 24-gallery-neighbours.feature.
#
# Played in the pass WITHOUT the optional mods (wsl-deps.galerie.map): Nice Bill Tab and Better Workbench Management
# replace or extend the bills tab, and image 1 must show the tab a player who has none of them sees. Nothing here asserts
# what a picture shows: a green scenario says the trip happened, and each image is opened and looked at before it goes
# into the gallery folder.
#
# The colony is Nelim's Pickle Tools' zen meadow studio (fixture nelim-zen-meadow-studio, chosen by the owner on
# 2026-09-27), not Pickle's played `test-colony`. Every scene is built in its "display" pavilion, the empty interior meant
# for mod demonstrations, which the scenario closes, roofs and lights (the studio leaves its roofs off on purpose): a bench
# under an open sky reads "Work speed factor: 40% (outdoors)", and a roofed interior with no light is too dark to sell
# anything. The bench is built by the scenario, with recipes of its own, and never borrowed from another mod.
# Composition (owner, 2026-10-02): the camera is close on the bench (distance 5), the floor is the game's wood plank
# parquet, flowering plant pots stand around the bench, image 1 carries one bill placed by hand (no mark) beside the three
# automatic ones, and images 1 to 3 clear the letter stack and the alerts. Image 4 keeps its letter: it is the picture.
# The shots are @review because that is what makes Pickle keep them as pictures a person has to open.
@review @gallery @requires:nelim.pickletools.screenshotmode
Feature: the captures of the Workshop page, from the pass without the optional mods

  # Image 1. The whole pitch in one picture: the tab shows what is left to make, not a wall of configuration. Three
  # automatic bills, each marked, on a bench that has far more recipes than that.
  Scenario: image 1, a bills tab holding three automatic bills on a bench with many recipes
    Given the save "nelim-zen-meadow-studio" is loaded
    And game speed is paused
    And the Learning helper is switched off
    And the studio's display pavilion is closed, roofed and lit
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (125, 96)
    Then the cell (125, 96) is lit at least 50 percent
    And Bill Autopilot allows 3 automatic bills per bench
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot syncs the "HandTailoringBench" at (125, 96)
    Then Bill Autopilot has 3 bills up on the "HandTailoringBench" at (125, 96)
    When I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (125, 96)
    When the camera looks at (125, 96) from a distance of 5
    And the bills tab of the Bill Autopilot bench "HandTailoringBench" at (125, 96) is opened
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And I take a screenshot "gallery 1: the bills tab, three automatic bills on a bench with many recipes"
    And I close all dialogs

  # Image 2. Where "say it once" happens: the profile window with every group collapsed but the one that holds an
  # overridden recipe, which stands out because its numbers are its own.
  Scenario: image 2, the profile window with one group open and an overridden recipe in it
    Given the save "nelim-zen-meadow-studio" is loaded
    And game speed is paused
    And the Learning helper is switched off
    And the studio's display pavilion is closed, roofed and lit
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (125, 96)
    Then the cell (125, 96) is lit at least 50 percent
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot overrides "Make_Patchleather" on "HandTailoringBench" to keep 200, restarting at 100
    When the camera looks at (125, 96) from a distance of 5
    And Bill Autopilot's profile window for "HandTailoringBench" is opened
    And Bill Autopilot's profile window keeps open only the group of "Make_Patchleather"
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And I take a screenshot "gallery 2: the profile window, one group open, an overridden recipe"
    And I close all dialogs

  # Image 3. The one moment a lot of production can start at once, and the mod asks first, naming its count.
  Scenario: image 3, the confirmation when a workbench type is switched on
    Given the save "nelim-zen-meadow-studio" is loaded
    And game speed is paused
    And the Learning helper is switched off
    And the studio's display pavilion is closed, roofed and lit
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (125, 96)
    Then the cell (125, 96) is lit at least 50 percent
    And I close all dialogs
    When the camera looks at (125, 96) from a distance of 5
    And Bill Autopilot's toggle is used to switch "HandTailoringBench" on
    Then Bill Autopilot asks before taking the recipes it would take on "HandTailoringBench"
    When Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And I take a screenshot "gallery 3: the confirmation naming how many recipes it is about to take"
    And the Bill Autopilot confirmation is refused

  # Image 4. The point of the mod: a recipe unlocked by research arrives suspended, a letter names it, and nothing is
  # spent without an answer. The letter sits in the stack and the suspended bill in the open tab, in one frame.
  Scenario: image 4, a suspended bill for a newly researched recipe, with its letter
    Given the save "nelim-zen-meadow-studio" is loaded
    And game speed is paused
    And the Learning helper is switched off
    And the studio's display pavilion is closed, roofed and lit
    And Bill Autopilot settings are at their defaults
    And a "FueledStove" is built at (125, 96)
    Then the cell (125, 96) is lit at least 50 percent
    And Bill Autopilot only takes "Make_Pemmican" on "FueledStove"
    And Bill Autopilot is switched on for "FueledStove"
    And Bill Autopilot syncs the "FueledStove" at (125, 96)
    And Bill Autopilot has never met "Make_Pemmican" on "FueledStove"
    When research "Pemmican" is finished
    And Bill Autopilot syncs the "FueledStove" at (125, 96)
    And I wait 120 ticks
    Then Bill Autopilot has announced "Make_Pemmican" on "FueledStove"
    When the camera looks at (125, 96) from a distance of 5
    And the bills tab of the Bill Autopilot bench "FueledStove" at (125, 96) is opened
    And I take a screenshot "gallery 4: a suspended bill for a newly researched recipe, with the letter that named it"
    And I close all dialogs
