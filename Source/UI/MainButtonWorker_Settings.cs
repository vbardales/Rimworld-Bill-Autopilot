using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>Optional shortcut to the same native dialog used by Mod options.</summary>
    public class MainButtonWorker_Settings : MainButtonWorker
    {
        public override void Activate()
        {
            Find.WindowStack.Add(new Dialog_ModSettings(BillAutopilotMod.Instance));
        }
    }
}
