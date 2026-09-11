using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// The engine. For each workbench it makes a bill exist when there is work, and takes it down when
    /// there is none. The tab list therefore stays short: it shows what is left to make, not the
    /// configuration, which lives in the workbench type's profile.
    /// </summary>
    public static class AutoBillSync
    {
        /// <summary>True during our own deletions: Notify_BillDeleted must not read them as a refusal.</summary>
        public static bool SuppressDeleteCapture;

        private static readonly HashSet<Bill> BusyBills = new HashSet<Bill>();
        private static Map busyMap;
        private static int busyTick = -1;

        public static void Sync(Building_WorkTable table)
        {
            var state = BillAutopilotState.Current;
            if (state == null) return;

            var profile = BillAutopilotMod.Settings.ProfileFor(table.def);
            var stack = table.billStack;

            if (profile == null || !profile.enabled)
            {
                DropAll(state, stack);
                return;
            }

            state.SeedIfNeeded(table.def);

            // Taking stock of the pile: ours on one side, the hand-placed ones on the other.
            var autos = new Dictionary<RecipeDef, Bill_Production>();
            var manual = new HashSet<RecipeDef>();
            var bills = stack.Bills;
            for (int i = 0; i < bills.Count; i++)
            {
                var bill = bills[i];
                if (bill?.recipe == null) continue;

                if (state.IsAuto(bill))
                {
                    if (bill is Bill_Production production) autos[bill.recipe] = production;
                }
                else
                {
                    manual.Add(bill.recipe);
                }
            }

            var recipes = table.def.AllRecipes;
            var handled = new HashSet<RecipeDef>();
            // Our cap, but never beyond the game's: 15 in vanilla, 125 when Better Workbench Management
            // sees No Max Bills. Past that, the "Add" button disappears.
            int cap = Mathf.Min(BillAutopilotMod.Settings.maxAutoBillsPerTable,
                BetterWorkbenchesCompat.MaxBills - manual.Count);
            int budget = cap - autos.Count;

            for (int i = 0; i < recipes.Count; i++)
            {
                var recipe = recipes[i];
                if (recipe == null || !handled.Add(recipe)) continue;

                autos.TryGetValue(recipe, out var existing);

                // A hand-placed bill always wins: the autopilot stands back from that recipe. A recipe hidden
                // elsewhere (Nice Bill Tab - Expansion) counts as excluded: hiding it says you do not
                // want it here.
                bool available = recipe.AvailableNow && recipe.AvailableOnNow(table)
                                 && !HiddenRecipesCompat.IsHidden(table, recipe);
                if (!available || manual.Contains(recipe))
                {
                    if (existing != null) Remove(state, stack, table, recipe, existing);
                    continue;
                }

                bool countable = RecipeProbe.CanCount(recipe);
                var mode = profile.ModeFor(recipe, countable);

                if (mode == AutoMode.Excluded)
                {
                    if (existing != null) Remove(state, stack, table, recipe, existing);
                    continue;
                }

                // A recipe never seen on this workbench type: it arrives suspended, and announces itself.
                if (!state.IsKnown(table.def, recipe))
                {
                    if (existing != null)
                    {
                        state.MarkKnown(table.def, recipe);
                        continue;
                    }

                    // No room left: it is not marked as seen, so it comes back on the next pass.
                    if (budget <= 0) continue;

                    state.MarkKnown(table.def, recipe);
                    state.MarkPending(table.def, recipe);
                    Create(state, table, recipe, profile, mode, suspended: true);
                    state.QueueNewRecipeNotice(table.def, recipe);
                    budget--;
                    continue;
                }

                // Announced but not yet accepted: wait for the player to unsuspend the bill.
                if (state.IsPending(table.def, recipe))
                {
                    if (existing == null || existing.suspended) continue;
                    state.Accept(table.def, recipe);
                }

                if (existing != null)
                {
                    // Suspended by the player: leave it alone, neither restarted nor removed.
                    if (existing.suspended) continue;

                    CaptureDrift(state, table.def, recipe, existing, profile);

                    if (ShouldRetire(table, recipe, existing, profile, mode))
                    {
                        Remove(state, stack, table, recipe, existing);
                        budget++;
                    }
                }
                else if (budget > 0 && ShouldMaterialise(state, table, recipe, profile, mode))
                {
                    Create(state, table, recipe, profile, mode, suspended: false);
                    budget--;
                }
            }

            // Our bills whose recipe has left the workbench (mod removed, def repatched).
            foreach (var pair in autos)
            {
                if (!handled.Contains(pair.Key)) Remove(state, stack, table, pair.Key, pair.Value);
            }
        }

        // --- Decisions ---------------------------------------------------------------------------

        private static bool ShouldMaterialise(BillAutopilotState state, Building_WorkTable table,
            RecipeDef recipe, BenchProfile profile, AutoMode mode)
        {
            if (mode == AutoMode.Always) return true;
            if (mode != AutoMode.Maintain && mode != AutoMode.Custom) return false;

            // The probe bill counts the way the real one will: otherwise the threshold that fires and the
            // number the bill displays are talking about two different figures.
            var memory = state.MemoryFor(table, recipe);

            // A mode from another mod, whether from the profile or from the bill we took down: it is the one
            // that says whether there is work again, since our comparisons mean nothing in its
            // yardstick.
            var foreign = mode == AutoMode.Custom
                ? profile.RepeatModeFor(recipe)
                : Resolve(memory?.repeatModeDefName);

            if (foreign != null)
            {
                return RecipeProbe.TryShouldDoNow(table, recipe, memory, foreign,
                    profile.TargetFor(recipe), profile.FloorFor(recipe), out bool due) && due;
            }

            if (!RecipeProbe.TryCount(table, recipe, memory, out int count)) return false;

            int target = profile.TargetFor(recipe);
            int floor = profile.FloorFor(recipe);

            // The low band fires, the target stops: without that gap the bill would flicker on every unit.
            return count <= floor && count < target;
        }

        private static bool ShouldRetire(Building_WorkTable table, RecipeDef recipe,
            Bill_Production bill, BenchProfile profile, AutoMode mode)
        {
            if (mode == AutoMode.Always) return false;
            if (IsBusy(table.Map, bill)) return false;

            // A mode from another mod: its thresholds are not ours. "One per person" depends on how many
            // colonists there are, "with surplus" on the ingredient stock. It alone knows when things
            // are full, so it is asked instead of comparing numbers of our own.
            if (IsForeignMode(bill.repeatMode)) return !bill.ShouldDoNow();

            // Here the real bill exists: measure with what it carries, not with a memory.
            if (!RecipeProbe.TryCount(table, recipe, BetterWorkbenchesCompat.Capture(bill), out int count))
            {
                return false;
            }

            return count >= profile.TargetFor(recipe);
        }

        /// <summary>
        /// A repeat mode that is neither ours nor the game's: set by another mod, and therefore read by
        /// that mod alone.
        /// </summary>
        private static BillRepeatModeDef Resolve(string defName)
        {
            return string.IsNullOrEmpty(defName)
                ? null
                : DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(defName);
        }

        private static bool IsForeignMode(BillRepeatModeDef mode)
        {
            return mode != null
                   && mode != BillRepeatModeDefOf.TargetCount
                   && mode != BillRepeatModeDefOf.Forever
                   && mode != BillRepeatModeDefOf.RepeatCount;
        }

        private static bool IsBusy(Map map, Bill bill)
        {
            if (map == null) return false;

            int tick = Find.TickManager.TicksGame;
            if (busyMap != map || busyTick != tick)
            {
                BusyBills.Clear();
                busyMap = map;
                busyTick = tick;

                var pawns = map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < pawns.Count; i++)
                {
                    var job = pawns[i].CurJob;
                    if (job?.bill != null) BusyBills.Add(job.bill);
                }
            }
            return BusyBills.Contains(bill);
        }

        // --- Writing ----------------------------------------------------------------------------

        private static void Create(BillAutopilotState state, Building_WorkTable table,
            RecipeDef recipe, BenchProfile profile, AutoMode mode, bool suspended)
        {
            if (!(recipe.MakeNewBill() is Bill_Production bill)) return;

            var stamp = new BillStamp
            {
                mode = mode,
                repeatModeDefName = mode == AutoMode.Custom
                    ? profile.RepeatModeFor(recipe)?.defName
                    : null,
                targetCount = profile.TargetFor(recipe),
                floorCount = profile.FloorFor(recipe),
            };

            Apply(bill, stamp);
            bill.suspended = suspended;

            table.billStack.AddBill(bill);
            state.Claim(bill, stamp);
            NiceBillTabCompat.NotifyBillsChanged();

            // Better Workbench Management's workbench restriction: its own hook applies it from the SELECTED
            // bench, which means nothing when creating from a tick.
            BetterWorkbenchesCompat.ApplyWorktableRestriction(table, bill);

            // Then the bill is given back what its predecessor carried: name, widened counting, product
            // filter, membership of a linked bill group.
            var memory = state.MemoryFor(table, recipe);
            if (memory != null)
            {
                if (memory.name != null) bill.playerCustomName = memory.name;

                // A mode from another mod takes its place again, with the counters it reads its own way:
                // "+X per person" in Everybody Gets One, an ingredient surplus elsewhere.
                var remembered = memory.repeatModeDefName == null
                    ? null
                    : DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(memory.repeatModeDefName);
                if (remembered != null) bill.repeatMode = remembered;

                BetterWorkbenchesCompat.Restore(bill, memory);
            }
        }

        private static void Apply(Bill_Production bill, BillStamp stamp)
        {
            if (stamp.mode == AutoMode.Always)
            {
                bill.repeatMode = BillRepeatModeDefOf.Forever;
                return;
            }

            // A mode from another mod is set as is. Both counters follow it: in Everybody Gets One they mean
            // "+X per person" or "X per person", elsewhere something else. They are passed on without
            // being interpreted.
            if (stamp.mode == AutoMode.Custom)
            {
                var custom = stamp.repeatModeDefName == null
                    ? null
                    : DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(stamp.repeatModeDefName);

                if (custom != null)
                {
                    bill.repeatMode = custom;
                    bill.targetCount = stamp.targetCount;
                    bill.pauseWhenSatisfied = true;
                    bill.unpauseWhenYouHave = stamp.floorCount;
                    return;
                }
            }

            bill.repeatMode = BillRepeatModeDefOf.TargetCount;
            bill.targetCount = stamp.targetCount;
            bill.pauseWhenSatisfied = true;
            bill.unpauseWhenYouHave = stamp.floorCount;
        }

        /// <summary>
        /// Takes an automatic bill down. What other mods had put on it is read first: Better Workbench
        /// Management prefixes BillStack.Delete to erase its extended data and pull the bill out of its
        /// link group. Without that reading, a name, a widened count or a link would vanish every time
        /// a stock filled up.
        /// </summary>
        private static void Remove(BillAutopilotState state, BillStack stack, Thing table,
            RecipeDef recipe, Bill bill)
        {
            if (table != null && recipe != null && bill is Bill_Production production)
            {
                var memory = BetterWorkbenchesCompat.Capture(production);

                if (production.playerCustomName != null)
                {
                    if (memory == null) memory = new BillMemory();
                    memory.name = production.playerCustomName;
                }

                // A repeat mode from another mod is kept as is: it is a choice of the player's that nothing
                // else would catch.
                var mode = production.repeatMode;
                if (mode != null && mode != BillRepeatModeDefOf.TargetCount
                                 && mode != BillRepeatModeDefOf.Forever)
                {
                    if (memory == null) memory = new BillMemory();
                    memory.repeatModeDefName = mode.defName;
                }

                state.Remember(table, recipe, memory);
            }

            SuppressDeleteCapture = true;
            try
            {
                stack.Delete(bill);
            }
            finally
            {
                SuppressDeleteCapture = false;
            }
            state.Disown(bill);
            NiceBillTabCompat.NotifyBillsChanged();
        }

        public static void DropAll(BillAutopilotState state, BillStack stack)
        {
            var table = stack.billGiver as Thing;
            var bills = stack.Bills;
            for (int i = bills.Count - 1; i >= 0; i--)
            {
                if (state.IsAuto(bills[i])) Remove(state, stack, table, bills[i].recipe, bills[i]);
            }
        }

        /// <summary>
        /// Has the player changed the mode or the counters of an automatic bill in the tab? It is recorded
        /// as an override on the recipe, otherwise the setting would be lost the next time the bill came
        /// down. Adjusting in the tab is adjusting the profile.
        /// </summary>
        private static void CaptureDrift(BillAutopilotState state, ThingDef bench,
            RecipeDef recipe, Bill_Production bill, BenchProfile profile)
        {
            var stamp = state.StampOf(bill);
            if (stamp == null) return;

            // RepeatCount ("x1") makes no sense as a standing order: the bill would be recreated endlessly
            // once finished. It is left as it is and nothing is recorded.
            if (bill.repeatMode == BillRepeatModeDefOf.RepeatCount) return;

            // A mode from elsewhere (Everybody Gets One adds three) is neither TargetCount nor Forever.
            // Flattening it to one of ours would destroy the player's choice in silence, so it is
            // recorded as is in the profile, like any other setting made in the tab.
            if (IsForeignMode(bill.repeatMode))
            {
                if (stamp.repeatModeDefName == bill.repeatMode.defName) return;

                var custom = profile.RuleForWriting(recipe);
                custom.mode = AutoMode.Custom;
                custom.repeatMode = bill.repeatMode.defName;
                custom.targetCount = bill.targetCount;
                custom.floorCount = bill.unpauseWhenYouHave;

                stamp.mode = AutoMode.Custom;
                stamp.repeatModeDefName = bill.repeatMode.defName;
                stamp.targetCount = bill.targetCount;
                stamp.floorCount = bill.unpauseWhenYouHave;

                BillAutopilotMod.Instance.WriteSettings();
                Messages.Message(
                    "BillAutopilot.DriftCaptured".Translate(recipe.LabelCap, bench.LabelCap),
                    MessageTypeDefOf.SilentInput, historical: false);
                return;
            }

            var actualMode = bill.repeatMode == BillRepeatModeDefOf.Forever ? AutoMode.Always : AutoMode.Maintain;
            bool modeChanged = actualMode != stamp.mode;
            bool countsChanged = actualMode == AutoMode.Maintain &&
                                 (bill.targetCount != stamp.targetCount || bill.unpauseWhenYouHave != stamp.floorCount);

            if (!modeChanged && !countsChanged) return;

            var rule = profile.RuleForWriting(recipe);
            rule.mode = actualMode;
            if (actualMode == AutoMode.Maintain)
            {
                rule.targetCount = bill.targetCount;
                rule.floorCount = bill.unpauseWhenYouHave;
            }
            else
            {
                rule.targetCount = -1;
                rule.floorCount = -1;
            }

            stamp.mode = actualMode;
            stamp.targetCount = bill.targetCount;
            stamp.floorCount = bill.unpauseWhenYouHave;

            BillAutopilotMod.Instance.WriteSettings();
            Messages.Message(
                "BillAutopilot.DriftCaptured".Translate(recipe.LabelCap, bench.LabelCap),
                MessageTypeDefOf.SilentInput, historical: false);
        }
    }
}
