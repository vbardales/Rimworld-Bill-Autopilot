# No Max Bills, alone. The pass avec-nomaxbills mounts No Max Bills: Redux and nothing else, in particular not
# Better Workbench Management.
#
# Redux lifts the game's limit of fifteen bills from the interface and defines the type that tells other mods it
# is there. The autopilot read the game's constant unless Better Workbench Management was present to report a
# higher ceiling, so with No Max Bills alone its cap stopped at fifteen while the interface allowed more. The
# correction asks for No Max Bills itself. 10-bill-cap.feature reads the ceiling live and covers the case with
# Better Workbench Management (avec-facultatifs); this is the case without it.
@requires:justharry.nomaxbillsredux
Feature: No Max Bills alone lifts the ceiling the cap is measured against

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)

  Scenario: the ceiling is above the game's fifteen
    Then Bill Autopilot's bill ceiling is above 15

  Scenario: a cap above fifteen is honoured
    Given Bill Autopilot allows 16 automatic bills per bench
    And Bill Autopilot default mode for "HandTailoringBench" is "always"
    And Bill Autopilot is switched on for "HandTailoringBench"
    Then Bill Autopilot has more than 16 recipes to take on "HandTailoringBench"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has 16 bills up on the "HandTailoringBench" at (140, 155)
    And Bill Autopilot leaves room for another bill on the "HandTailoringBench" at (140, 155)
    And no errors were logged
