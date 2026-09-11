using System;
using System.Reflection;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Bridge to Nice Bill Tab (assembly NiceBillTab, packageId Andromeda.NiceBillTab).
    ///
    /// It replaces the bills tab wholesale through a blocking prefix on ITab_Bills.FillTab, and keeps
    /// the displayed list in a static field, TabBillsDrawer.filteredBills, rebuilt only when
    /// shouldRefreshFilter is raised. All of its own paths raise it: add, delete, drag-and-drop.
    ///
    /// The autopilot, however, adds and removes bills outside its interface. Without a word, its list
    /// keeps bills that no longer exist: they go on being drawn, and its drag-and-drop reinserts an
    /// already deleted bill into the stack, since it reorders from that list. So we simply work its
    /// own mechanism.
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

        /// <summary>Call after any bill put up or taken down outside its own interface.</summary>
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
                // Harmless: its list will rebuild the next time the tab is opened.
            }
        }
    }
}
