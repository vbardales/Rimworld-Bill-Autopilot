using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Le moteur. Pour chaque etabli, il fait exister une bill quand il y a du travail, et la retire
    /// quand il n'y en a plus. La liste de l'onglet reste donc courte : elle montre ce qu'il reste a
    /// faire, pas la configuration - celle-ci vit dans le profil du type d'etabli.
    /// </summary>
    public static class AutoBillSync
    {
        /// <summary>Vrai pendant nos propres suppressions : Notify_BillDeleted ne doit pas les prendre pour un refus.</summary>
        public static bool SuppressDeleteCapture;

        private static readonly HashSet<Bill> BusyBills = new HashSet<Bill>();
        private static Map busyMap;
        private static int busyTick = -1;

        public static void Sync(Building_WorkTable table)
        {
            var state = BillAutopilotGameComponent.Current;
            if (state == null) return;

            var profile = BillAutopilotMod.Settings.ProfileFor(table.def);
            var stack = table.billStack;

            if (profile == null || !profile.enabled)
            {
                DropAll(state, stack);
                return;
            }

            state.SeedIfNeeded(table.def);

            // Inventaire de la pile : nos bills d'un cote, celles posees a la main de l'autre.
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
            int budget = BillAutopilotMod.Settings.maxAutoBillsPerTable - autos.Count;

            for (int i = 0; i < recipes.Count; i++)
            {
                var recipe = recipes[i];
                if (recipe == null || !handled.Add(recipe)) continue;

                autos.TryGetValue(recipe, out var existing);

                // Une bill posee a la main l'emporte toujours : le pilote se retire de cette recette.
                bool available = recipe.AvailableNow && recipe.AvailableOnNow(table);
                if (!available || manual.Contains(recipe))
                {
                    if (existing != null) Remove(state, stack, existing);
                    continue;
                }

                bool countable = RecipeProbe.CanCount(recipe);
                var mode = profile.ModeFor(recipe, countable);

                if (mode == AutoMode.Excluded)
                {
                    if (existing != null) Remove(state, stack, existing);
                    continue;
                }

                // Recette jamais vue sur ce type d'etabli : elle arrive suspendue, et se signale.
                if (!state.IsKnown(table.def, recipe))
                {
                    if (existing != null)
                    {
                        state.MarkKnown(table.def, recipe);
                        continue;
                    }

                    // Plus de place : on ne la marque pas comme vue, elle se representera au passage suivant.
                    if (budget <= 0) continue;

                    state.MarkKnown(table.def, recipe);
                    state.MarkPending(table.def, recipe);
                    Create(state, table, recipe, profile, mode, suspended: true);
                    state.QueueNewRecipeNotice(table.def, recipe);
                    budget--;
                    continue;
                }

                // Signalee mais pas encore acceptee : on attend que la joueuse reactive la bill.
                if (state.IsPending(table.def, recipe))
                {
                    if (existing == null || existing.suspended) continue;
                    state.Accept(table.def, recipe);
                }

                if (existing != null)
                {
                    // Suspendue par la joueuse : on n'y touche pas, ni pour la relancer ni pour la retirer.
                    if (existing.suspended) continue;

                    CaptureDrift(state, table.def, recipe, existing, profile);

                    if (ShouldRetire(table, recipe, existing, profile, mode))
                    {
                        Remove(state, stack, existing);
                        budget++;
                    }
                }
                else if (budget > 0 && ShouldMaterialise(table, recipe, profile, mode))
                {
                    Create(state, table, recipe, profile, mode, suspended: false);
                    budget--;
                }
            }

            // Nos bills dont la recette a quitte l'etabli (mod retire, def repatchee).
            foreach (var pair in autos)
            {
                if (!handled.Contains(pair.Key)) Remove(state, stack, pair.Value);
            }
        }

        // --- Decisions ---------------------------------------------------------------------------

        private static bool ShouldMaterialise(Building_WorkTable table, RecipeDef recipe,
            BenchProfile profile, AutoMode mode)
        {
            if (mode == AutoMode.Always) return true;
            if (mode != AutoMode.Maintain) return false;

            if (!RecipeProbe.TryCount(table, recipe, out int count)) return false;

            int target = profile.TargetFor(recipe);
            int floor = profile.FloorFor(recipe);

            // La bande basse declenche, la cible arrete : sans cet ecart la bill clignoterait a chaque unite.
            return count <= floor && count < target;
        }

        private static bool ShouldRetire(Building_WorkTable table, RecipeDef recipe,
            Bill_Production bill, BenchProfile profile, AutoMode mode)
        {
            if (mode == AutoMode.Always) return false;
            if (IsBusy(table.Map, bill)) return false;
            if (!RecipeProbe.TryCount(table, recipe, out int count)) return false;

            return count >= profile.TargetFor(recipe);
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

        // --- Ecriture ----------------------------------------------------------------------------

        private static void Create(BillAutopilotGameComponent state, Building_WorkTable table,
            RecipeDef recipe, BenchProfile profile, AutoMode mode, bool suspended)
        {
            if (!(recipe.MakeNewBill() is Bill_Production bill)) return;

            var stamp = new BillStamp
            {
                mode = mode,
                targetCount = profile.TargetFor(recipe),
                floorCount = profile.FloorFor(recipe),
            };

            Apply(bill, stamp);
            bill.suspended = suspended;

            table.billStack.AddBill(bill);
            state.Claim(bill, stamp);
        }

        private static void Apply(Bill_Production bill, BillStamp stamp)
        {
            if (stamp.mode == AutoMode.Always)
            {
                bill.repeatMode = BillRepeatModeDefOf.Forever;
                return;
            }

            bill.repeatMode = BillRepeatModeDefOf.TargetCount;
            bill.targetCount = stamp.targetCount;
            bill.pauseWhenSatisfied = true;
            bill.unpauseWhenYouHave = stamp.floorCount;
        }

        private static void Remove(BillAutopilotGameComponent state, BillStack stack, Bill bill)
        {
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
        }

        public static void DropAll(BillAutopilotGameComponent state, BillStack stack)
        {
            var bills = stack.Bills;
            for (int i = bills.Count - 1; i >= 0; i--)
            {
                if (state.IsAuto(bills[i])) Remove(state, stack, bills[i]);
            }
        }

        /// <summary>
        /// La joueuse a change le mode ou les compteurs d'une bill automatique dans l'onglet ?
        /// On l'inscrit comme surcharge de la recette, sinon le reglage serait perdu au prochain
        /// retrait de la bill. Regler dans l'onglet, c'est regler le profil.
        /// </summary>
        private static void CaptureDrift(BillAutopilotGameComponent state, ThingDef bench,
            RecipeDef recipe, Bill_Production bill, BenchProfile profile)
        {
            var stamp = state.StampOf(bill);
            if (stamp == null) return;

            // RepeatCount ("x1") n'a pas de sens pour une consigne permanente : la bill se recreerait
            // sans fin une fois terminee. On la laisse telle quelle et on n'enregistre rien.
            if (bill.repeatMode == BillRepeatModeDefOf.RepeatCount) return;

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
