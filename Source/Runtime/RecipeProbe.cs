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
