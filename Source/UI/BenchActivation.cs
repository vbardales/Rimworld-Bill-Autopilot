using System.Linq;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Le seul chemin par lequel un profil d'etabli s'allume ou s'eteint - reglages, fenetre de
    /// profil, gizmo sur l'etabli.
    ///
    /// Pourquoi ce detour : cocher un type d'etabli accepte en silence TOUTES ses recettes deja
    /// debloquees. Sur un atelier d'usinage, cela lance vingt-cinq productions d'un coup. C'est
    /// ecrit dans la description du mod, mais personne ne la lit avant de cocher une case : on le dit
    /// donc au moment ou ca se decide, avec le nombre exact et la cible.
    /// </summary>
    internal static class BenchActivation
    {
        public static void Toggle(ThingDef bench, bool turnOn)
        {
            var profile = BillAutopilotMod.Settings.ProfileForWriting(bench);

            if (!turnOn)
            {
                Commit(profile, enabled: false);
                return;
            }

            // Deja passe par la dans cette partie : le stock initial est absorbe, plus rien ne sera
            // pris en silence. Rallumer apres une pause ne merite pas de question.
            var state = BillAutopilotState.Current;
            if (state != null && state.IsSeeded(bench))
            {
                Commit(profile, enabled: true);
                return;
            }

            int count = RecipesTaken(bench, profile);

            TaggedString text = profile.defaultMode == AutoMode.Always
                ? "BillAutopilot.Confirm.BodyAlways".Translate(count, bench.LabelCap)
                : "BillAutopilot.Confirm.BodyMaintain".Translate(count, bench.LabelCap, profile.targetCount);

            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                text,
                () => Commit(profile, enabled: true),
                destructive: false,
                title: "BillAutopilot.Confirm.Title".Translate(bench.LabelCap)));
        }

        private static void Commit(BenchProfile profile, bool enabled)
        {
            profile.enabled = enabled;
            BillAutopilotMod.Instance.WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        /// <summary>
        /// Combien de recettes le pilote prendrait en charge. Le test de comptabilite est fait a la
        /// main plutot que par RecipeProbe : cette fenetre s'ouvre aussi depuis le menu principal, ou
        /// il n'y a pas de partie pour fabriquer une bill temoin.
        /// </summary>
        private static int RecipesTaken(ThingDef bench, BenchProfile profile)
        {
            return bench.AllRecipes
                .Where(r => r != null)
                .Distinct()
                .Count(r =>
                {
                    if (!r.AvailableNow) return false;

                    bool countable = r.products != null && r.products.Count == 1 && r.specialProducts == null;
                    return profile.ModeFor(r, countable) != AutoMode.Excluded;
                });
        }
    }
}
