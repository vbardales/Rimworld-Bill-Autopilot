using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Announces at startup what the mod found around it.
    ///
    /// Without this, the compatibility probes only fire on the first sync pass over a workbench whose
    /// profile is switched on: as long as no profile is ticked the log says nothing, and there is no
    /// way to tell whether an integration took. Here the line is always written.
    /// </summary>
    [StaticConstructorOnStartup]
    internal static class BillAutopilotStartup
    {
        static BillAutopilotStartup()
        {
            // This patch aims at another mod's assembly: it cannot be declared by attribute, and it
            // waits for everything to be loaded, hence its place here rather than in PatchAll().
            DubsMintMenusCompat.Install(BillAutopilotMod.HarmonyInstance);

            Log.Message("[Bill Autopilot] Integrations: "
                        + "Better Workbench Management " + Found(BetterWorkbenchesCompat.Active)
                        + ", hidden recipes (Nice Bill Tab - Expansion) "
                        + Found(HiddenRecipesCompat.Available)
                        + ", Dubs Mint Menus " + Found(DubsMintMenusCompat.Active)
                        + ", Nice Bill Tab " + Found(NiceBillTabCompat.Active)
                        + ". Bill cap per workbench: " + BetterWorkbenchesCompat.MaxBills + ".");
        }

        private static string Found(bool present) => present ? "found" : "not found";
    }
}
