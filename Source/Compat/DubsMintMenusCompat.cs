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
    /// Bridge to Dubs Mint Menus (assembly DubsMintMenus, packageId dubwise.dubsmintmenus).
    ///
    /// Its bill menu does not take the tab over (its BillStack.DoListing patch is a void prefix that
    /// only adjusts the rect), so nothing needs reconciling there. Its **bench templates** do:
    /// MakeBenchTemplate photographs EVERY bill standing on a workbench, ours included.
    ///
    /// The trap: a template made from an autopiloted bench captures whatever the queue happened to
    /// hold; re-applied later, it puts those bills back as hand-placed ones, and the autopilot retires
    /// from those recipes for good without a word. So our bills are taken out of the template right
    /// after it is created: what the player meant to record is what she put there, not what the
    /// autopilot happened to be doing.
    ///
    /// Applying a template, on the other hand, needs nothing: ApplyTemplateToBench clones with
    /// InitializeAfterClone(), so the placed bills carry fresh ids, absent from our stamps, and the
    /// autopilot reads them as placed by hand. That is the intended behaviour.
    /// </summary>
    internal static class DubsMintMenusCompat
    {
        private static PropertyInfo templatesProperty;
        private static FieldInfo templateBillsField;
        private static bool active;

        public static bool Active => active;

        /// <summary>
        /// Installs the postfix. Called from the startup static constructor: the Dubs Mint Menus assembly
        /// is loaded well before that, and we are on the main thread.
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
        /// The parameter carries the same name as in the original method: Harmony matches it by name.
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

                // The template just created is the last one in the list.
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
