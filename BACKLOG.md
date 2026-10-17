# Backlog - Bill Autopilot

Mod own backlog. Opened 2026-09-26. Nothing started; nothing block 1.0.0 unless say so.
Anything touch `Mod/`, `Source/` or `Tests/` change build 1.0.0 passes played on
(`4B89FBA7`), so go before `publish` (with replay of what touch) or into later version.

## Order in which recipes are taken (owner, 2026-09-26)

- [ ] **Let player sort order autopilot take recipes.** Today order = whatever
  bench offer, and when per-bench cap reached, last recipes left out (seen in
  tests: stove filled allowance with cooking recipes before reach pemmican). Order only matter
  when cap bite, but then decide what player get.
  Open questions, settle with owner before any code:
  - **Which sorts:** game own order, alphabetical, by product group, by hand (drag recipe up or down), or
    pinned first (a short list that always goes before the rest), or by need (lowest stock against target first).
  - **Where it lives:** profile window natural place, per workbench type, next to per-recipe
    override; controls reachable with a pointer alone (no keyboard, Steam Deck), like the rest.
  - **What it must not break:** hand-placed bills still win; recipe left out stays left out; recipe unlocked
    later still arrives suspended, wherever it falls in the order; the order is per bench type and is saved with
    the profile, without a class of its own in the save.
  - **Neighbours:** Nice Bill Tab and Better Workbench Management order tab own rows; autopilot order
    is about which bills are put up, not how they are listed, and the two must not be confused.
  - **Tests:** scenario with cap lower than recipe count and two orders, check which recipes up
    each time, plus French and English labels.
  To read first: `Source/Runtime/AutoBillSync.cs` (how recipes chosen up to cap) and
  `Source/UI/Dialog_BenchProfile.cs` (where control would sit).

## Rule for the 1.0.0 (owner, 2026-09-26)

Correction go into 1.0.0; check only verify behaviour already right go into 1.0.1.

## Integration with No Max Bills (owner, 2026-09-26): corrected in 1.0.0, to be replayed

- [x] **Game with No Max Bills but without Better Workbench Management kept our ceiling at 15.** No Max Bills:
  Redux lift limit only in interface (`BillStack.DoListing`, `ITab_Bills.FillTab`), and raise BWM own
  ceiling from 125 to `int.MaxValue`. Done in code: `NoMaxBillsCompat` find it by its type
  `NoMaxBills.Patch_BillStack_DoListing` (in Redux and in original by KiameV); `MaxBills` ask BWM first
  and No Max Bills when BWM not answer. With no limit settings slider span 1 to 100 and sentence say
  "the game sets no limit" instead of print 2147483647 (new keys `Settings.MaxBillsFree.One/.Many`, English and
  French). Scenarios: `26-no-max-bills.feature`, pass `avec-nomaxbills` (`wsl-deps.avec-nomaxbills.map`).
  **Played and green, `8240`, 2026-09-27, 2 of 2**, after `92fe` first attempt failed before any scenario ran on
  filter-syntax mistake of this session (fixed: `-Filter "feature 26"` not valid, working form is bare
  file name `26-no-max-bills`). `Player.log` confirm fallback: `Integrations: ... No Max Bills found. Bill cap
  per workbench: 2147483647`, and "a cap above fifteen is honoured" passed with 16 bills genuinely standing.

  **Harmony crash real, still happen, and answered.** No Max Bills: Redux
  `RaiseBillCountLimitForImprovedWorkbenches` find its Harmony target by reflecting into `ImprovedWorkbenches.Main`
  (BWM); absent BWM, reflection yield no method, Harmony refuse empty `[HarmonyTargetMethods]` result
  (`Undefined target method`), and mod own constructor throw, logged as `[ERROR] Error while instantiating a
  mod of type NoMaxBillsRedux.NoMaxBillsReduxMod`: present in `8240` log too, unchanged. **Answered by green
  result: `RaiseBillCountLimitForVanillaBillStackMethods` (patch that actually lift game 15-bill interface
  limit) ran anyway**: `Harmony.PatchAll()` type-enumeration order happen reach it before one that throw,
  in this build. So No Max Bills: Redux own interface limit work even though log error on every load
  without BWM. Worth report to its author (`justharry.nomaxbillsredux`, Workshop 3526216885) for error itself,
  owner call whether send: but block nothing here.
  **Still to do:** description say raised ceiling honoured "when No Max Bills is present" through BWM
  only; reword now fallback proven, and check feature 10 or settings page not read
  old "the game allows" sentence in pass with both No Max Bills and BWM.

## Loose ends of the publication

- [ ] **Choose Your Recipe has no scenario, and reading it turn up one trap (2026-09-26).** Description say
  "nothing needed" and thank-you draft say it read, not run. What its code do (decompiled from
  `1.6/Assemblies/ChooseYourRecipe.dll`, Workshop 3263007587): choice kept in mod own settings (per
  workbench type, by defName), and `DisabledRecipesComponent.ApplyDisabledRecipes` remove recipes from
  `ThingDef.allRecipesCached`, rebuilt when game loaded and each time its window closed. Autopilot read
  `table.def.AllRecipes` (`AutoBillSync.cs:58`), so disabled recipe never seen, and automatic bill whose
  recipe just disabled taken down by "recipe has left the workbench" loop (`AutoBillSync.cs:141`).
  Both hold from code, **neither played**.
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
  - **Behaviour, kept:** recipe disabled when autopilot switched on not absorbed by opening seed;
    enabled later, it arrives suspended with a letter like a newly researched one (nothing is spent unasked).
  - **For 1.0.1, verification only:** running bill taken down when recipe leave; recipe disabled before
    first pass gets no bill; a hand-placed bill on a recipe that leaves is left alone; and a real pass with Choose
    Your Recipe itself, setting its disabled list and reloading its component (the mod is staged in
    `avec-facultatifs`, no scenario reads it). Until then the description keeps saying "nothing needed" and the
    thank-you says it was read, not run.
- [x] **`Mod/README.template.md` is gone.** DONE 2026-09-27: read both `README.md` and `TESTING.md` in full (786
  lines together), neither mention it. Nothing to fix before commit.
- [ ] **`TESTING.md` badly stale (found 2026-09-27, not fixed).** Still read "Nothing has been released...
  Never run once: everything written since" and mark most scenarios "Confirmed once" or "Never run", though
  `STATUS.md` and `docs/runs/pickle-runs.md` show most of suite green since 2026-09-25 (French passes included).
  Also say Choose Your Recipe "Not applicable" to test because "nothing of this mod's is left to assert",
  which pending-trap finding above contradict. Rewrite against current `STATUS.md`/`pickle-runs.md` owed
  before `tested`, but separate, larger pass: not attempted here to stay in scope of README.template
  check.

## Suite audit: the setter/assertion text-collision bug class (2026-09-27)

- [x] **Checked every other feature for same defect that broke `5d66` image 2.** DONE: Pickle bind step by
  its literal text, ignore `Given`/`When`/`Then` word written in feature, so line meant to *set*
  something that happen share exact wording with `[Then]` assertion elsewhere silently bind to
  assertion and set nothing: `Check-Steps.ps1` cannot catch this, since only prove every line resolve to
  *some* step, not *intended* one. Read all 26 feature files against full list of `[Then(...)]` texts across
  every step file (`ProfileSteps`, `BillSteps`, `ActivationSteps`, `StateSteps`, `LetterSteps`, `ShortcutSteps`,
  `RemovalSteps`, `BwmDetailSteps`, `IntegrationSteps`). One other line use exact assertion text that bit us
  (`19-profile-window.feature:42`, `Then Bill Autopilot keeps 200 of "Make_Patchleather"...`), but correctly:
  follow real setter (`is set to keep 200, restarting at 100`, `BillSteps.Retarget`) and sync, asserting that
  feature 07 drift capture wrote override to profile: `Then` right word there. No other collision
  found. Nothing to fix.