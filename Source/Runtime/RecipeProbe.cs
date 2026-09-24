using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Counts a recipe's products without leaving a trace in the game.
    /// RecipeWorkerCounter only knows how to work on a Bill_Production (it reads hpRange,
    /// qualityRange, includeEquipped from it) and reaches the map through billStack.billGiver. One
    /// probe bill per recipe is therefore kept in a detached stack whose giver is reattached before
    /// each measurement. These bills never join a workbench, so they are never saved.
    /// </summary>
    internal static class RecipeProbe
    {
        private static readonly BillStack ProbeStack = new BillStack(null);
        private static readonly Dictionary<RecipeDef, Bill_Production> Probes =
            new Dictionary<RecipeDef, Bill_Production>();

        public static void Reset()
        {
            Probes.Clear();
            ProbeStack.billGiver = null;
        }

        private static Bill_Production GetProbe(RecipeDef recipe)
        {
            if (Probes.TryGetValue(recipe, out var probe)) return probe;

            probe = recipe.MakeNewBill() as Bill_Production;
            Probes[recipe] = probe;
            return probe;
        }

        /// <summary>
        /// Can the game count this recipe's product? False for smelting a weapon, cremation, surgery and
        /// the like - and TRUE for butchering, whose counter counts raw meat.
        ///
        /// This is the one answer every part of the mod uses: the sync pass, the profile window and the
        /// confirmation that announces how many recipes will be taken. The window and the confirmation used
        /// to work it out by hand from the recipe's products, which is only what the BASE counter does;
        /// RecipeWorkerCounter has subclasses that decide otherwise, and for butchering the two answers
        /// differed, so the window showed Never for a recipe the engine ran in stock and the confirmation
        /// announced fewer recipes than were taken.
        /// </summary>
        public static bool CanCount(RecipeDef recipe)
        {
            // No game yet - the settings page opened from the main menu - and so no bill to build a probe
            // from: Bill's constructor takes its id from the game.
            if (Verse.Current.Game == null) return CanCountWithoutGame(recipe);

            var probe = GetProbe(recipe);
            if (probe == null) return false;

            try
            {
                return recipe.WorkerCounter.CanCountProducts(probe);
            }
            catch (Exception e)
            {
                Log.WarningOnce(
                    "[Bill Autopilot] CanCountProducts failed on " + recipe.defName + ": " + e.Message,
                    recipe.shortHash ^ 0x5A11);
                return false;
            }
        }

        /// <summary>
        /// The same question with no bill. A counter of its own class is asked directly with none: the ones
        /// that ignore the bill (butchering) answer, and one that reads it throws and falls through to what
        /// the base counter decides from the recipe alone, which is exactly the base class's own rule.
        /// </summary>
        private static bool CanCountWithoutGame(RecipeDef recipe)
        {
            try
            {
                var counter = recipe.WorkerCounter;
                if (counter != null && counter.GetType() != typeof(RecipeWorkerCounter))
                {
                    return counter.CanCountProducts(null);
                }
            }
            catch (Exception)
            {
                // A counter that needs its bill: the base rule below is the best answer without one.
            }

            return recipe.specialProducts == null && recipe.products != null && recipe.products.Count == 1;
        }

        /// <summary>
        /// Is there work to do under a repeat mode set by another mod? The probe bill is dressed in
        /// that mode and asked ShouldDoNow: the mod that owns the mode has a prefix on it, and answers
        /// by its own yardstick. This avoids reimplementing "one per person" or "with surplus", and
        /// will hold for any mode another mod adds tomorrow.
        /// </summary>
        public static bool TryShouldDoNow(Building_WorkTable table, RecipeDef recipe, BillMemory memory,
            BillRepeatModeDef mode, int targetCount, int floorCount, out bool due)
        {
            due = false;
            var probe = GetProbe(recipe);
            if (probe == null || table?.Map == null || mode == null) return false;

            var previousStack = probe.billStack;
            var previousMode = probe.repeatMode;
            ProbeStack.billGiver = table;
            probe.billStack = ProbeStack;
            BetterWorkbenchesCompat.PrimeProbe(probe, memory);
            try
            {
                probe.repeatMode = mode;
                probe.targetCount = targetCount;
                probe.unpauseWhenYouHave = floorCount;
                probe.pauseWhenSatisfied = true;
                probe.paused = false;
                probe.suspended = false;

                due = probe.ShouldDoNow();
                return true;
            }
            catch (Exception e)
            {
                Log.WarningOnce(
                    "[Bill Autopilot] ShouldDoNow failed for " + recipe.defName + " under repeat mode "
                    + mode.defName + ": " + e.Message,
                    recipe.shortHash ^ 0x5A13);
                return false;
            }
            finally
            {
                probe.repeatMode = previousMode;
                probe.billStack = previousStack;
                ProbeStack.billGiver = null;
            }
        }

        /// <summary>
        /// The current stock of the recipe's product, on the workbench's map. <paramref name="memory"/>
        /// carries what other mods add to the count (inventories, extra products): without it the probe
        /// would count the vanilla way while the real bill counts another way.
        /// </summary>
        public static bool TryCount(Building_WorkTable table, RecipeDef recipe, BillMemory memory,
            out int count)
        {
            count = 0;
            var probe = GetProbe(recipe);
            if (probe == null || table?.Map == null) return false;

            var previousStack = probe.billStack;
            ProbeStack.billGiver = table;
            probe.billStack = ProbeStack;
            BetterWorkbenchesCompat.PrimeProbe(probe, memory);
            try
            {
                if (!recipe.WorkerCounter.CanCountProducts(probe)) return false;
                count = recipe.WorkerCounter.CountProducts(probe);
                return true;
            }
            catch (Exception e)
            {
                Log.WarningOnce(
                    "[Bill Autopilot] CountProducts failed on " + recipe.defName + ": " + e.Message,
                    recipe.shortHash ^ 0x5A12);
                return false;
            }
            finally
            {
                probe.billStack = previousStack;
                ProbeStack.billGiver = null;
            }
        }
    }
}
