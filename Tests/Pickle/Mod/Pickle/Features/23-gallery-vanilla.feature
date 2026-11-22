# The captures of the Workshop page (PUBLICATION.md, "Screenshots, in upload order"), produced by a scenario rather than by
# hand so that they can be remade after any interface change. Images 1 to 4 and 6 are here, image 5 (the settings page naming
# the neighbours it found) needs the pass with the neighbours and is in 24-gallery-neighbours.feature.
#
# Played in the pass WITHOUT the optional mods (wsl-deps.galerie.map): Nice Bill Tab and Better Workbench Management
# replace or extend the bills tab, and the tab must be the one a player who has none of them sees. Nothing here asserts
# what a picture shows: a green scenario says the trip happened, and each image is opened and looked at before it goes
# into the gallery folder.
#
# SHOTLIST of 2026-11-18 (write-shotlist, v3, rewrite after the REDO verdict of score-gallery on the captures of runs 9a90 and 1718 and
# the earlier ones of 2b53 and efe3; v2 of 2026-11-13 kept below it in git history)
#
# Place: Sanctuary of Nelim (fixture Nelims-tribe, mod Nelim's Sanctuary Backlot), hearth hall (hearth-hall, framed on
#   (181, 115)). The undecorated fixture is kept: the decorated fixture z3 needs a large decor mod set, and the tab must stay
#   vanilla. The hall is emptied by the area step that keeps the roof holders (the four columns stay), then staged as a
#   tailor's corner. Roofed room: four standing torches and a brazier, lit (never remove the roof). NSB 2026-11-18 plans a
#   rebuilt save Nelims-tribe-v2 (nothing built): this shotlist keeps Nelims-tribe.
# Protagonist: Nelim (NSB, free). Look, kept in all her scenes (continuity): canon hair (Roo_HD_Hair_Highpony, canon colour,
#   no change, no property change to validate), a basic shirt dyed teal rgb (40, 110, 120) (outfit free; teal against the orange
#   bench and the warm light), Apparel_Pants worn explicitly (2026-11-22: the fixture pants were not drawn, captures 1, 4, 6 of 2026-11-20 showed bare legs), her own face, a smile. Animals: Atticus and Rain (NSB thrumbos, free, live in
#   the hall) asleep by the brazier in image 1 only, Atticus facing the camera (South), Rain in profile (East); the mod adds no animal,
#   so rule 5 has nothing to show.
# Story, midday of one day (clear sky, game paused for each capture, 2,500 ticks per game hour): Nelim, a tailor who also feeds
#   the Sanctuary, has stopped rewriting her bench's bill list by hand. She said once what the bench is for and the bench keeps
#   its own list. Image 1 12:00 (+120 ticks of set-up): she works while the tab shows the list the bench keeps. Image 2 and 3 are
#   the windows where she said it once and the one question it asked. Image 4 12:15 (+625 ticks, staged, research does not
#   finish in 15 game minutes): a new recipe waits suspended for her answer. Image 5 the neighbours it found. Image 6 12:45
#   (+1,875 ticks): she answered, the list is hers again.
#
# SHOTS (one line per image: place, hour, subject, composition, living things, decor, what it says, instant, how obtained)
#   1. tailor-1-hall, story scene, WIDE (zoom 7 on the cell (184, 114): Nelim and the bench right of the tab, which covers the left
#      third of the screen; foreground the bench and the brazier, background the four columns and the torches, both thrumbos in
#      frame on the right). Hearth hall, 12:00, clear, warm torch and brazier light on the parquet. Subject: Nelim at the hand
#      tailoring bench with the bills tab open: three marked automatic bills and one hand-placed bill (no mark). Says: the tab shows
#      what is left to make, not a wall. Living: Nelim mid-stitch (the job DoBill forced on the bench, then 120 ticks, then the state
#      check "at the building and working", gaze toward the bench), the two NSB thrumbos asleep by the brazier: Atticus at (186, 116)
#      facing South (front), Rain at (186, 113) facing East (profile). Decor in frame, grouped around the bench: HandTailoringBench,
#      Brazier lit at (184, 115), PlantPot with Plant_Daylily fully grown x2 near (179, 113) and (183, 118), Shelf near (176, 114),
#      TorchLamp x4 lit, Column x4 (roof holders), parquet kept. The decor is removed after the shot. Legibility (j): the step
#      "the interface scale is 125 percent" (restored after the scenario), HUD hidden so the tab, the bench and the thrumbos are the
#      brightest blocks; mark width checked on the 268 px shrink, fallback zoom 5 and a tighter crop (scene 6 is then zoom 6) on the tab and the bench.
#      Subtitle: "Nelim sews. The bench keeps its own list."
#      Instant: she is in the middle of the next pair of pants (the hand-placed bill Make_Apparel_Pants, cloth stacked on the bench); the tab already lists what follows.
#   2. tailor-2-profile, interface window (set aside, no subtitle): the profile window, one group open, one overridden recipe.
#      Says: "say it once" happens here, reachable with a pointer alone. Taken on window-backdrop-for-height, scale 125, HUD hidden,
#      cropped sideways.
#   3. tailor-3-confirm, interface window (set aside, no subtitle): the confirmation when a bench type is switched on, naming how
#      many recipes it takes and at what target. Taken on window-backdrop-for-height, scale 125, HUD hidden, cropped sideways.
#   4. tailor-4-stove, story scene, MEDIUM-TIGHT (zoom 4 on the cell (183, 115) beside the stove at (181, 115), v4 of 2026-11-20 after the REDO of the zoom 3 take: Nelim cut at the frame edge, bare background; a higher zoom number is wider, so 4 keeps her whole with the shelf, pots and torches in frame). Same hall, 12:15, clear. Subject: the suspended
#      bill row of a newly researched recipe (pemmican) in the stove's bills tab, with the letter that named it left out of the frame (it sits in the
#      right-hand stack, outside the crop of this take, not a co-subject). Says: nothing is spent without an answer. Living: Nelim beside the stove (she feeds the
#      Sanctuary), working a hand-placed meal bill while the new one waits, same teal shirt, same hair, smile, gaze toward the
#      tab. Decor in frame only: FueledStove, Brazier lit at (184, 115), two PlantPot with Plant_Daylily fully grown, four
#      TorchLamp lit, Shelf near (190, 114), parquet; filth removed. Composition: foreground Nelim and the stove, background the
#      torch and the pots, subject (the tab row) at the left, Nelim right of centre. How obtained: the job DoBill forced on the
#      stove, then 120 ticks and the state check "at the building and working". Same interface scale (125 percent) and hidden HUD as
#      image 1; the tab now shows its first row because it fits the screen.
#      Legibility (j): the suspended row and its "SUSPENDED" overlay are checked on the 268 px shrink; fallback: zoom 2 and the tab row kept inside the crop.
#      Subtitle: "A new recipe waits for her answer."
#      Instant: a new recipe has just been announced; she goes on with the meal bill she has on, her gaze on the stove, and the new row waits (the letter stays in the right-hand stack as a trace, not a co-subject).
#   5. tailor-5-neighbours, interface window (set aside): the settings page naming the neighbours it found (24-gallery-neighbours),
#      on window-backdrop-for-height, scale 125, HUD hidden, cropped sideways.
#   6. tailor-6-answered, story scene, MEDIUM (zoom 5 on the stove at (181, 115), the columns and torches visible). Same hall, 12:45,
#      clear. Subject: the closing: the pemmican bill she accepted is no longer suspended and the stove has taken it up. Says: she
#      answered once, the list is hers again. Living: Nelim at the stove cooking the accepted recipe, same look, smile. Decor in
#      frame: FueledStove, Brazier lit at (184, 115), two PlantPot with Plant_Daylily fully grown, Shelf near (176, 114), TorchLamp
#      x4 lit, Column x4. Composition: foreground Nelim and the stove, background the torches and columns, tab at the left showing the
#      Pemmican row (no longer suspended). Instant: she has just unsuspended the bill and the stove has taken it up. How obtained:
#      the bill unsuspended by the step, the job DoBill forced (meat and potatoes spawned beside the stove), then 120 ticks and the
#      state check "at the building and working". Same interface scale and hidden HUD as image 1.
#      Legibility (j): the Pemmican row without its suspended mark is checked on the 268 px shrink; fallback: zoom 4.
#      Subtitle: "She answered once. The list is hers again."
#   Series: wide (1), tight (4), medium (6), an opener that places the hall (1), a closer that answers the story (6), windows
#   between them; the tab and the letter stay vanilla (pass without the optional mods). Props: vanilla only, none from a mod, so no
#   thank-you line owed (Cat asked for more hall furnishing, answer not awaited; vanilla stays the default).
#   Director pass (Opus sub-agent, 2026-11-18): KEPT: scene 4 instant no longer has her read the letter (a pawn cannot look at the interface), letter
#   is a trace not a co-subject; zoom direction fixed (3 tight, 5 medium, 7 wide); hall layout frozen across scenes 1, 4, 6 (brazier (184, 115),
#   four torches, pots near (179, 113) and (183, 118), Shelf near (176, 114)); scene 1 names the pants bill. REFUSED: one bench for the whole
#   series (needs a Complex Clothing research and new recipe steps on the tailoring bench: cost and risk, the stove carries the new-recipe
#   moment), finished pemmican or awake thrumbos in scene 6 (the bill was just accepted, nothing is produced yet), cutting the clock times.
#
# SHOTLIST v4 of 2026-11-19 (write-shotlist after the REDO verdict of score-gallery on the captures of run 0f1b: images 1, 4, 6 all
#   under 4 on a note; windows 2, 3, 5 kept as candidates, cropped). Changes to images 1, 4 and 6 only, everything else of v3 stands:
#   - Decor (b, decoration under 4 at 3.5 on image 4: one torch visible, pots cropped): the hall is furnished as a lived-in tailor's
#     and cook's corner with the vanilla Core pieces Cat photographed (catalogue bap-hearth-furniture, answer of 2026-11-18, vanilla,
#     no power, no dependency, nothing added to About.xml, so no thank-you line owed). Placed above and below the subject's column, since
#     the crop shows only about four cells beside the tab: Table2x2c near (179, 118), DiningChair near (181, 118), Armchair near (178, 116),
#     Bookcase near (180, 111), Dresser near (182, 111), Stool near (183, 112), plus the v3 Shelf, two PlantPot with Plant_Daylily
#     (the second near (183, 118)), Brazier and four TorchLamp. Same layout in images 1, 4 and 6 (hall layout frozen). Not used:
#     Carpet_MindbendA (Ideology, dependency), no vanilla rug exists.
#   - Legibility (j weak at 268 px on all three, the tab is only 525 px of 1920): the capture is CROPPED sideways after the shot to the
#     left 1100 px of the frame (full height), which holds the bills tab and the subject; rate-gallery crops it and score-gallery rates
#     the crop at 268 px (ffmpeg shrink), judging j on shapes (row marks, the SUSPENDED band), not on words. Interface scale stays 125
#     (the tab grows with the scale, so 150 would leave less map in the crop). The camera is framed so the subject lands near pixel 800:
#     image 1 centre (184, 114) at zoom 7, image 4 centre (183, 115) at zoom 4, image 6 centre (183, 115) at zoom 5.
#   - Living things (c): the thrumbos move inside the crop of image 1: Atticus at (186, 116) facing South, Rain at (186, 113) facing East.
#   - Image 4 v5 (2026-11-20, take 4 scored REDO: Nelim whole but decor all under the bills tab or off frame): furniture moved to the free map east of the stove (cells 185 to 190, rows 111 to 118), potatoes at (184, 116).
#   - Image 4 no longer carries the letter in frame (the letter stack sits at the right edge, outside the crop): the story "nothing is
#     spent without an answer" is carried by the large SUSPENDED overlay on the pemmican row (read legible on the capture of 0f1b), Nelim
#     at the stove and the furniture around her. The scenario still leaves the letter on screen; it is cut by the crop, not hidden.
#   - Paused mid-play: check on each capture that the work progress bar over Nelim and the sleep marks over the thrumbos survive the
#     HUD-hiding steps. Shared landmark of images 1 and 6: the Brazier at (184, 115); the pemmican row sits in the same screen spot in 4 and 6.
#   - Director pass v4 (Opus sub-agent, 2026-11-19): KEPT remarks 1, 2, 3, 5, 6, 7 and 8 (landmark for 1 and 6 only: the Brazier is outside
#     the crop of 4), the crop judged on shapes (4). REFUSED the 150 percent scale of 4 (the tab scales with it, the crop would hold less map).
#
# WHAT THE CAPTURES TAUGHT (read on the pictures, 2026-11-14 to 2026-11-18)
#   - Interface scale 175 percent makes the bills tab taller than the screen: its top rows leave the frame (image 1: first auto
#     bill cut, image 4 and 6: the list is an empty dark panel, the rows are above the top edge). Measured on the captures: the tab is
#     480 x scale tall and ends above the inspect pane, so it fits only up to about 150 percent. v3 uses 125 percent (tab top about
#     35 percent below the screen top), the safe side.
#   - Zoom number 3 is very tight in this build (one cell about 135 px at 1920): Atticus asleep at (187, 116) filled the top right of image 1.
#     v3 frames image 1 at zoom 7 so the hall, the columns, the torches and both thrumbos read.
#   - HUD noise in every frame (resource list, mouse-over readout, weather panel, speed controls, colonist bar, a second "Nelim" label,
#     selection brackets, the beige carpet corner of the hall). v3 hides them with the NPT presentation steps (Given: colonist bar,
#     resource readout, main bar speed controls and mouse-over readout, alerts, tooltips, selection brackets, item and name labels).
#   - Windows (images 2, 3, 5) sat over the busy hall with the full HUD and small text. v3 takes them on the bamboo backdrop
#     (window-backdrop-for-height, GALLERY.md "Relire les captures"), at interface scale 125 percent, HUD hidden, then crops them
#     sideways (Crop-SettingsShot.ps1 for the settings page; the other two by the same 5 px rule).
#
@review @gallery @requires:nelim.pickletools.screenshotmode @requires:nelim.pickletools.camerazoom @requires:nelim.pickletools.colonistrace @requires:nelim.pickletools.interfacescale @requires:nelim.pickletools.screenshotstudio @requires:nelim.pickletools.stagedecor
Feature: the captures of the Workshop page, from the pass without the optional mods

  # Image 1. The whole pitch in one picture.
  Scenario: image 1, a bills tab holding three automatic bills on a bench with many recipes
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
    And Nelim's Pickle Tools: the area from (172, 106) to (190, 124) is emptied, the roof holders kept
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Nelim's Pickle Tools: I place the decor "Brazier" at (184, 115)
    And Nelim's Pickle Tools: the decor "Brazier" at (184, 115) is lit
    And Nelim's Pickle Tools: I place the decor "PlantPot" holding "Plant_Daylily" fully grown near (179, 113)
    And Nelim's Pickle Tools: I place the decor "Shelf" near (176, 114)
    And Nelim's Pickle Tools: I place the decor "PlantPot" holding "Plant_Daylily" fully grown near (183, 118)
    And Nelim's Pickle Tools: I place the decor "Table2x2c" near (179, 118)
    And Nelim's Pickle Tools: I place the decor "DiningChair" near (181, 118)
    And Nelim's Pickle Tools: I place the decor "Armchair" near (178, 116)
    And Nelim's Pickle Tools: I place the decor "Bookcase" near (180, 111)
    And Nelim's Pickle Tools: I place the decor "Dresser" near (182, 111)
    And Nelim's Pickle Tools: I place the decor "Stool" near (183, 112)
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (181, 115)
    Then the cell (181, 115) is lit at least 50 percent
    And Bill Autopilot allows 3 automatic bills per bench
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot syncs the "HandTailoringBench" at (181, 115)
    Then Bill Autopilot has 3 bills up on the "HandTailoringBench" at (181, 115)
    When I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (181, 115)
    And Nelim's Pickle Tools: the animal "Atticus" lies at (186, 116) facing South
    And Nelim's Pickle Tools: the animal "Rain" lies at (186, 113) facing East
    And Nelim's Pickle Tools: "Nelim" wears "Apparel_Pants"
    And Nelim's Pickle Tools: "Nelim" also wears "Apparel_BasicShirt" dyed rgb (40, 110, 120)
    And Nelim's Pickle Tools: "Nelim" hairstyle is "Roo_HD_Hair_Highpony"
    And Nelim's Pickle Tools: the clothes worn by "Nelim" are logged
    And Nelim's Pickle Tools: 50 of the item "Cloth" are spawned at (182, 116)
    And Nelim's Pickle Tools: "Nelim" is forced into the job "DoBill" on the building "HandTailoringBench" at (181, 115)
    And Nelim's Pickle Tools: I let 120 ticks pass
    Then Nelim's Pickle Tools: "Nelim" is at the building "HandTailoringBench" at (181, 115) and working
    When Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (184, 114) at zoom 7
    And the bills tab of the Bill Autopilot bench "HandTailoringBench" at (181, 115) is opened
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And Nelim's Sanctuary: Nelim has her own face
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
    And Nelim's Pickle Tools: the colonist bar is hidden
    And Nelim's Pickle Tools: the resource readout is hidden
    And Nelim's Pickle Tools: the main bar, the speed controls and the mouse-over readout are hidden
    And Nelim's Pickle Tools: the alerts are hidden
    And Nelim's Pickle Tools: the tooltips are hidden
    And Nelim's Pickle Tools: the selection brackets are hidden
    And Nelim's Pickle Tools: the item and name labels are hidden
    And Nelim's Pickle Tools: the interface scale is 125 percent
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
    When Nelim's Sanctuary: I am at the sanctuary "window-backdrop-for-height"
    And Bill Autopilot's profile window for "HandTailoringBench" is opened
    And Bill Autopilot's profile window keeps open only the group of "Make_Patchleather"
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 2: the profile window, one group open, an overridden recipe"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed

  # Image 3. The one moment a lot of production can start at once, and the mod asks first, naming its count.
  Scenario: image 3, the confirmation dialog when a workbench type is switched on
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
    When Nelim's Sanctuary: I am at the sanctuary "window-backdrop-for-height"
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
    And Nelim's Pickle Tools: the colonist bar is hidden
    And Nelim's Pickle Tools: the resource readout is hidden
    And Nelim's Pickle Tools: the main bar, the speed controls and the mouse-over readout are hidden
    And Nelim's Pickle Tools: the alerts are hidden
    And Nelim's Pickle Tools: the tooltips are hidden
    And Nelim's Pickle Tools: the selection brackets are hidden
    And Nelim's Pickle Tools: the item and name labels are hidden
    And Nelim's Pickle Tools: the interface scale is 125 percent
    And Nelim's Pickle Tools: the area from (172, 106) to (190, 124) is emptied, the roof holders kept
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Nelim's Pickle Tools: I place the decor "Brazier" at (184, 115)
    And Nelim's Pickle Tools: the decor "Brazier" at (184, 115) is lit
    And Nelim's Pickle Tools: I place the decor "PlantPot" holding "Plant_Daylily" fully grown near (179, 113)
    And Nelim's Pickle Tools: I place the decor "Shelf" near (190, 114)
    And Nelim's Pickle Tools: I place the decor "PlantPot" holding "Plant_Daylily" fully grown near (188, 113)
    And Nelim's Pickle Tools: I place the decor "Table2x2c" near (179, 118)
    And Nelim's Pickle Tools: I place the decor "DiningChair" near (181, 118)
    And Nelim's Pickle Tools: I place the decor "Armchair" near (186, 116)
    And Nelim's Pickle Tools: I place the decor "Bookcase" near (187, 111)
    And Nelim's Pickle Tools: I place the decor "Dresser" near (189, 111)
    And Nelim's Pickle Tools: I place the decor "Stool" near (186, 112)
    And Bill Autopilot settings are at their defaults
    And a "FueledStove" is built at (181, 115)
    And Nelim's Pickle Tools: the building "FueledStove" at (181, 115) is fuelled
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
    When I add bill "CookMealSimple" to the "FueledStove" at (181, 115)
    And Nelim's Pickle Tools: "Nelim" wears "Apparel_Pants"
    And Nelim's Pickle Tools: "Nelim" also wears "Apparel_BasicShirt" dyed rgb (40, 110, 120)
    And Nelim's Pickle Tools: "Nelim" hairstyle is "Roo_HD_Hair_Highpony"
    And Nelim's Pickle Tools: the clothes worn by "Nelim" are logged
    And Nelim's Sanctuary: Nelim has her own face
    And Nelim's Pickle Tools: 20 of the item "RawPotatoes" are spawned at (184, 116)
    And Nelim's Pickle Tools: "Nelim" is forced into the job "DoBill" on the building "FueledStove" at (181, 115)
    And Nelim's Pickle Tools: I let 120 ticks pass
    Then Nelim's Pickle Tools: "Nelim" is at the building "FueledStove" at (181, 115) and working
    When Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (183, 115) at zoom 4
    And the bills tab of the Bill Autopilot bench "FueledStove" at (181, 115) is opened
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 4: a suspended bill for a newly researched recipe, with the letter that named it"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed

  # Image 6. The closing: she answered once, and the stove has taken the accepted recipe up.
  Scenario: image 6, the accepted recipe is no longer suspended and the stove works it
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
    And Nelim's Pickle Tools: the area from (172, 106) to (190, 124) is emptied, the roof holders kept
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 113)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 113) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (178, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (178, 117) is lit
    And Nelim's Pickle Tools: I place the decor "TorchLamp" at (184, 117)
    And Nelim's Pickle Tools: the decor "TorchLamp" at (184, 117) is lit
    And Nelim's Pickle Tools: I place the decor "Brazier" at (184, 115)
    And Nelim's Pickle Tools: the decor "Brazier" at (184, 115) is lit
    And Nelim's Pickle Tools: I place the decor "PlantPot" holding "Plant_Daylily" fully grown near (179, 113)
    And Nelim's Pickle Tools: I place the decor "Shelf" near (176, 114)
    And Nelim's Pickle Tools: I place the decor "PlantPot" holding "Plant_Daylily" fully grown near (183, 118)
    And Nelim's Pickle Tools: I place the decor "Table2x2c" near (179, 118)
    And Nelim's Pickle Tools: I place the decor "DiningChair" near (181, 118)
    And Nelim's Pickle Tools: I place the decor "Armchair" near (178, 116)
    And Nelim's Pickle Tools: I place the decor "Bookcase" near (180, 111)
    And Nelim's Pickle Tools: I place the decor "Dresser" near (182, 111)
    And Nelim's Pickle Tools: I place the decor "Stool" near (183, 112)
    And Nelim's Pickle Tools: I place the decor "Stool" near (181, 113)
    And Bill Autopilot settings are at their defaults
    And a "FueledStove" is built at (181, 115)
    And Nelim's Pickle Tools: the building "FueledStove" at (181, 115) is fuelled
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
    When Bill Autopilot's bill for "Make_Pemmican" on the "FueledStove" at (181, 115) is unsuspended
    And Nelim's Pickle Tools: "Nelim" wears "Apparel_Pants"
    And Nelim's Pickle Tools: "Nelim" also wears "Apparel_BasicShirt" dyed rgb (40, 110, 120)
    And Nelim's Pickle Tools: "Nelim" hairstyle is "Roo_HD_Hair_Highpony"
    And Nelim's Pickle Tools: the clothes worn by "Nelim" are logged
    And Nelim's Sanctuary: Nelim has her own face
    And Nelim's Pickle Tools: 20 of the item "RawPotatoes" are spawned at (184, 116)
    And Nelim's Pickle Tools: 40 of the item "Meat_Muffalo" are spawned at (184, 116)
    And Nelim's Pickle Tools: "Nelim" is forced into the job "DoBill" on the building "FueledStove" at (181, 115)
    And Nelim's Pickle Tools: I let 120 ticks pass
    Then Nelim's Pickle Tools: "Nelim" is at the building "FueledStove" at (181, 115) and working
    When Nelim's Sanctuary: I am at the sanctuary "hearth-hall"
    And Nelim's Pickle Tools: I frame the cell (183, 115) at zoom 5
    And the bills tab of the Bill Autopilot bench "FueledStove" at (181, 115) is opened
    And Nelim's Pickle Tools: the letters and the alerts are cleared from the screen
    And Nelim's Pickle Tools: the messages are cleared from the screen
    And I take a screenshot "gallery 6: the accepted recipe is no longer suspended and the stove works it"
    And I close all dialogs
    And Nelim's Pickle Tools: the decor is removed
