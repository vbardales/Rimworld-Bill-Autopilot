# TESTING.md scenario 18. The constraint that shaped the whole interface.
#
# This mod is meant to be played with a pointer alone - the Steam Deck case - so there is no search
# field anywhere in the profile window, deliberately. What replaces it is grouping by product
# category, collapsible groups, and a filter for the recipes already overridden. Whether that is
# actually enough to cross a workshop of sixty recipes with a thumbstick is a judgement about a
# layout, and no assertion can make it.
#
# So these scenarios are @review: they set a bench up, open the window and attach pictures. Their
# green says the trip happened and the window drew without throwing. It says nothing about what the
# images show, and it must not be counted as a visual check performed.
#
# What a person is being asked to look at is written into each screenshot's name. The last one is
# the one that matters most in the French pass: in developer mode - and every Pickle run is in
# developer mode - a key missing from the active language comes back as accented gibberish rather
# than as clean English, so a missing translation is visible in the picture instead of invisible.
Feature: the profile window, as a person has to read it

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And I close all dialogs

  @review
  Scenario: the window as it opens, grouped by product category
    When Bill Autopilot's profile window for "HandTailoringBench" is opened
    And I take a screenshot "profile window: groups, counters and the collapse control - every one reachable with a pointer"
    And I close all dialogs

  # With an override set, the window has something to show in its filter and in its clear control,
  # and one row differs from the bench default. An empty window would photograph well and prove
  # nothing.
  @review
  Scenario: the window with one recipe overridden
    Given Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    When Bill Autopilot's bill for "Make_Patchleather" on the "HandTailoringBench" at (140, 155) is set to keep 200, restarting at 100
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot keeps 200 of "Make_Patchleather" on "HandTailoringBench", restarting at 100
    When Bill Autopilot's profile window for "HandTailoringBench" is opened
    And I take a screenshot "profile window: one overridden recipe, the filter and the clear control"
    And I close all dialogs

  # The uncountable case has a row of its own kind, with its own setting and its own tooltip, and it
  # is gathered under its own group at the end of the list.
  @review
  Scenario: the window on a bench whose recipes the game cannot count
    Given a "ButcherSpot" is built at (146, 155)
    And Bill Autopilot is switched on for "ButcherSpot"
    When Bill Autopilot's profile window for "ButcherSpot" is opened
    And I take a screenshot "profile window: a bench with an uncountable recipe, and its own setting"
    And I close all dialogs
    And no errors were logged
