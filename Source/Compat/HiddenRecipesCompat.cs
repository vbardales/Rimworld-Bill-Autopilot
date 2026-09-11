using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Recipes hidden elsewhere, which the autopilot must not put back behind the player's back.
    ///
    /// Nice Bill Tab - Expansion (HICON.NiceBillTabExpansion) hides recipes per workbench, but only in
    /// its own add menu: nothing stops a mod from putting one up. So it is asked.
    ///
    /// Choose Your Recipe needs nothing: it removes disabled recipes from def.allRecipesCached, so
    /// they are already gone from the AllRecipes we walk.
    /// </summary>
    internal static class HiddenRecipesCompat
    {
        private static bool probed;
        /// <summary>
        /// A delegate, not a MethodInfo: this test lands on EVERY recipe of EVERY workbench on every
        /// sync pass. A reflection Invoke would cost a hundred times a direct call there, sixty times
        /// a tick on a well-stocked workshop.
        /// </summary>
        private static Func<Building_WorkTable, RecipeDef, bool> isHidden;

        private static void Probe()
        {
            if (probed) return;
            probed = true;

            try
            {
                foreach (var mod in LoadedModManager.RunningModsListForReading)
                {
                    foreach (var assembly in mod.assemblies.loadedAssemblies)
                    {
                        if (assembly.GetName().Name != "NiceBillTabExpansion") continue;

                        var store = assembly.GetType("NiceBillTabExpansion.HiddenRecipeStore")
                                    ?? FindByName(assembly, "HiddenRecipeStore");

                        var method = store?.GetMethod("IsHidden",
                            BindingFlags.Static | BindingFlags.Public,
                            null,
                            new[] { typeof(Building_WorkTable), typeof(RecipeDef) },
                            null);

                        if (method != null)
                        {
                            isHidden = (Func<Building_WorkTable, RecipeDef, bool>)Delegate.CreateDelegate(
                                typeof(Func<Building_WorkTable, RecipeDef, bool>), method);
                        }

                        if (isHidden != null)
                        {
                            Log.Message("[Bill Autopilot] Nice Bill Tab - Expansion found: "
                                        + "its hidden recipes will be treated as excluded.");
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                isHidden = null;
                Log.Warning("[Bill Autopilot] Could not read hidden recipes: " + e.Message);
            }
        }

        private static Type FindByName(Assembly assembly, string name)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == name) return type;
            }
            return null;
        }

        public static bool Available
        {
            get
            {
                Probe();
                return isHidden != null;
            }
        }

        public static bool IsHidden(Building_WorkTable table, RecipeDef recipe)
        {
            Probe();
            if (isHidden == null || table == null || recipe == null) return false;

            try
            {
                return isHidden(table, recipe);
            }
            catch
            {
                return false;
            }
        }
    }
}
