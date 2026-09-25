using System;
using System.Reflection;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The two things Better Workbench Management does to a bill that this mod has to get right and that the
    /// down-and-up scenarios of feature 13 do not reach: the workbench restriction applied to a bill created
    /// from a tick, and the agreement between the count the autopilot decides with and the count the bill
    /// itself reports.
    ///
    /// Reached by reflection, like the rest of IntegrationSteps, so that the minimal pass can load this assembly
    /// without the neighbour. Both read Better Workbench Management's own objects back rather than the mod's
    /// copy of them.
    /// </summary>
    [PickleSteps]
    public class BwmDetailSteps
    {
        private const BindingFlags Any = BindingFlags.Static | BindingFlags.Instance
                                         | BindingFlags.Public | BindingFlags.NonPublic;

        // --- The workbench restriction ------------------------------------------------------------

        /// <summary>
        /// BWM's own hook applies a workbench's restriction to a bill from the SELECTED bench, which means
        /// nothing when the bill is created by a tick. The mod applies it itself
        /// (<c>BetterWorkbenchesCompat.ApplyWorktableRestriction</c>). The restriction is set here the way
        /// BWM's interface sets it: on its own per-bench record.
        /// </summary>
        [Given("Better Workbench Management restricts the {string} at \\({int}, {int}\\) to non-mechs with a skill range of {int} to {int}")]
        public void Restrict(PickleContext ctx, string benchDefName, int x, int z, int min, int max)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var type = Assembly(ctx, "ImprovedWorkbenches")
                ?.GetType("ImprovedWorkbenches.WorktableRestrictionDataStorage");
            ctx.Require(type != null,
                "ImprovedWorkbenches.WorktableRestrictionDataStorage not found: this scenario needs Better "
                + "Workbench Management staged, which its @requires tag should have ensured");

            var storage = Find.World?.GetComponent(type);
            ctx.Require(storage != null,
                "Better Workbench Management's WorktableRestrictionDataStorage is not on the world");

            var get = type.GetMethod("GetWorktableRestrictionData", new[] { typeof(int) });
            ctx.Require(get != null,
                "WorktableRestrictionDataStorage.GetWorktableRestrictionData(int) no longer exists: update these steps");

            var data = get.Invoke(storage, new object[] { table.thingIDNumber });
            ctx.Require(data != null, "Better Workbench Management returned no restriction record for the bench");

            SetField(ctx, data, "isRestricted", true);
            SetField(ctx, data, "restrictionNonMechsOnly", true);
            SetField(ctx, data, "restrictionAllowedSkillRange", new IntRange(min, max));
        }

        [Then("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) carries the restriction of the bench: non-mechs with a skill range of {int} to {int}")]
        public void AssertRestriction(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z, int min, int max)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var bill = Driver.AutoBill(ctx, table, Driver.Recipe(ctx, recipeDefName));
            ctx.Require(bill != null,
                $"the autopilot has no bill up for {recipeDefName}: {Driver.Describe(ctx, table)}");

            ctx.Assert(bill.NonMechsOnly,
                "the bill the autopilot created is not restricted to non-mechs. The bench's restriction was set "
                + "through Better Workbench Management, and BWM's own hook reads the SELECTED bench, which means "
                + "nothing for a bill created from a tick: this is the case the mod's bridge exists for");
            ctx.Assert(bill.allowedSkillRange.min == min && bill.allowedSkillRange.max == max,
                $"the bill's allowed skill range is {bill.allowedSkillRange.min} to {bill.allowedSkillRange.max}, "
                + $"not the {min} to {max} set on the bench");
        }

        // --- The widened count --------------------------------------------------------------------

        /// <summary>
        /// BWM's postfix on <c>RecipeWorkerCounter.CountProducts</c> adds what a bill's extended data says to
        /// count (here an additional product). It does nothing for a bill without extended data, which is the
        /// probe bill's case, so the mod grafts the same reading on before it measures.
        /// </summary>
        [Given("Better Workbench Management also counts {string} toward Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\)")]
        public void AddProduct(PickleContext ctx, string extraDefName, string recipeDefName,
            string benchDefName, int x, int z)
        {
            var extra = DefDatabase<ThingDef>.GetNamedSilentFail(extraDefName);
            ctx.Require(extra != null, $"no ThingDef named '{extraDefName}'");

            var table = Driver.Bench(ctx, benchDefName, x, z);
            var bill = Driver.AutoBill(ctx, table, Driver.Recipe(ctx, recipeDefName));
            ctx.Require(bill != null,
                $"the autopilot has no bill up for {recipeDefName}: {Driver.Describe(ctx, table)}");

            var storageType = Assembly(ctx, "ImprovedWorkbenches")
                ?.GetType("ImprovedWorkbenches.ExtendedBillDataStorage");
            ctx.Require(storageType != null, "ImprovedWorkbenches.ExtendedBillDataStorage not found");
            var storage = Find.World?.GetComponent(storageType);
            ctx.Require(storage != null, "Better Workbench Management's ExtendedBillDataStorage is not on the world");

            var create = storageType.GetMethod("GetOrCreateExtendedDataFor");
            ctx.Require(create != null, "ExtendedBillDataStorage.GetOrCreateExtendedDataFor no longer exists");
            var data = create.Invoke(storage, new object[] { bill });
            ctx.Require(data != null, "Better Workbench Management would not create extended data for this bill");

            var filter = new ThingFilter();
            filter.SetAllow(extra, true);
            SetField(ctx, data, "ProductAdditionalFilter", filter);
        }

        /// <summary>
        /// The claim of TESTING.md scenario 13: the threshold that fires and the number the bill displays are
        /// the same figure. Both are read from the game: the bill's own count through its recipe's counter (BWM's
        /// postfix runs there), the autopilot's through its own probe, given what the live bill carries exactly as
        /// the sync pass gives it. A second implementation in the test could only agree with itself.
        /// </summary>
        [Then("Bill Autopilot counts the same as the bill does for {string} on the {string} at \\({int}, {int}\\), and at least {int}")]
        public void AssertSameCount(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z, int atLeast)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var bill = Driver.AutoBill(ctx, table, recipe);
            ctx.Require(bill != null,
                $"the autopilot has no bill up for {recipeDefName}: {Driver.Describe(ctx, table)}");

            int billsOwn = recipe.WorkerCounter.CountProducts(bill);

            var mod = typeof(BillAutopilotMod).Assembly;
            var compat = mod.GetType("BillAutopilot.BetterWorkbenchesCompat");
            var capture = compat?.GetMethod("Capture", Any);
            ctx.Require(capture != null, "BetterWorkbenchesCompat.Capture no longer exists: update these steps");
            var memory = capture.Invoke(null, new object[] { bill });

            var probe = mod.GetType("BillAutopilot.RecipeProbe");
            var tryCount = probe?.GetMethod("TryCount", Any);
            ctx.Require(tryCount != null, "RecipeProbe.TryCount no longer exists: update these steps");
            var args = new object[] { table, recipe, memory, 0 };
            bool ok = (bool)tryCount.Invoke(null, args);
            ctx.Require(ok, "the autopilot's probe could not count this recipe");
            int ours = (int)args[3];

            ctx.Assert(billsOwn >= atLeast,
                $"the bill counts {billsOwn}, fewer than the {atLeast} expected: Better Workbench Management's "
                + "widened count did not reach the bill, so the scenario says nothing about agreement");
            ctx.Assert(ours == billsOwn,
                $"the autopilot decides with {ours} and the bill counts {billsOwn}: the threshold that fires and "
                + "the number the bill displays are no longer the same figure");
        }

        // --- helpers ------------------------------------------------------------------------------

        private static void SetField(PickleContext ctx, object target, string name, object value)
        {
            var field = target.GetType().GetField(name, Any);
            ctx.Require(field != null, $"{target.GetType().FullName}.{name} no longer exists: update these steps");
            field.SetValue(target, value);
        }

        private static Assembly Assembly(PickleContext ctx, string name)
        {
            foreach (var mod in LoadedModManager.RunningModsListForReading)
            {
                foreach (var assembly in mod.assemblies.loadedAssemblies)
                {
                    if (assembly.GetName().Name == name) return assembly;
                }
            }
            ctx.Require(false, $"no assembly named '{name}' is loaded in this pass");
            return null;
        }
    }
}
