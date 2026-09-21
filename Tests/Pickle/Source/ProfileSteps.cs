using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// Writing and reading the configuration: the bench profiles and the three global settings.
    ///
    /// These steps set the profile directly instead of clicking through the profile window. That is
    /// deliberate and it is not a shortcut around the interface: the window's own layout, grouping,
    /// filters and pointer-only reachability are what 17-profile-window.feature photographs and a
    /// person judges. Every other scenario here is about what the ENGINE does with a configuration,
    /// so it states the configuration in one line and spends the run on the part only a running game
    /// can show.
    ///
    /// Every step text starts with "Bill Autopilot". Pickle loads the steps of every installed suite
    /// into one namespace, so two suites sharing a step text produce "Ambiguous step" and fail
    /// scenarios that are perfectly healthy.
    /// </summary>
    [PickleSteps]
    public class ProfileSteps
    {
        /// <summary>
        /// The settings are global and they are written to disk, so they outlive a scenario, a
        /// fixture load and the run itself. Without this step the second scenario of a pass inherits
        /// whatever the first left behind, and a suite that passes in order fails when a filter runs
        /// one scenario on its own.
        /// </summary>
        [Given("Bill Autopilot settings are at their defaults")]
        public void ResetToDefaults(PickleContext ctx)
        {
            var settings = Driver.Settings(ctx);

            settings.notifyNewRecipes = true;
            settings.markAutomaticBills = true;
            settings.syncIntervalTicks = 600;
            settings.maxAutoBillsPerTable = 8;

            // The dictionary itself is private: AllProfiles only reads it, and ProfileForWriting only
            // adds. Emptying it is what "clean configuration" means for this mod, so it is reached
            // once, by name, with a failure that says so.
            var field = typeof(BillAutopilotSettings)
                .GetField("profiles", BindingFlags.Instance | BindingFlags.NonPublic);
            ctx.Require(field != null,
                "BillAutopilotSettings.profiles no longer exists: the mod renamed the profile store, "
                + "so this step can no longer give a scenario a clean configuration");

            var profiles = field.GetValue(settings) as Dictionary<string, BenchProfile>;
            ctx.Require(profiles != null, "BillAutopilotSettings.profiles is not the expected dictionary");
            profiles.Clear();

            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        [Given("Bill Autopilot is switched on for {string}")]
        public void EnableBench(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            Driver.Settings(ctx).ProfileForWriting(bench).enabled = true;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        [Then("Bill Autopilot is on for {string}")]
        public void AssertEnabled(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            ctx.Assert(Driver.Settings(ctx).IsEnabled(bench),
                $"the autopilot is off for {benchDefName}");
        }

        [Then("Bill Autopilot is off for {string}")]
        public void AssertDisabled(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            ctx.Assert(!Driver.Settings(ctx).IsEnabled(bench),
                $"the autopilot is on for {benchDefName}");
        }

        [Given("Bill Autopilot default mode for {string} is {string}")]
        public void SetDefaultMode(PickleContext ctx, string benchDefName, string mode)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var profile = Driver.Settings(ctx).ProfileForWriting(bench);
            var wanted = Driver.Mode(ctx, mode);

            ctx.Require(wanted != AutoMode.Inherit,
                "a bench default cannot be 'default': it IS the default. Write 'keep in stock', "
                + "'always', 'never' or set a repeat mode from another mod instead");

            profile.defaultMode = wanted;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        /// <summary>
        /// A repeat mode belonging to another mod, named by defName rather than by label: the label
        /// is translated and the defName is not. The mode is looked up first, so a scenario naming a
        /// mode its pass did not stage fails here, naming the modes that DO exist, instead of
        /// failing three steps later on a bill that was never set.
        /// </summary>
        [Given("Bill Autopilot default mode for {string} is the repeat mode {string}")]
        public void SetDefaultCustomMode(PickleContext ctx, string benchDefName, string repeatModeDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var mode = DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(repeatModeDefName);
            ctx.Require(mode != null,
                $"no BillRepeatModeDef named '{repeatModeDefName}'. This pass loaded: "
                + string.Join(", ", DefDatabase<BillRepeatModeDef>.AllDefsListForReading
                    .ConvertAll(d => d.defName).ToArray()));

            var profile = Driver.Settings(ctx).ProfileForWriting(bench);
            profile.defaultMode = AutoMode.Custom;
            profile.defaultRepeatMode = mode.defName;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        /// <summary>
        /// A profile left pointing at a repeat mode that is not in this game - what a player gets by
        /// disabling the mod that provided it and loading the save again.
        ///
        /// Written with a defName no mod owns, so the scenario using it runs in every pass including
        /// the minimal one. It reaches the same resolution, through the same line, as the case it
        /// stands for: the profile holds a name, the lookup comes back empty, and the mod has to
        /// fall back to one of its own modes rather than put up a bill with no mode at all.
        /// </summary>
        [Given("Bill Autopilot default mode for {string} is a repeat mode no longer in this game")]
        public void SetMissingCustomMode(PickleContext ctx, string benchDefName)
        {
            const string missing = "BillAutopilot_PickleTests_NoSuchRepeatMode";
            ctx.Require(DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(missing) == null,
                $"a mod in this pass defines a BillRepeatModeDef called '{missing}', so this step no "
                + "longer sets up the missing-provider case it exists for");

            var bench = Driver.BenchDef(ctx, benchDefName);
            var profile = Driver.Settings(ctx).ProfileForWriting(bench);
            profile.defaultMode = AutoMode.Custom;
            profile.defaultRepeatMode = missing;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        [Given("Bill Autopilot keeps {int} of everything on {string}, restarting at {int}")]
        public void SetCounts(PickleContext ctx, int target, string benchDefName, int floor)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var profile = Driver.Settings(ctx).ProfileForWriting(bench);
            profile.defaultMode = AutoMode.Maintain;
            profile.targetCount = target;
            profile.floorCount = floor;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        [Given("Bill Autopilot uncountable recipes on {string} are {string}")]
        public void SetUncountableMode(PickleContext ctx, string benchDefName, string mode)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var wanted = Driver.Mode(ctx, mode);
            ctx.Require(wanted == AutoMode.Always || wanted == AutoMode.Excluded,
                "a recipe the game cannot count can only be 'always' or 'never': 'keep in stock' is "
                + "what it has no way of meaning");

            Driver.Settings(ctx).ProfileForWriting(bench).uncountableMode = wanted;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        [Given("Bill Autopilot allows {int} automatic bills per bench")]
        public void SetCap(PickleContext ctx, int cap)
        {
            Driver.Settings(ctx).maxAutoBillsPerTable = cap;
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        [Given("Bill Autopilot does not mark automatic bills")]
        public void MarkOff(PickleContext ctx)
        {
            Driver.Settings(ctx).markAutomaticBills = false;
            Driver.Mod(ctx).WriteSettings();
        }

        /// <summary>
        /// Setting a recipe back to Default in the profile window, which is how a refusal is undone.
        /// Worth its own step rather than a fresh <c>settings are at their defaults</c>: the point of
        /// the scenario that uses it is that the rest of the configuration survives, so wiping
        /// everything would prove nothing.
        /// </summary>
        [When("Bill Autopilot's rule for {string} on {string} is set back to the default")]
        public void ClearRule(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            profile.ClearRule(recipe);
            Driver.Mod(ctx).WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        /// <summary>
        /// The profile window, opened the way the workbench gizmo opens it. Used by the scenarios
        /// that photograph it: what those pictures are for - the grouping by product category, the
        /// collapsed state, the override filter and whether every control is reachable with a
        /// pointer alone - is layout, and layout is read by an eye, not asserted.
        ///
        /// Frames rather than ticks: a window force-pauses nothing here, but the layout pass is what
        /// builds the groups, and frames advance whether or not the game is running.
        /// </summary>
        [When("Bill Autopilot's profile window for {string} is opened")]
        public async System.Threading.Tasks.Task OpenProfileWindow(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            Find.WindowStack.Add(new Dialog_BenchProfile(bench));
            await ctx.WaitFrames(3);
        }

        // --- Reading the profile back --------------------------------------------------------------

        [Then("Bill Autopilot has {string} set to {string} on {string}")]
        public void AssertRecipeMode(PickleContext ctx, string recipeDefName, string mode, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            var rule = profile.RuleFor(recipe);
            var actual = rule == null ? AutoMode.Inherit : rule.mode;
            var wanted = Driver.Mode(ctx, mode);

            ctx.Assert(actual == wanted,
                $"{recipeDefName} on {benchDefName} is set to '{Driver.ModeName(actual)}', "
                + $"not '{Driver.ModeName(wanted)}'");
        }

        [Then("Bill Autopilot has no override for {string} on {string}")]
        public void AssertNoOverride(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            if (profile == null) return;

            var rule = profile.RuleFor(recipe);
            ctx.Assert(rule == null || rule.IsDefault,
                $"{recipeDefName} on {benchDefName} carries an override: mode "
                + $"'{Driver.ModeName(rule.mode)}', target {rule.targetCount}, floor {rule.floorCount}");
        }

        [Then("Bill Autopilot keeps {int} of {string} on {string}, restarting at {int}")]
        public void AssertRecipeCounts(PickleContext ctx, int target, string recipeDefName,
            string benchDefName, int floor)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            ctx.Assert(profile.TargetFor(recipe) == target,
                $"{recipeDefName} on {benchDefName} is kept at {profile.TargetFor(recipe)}, not {target}");
            ctx.Assert(profile.FloorFor(recipe) == floor,
                $"{recipeDefName} on {benchDefName} restarts at {profile.FloorFor(recipe)}, not {floor}");
        }

        /// <summary>
        /// The recipe's effective mode, resolved the way the sync pass resolves it: override, then
        /// bench default, then the two fallbacks - a repeat mode whose owner has gone, and a recipe
        /// the game cannot count. Reading it here is what makes a scenario able to say WHY a bill is
        /// absent rather than only that it is.
        /// </summary>
        [Then("Bill Autopilot would run {string} on {string} as {string}")]
        public void AssertEffectiveMode(PickleContext ctx, string recipeDefName, string benchDefName, string mode)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            bool countable = Driver.CanCount(ctx, recipe);
            var actual = profile.ModeFor(recipe, countable);
            var wanted = Driver.Mode(ctx, mode);

            ctx.Assert(actual == wanted,
                $"{recipeDefName} on {benchDefName} resolves to '{Driver.ModeName(actual)}', not "
                + $"'{Driver.ModeName(wanted)}' (the game "
                + (countable ? "can" : "cannot") + " count this recipe's product)");
        }

        [Then("Bill Autopilot can count {string}")]
        public void AssertCountable(PickleContext ctx, string recipeDefName)
        {
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.CanCount(ctx, recipe),
                $"{recipeDefName} reports no countable product, so 'keep a stock' cannot apply to it");
        }

        [Then("Bill Autopilot cannot count {string}")]
        public void AssertUncountable(PickleContext ctx, string recipeDefName)
        {
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(!Driver.CanCount(ctx, recipe),
                $"{recipeDefName} reports a countable product, so this scenario is no longer testing "
                + "the uncountable path the game's own RecipeWorkerCounter decides");
        }
    }
}
