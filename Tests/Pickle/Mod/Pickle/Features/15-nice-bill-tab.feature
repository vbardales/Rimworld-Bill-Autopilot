# TESTING.md scenario 15. The most dangerous interaction in the mod.
#
# Nice Bill Tab redraws the whole tab from a cached list of the rows it shows, and reorders from
# that list before writing back into the stack. A stale entry there is not cosmetic: dragging the
# rows can put a deleted bill back.
#
# What can be asserted, and what is asserted here, is the CAUSE. Every time the autopilot puts a
# bill up or takes one down, it sets the flag that tells that list to rebuild. If the flag is not
# set, the list is stale and the drag is dangerous - so the flag is the line that changes when this
# integration breaks, and photographing the tab instead would leave a failure with nowhere to point.
#
# The drag itself stays manual, and Tests/Pickle/README.md says so. It is a gesture a person
# performs inside another mod's interface, and a Pickle click lands on whatever window owns the
# point: a drag driven from here would be testing Nice Bill Tab, not this mod.
@requires:Andromeda.NiceBillTab
Feature: Nice Bill Tab's cached row list is told whenever the autopilot changes the stack

  Background:
    Given the save "test-colony" is loaded
    And Bill Autopilot settings are at their defaults
    And a "HandTailoringBench" is built at (140, 155)
    And Bill Autopilot is switched on for "HandTailoringBench"
    And Bill Autopilot only takes "Make_Patchleather" on "HandTailoringBench"
    And Bill Autopilot keeps 50 of everything on "HandTailoringBench", restarting at 25

  Scenario: the mod found it
    Then Bill Autopilot found Nice Bill Tab

  # This scenario has to START from a bench with no bill. The Background sets no stock, and with nothing in
  # stock the bill is wanted at once: the game keeps ticking between steps, and the autopilot's own tick sync
  # put it up before the cache was marked up to date, so the explicit sync found it already there and raised
  # nothing. That is why the first two runs of this scenario were red (2026-09-25; the take-down scenario passed
  # on the same bridge, and Nice Bill Tab resets the flag only when its tab is drawn, which nothing here does).
  # The stock is therefore raised above the target first and the bench synced, so that whatever the tick put up
  # is taken down; only then is the cache marked up to date, and the stock lowered to bring the bill up.
  Scenario: putting a bill up tells the list to rebuild
    Given the Bill Autopilot test stockpile holds 60 "Leather_Patch"
    And Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Nice Bill Tab's row cache is marked as up to date
    And the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Nice Bill Tab's row cache has been told to rebuild

  # The dangerous direction. A row for a bill that no longer exists is what a drag can resurrect, so
  # the flag matters more on the way down than on the way up.
  Scenario: taking a bill down tells the list to rebuild
    Given the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has a bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)

    Given Nice Bill Tab's row cache is marked as up to date
    And the Bill Autopilot test stockpile holds 60 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    Then Bill Autopilot has no bill up for "Make_Patchleather" on the "HandTailoringBench" at (140, 155)
    And Nice Bill Tab's row cache has been told to rebuild
    And no errors were logged

  # @review: the tab as Nice Bill Tab draws it, for a person to look at. Its green says the trip
  # happened, nothing about what the image shows - in particular, whether the automatic marker
  # survives a tab that builds its rows its own way. That question is what the picture is for.
  @review
  Scenario: the replaced tab, for a person to look at
    Given the Bill Autopilot test stockpile holds 10 "Leather_Patch"
    When Bill Autopilot syncs the "HandTailoringBench" at (140, 155)
    And I add bill "Make_Apparel_Pants" to the "HandTailoringBench" at (140, 155)
    And the bills tab of the Bill Autopilot bench "HandTailoringBench" at (140, 155) is opened
    And I take a screenshot "Nice Bill Tab: does the automatic marker survive a redrawn tab"
