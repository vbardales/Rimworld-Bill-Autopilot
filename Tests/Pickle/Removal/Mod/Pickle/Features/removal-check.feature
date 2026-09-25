# The second launch of the removal chain (see 21-removal-write.feature). Bill Autopilot and its own test companion are
# out of the mod list: `mod "nelim.billautopilot" is not loaded` is the line that says the launch really was without
# it, and the reason this is a scenario of a mod that does not depend on Bill Autopilot.
Feature: A game saved with Bill Autopilot, loaded without it

  Scenario: it loads and runs without the mod, and the bills it put up stay behind as ordinary bills
    Given mod "nelim.billautopilot" is not loaded
    And the save "billautopilot-with-mod" is loaded
    And game speed is fast
    When I wait 250 ticks
    Then no errors were logged
    And the engine is alive
    And the "HandTailoringBench" has 1 bills
    When I save and reload as "billautopilot-without-mod"
    Then no errors were logged
    And the engine is alive
