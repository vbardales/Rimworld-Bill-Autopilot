# Backlog - Bill Autopilot

This mod's own backlog. Opened 2026-09-26. Nothing here is started; nothing here blocks 1.0.0 unless it says so.
Anything that touches `Mod/`, `Source/` or `Tests/` changes the build the 1.0.0 passes were played on
(`4B89FBA7`), so it goes either before the `publish` (with a replay of what it touches) or into a later version.

## Order in which recipes are taken (owner, 2026-09-26)

- [ ] **Let the player sort the order in which the autopilot takes recipes.** Today the order is whatever the
  bench offers, and when the per-bench cap is reached the recipes that come last are the ones left out (seen in
  the tests: a stove filled its allowance with cooking recipes before reaching pemmican). The order only matters
  when the cap bites, but then it decides what the player gets.
  Open questions, to settle with the owner before any code:
  - **Which sorts:** the game's own order, alphabetical, by product group, by hand (drag a recipe up or down), or
    pinned first (a short list that always goes before the rest), or by need (lowest stock against target first).
  - **Where it lives:** the profile window is the natural place, per workbench type, next to the per-recipe
    override; controls reachable with a pointer alone (no keyboard, Steam Deck), like the rest.
  - **What it must not break:** hand-placed bills still win; a recipe left out stays left out; a recipe unlocked
    later still arrives suspended, wherever it falls in the order; the order is per bench type and is saved with
    the profile, without a class of its own in the save.
  - **Neighbours:** Nice Bill Tab and Better Workbench Management order the tab's own rows; the autopilot's order
    is about which bills are put up, not how they are listed, and the two must not be confused.
  - **Tests:** a scenario with a cap lower than the recipe count and two orders, checking which recipes are up
    each time, plus French and English labels.
  To read first: `Source/Runtime/AutoBillSync.cs` (how recipes are chosen up to the cap) and
  `Source/UI/Dialog_BenchProfile.cs` (where the control would sit).

## Rule for the 1.0.0 (owner, 2026-09-26)

A correction goes into 1.0.0; a check that only verifies behaviour already right goes into 1.0.1.

## Integration with No Max Bills (owner, 2026-09-26): corrected in 1.0.0, to be replayed

- [x] **A game with No Max Bills but without Better Workbench Management kept our ceiling at 15.** No Max Bills:
  Redux lifts the limit only in the interface (`BillStack.DoListing`, `ITab_Bills.FillTab`), and raises BWM's own
  ceiling from 125 to `int.MaxValue`. Done in code: `NoMaxBillsCompat` finds it by its type
  `NoMaxBills.Patch_BillStack_DoListing` (in the Redux and in the original by KiameV); `MaxBills` asks BWM first
  and No Max Bills when BWM does not answer. With no limit the settings slider spans 1 to 100 and the sentence says
  "the game sets no limit" instead of printing 2147483647 (new keys `Settings.MaxBillsFree.One/.Many`, English and
  French). Scenarios: `26-no-max-bills.feature`, pass `avec-nomaxbills` (`wsl-deps.avec-nomaxbills.map`).
  **Played and green, `8240`, 2026-09-27, 2 of 2**, after `92fe`'s first attempt failed before any scenario ran on a
  filter-syntax mistake of this session's (fixed: `-Filter "feature 26"` is not valid, the working form is the bare
  file name `26-no-max-bills`). `Player.log` confirms the fallback: `Integrations: ... No Max Bills found. Bill cap
  per workbench: 2147483647`, and "a cap above fifteen is honoured" passed with 16 bills genuinely standing.

  **The Harmony crash is real, still happens, and is answered.** No Max Bills: Redux's
  `RaiseBillCountLimitForImprovedWorkbenches` finds its Harmony target by reflecting into `ImprovedWorkbenches.Main`
  (BWM); absent BWM, that reflection yields no method, Harmony refuses the empty `[HarmonyTargetMethods]` result
  (`Undefined target method`), and the mod's own constructor throws, logged as `[ERROR] Error while instantiating a
  mod of type NoMaxBillsRedux.NoMaxBillsReduxMod` — present in `8240`'s log too, unchanged. **Answered by the green
  result: `RaiseBillCountLimitForVanillaBillStackMethods` (the patch that actually lifts the game's 15-bill interface
  limit) ran anyway** — `Harmony.PatchAll()`'s type-enumeration order happens to reach it before the one that throws,
  in this build. So No Max Bills: Redux's own interface limit works even though it logs an error on every load
  without BWM. Worth a report to its author (`justharry.nomaxbillsredux`, Workshop 3526216885) for the error itself —
  owner's call whether to send it — but it blocks nothing here.
  **Still to do:** the description says the raised ceiling is honoured "when No Max Bills is present" through BWM
  only; reword it now that the fallback is proven, and check that feature 10 or the settings page does not read the
  old "the game allows" sentence in a pass that has both No Max Bills and BWM.

## Loose ends of the publication

- [ ] **Choose Your Recipe has no scenario, and reading it turned up one trap (2026-09-26).** The description says
  "nothing needed" and the thank-you draft says it was read, not run. What its code does (decompiled from
  `1.6/Assemblies/ChooseYourRecipe.dll`, Workshop 3263007587): the choice is kept in the mod's own settings (per
  workbench type, by defName), and `DisabledRecipesComponent.ApplyDisabledRecipes` removes the recipes from
  `ThingDef.allRecipesCached`, rebuilt when a game is loaded and each time its window is closed. The autopilot reads
  `table.def.AllRecipes` (`AutoBillSync.cs:58`), so a disabled recipe is never seen, and an automatic bill whose
  recipe has just been disabled is taken down by the "recipe has left the workbench" loop (`AutoBillSync.cs:141`).
  Both hold from the code, **neither was played**.
  - [x] **Trap, corrected in 1.0.0, replayed green 2026-09-27 (`b2c7`, 13 of 13).** a recipe announced (suspended bill and letter, so "pending") and
    then disabled in Choose Your Recipe before the player answers had its bill taken down by that loop, but
    `pending` was not cleared. Once enabled again, `IsPending` was true and no bill existed, so the recipe was
    skipped for good, on any bench of that type. Now `AutoBillSync.Remove` calls `BillAutopilotState.Unannounce`:
    a bill taken down by the autopilot itself forgets the open question, and the recipe is announced afresh when it
    returns (the player's own deletion is another path and still means refusal). The same trap existed when a
    pending bill was removed for another reason (recipe hidden, excluded in the profile, hand-placed bill, bench
    switched off); the one change covers them. Scenario: `25-recipe-taken-off-the-bench.feature`, which edits
    `ThingDef.AllRecipes` the way that mod does (`RecipeListSteps.cs`) and so runs in every pass. **Played and
    green**: "the announced recipe leaves and returns, and the question is asked again", `docs/runs/evidence/
    2026-09-27-replay-fixes-v2`.
  - **Behaviour, kept:** a recipe disabled when the autopilot was switched on is not absorbed by the opening seed;
    enabled later, it arrives suspended with a letter like a newly researched one (nothing is spent unasked).
  - **For 1.0.1, verification only:** a running bill taken down when its recipe leaves; a recipe disabled before the
    first pass gets no bill; a hand-placed bill on a recipe that leaves is left alone; and a real pass with Choose
    Your Recipe itself, setting its disabled list and reloading its component (the mod is staged in
    `avec-facultatifs`, no scenario reads it). Until then the description keeps saying "nothing needed" and the
    thank-you says it was read, not run.
- [x] **`Mod/README.template.md` is gone.** DONE 2026-09-27: read both `README.md` and `TESTING.md` in full (786
  lines together), neither mentions it. Nothing to fix before the commit.
- [ ] **`TESTING.md` is badly stale (found 2026-09-27, not fixed).** It still reads "Nothing has been released...
  Never run once: everything written since" and marks most scenarios "Confirmed once" or "Never run", though
  `STATUS.md` and `docs/runs/pickle-runs.md` show most of the suite green since 2026-09-25 (French passes included).
  It also says Choose Your Recipe is "Not applicable" to test because "nothing of this mod's is left to assert",
  which the pending-trap finding above contradicts. A rewrite against the current `STATUS.md`/`pickle-runs.md` is
  owed before `tested`, but is a separate, larger pass — not attempted here to stay in scope of the README.template
  check.

## Suite audit: the setter/assertion text-collision bug class (2026-09-27)

- [x] **Checked every other feature for the same defect that broke `5d66`'s image 2.** DONE: Pickle binds a step by
  its literal text, ignoring the `Given`/`When`/`Then` word written in the feature, so a line meant to *set*
  something that happens to share its exact wording with a `[Then]` assertion elsewhere silently binds to the
  assertion and sets nothing — `Check-Steps.ps1` cannot catch this, since it only proves every line resolves to
  *some* step, not the *intended* one. Read all 26 feature files against the full list of `[Then(...)]` texts across
  every step file (`ProfileSteps`, `BillSteps`, `ActivationSteps`, `StateSteps`, `LetterSteps`, `ShortcutSteps`,
  `RemovalSteps`, `BwmDetailSteps`, `IntegrationSteps`). One other line uses the exact assertion text that bit us
  (`19-profile-window.feature:42`, `Then Bill Autopilot keeps 200 of "Make_Patchleather"...`), but correctly: it
  follows a real setter (`is set to keep 200, restarting at 100`, `BillSteps.Retarget`) and a sync, asserting that
  feature 07's drift capture wrote the override to the profile — `Then` is the right word there. No other collision
  found. Nothing to fix.
