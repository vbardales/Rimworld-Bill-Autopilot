using System.Linq;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// Switching a workbench type on, and the question it asks first.
    ///
    /// This is the one moment where a great deal of production can start in a single click, so the
    /// mod names how many recipes it is about to take and at what target. The scenarios here go
    /// through <c>BenchActivation.Toggle</c>, which is the single path the gizmo, the settings page
    /// and the profile window all end up in - a step that set <c>profile.enabled</c> would skip the
    /// very dialog it is meant to watch.
    /// </summary>
    [PickleSteps]
    public class ActivationSteps
    {
        [When("Bill Autopilot's toggle is used to switch {string} on")]
        public void ToggleOn(PickleContext ctx, string benchDefName)
        {
            Driver.Toggle(ctx, Driver.BenchDef(ctx, benchDefName), turnOn: true);
        }

        [When("Bill Autopilot's toggle is used to switch {string} off")]
        public void ToggleOff(PickleContext ctx, string benchDefName)
        {
            Driver.Toggle(ctx, Driver.BenchDef(ctx, benchDefName), turnOn: false);
        }

        /// <summary>
        /// The whole sentence is rebuilt from the mod's own keys and compared, rather than searched
        /// for a number. Three reasons. The count is an argument inside a translated sentence, so a
        /// substring search for "25" would also match a target of 250. Rebuilding through
        /// <c>.Translate()</c> holds in whatever language the pass runs in, which is the point of
        /// running this suite twice. And the count is taken from the mod's own intake calculation,
        /// so what the check really compares is the number the dialog SHOWS against the number of
        /// recipes it is about to take - which is the promise the dialog makes.
        ///
        /// The scenario therefore never spells a count. It cannot: how many recipes a workbench has
        /// available depends on the fixture's research, and a scenario carrying a literal would go
        /// red the day a DLC adds a recipe.
        /// </summary>
        [Then("Bill Autopilot asks before taking the recipes it would take on {string}")]
        public void AssertConfirmation(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var dialog = OpenConfirmation();
            ctx.Assert(dialog != null,
                "no confirmation is open: switching a workbench type on absorbed everything already "
                + "unlocked without asking, which is the one thing this dialog exists to prevent");

            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            int taken = Driver.RecipesTaken(ctx, bench, profile);
            ctx.Assert(taken > 0,
                $"the autopilot would take no recipe at all on {benchDefName}, so this scenario proves "
                + "nothing about a dialog that exists to warn about a large intake");
            TaggedString count = RecipeCount.Phrase(taken);

            TaggedString expected;
            if (profile.defaultMode == AutoMode.Always)
            {
                expected = "BillAutopilot.Confirm.BodyAlways".Translate(count, bench.LabelCap);
            }
            else if (profile.defaultMode == AutoMode.Custom
                     && DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(profile.defaultRepeatMode ?? "") is
                         BillRepeatModeDef mode && mode != null)
            {
                expected = "BillAutopilot.Confirm.BodyCustom".Translate(count, bench.LabelCap, mode.LabelCap);
            }
            else
            {
                expected = "BillAutopilot.Confirm.BodyMaintain".Translate(count, bench.LabelCap, profile.targetCount);
            }

            ctx.Assert(dialog.text == expected.Resolve(),
                $"the confirmation reads:\n  {dialog.text}\nand the intake it is about to perform is:\n"
                + $"  {expected.Resolve()}\nA difference here means the dialog announces one thing and "
                + "the autopilot does another");
        }

        /// <summary>
        /// A guard, not an assertion about the mod: several scenarios only mean something on a bench
        /// that has more recipes available than the cap under test. Stated in the scenario so that a
        /// fixture whose research has moved fails by saying so, instead of passing a cap check that
        /// was never exercised.
        /// </summary>
        [Then("Bill Autopilot has more than {int} recipes to take on {string}")]
        public void AssertEnoughRecipes(PickleContext ctx, int floor, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            int count = Driver.RecipesTaken(ctx, bench, profile);
            ctx.Assert(count > floor,
                $"{benchDefName} offers {count} recipes the autopilot would take, which is not more "
                + $"than {floor}: this scenario cannot show a cap it never reaches. Pick a bench with "
                + "more recipes available in this fixture, or lower the cap");
        }

        /// <summary>
        /// What the confirmation announced against what the engine then did, which is the only way to check
        /// that they agree. Comparing the announced number with the mod's own calculation would agree with
        /// itself: the confirmation and the sync pass once used different countability rules, and for
        /// butchering the dialog announced one recipe while the engine put up two. Counted from the bills
        /// actually standing on a real bench.
        ///
        /// It only means something when every recipe the mod takes gets a bill straight away, so the scenario
        /// sets a target far above any stock.
        /// </summary>
        [Then("Bill Autopilot has put up a bill for every recipe its confirmation announced on the {string} at \\({int}, {int}\\)")]
        public void AssertAnnouncedIsTaken(PickleContext ctx, string benchDefName, int x, int z)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null, $"{benchDefName} has no profile at all");

            int announced = Driver.RecipesTaken(ctx, bench, profile);

            var state = Driver.State(ctx);
            int actual = 0;
            var bills = table.billStack.Bills;
            for (int i = 0; i < bills.Count; i++)
            {
                if (state.IsAuto(bills[i])) actual++;
            }

            ctx.Assert(actual == announced,
                $"the confirmation announced {announced} recipes on the {benchDefName} and the autopilot put "
                + $"up {actual} bills: {Driver.Describe(ctx, table)}. When these differ, the window and the "
                + "sync pass are deciding countability by different rules");
        }

        /// <summary>
        /// The line the settings page draws beside a workbench type, read from the mod itself. A workbench
        /// with one recipe used to read "1 recipes" (and "1 recettes" in French). The forms are compared
        /// through the mod's own keys, so the check holds in whichever language the pass runs in: the line
        /// must carry the singular phrase and must not carry the plural phrase built for a count of one.
        /// The bench needs exactly one recipe for that to mean anything, and says so if it does not.
        /// </summary>
        [Then("Bill Autopilot's settings line for {string} counts its recipes in the singular")]
        public void AssertSingularLine(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            ctx.Require(bench.AllRecipes.Count == 1,
                $"{benchDefName} has {bench.AllRecipes.Count} recipes here, not one: this scenario needs a "
                + "workbench type with a single recipe to prove anything about the singular form");

            var profile = Driver.Settings(ctx).ProfileFor(bench);
            string line = BillAutopilotMod.Summary(bench, profile);
            string singular = "BillAutopilot.Recipes.One".Translate().Resolve();
            string pluralForOne = "BillAutopilot.Recipes.Many".Translate(1).Resolve();

            ctx.Assert(line.Contains(singular),
                $"the settings line reads \"{line}\" and does not contain \"{singular}\"");
            ctx.Assert(!line.Contains(pluralForOne),
                $"the settings line reads \"{line}\": a count of one is written with the plural form "
                + $"\"{pluralForOne}\"");
        }

        /// <summary>
        /// The same check for the overridden count on the line of a workbench type that is on: French writes
        /// "1 surchargée", and used to write "1 surchargées". Needs exactly one override on the bench, which
        /// the scenario sets up, and says so if it does not hold.
        /// </summary>
        [Then("Bill Autopilot's settings line for {string} counts its one override in the singular")]
        public void AssertSingularOverride(PickleContext ctx, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var profile = Driver.Settings(ctx).ProfileFor(bench);
            ctx.Require(profile != null && profile.enabled && profile.OverrideCount == 1,
                $"{benchDefName} must be on with exactly one override for this step, it has "
                + $"{(profile == null ? "no profile" : profile.enabled ? profile.OverrideCount + " overrides" : "the autopilot off")}");

            string line = BillAutopilotMod.Summary(bench, profile);
            string singular = "BillAutopilot.Overridden.One".Translate().Resolve();
            string pluralForOne = "BillAutopilot.Overridden.Many".Translate(1).Resolve();

            ctx.Assert(line.Contains(singular),
                $"the settings line reads \"{line}\" and does not contain \"{singular}\"");
            ctx.Assert(!line.Contains(pluralForOne) || pluralForOne == singular,
                $"the settings line reads \"{line}\": one override is written with the plural form "
                + $"\"{pluralForOne}\"");
        }

        [Then("Bill Autopilot asks nothing")]
        public void AssertNoConfirmation(PickleContext ctx)
        {
            var dialog = OpenConfirmation();
            ctx.Assert(dialog == null,
                $"a confirmation is open, reading:\n  {dialog?.text}\nThe question is about the opening "
                + "intake, which has already happened in this game, so it must not be asked twice");
        }

        /// <summary>
        /// Accepting runs the dialog's own accept action, which is exactly what a click on its button
        /// runs. Not clicked for real: the button is labelled in the game's language, and a step
        /// spelling the English label would fail every French pass. Pickle also warns that a real
        /// click goes to whatever window owns the point, so another mod's window sitting over the
        /// dialog would report a dead button instead of a covered one.
        /// </summary>
        [When("the Bill Autopilot confirmation is accepted")]
        public void Accept(PickleContext ctx)
        {
            var dialog = OpenConfirmation();
            ctx.Require(dialog != null, "no confirmation is open to accept");
            ctx.Require(dialog.buttonAAction != null,
                "the confirmation has no accept action: Dialog_MessageBox.CreateConfirmation no longer "
                + "puts it on buttonAAction, so this step needs updating");

            dialog.buttonAAction();
            dialog.Close(doCloseSound: false);
        }

        /// <summary>
        /// Refusing closes the dialog without running its action, which is what the second button and
        /// the Escape key both do. Nothing must change.
        /// </summary>
        [When("the Bill Autopilot confirmation is refused")]
        public void Refuse(PickleContext ctx)
        {
            var dialog = OpenConfirmation();
            ctx.Require(dialog != null, "no confirmation is open to refuse");
            dialog.Close(doCloseSound: false);
        }

        /// <summary>
        /// The mod's confirmation, told apart from any other message box on the stack by its title,
        /// which is rebuilt from the mod's own key rather than matched on English text.
        /// </summary>
        private static Dialog_MessageBox OpenConfirmation()
        {
            var stack = Find.WindowStack;
            if (stack == null) return null;

            return stack.Windows.OfType<Dialog_MessageBox>()
                .FirstOrDefault(d => d.title != null && d.title == ExpectedTitleOfAnyBench(d.title));
        }

        /// <summary>
        /// The title carries the workbench label as its only argument, so it cannot be rebuilt
        /// without knowing which bench - but it CAN be recognised: a title that matches the mod's own
        /// pattern for some workbench is this mod's dialog. Every workbench in the game is a small
        /// list, and the check runs once per step.
        /// </summary>
        private static string ExpectedTitleOfAnyBench(string candidate)
        {
            var benches = DefDatabase<ThingDef>.AllDefsListForReading.Where(d => d.IsWorkTable);
            foreach (var bench in benches)
            {
                if ("BillAutopilot.Confirm.Title".Translate(bench.LabelCap).Resolve() == candidate) return candidate;
            }
            return null;
        }
    }
}
