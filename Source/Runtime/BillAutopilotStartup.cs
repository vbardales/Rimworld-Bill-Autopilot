using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Annonce au demarrage ce que le mod a trouve autour de lui.
    ///
    /// Sans cela, les sondes de compatibilite ne se declenchent qu'au premier passage de synchro sur
    /// un etabli dont le profil est actif : tant qu'aucun profil n'est coche, le journal ne dit rien
    /// et on ne peut pas savoir si l'integration s'est branchee. Ici, la ligne est toujours ecrite.
    /// </summary>
    [StaticConstructorOnStartup]
    internal static class BillAutopilotStartup
    {
        static BillAutopilotStartup()
        {
            Log.Message("[Bill Autopilot] Integrations: "
                        + "Better Workbench Management " + Found(BetterWorkbenchesCompat.Active)
                        + ", hidden recipes (Nice Bill Tab - Expansion) "
                        + Found(HiddenRecipesCompat.Available)
                        + ". Bill cap per workbench: " + BetterWorkbenchesCompat.MaxBills + ".");
        }

        private static string Found(bool present) => present ? "found" : "not found";
    }
}
