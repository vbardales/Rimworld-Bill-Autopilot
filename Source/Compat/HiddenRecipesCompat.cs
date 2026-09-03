using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Recettes masquees ailleurs, que le pilote ne doit pas reposer dans le dos de la joueuse.
    ///
    /// Nice Bill Tab - Expansion (HICON.NiceBillTabExpansion) masque des recettes par etabli, mais
    /// seulement dans son menu d'ajout : rien n'empeche un mod d'en poser une. On lui demande donc.
    ///
    /// Choose Your Recipe, lui, n'a besoin de rien : il retire les recettes desactivees de
    /// def.allRecipesCached, donc elles ne figurent deja plus dans AllRecipes que nous parcourons.
    /// </summary>
    internal static class HiddenRecipesCompat
    {
        private static bool probed;
        /// <summary>
        /// Un delegue, pas un MethodInfo : ce test tombe sur CHAQUE recette de CHAQUE etabli a chaque
        /// passage de synchro. Un Invoke par reflexion y couterait cent fois le prix d'un appel direct,
        /// soixante fois par tick sur un atelier bien fourni.
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
