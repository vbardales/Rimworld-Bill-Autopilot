using System;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// No Max Bills, KiameV's and its recreation No Max Bills: Redux. Both lift the game's limit of 15 bills
    /// from the interface (BillStack.DoListing and ITab_Bills.FillTab, nowhere else), and both define the
    /// type below, which is how Better Workbench Management tells that one of them is loaded. It is found
    /// the same way here, by that type, so the two are covered alike and without a dependency.
    /// </summary>
    internal static class NoMaxBillsCompat
    {
        private const string MarkerType = "NoMaxBills.Patch_BillStack_DoListing";

        private static bool probed;
        private static bool present;

        public static bool Present
        {
            get
            {
                if (probed) return present;
                probed = true;

                try
                {
                    present = GenTypes.GetTypeInAnyAssembly(MarkerType) != null;
                }
                catch (Exception e)
                {
                    present = false;
                    Log.Warning("[Bill Autopilot] Could not look for No Max Bills: " + e.Message);
                }
                return present;
            }
        }
    }
}
