using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Compte les produits d'une recette sans laisser de trace dans la partie.
    /// RecipeWorkerCounter ne sait travailler que sur une Bill_Production (il y lit hpRange,
    /// qualityRange, includeEquipped...) et remonte a la carte par billStack.billGiver. On garde donc
    /// une bill temoin par recette, dans une pile detachee dont on rebranche le donneur avant chaque
    /// mesure. Ces bills ne rejoignent jamais un etabli : elles ne sont pas sauvegardees.
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

        /// <summary>Le jeu sait-il compter le produit de cette recette ? Faux pour la decoupe, la fonte...</summary>
        public static bool CanCount(RecipeDef recipe)
        {
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
        /// Y a-t-il du travail selon un mode de repetition pose par un autre mod ? On habille la bill
        /// temoin de ce mode et on appelle ShouldDoNow : le mod qui possede le mode a un prefixe
        /// dessus, et repond avec son propre bareme. Cela evite de reimplementer "un par personne" ou
        /// "avec surplus", et vaudra pour tout mode qu'un autre mod ajoutera demain.
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
        /// Stock actuel du produit de la recette, sur la carte de l'etabli. <paramref name="memory"/>
        /// porte ce que les autres mods ajoutent au comptage (inventaires, produits additionnels) :
        /// sans lui, le temoin compterait a la facon vanilla pendant que la vraie bill compte autrement.
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
