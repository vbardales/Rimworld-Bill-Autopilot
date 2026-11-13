# The captures of the Workshop page (PUBLICATION.md, "Screenshots, in upload order"), produced by a scenario rather than by
# hand so that they can be remade after any interface change. Images 1 to 4 and 6 are here, image 5 (the settings page naming
# the neighbours it found) needs the pass with the neighbours and is in 24-gallery-neighbours.feature.
#
# Played in the pass WITHOUT the optional mods (wsl-deps.galerie.map): Nice Bill Tab and Better Workbench Management
# replace or extend the bills tab, and the tab must be the one a player who has none of them sees. Nothing here asserts
# what a picture shows: a green scenario says the trip happened, and each image is opened and looked at before it goes
# into the gallery folder.
#
# SHOTLIST of 2026-11-13 (write-shotlist, v2; director pass by an Opus sub-agent, remarks kept or refused below)
#
# Place: Sanctuary of Nelim (fixture Nelims-tribe, mod Nelim's Sanctuary Backlot), hearth hall (hearth-hall, framed on
#   (181, 115)). The undecorated fixture is kept: the decorated fixture z3 needs a large decor mod set, and the tab must stay
#   vanilla. The hall is emptied by the area step that keeps the roof holders (the four columns stay), then staged as a
#   tailor's corner. Roofed room: four standing torches and a brazier, lit (never remove the roof).
# Protagonist: Nelim (NSB, free). Look, kept in all her scenes (continuity): canon hair (Roo_HD_Hair_Highpony, canon colour,
#   no change: the copper-hair override of the older plan needs the owner's validation, PENDING, so it is dropped), a basic
#   shirt dyed teal rgb (40, 110, 120) (outfit free; teal against the orange bench and the warm light), default fixture pants,
#   her own face, a smile. Animal: Atticus (NSB thrumbo, free, lives in the hall) asleep beside the brazier, seen from the front,
#   in image 1 only; the mod adds no animal, so rule 5 has nothing to show.
# Story, midday of one day (clear sky, game paused for each capture, 2,500 ticks per game hour): Nelim, a tailor who also feeds
#   the Sanctuary, has stopped rewriting her bench's bill list by hand. She said once what the bench is for and the bench keeps
#   its own list. Image 1 12:00 (+60 ticks of set-up): she works while the tab shows the list the bench keeps. Image 2 and 3 are
#   the windows where she said it once and the one question it asked. Image 4 12:15 (+625 ticks, staged, research does not
#   finish in 15 game minutes): a new recipe waits suspended for her answer. Image 5 the neighbours it found. Image 6 12:45
#   (+1,875 ticks): she answered, the list is hers again.
#
# SHOTS (one line per image: place, hour, subject, composition, living things, decor, what it says, instant, how obtained)
#   1. tailor-1-hall, story scene, WIDE (zoom 3 on (181, 115), camera centre (183, 114), Nelim and the bench right of centre,
#      clear of the tab at the bottom left; foreground the bench and the brazier, background the four columns and the torches). Hearth hall, 12:00, clear, warm torch and brazier light on the parquet. Subject: Nelim at
#      the hand tailoring bench with the bills tab open: three marked automatic bills and one hand-placed bill (no mark). Says: the
#      tab shows what is left to make, not a wall. Living: Nelim mid-stitch (the job of the bill forced, paused on a state check
#      "colonist at the bench and working" plus a few ticks, gaze toward the bench), Atticus asleep beside the brazier facing the
#      camera. Decor in frame, grouped around the bench: HandTailoringBench, Brazier lit beside it, PlantPot with Plant_Daylily
#      fully grown x2 within 3 cells, TorchLamp x4 lit, Column x4 (roof holders), parquet kept. The decor is removed after the
#      shot. Legibility (j): the auto mark must read at 268 px: step "Nelims Pickle Tools: the interface scale is 175 percent" (NPT 2026-11-13: exists, restored after the scenario;
#      175 first, then 150 to 200 if the mark is still thin), mark width checked on the 268 px shrink; fallback zoom 4 and crop. Subtitle: "Nelim sews. The bench keeps its own list."
#      Instant: she is in the middle of the next pair; the tab already lists what follows. Director remarks: grouped decor, Atticus
#      for the animal criterion, camera off the tab, gaze, "just finished a pair" claim dropped: KEPT. UI scale: KEPT, the step exists.
#   2. tailor-2-profile, interface window (set aside, no subtitle): the profile window, one group open, one overridden recipe.
#      Says: "say it once" happens here, reachable with a pointer alone. Taken on window-backdrop-for-height, cropped.
#   3. tailor-3-confirm, interface window (set aside, no subtitle): the confirmation when a bench type is switched on, naming how
#      many recipes it takes and at what target. Taken on window-backdrop-for-height, cropped.
#   4. tailor-4-stove, story scene, TIGHT (zoom 5 on the stove at (176, 108)). Same hall, 12:15, clear. Subject: the suspended
#      bill row of a newly researched recipe (pemmican) in the stove's bills tab, with the letter icon that named it kept in the
#      right-hand stack (not cleared). Says: nothing is spent without an answer. Living: Nelim beside the stove (she feeds the
#      Sanctuary), mid-stir, same teal shirt, same hair, smile, gaze toward the tab. Decor in frame only: stove, Brazier lit, two
#      PlantPot with Plant_Daylily fully grown, one TorchLamp lit, parquet; filth removed. Composition: foreground Nelim and the stove,
#      background the torch and the pots, subject (the tab row) at the left, Nelim right of centre. How obtained: the job of an existing
#      bill forced at the stove, paused on the state check "colonist at the stove and working" plus a few ticks. Subtitle: "A new recipe waits for her answer."
#      Instant: the letter has just arrived and she looks at it before touching anything. Director remarks: stove refused (the
#      stove scene was played green, run 2c7e, and Nelim feeds the Sanctuary, NELIM-PROFIL), letter kept instead of cleared,
#      only in-frame decor named, continuity: KEPT. Same UI scale step as image 1.
#   5. tailor-5-neighbours, interface window (set aside): the settings page naming the neighbours it found (24-gallery-neighbours).
#   6. tailor-6-answered, story scene, MEDIUM (zoom 4 on the bench, (181, 115)). Same hall, 12:45, clear. Subject: the closing:
#      every bill in the tab is marked automatic and the recipe she accepted is among them, unsuspended. Says: she answered once,
#      the list is hers again. Living: Nelim at the bench working the new recipe, same look, smile. Decor in frame: bench, Brazier
#      lit, PlantPot with Plant_Daylily x2, TorchLamp x2 lit. Subtitle: "She answered once. The list is hers again."
#      Composition: foreground the bench and Nelim, background the torches and the pots, tab at the left. Instant: she has just unsuspended the bill and the bench has taken it up. How obtained: the bill unsuspended by the step, the job forced, paused on the state check "colonist at the bench and working" plus a few ticks. Director remark: a closing scene and a medium
#      shot: KEPT (6th image; PUBLICATION.md and the upload order follow).
#   Series: wide (1), tight (4), medium (6), an opener that places the hall (1), a closer that answers the story (6), windows
#   between them; the tab and the letter stay vanilla (pass without the optional mods).
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
    And Nelim's Pickle Tools: I place the decor "Column" at (178, 115)
    And Nelim's Pickle Tools: I place the decor "Column" at (185, 116)
    And Nelim's Pickle Tools: I place the decor "Column" at (185, 111)
    And Nelim's Pickle Tools: I place the decor "Column" at (190, 115)
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
    And Nelim's Pickle Tools: "Nelim" hairstyle is "Ponytail"
    And Nelim's Pickle Tools: "Nelim" hair colour is rgb (150, 75, 35)
    And Nelim's Pickle Tools: "Nelim" wears "Apparel_BasicShirt" dyed rgb (40, 110, 120)
    And Nelim's Sanctuary: Nelim has her own face
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
    And Nelim's Pickle Tools: I place the decor "Column" at (178, 115)
    And Nelim's Pickle Tools: I place the decor "Column" at (185, 116)
    And Nelim's Pickle Tools: I place the decor "Column" at (185, 111)
    And Nelim's Pickle Tools: I place the decor "Column" at (190, 115)
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
    And Nelim's Pickle Tools: I place the decor "Column" at (178, 115)
    And Nelim's Pickle Tools: I place the decor "Column" at (185, 116)
    And Nelim's Pickle Tools: I place the decor "Column" at (185, 111)
    And Nelim's Pickle Tools: I place the decor "Column" at (190, 115)
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
    And Nelim's Pickle Tools: the area from (172, 106) to (190, 124) is emptied, the roof holders kept
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
    When Bill Autopilot's gallery: the colonist "Nelim" stands at (181, 114) facing north
    And Nelim's Pickle Tools: "Nelim" hairstyle is "Ponytail"
    And Nelim's Pickle Tools: "Nelim" hair colour is rgb (150, 75, 35)
    And Nelim's Pickle Tools: "Nelim" wears "Apparel_BasicShirt" dyed rgb (40, 110, 120)
    And Nelim's Sanctuary: Nelim has her own face
    And Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (181, 115) at zoom 4
    And the bills tab of the Bill Autopilot bench "FueledStove" at (181, 115) is opened
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 4: a suspended bill for a newly researched recipe, with the letter that named it"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed
