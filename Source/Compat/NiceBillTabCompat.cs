using System;
using System.Reflection;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Pont vers Nice Bill Tab (assembly NiceBillTab, packageId Andromeda.NiceBillTab).
    ///
    /// Il remplace entierement l'onglet des travaux par un prefixe bloquant sur ITab_Bills.FillTab,
    /// et garde la liste affichee dans un champ statique, TabBillsDrawer.filteredBills, reconstruit
    /// seulement quand shouldRefreshFilter passe a vrai. Tous ses propres chemins le font : ajout,
    /// suppression, glisser-deposer.
    ///
    /// Le pilote, lui, ajoute et retire des bills hors de son UI. Sans prevenir, sa liste garde des
    /// bills qui n'existent plus : elles continuent de s'afficher, et son glisser-deposer reinsere
    /// dans la pile une bill deja supprimee, puisqu'il reordonne d'apres cette liste. On se contente
    /// donc d'actionner son propre mecanisme.
    /// </summary>
    internal static class NiceBillTabCompat
    {
        private static bool probed;
        private static FieldInfo shouldRefreshFilter;

        public static bool Active
        {
            get
            {
                Probe();
                return shouldRefreshFilter != null;
            }
        }

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
                        if (assembly.GetName().Name != "NiceBillTab") continue;

                        shouldRefreshFilter = assembly.GetType("NiceBillTab.TabBillsDrawer")
                            ?.GetField("shouldRefreshFilter", BindingFlags.Static | BindingFlags.Public);

                        if (shouldRefreshFilter == null)
                        {
                            Log.Warning("[Bill Autopilot] Nice Bill Tab found, but its bill list cache "
                                        + "could not be reached; its list may show bills that are gone.");
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                shouldRefreshFilter = null;
                Log.Warning("[Bill Autopilot] Could not hook into Nice Bill Tab: " + e.Message);
            }
        }

        /// <summary>A appeler apres toute pose ou tout retrait de bill fait hors de son interface.</summary>
        public static void NotifyBillsChanged()
        {
            Probe();
            if (shouldRefreshFilter == null) return;

            try
            {
                shouldRefreshFilter.SetValue(null, true);
            }
            catch
            {
                // Sans consequence : sa liste se reconstruira a la prochaine ouverture de l'onglet.
            }
        }
    }
}
