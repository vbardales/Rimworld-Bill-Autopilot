using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The per-game state: which recipes the autopilot has already met, which ones it has announced
    /// and is still waiting for an answer about, and what it remembers of a bill it took down.
    ///
    /// Why these are worth asserting rather than inferring from the bill stack: an absent bill has
    /// several possible reasons - excluded, unknown, announced and unanswered, capped out, or simply
    /// full stock - and they look identical on a bench. A scenario that could only see the stack
    /// would report "no bill" and leave the reader to guess which of the five it was.
    ///
    /// The whole of this state is written into the save's game node rather than a GameComponent, so
    /// these same steps after a reload are what proves the graft survives a round trip.
    /// </summary>
    [PickleSteps]
    public class StateSteps
    {
        [Then("Bill Autopilot has already met {string} on {string}")]
        public void AssertKnown(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.State(ctx).IsKnown(bench, recipe),
                $"{recipeDefName} counts as new on {benchDefName}: it would be announced again, which "
                + "after a reload is exactly the duplicate letter the save round trip exists to prevent");
        }

        [Then("Bill Autopilot has never met {string} on {string}")]
        public void AssertUnknown(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(!Driver.State(ctx).IsKnown(bench, recipe),
                $"{recipeDefName} is already known on {benchDefName}, so this scenario can no longer "
                + "show what happens when a recipe arrives for the first time");
        }

        /// <summary>
        /// A recipe announced and not yet answered. While it stands here NO bench of that type may
        /// act on it: without that rule the first bench would show the suspended bill and a second
        /// bench would quietly start producing, leaving the question asked but already answered by
        /// the mod itself.
        /// </summary>
        [Then("Bill Autopilot is still waiting for an answer about {string} on {string}")]
        public void AssertPending(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.State(ctx).IsPending(bench, recipe),
                $"{recipeDefName} on {benchDefName} is no longer pending, so any other bench of that "
                + "kind is free to start it");
        }

        [Then("Bill Autopilot is no longer waiting for an answer about {string} on {string}")]
        public void AssertAccepted(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(!Driver.State(ctx).IsPending(bench, recipe),
                $"{recipeDefName} on {benchDefName} is still pending: unsuspending its bill did not "
                + "read as an acceptance");
        }

        [Then("Bill Autopilot remembers the name {string} for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertMemoryName(PickleContext ctx, string name, string recipeDefName,
            string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var memory = Driver.State(ctx).MemoryFor(table, recipe);

            ctx.Assert(memory != null,
                $"nothing is remembered for {recipeDefName} on the bench at ({x}, {z}): whatever the "
                + "bill carried was dropped when it came down");
            ctx.Assert(memory.name == name,
                $"the bench at ({x}, {z}) remembers the name '{memory.name ?? "nothing"}' for "
                + $"{recipeDefName}, not '{name}'. Two benches sharing one entry is the old "
                + "per-workbench-type key coming back");
        }

        [Then("Bill Autopilot remembers nothing for {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertNoMemory(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            ctx.Assert(Driver.State(ctx).MemoryFor(table, recipe) == null,
                $"the bench at ({x}, {z}) still remembers something for {recipeDefName}. After an "
                + "explicit refusal there is nothing left to restore");
        }

        /// <summary>
        /// How many bills the autopilot is holding something for, across every bench on every map.
        /// This is how a scenario watches an entry being dropped for a bench that no longer exists:
        /// the entry cannot be asked for by cell once its bench is deconstructed, and
        /// <c>thingIDNumber</c> is never reused, so an entry whose bench is gone would otherwise sit
        /// in the save for the rest of the game with nothing able to see it.
        ///
        /// Counted before and after rather than pinned to a number. The fixture is a real colony and
        /// may already carry a bench of the kind a scenario is working with, so an absolute count
        /// would be asserting against the fixture's furniture as much as against the mod - and would
        /// go red the day someone adds a workbench to the save.
        /// </summary>
        [When("Bill Autopilot's memory is counted")]
        public void RememberMemoryCount(PickleContext ctx) => ctx.Set(new RememberedMemoryCount
        {
            Count = Memories(ctx).Count,
        });

        [Then("Bill Autopilot is holding something for one bill fewer than before")]
        public void AssertOneFewer(PickleContext ctx)
        {
            var before = ctx.Get<RememberedMemoryCount>();
            ctx.Require(before != null,
                "nothing was counted first: put 'Bill Autopilot's memory is counted' before this step");

            var memories = Memories(ctx);
            ctx.Assert(memories.Count == before.Count - 1,
                $"the autopilot was holding something for {before.Count} bills and now holds "
                + $"{memories.Count}: "
                + (memories.Count == 0 ? "nothing at all" : string.Join(", ", new List<string>(memories.Keys).ToArray())));
        }

        private static Dictionary<string, BillMemory> Memories(PickleContext ctx)
        {
            var field = typeof(BillAutopilotState)
                .GetField("memories", BindingFlags.Instance | BindingFlags.NonPublic);
            ctx.Require(field != null,
                "BillAutopilotState.memories no longer exists: the mod renamed its per-bench store, "
                + "update these steps");

            var memories = field.GetValue(Driver.State(ctx)) as Dictionary<string, BillMemory>;
            ctx.Require(memories != null, "BillAutopilotState.memories is not the expected dictionary");
            return memories;
        }

        // --- What the mod found around it ---------------------------------------------------------

        /// <summary>
        /// Asked of the mod's own probe, not of the mod list, and the difference is the point.
        /// TESTING.md scenario 1 names it as the fastest way to tell a dead integration from a
        /// missing mod: a neighbour that is loaded but whose interface has been renamed reports
        /// "not found" here while the mod list still shows it.
        /// </summary>
        [Then("Bill Autopilot found Better Workbench Management")]
        public void AssertBwm(PickleContext ctx) =>
            ctx.Assert(Driver.BridgeActive(ctx, "BetterWorkbenchesCompat", "Active"),
                "Bill Autopilot reports Better Workbench Management as not found while this pass "
                + "staged it: the mod is there and the interface this bridge reaches by reflection "
                + "is not. Everything else goes on working; that one integration is dead");

        [Then("Bill Autopilot found Nice Bill Tab")]
        public void AssertNbt(PickleContext ctx) =>
            ctx.Assert(Driver.BridgeActive(ctx, "NiceBillTabCompat", "Active"),
                "Bill Autopilot reports Nice Bill Tab as not found while this pass staged it: without "
                + "this bridge its cached row list is never told, and a drag can bring a deleted bill back");

        [Then("Bill Autopilot found Dubs Mint Menus")]
        public void AssertDmm(PickleContext ctx) =>
            ctx.Assert(Driver.BridgeActive(ctx, "DubsMintMenusCompat", "Active"),
                "Bill Autopilot reports Dubs Mint Menus as not found while this pass staged it: "
                + "without this bridge a bench template photographs the autopilot's passing queue");

        [Then("Bill Autopilot found the hidden-recipe store")]
        public void AssertHidden(PickleContext ctx) =>
            ctx.Assert(Driver.BridgeActive(ctx, "HiddenRecipesCompat", "Available"),
                "Bill Autopilot reports Nice Bill Tab - Expansion's hidden-recipe store as not found "
                + "while this pass staged it");

        /// <summary>
        /// The one integration check that is correct in EVERY pass, and the reason it exists.
        ///
        /// A scenario asserting "found" fails the pass that stages nothing, and one asserting "not
        /// found" fails the pass that stages everything; between them they would need two copies of
        /// the same feature, kept in step by hand. What actually has to hold, in both, is a
        /// correspondence: the mod reports a neighbour found exactly when that neighbour is loaded.
        ///
        /// Each half of a divergence means something different, and the failure says which. Loaded
        /// but not found is a bridge that has gone dead - the neighbour renamed what this code
        /// reaches by reflection, the feature is silently lost, and nothing else breaks. Found but
        /// not loaded is a detection answering yes to something else entirely.
        /// </summary>
        [Then("Bill Autopilot's integration report matches the mods this pass loaded")]
        public void AssertIntegrationReport(PickleContext ctx)
        {
            Check(ctx, "BetterWorkbenchesCompat", "Active", "falconne.BWM", "Better Workbench Management");
            Check(ctx, "NiceBillTabCompat", "Active", "Andromeda.NiceBillTab", "Nice Bill Tab");
            Check(ctx, "DubsMintMenusCompat", "Active", "dubwise.dubsmintmenus", "Dubs Mint Menus");
            Check(ctx, "HiddenRecipesCompat", "Available", "HICON.NiceBillTabExpansion",
                "Nice Bill Tab - Expansion");
        }

        private static void Check(PickleContext ctx, string type, string member, string packageId, string label)
        {
            bool found = Driver.BridgeActive(ctx, type, member);
            bool loaded = ModLister.GetActiveModWithIdentifier(packageId, ignorePostfix: true) != null;

            if (found == loaded) return;

            ctx.Assert(false, loaded
                ? $"{label} ({packageId}) is loaded in this pass and Bill Autopilot reports it as not "
                  + "found: the mod is there and the interface this bridge reaches by reflection is "
                  + "not. Everything else goes on working; that one integration is dead until it is "
                  + "repaired, and this is the failure most likely to arrive with someone else's update"
                : $"{label} ({packageId}) is not loaded in this pass and Bill Autopilot reports it as "
                  + "found: the detection is answering yes to something that is not that mod");
        }
    }

    /// <summary>
    /// A number carried from one step to the next. PickleContext.Set stores by type, so the count
    /// needs a type of its own rather than a bare int shared with whatever else a scenario holds.
    /// </summary>
    public class RememberedMemoryCount
    {
        public int Count;
    }
}
