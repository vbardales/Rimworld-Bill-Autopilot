# TESTING.md scenario 1, the part a program can answer.
#
# Every other scenario in this suite depends on this one: if the Harmony patches did not take, a
# bench simply never gets a bill and every failure below would point at the wrong thing.
#
# No save is loaded. Mods and defs are settled before a game exists, so these run at the main menu
# in about a second, against ten to fifteen for a scenario that loads the fixture.
Feature: Bill Autopilot loads, and says what it found around it

  Scenario: the mod is loaded, after Harmony
    Then mod "nelim.billautopilot" is loaded
    And mod "nelim.billautopilot" loads after "brrainz.harmony"

  # The startup constructor installs the Dubs Mint Menus patch and writes the integration line, and
  # PatchAll runs the seven attribute patches. A throw in any of them is logged rather than fatal,
  # so without this check a half-patched mod would go on to fail the next fifteen scenarios one by
  # one instead of failing here once.
  Scenario: loading logged no error
    Then no errors were logged

  # The optional shortcut RIMMSQOL and its kind are meant to be able to reveal. Its default
  # visibility is checked in 18, in a game, because the main bar's worker has no opinion at the
  # main menu.
  Scenario: the hidden settings shortcut is declared
    Then def "BillAutopilot_Settings" of type "MainButtonDef" exists

  # The one integration check that is correct in every pass. Asserting "found" would fail the pass
  # that stages nothing and asserting "not found" would fail the pass that stages everything; what
  # has to hold in both is that the mod reports a neighbour found exactly when it is loaded.
  #
  # A divergence means one of two different things and the failure says which: loaded but not found
  # is a bridge gone dead - the neighbour renamed what this code reaches by reflection, the feature
  # is lost in silence and nothing crashes - and that is the failure most likely to arrive with
  # someone else's update.
  Scenario: what the mod reports found is what this pass loaded
    Then Bill Autopilot's integration report matches the mods this pass loaded
