using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Pont vers Dubs Mint Menus (assembly DubsMintMenus, packageId dubwise.dubsmintmenus).
    ///
    /// Son menu des travaux ne prend pas la main sur l'onglet - son patch de BillStack.DoListing est
    /// un prefixe void qui ajuste le rectangle - donc rien a reconcilier de ce cote. En revanche il
    /// offre des **modeles d'etabli** : MakeBenchTemplate photographie TOUS les travaux presents sur
    /// un etabli, les notres compris.
    ///
    /// Le piege : un modele fabrique depuis un etabli sous pilote capture la file du moment ;
    /// reapplique plus tard, il repose ces travaux comme poses a la main, et le pilote s'efface
    /// definitivement sur ces recettes sans rien dire. On retire donc nos travaux du modele juste
    /// apres sa creation - ce que la joueuse voulait enregistrer, c'est ce qu'elle a mis, pas ce que
    /// le pilote passait par la.
    ///
    /// Appliquer un modele, en revanche, ne demande rien : ApplyTemplateToBench clone avec
    /// InitializeAfterClone(), les travaux poses ont donc un identifiant neuf, absent de nos
    /// empreintes, et le pilote les voit comme poses a la main. C'est le comportement voulu.
    /// </summary>
    internal static class DubsMintMenusCompat
    {
        private static PropertyInfo templatesProperty;
        private static FieldInfo templateBillsField;
        private static bool active;

        public static bool Active => active;

        /// <summary>
        /// Pose le postfix. Appele depuis le constructeur statique de demarrage : l'assembly de Dubs
        /// Mint Menus est chargee bien avant, et on est sur le thread principal.
        /// </summary>
        public static void Install(Harmony harmony)
        {
            try
            {
                var assembly = FindAssembly();
                if (assembly == null) return;

                var patchType = assembly.GetType("DubsMintMenus.Patch_BillStack_DoListing");
                var templateType = assembly.GetType("DubsMintMenus.FBenchTemplate");
                var settingsType = assembly.GetType("DubsMintMenus.Settings");

                var makeTemplate = patchType?.GetMethod("MakeBenchTemplate",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                templatesProperty = settingsType?.GetProperty("fbenchTemplates",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                templateBillsField = templateType?.GetField("FBills");

                if (makeTemplate == null || templatesProperty == null || templateBillsField == null)
                {
                    Log.Warning("[Bill Autopilot] Dubs Mint Menus found, but its bench templates could "
                                + "not be reached; templates may capture autopilot bills.");
                    return;
                }

                harmony.Patch(makeTemplate, postfix: new HarmonyMethod(
                    typeof(DubsMintMenusCompat), nameof(AfterMakeBenchTemplate)));

                active = true;
                Log.Message("[Bill Autopilot] Dubs Mint Menus found: autopilot bills will be kept out "
                            + "of bench templates.");
            }
            catch (Exception e)
            {
                active = false;
                Log.Warning("[Bill Autopilot] Could not hook into Dubs Mint Menus: " + e.Message);
            }
        }

        private static Assembly FindAssembly()
        {
            foreach (var mod in LoadedModManager.RunningModsListForReading)
            {
                foreach (var assembly in mod.assemblies.loadedAssemblies)
                {
                    if (assembly.GetName().Name == "DubsMintMenus") return assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// Le parametre porte le meme nom que dans la methode d'origine : Harmony l'apparie par nom.
        /// </summary>
        public static void AfterMakeBenchTemplate(IBillGiver p)
        {
            try
            {
                var state = BillAutopilotState.Current;
                if (state == null || !(p is Building_WorkTable table)) return;

                var ours = new HashSet<RecipeDef>();
                var bills = table.billStack.Bills;
                for (int i = 0; i < bills.Count; i++)
                {
                    if (state.IsAuto(bills[i]) && bills[i].recipe != null) ours.Add(bills[i].recipe);
                }
                if (ours.Count == 0) return;

                // Le modele qui vient d'etre cree est le dernier de la liste.
                if (!(templatesProperty.GetValue(null) is IList templates) || templates.Count == 0) return;
                if (!(templateBillsField.GetValue(templates[templates.Count - 1]) is IList templateBills)) return;

                int removed = 0;
                for (int i = templateBills.Count - 1; i >= 0; i--)
                {
                    if (templateBills[i] is Bill bill && bill.recipe != null && ours.Contains(bill.recipe))
                    {
                        templateBills.RemoveAt(i);
                        removed++;
                    }
                }

                if (removed > 0)
                {
                    Messages.Message(
                        "BillAutopilot.TemplateStripped".Translate(removed),
                        MessageTypeDefOf.SilentInput, historical: false);
                }
            }
            catch (Exception e)
            {
                Log.WarningOnce("[Bill Autopilot] Could not clean a Dubs Mint Menus bench template: "
                                + e.Message, 0x5A24);
            }
        }
    }
}
