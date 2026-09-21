using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The bill stack of a real workbench: what the autopilot has put up, what the player does to it
    /// in the tab, and what it looks like afterwards.
    ///
    /// The edits below are the ones the bills tab performs. Changing a bill's target in the tab
    /// writes <c>targetCount</c> and <c>unpauseWhenYouHave</c> on the bill; deleting it calls
    /// <c>BillStack.Delete</c>, which is what raises <c>Notify_BillDeleted</c> and therefore what the
    /// mod's refusal hook hangs off. Driving the widgets with real clicks would test RimWorld's
    /// number buttons rather than this mod, and a click landing in another mod's window - the failure
    /// mode Pickle warns about - would read as a dead control.
    ///
    /// Every bill is named by its recipe AND the cell its bench stands on. Two benches of the same
    /// kind, set differently, is the scenario this mod's per-bench memory exists for: a lookup that
    /// took the first bench of a def could not see the bug it is meant to catch.
    /// </summary>
    [PickleSteps]
    public class BillSteps
    {
        // --- Making the engine run --------------------------------------------------------------

        /// <summary>
        /// One sync pass over one bench, which is what opening its bills tab triggers. Called
        /// directly rather than by opening the tab: the tab's own prefix is throttled to twice a
        /// second in REAL time, so under a runner driving a thousand ticks a second a scenario that
        /// opened the tab would get one sync and not know which pass it was looking at.
        /// </summary>
        [When("Bill Autopilot syncs the {string} at \\({int}, {int}\\)")]
        public void Sync(PickleContext ctx, string benchDefName, int x, int z)
        {
            Driver.State(ctx);
            AutoBillSync.Sync(Driver.Bench(ctx, benchDefName, x, z));
        }

        /// <summary>
        /// Selecting the bench and opening its bills tab, the way a player reaches it. Used by the
        /// scenarios that take a picture, and worth having for its own sake: opening the tab is what
        /// fires the mod's FillTab prefix, so this is the one step that exercises the
        /// sync-on-open path rather than calling the engine directly.
        ///
        /// The bench is named by cell, never by label: <c>I select {string}</c> matches a thing by
        /// its displayed name, and a scenario spelling "hand tailoring bench" would find nothing at
        /// all in the French pass.
        /// </summary>
        [When("the bills tab of the Bill Autopilot bench {string} at \\({int}, {int}\\) is opened")]
        public async System.Threading.Tasks.Task OpenBillsTab(PickleContext ctx, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);

            Find.Selector.ClearSelection();
            Find.Selector.Select(table, playSound: false, forceDesignatorDeselect: false);
            InspectPaneUtility.OpenTab(typeof(ITab_Bills));

            // Frames, not ticks: the tab draws on a layout pass, and its own prefix is throttled in
            // real time. Frames advance even with the game paused, which a tick wait would not.
            await ctx.WaitFrames(3);
        }

        // --- What the player does in the tab -------------------------------------------------------

        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is deleted")]
        public void DeleteAuto(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var bill = Driver.AutoBill(ctx, table, recipe);
            ctx.Require(bill != null,
                $"there is no automatic bill for {recipeDefName} to delete: {Driver.Describe(ctx, table)}");

            table.billStack.Delete(bill);
        }

        /// <summary>
        /// Deleting a bill the PLAYER placed, which must do none of what deleting an automatic one
        /// does. Separate from the step above on purpose: a scenario that asked for "the bill" would
        /// pass whichever one the lookup happened to find first, and the whole point here is which of
        /// the two was deleted.
        /// </summary>
        [When("the hand-placed bill for {string} on the Bill Autopilot bench {string} at \\({int}, {int}\\) is deleted")]
        public void DeleteManual(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var bill = Driver.ManualBill(ctx, table, recipe);
            ctx.Require(bill != null,
                $"there is no hand-placed bill for {recipeDefName} to delete: {Driver.Describe(ctx, table)}");

            table.billStack.Delete(bill);
        }

        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is unsuspended")]
        public void Unsuspend(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            Auto(ctx, recipeDefName, benchDefName, x, z).suspended = false;
        }

        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is set to keep {int}, restarting at {int}")]
        public void Retarget(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z,
            int target, int floor)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            bill.repeatMode = BillRepeatModeDefOf.TargetCount;
            bill.targetCount = target;
            bill.pauseWhenSatisfied = true;
            bill.unpauseWhenYouHave = floor;
        }

        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is set to repeat forever")]
        public void Forever(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            Auto(ctx, recipeDefName, benchDefName, x, z).repeatMode = BillRepeatModeDefOf.Forever;
        }

        /// <summary>
        /// "Do X times" - the mode the mod deliberately refuses to record. A standing order that says
        /// "make one" would be remade the moment it finished, forever, so the capture leaves it
        /// alone; this step exists to let a scenario prove that nothing was written.
        /// </summary>
        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is set to do it {int} times")]
        public void RepeatCount(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z, int times)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            bill.repeatMode = BillRepeatModeDefOf.RepeatCount;
            bill.repeatCount = times;
        }

        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is set to the repeat mode {string}")]
        public void ForeignMode(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z,
            string repeatModeDefName)
        {
            var mode = DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(repeatModeDefName);
            ctx.Require(mode != null,
                $"no BillRepeatModeDef named '{repeatModeDefName}'. This pass loaded: "
                + string.Join(", ", DefDatabase<BillRepeatModeDef>.AllDefsListForReading
                    .ConvertAll(d => d.defName).ToArray()));

            Auto(ctx, recipeDefName, benchDefName, x, z).repeatMode = mode;
        }

        /// <summary>
        /// A custom name, the way vanilla renaming and Better Workbench Management both write it.
        /// Named per bench: the entire point of scenario 11 is that two benches of the same kind
        /// carrying different names must each get their own back.
        /// </summary>
        [When("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is renamed {string}")]
        public void Rename(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z, string name)
        {
            Auto(ctx, recipeDefName, benchDefName, x, z).playerCustomName = name;
        }

        // --- What the stack looks like afterwards -------------------------------------------------

        [Then("Bill Autopilot has {int} bills up on the {string} at \\({int}, {int}\\)")]
        public void AssertAutoCount(PickleContext ctx, int expected, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var state = Driver.State(ctx);

            int actual = 0;
            var bills = table.billStack.Bills;
            for (int i = 0; i < bills.Count; i++)
            {
                if (state.IsAuto(bills[i])) actual++;
            }

            ctx.Assert(actual == expected,
                $"the autopilot has {actual} bills up, not {expected}: {Driver.Describe(ctx, table)}");
        }

        [Then("Bill Autopilot has a bill up for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertAutoPresent(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.AutoBill(ctx, table, recipe) != null,
                $"the autopilot has no bill up for {recipeDefName}: {Driver.Describe(ctx, table)}");
        }

        [Then("Bill Autopilot has no bill up for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertAutoAbsent(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.AutoBill(ctx, table, recipe) == null,
                $"the autopilot still has a bill up for {recipeDefName}: {Driver.Describe(ctx, table)}");
        }

        [Then("Bill Autopilot left the hand-placed bill for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertManualPresent(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.ManualBill(ctx, table, recipe) != null,
                $"the hand-placed bill for {recipeDefName} is gone: {Driver.Describe(ctx, table)}");
        }

        [Then("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is suspended")]
        public void AssertSuspended(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            ctx.Assert(bill.suspended,
                $"the automatic bill for {recipeDefName} is running: a recipe never seen on this bench "
                + "type must arrive suspended, so that nothing is spent before the player answers");
        }

        [Then("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is running")]
        public void AssertRunning(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            ctx.Assert(!bill.suspended, $"the automatic bill for {recipeDefName} is suspended");
        }

        [Then("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) keeps {int}, restarting at {int}")]
        public void AssertBillCounts(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z,
            int target, int floor)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            ctx.Assert(bill.repeatMode == BillRepeatModeDefOf.TargetCount,
                $"the bill for {recipeDefName} is in repeat mode '{bill.repeatMode?.defName ?? "none"}', "
                + "so it keeps no stock at all");
            ctx.Assert(bill.targetCount == target,
                $"the bill for {recipeDefName} keeps {bill.targetCount}, not {target}");
            ctx.Assert(bill.unpauseWhenYouHave == floor,
                $"the bill for {recipeDefName} restarts at {bill.unpauseWhenYouHave}, not {floor}");
        }

        [Then("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) has the repeat mode {string}")]
        public void AssertRepeatMode(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z,
            string repeatModeDefName)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            ctx.Assert(bill.repeatMode?.defName == repeatModeDefName,
                $"the bill for {recipeDefName} is in repeat mode "
                + $"'{bill.repeatMode?.defName ?? "none"}', not '{repeatModeDefName}'");
        }

        [Then("Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) is named {string}")]
        public void AssertName(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z, string name)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            ctx.Assert(bill.playerCustomName == name,
                $"the bill for {recipeDefName} is named '{bill.playerCustomName ?? "nothing"}', not '{name}'. "
                + "A name landing on the wrong bench is the per-workbench-type key coming back");
        }

        // --- The mark in the label ---------------------------------------------------------------

        /// <summary>
        /// Read through <c>LabelCap</c>, which is the property the vanilla tab, Nice Bill Tab, Dubs
        /// Mint Menus and Better Workbench Management all draw their row from - that is why the mod
        /// marks there and patches none of them. The expected marker is resolved through the mod's
        /// own translation key, never spelled out: a scenario carrying "(auto)" would pass in English
        /// and fail the French pass on a correctly translated marker.
        /// </summary>
        [Then("Bill Autopilot marks its bill for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertMarked(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            string marker = Driver.Marker();
            ctx.Assert(bill.LabelCap.EndsWith(marker),
                $"the automatic bill's label reads '{bill.LabelCap}' and does not end with the marker "
                + $"'{marker}', so nothing on screen tells it apart from a bill placed by hand");
        }

        [Then("Bill Autopilot does not mark the hand-placed bill for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertManualUnmarked(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var bill = Driver.ManualBill(ctx, table, recipe);
            ctx.Require(bill != null,
                $"there is no hand-placed bill for {recipeDefName}: {Driver.Describe(ctx, table)}");

            string marker = Driver.Marker();
            ctx.Assert(!bill.LabelCap.EndsWith(marker),
                $"a bill the player placed reads '{bill.LabelCap}', which ends with the automatic "
                + $"marker '{marker}'");
        }

        [Then("Bill Autopilot does not mark its bill for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertAutoUnmarked(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var bill = Auto(ctx, recipeDefName, benchDefName, x, z);
            string marker = Driver.Marker();
            ctx.Assert(!bill.LabelCap.EndsWith(marker),
                $"marking is off, yet the automatic bill still reads '{bill.LabelCap}'");
        }

        // --- The cap ------------------------------------------------------------------------------

        /// <summary>
        /// The arithmetic that keeps the Add button alive. RimWorld hides it at
        /// <c>BillStack.Bills.Count >= 15</c>, or at whatever ceiling Better Workbench Management
        /// reports when No Max Bills is present, so the question is asked against the live ceiling
        /// rather than against the number 15.
        /// </summary>
        [Then("Bill Autopilot leaves room for another bill on the {string} at \\({int}, {int}\\)")]
        public void AssertRoomLeft(PickleContext ctx, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            int ceiling = MaxBills(ctx);
            ctx.Assert(table.billStack.Bills.Count < ceiling,
                $"the bench carries {table.billStack.Bills.Count} bills against a ceiling of {ceiling}: "
                + $"the Add button is gone. {Driver.Describe(ctx, table)}");
        }

        [Then("Bill Autopilot counts {int} of {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertCounted(PickleContext ctx, int expected, string recipeDefName, string benchDefName,
            int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);

            ctx.Assert(Driver.TryCount(ctx, table, recipe, out int actual),
                $"the autopilot cannot count {recipeDefName} at all, so no stock threshold applies to it");
            ctx.Assert(actual == expected,
                $"the autopilot counts {actual} of {recipeDefName}, not {expected}. When this disagrees "
                + "with what the bill displays, the probe is counting the vanilla way while the bill "
                + "counts another way");
        }

        // --- Shared ------------------------------------------------------------------------------

        private static Bill_Production Auto(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var bill = Driver.AutoBill(ctx, table, recipe);
            ctx.Require(bill != null,
                $"the autopilot has no bill up for {recipeDefName} on the {benchDefName} at ({x}, {z}): "
                + Driver.Describe(ctx, table));
            return bill;
        }

        private static int MaxBills(PickleContext ctx)
        {
            var type = typeof(BillAutopilotMod).Assembly.GetType("BillAutopilot.BetterWorkbenchesCompat");
            ctx.Require(type != null,
                "BillAutopilot.BetterWorkbenchesCompat no longer exists: update these steps");

            var property = type.GetProperty("MaxBills", Driver.StaticAny);
            ctx.Require(property != null, "BetterWorkbenchesCompat.MaxBills no longer exists: update these steps");
            return (int)property.GetValue(null);
        }
    }
}
