using System.Collections.Generic;
using RimWorld;
using UnityEngine;
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
            // Notre plafond, mais jamais au-dela de celui du jeu : 15 en vanilla, 125 quand Better
            // Workbench Management voit No Max Bills. Au-dela, le bouton "Ajouter" disparait.
            int cap = Mathf.Min(BillAutopilotMod.Settings.maxAutoBillsPerTable,
                BetterWorkbenchesCompat.MaxBills - manual.Count);
            int budget = cap - autos.Count;

            for (int i = 0; i < recipes.Count; i++)
            {
                var recipe = recipes[i];
                if (recipe == null || !handled.Add(recipe)) continue;

                autos.TryGetValue(recipe, out var existing);

                // Une bill posee a la main l'emporte toujours : le pilote se retire de cette recette.
                // Une recette masquee ailleurs (Nice Bill Tab - Expansion) est traitee comme exclue :
                // la masquer, c'est dire qu'on n'en veut pas ici.
                bool available = recipe.AvailableNow && recipe.AvailableOnNow(table)
                                 && !HiddenRecipesCompat.IsHidden(table, recipe);
                if (!available || manual.Contains(recipe))
                {
                    if (existing != null) Remove(state, stack, table.def, recipe, existing);
                    continue;
                }

                bool countable = RecipeProbe.CanCount(recipe);
                var mode = profile.ModeFor(recipe, countable);

                if (mode == AutoMode.Excluded)
                {
                    if (existing != null) Remove(state, stack, table.def, recipe, existing);
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
                        Remove(state, stack, table.def, recipe, existing);
                        budget++;
                    }
                }
                else if (budget > 0 && ShouldMaterialise(state, table, recipe, profile, mode))
                {
                    Create(state, table, recipe, profile, mode, suspended: false);
                    budget--;
                }
            }

            // Nos bills dont la recette a quitte l'etabli (mod retire, def repatchee).
            foreach (var pair in autos)
            {
                if (!handled.Contains(pair.Key)) Remove(state, stack, table.def, pair.Key, pair.Value);
            }
        }

        // --- Decisions ---------------------------------------------------------------------------

        private static bool ShouldMaterialise(BillAutopilotState state, Building_WorkTable table,
            RecipeDef recipe, BenchProfile profile, AutoMode mode)
        {
            if (mode == AutoMode.Always) return true;
            if (mode != AutoMode.Maintain && mode != AutoMode.Custom) return false;

            // La bill temoin compte comme comptera la vraie : sinon le seuil qui declenche et celui
            // qu'affiche la bill parlent de deux nombres differents.
            var memory = state.MemoryFor(table.def, recipe);

            // Un mode d'un autre mod, qu'il vienne du profil ou de la bill qu'on avait retiree : c'est
            // lui qui dira s'il y a de nouveau du travail, nos comparaisons ne veulent rien dire dans
            // son bareme.
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

            // La bande basse declenche, la cible arrete : sans cet ecart la bill clignoterait a chaque unite.
            return count <= floor && count < target;
        }

        private static bool ShouldRetire(Building_WorkTable table, RecipeDef recipe,
            Bill_Production bill, BenchProfile profile, AutoMode mode)
        {
            if (mode == AutoMode.Always) return false;
            if (IsBusy(table.Map, bill)) return false;

            // Mode venu d'un autre mod : ses seuils ne sont pas les notres - "un par personne" depend
            // du nombre de colons, "avec surplus" du stock d'ingredients. Lui seul sait quand c'est
            // plein, alors on le lui demande au lieu de comparer nos propres nombres.
            if (IsForeignMode(bill.repeatMode)) return !bill.ShouldDoNow();

            // Ici la vraie bill existe : on mesure avec ce qu'elle porte, pas avec un souvenir.
            if (!RecipeProbe.TryCount(table, recipe, BetterWorkbenchesCompat.Capture(bill), out int count))
            {
                return false;
            }

            return count >= profile.TargetFor(recipe);
        }

        /// <summary>
        /// Un mode de repetition qui n'est ni le notre ni celui du jeu : pose par un autre mod, donc
        /// interprete par lui seul.
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

        // --- Ecriture ----------------------------------------------------------------------------

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

            // La restriction d'etabli de Better Workbench Management : son propre crochet la pose
            // depuis l'etabli SELECTIONNE, ce qui ne veut rien dire quand on cree depuis un tick.
            BetterWorkbenchesCompat.ApplyWorktableRestriction(table, bill);

            // Puis on rend a la bill ce que la precedente portait : nom, comptage elargi, filtre de
            // produits, appartenance a un groupe de bills liees.
            var memory = state.MemoryFor(table.def, recipe);
            if (memory != null)
            {
                if (memory.name != null) bill.playerCustomName = memory.name;

                // Le mode d'un autre mod reprend sa place, avec les compteurs qu'il interprete a sa
                // facon : "+X par personne" chez Everybody Gets One, un surplus d'ingredients ailleurs.
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

            // Un mode venu d'un autre mod se pose tel quel. Les deux compteurs le suivent : chez
            // Everybody Gets One ils veulent dire "+X par personne" ou "X par personne", ailleurs
            // autre chose. On les transmet sans les interpreter.
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
        /// Retirer une bill automatique. On releve d'abord ce que les autres mods lui avaient pose :
        /// Better Workbench Management prefixe BillStack.Delete pour effacer ses donnees etendues et
        /// sortir la bill de son groupe de liens. Sans ce releve, un nom, un comptage elargi ou un lien
        /// disparaitrait a chaque fois qu'un stock se remplit.
        /// </summary>
        private static void Remove(BillAutopilotState state, BillStack stack, ThingDef bench,
            RecipeDef recipe, Bill bill)
        {
            if (bench != null && recipe != null && bill is Bill_Production production)
            {
                var memory = BetterWorkbenchesCompat.Capture(production);

                if (production.playerCustomName != null)
                {
                    if (memory == null) memory = new BillMemory();
                    memory.name = production.playerCustomName;
                }

                // Un mode de repetition venu d'un autre mod se retient tel quel : c'est un choix de
                // la joueuse que rien d'autre ne rattraperait.
                var mode = production.repeatMode;
                if (mode != null && mode != BillRepeatModeDefOf.TargetCount
                                 && mode != BillRepeatModeDefOf.Forever)
                {
                    if (memory == null) memory = new BillMemory();
                    memory.repeatModeDefName = mode.defName;
                }

                state.Remember(bench, recipe, memory);
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
            var bench = (stack.billGiver as Thing)?.def;
            var bills = stack.Bills;
            for (int i = bills.Count - 1; i >= 0; i--)
            {
                if (state.IsAuto(bills[i])) Remove(state, stack, bench, bills[i].recipe, bills[i]);
            }
        }

        /// <summary>
        /// La joueuse a change le mode ou les compteurs d'une bill automatique dans l'onglet ?
        /// On l'inscrit comme surcharge de la recette, sinon le reglage serait perdu au prochain
        /// retrait de la bill. Regler dans l'onglet, c'est regler le profil.
        /// </summary>
        private static void CaptureDrift(BillAutopilotState state, ThingDef bench,
            RecipeDef recipe, Bill_Production bill, BenchProfile profile)
        {
            var stamp = state.StampOf(bill);
            if (stamp == null) return;

            // RepeatCount ("x1") n'a pas de sens pour une consigne permanente : la bill se recreerait
            // sans fin une fois terminee. On la laisse telle quelle et on n'enregistre rien.
            if (bill.repeatMode == BillRepeatModeDefOf.RepeatCount) return;

            // Un mode venu d'ailleurs - Everybody Gets One en ajoute trois - n'est ni TargetCount ni
            // Forever. Le ramener a l'un des notres detruirait le choix de la joueuse en silence : on
            // l'inscrit tel quel dans le profil, comme n'importe quel autre reglage fait dans l'onglet.
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
