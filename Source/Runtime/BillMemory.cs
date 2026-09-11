using System.Collections.Generic;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// What another mod had put on an automatic bill, held from the moment the autopilot takes it
    /// down to the moment it puts it back. Keyed by "BenchDef/RecipeDef", the granularity of the
    /// profile: two benches of the same kind are meant to behave alike, that is the whole point.
    ///
    /// No Class attribute is written: the fields are primitive and ThingFilter is a game type,
    /// deep-saved in a field of its own type. Removing the mod therefore leaves nodes that nobody
    /// reads, and no error. See BillAutopilotState.
    /// </summary>
    public class BillMemory : IExposable
    {
        /// <summary>
        /// A repeat mode set by another mod, when it is neither TargetCount nor Forever. Everybody
        /// Gets One adds three (TD_PersonCount, TD_XPerPerson, TD_WithSurplusIng) and other mods may
        /// add more: the defName is kept without any attempt to understand what it means.
        /// </summary>
        public string repeatModeDefName;

        /// <summary>Better Workbench Management: also count away from the home map.</summary>
        public bool countAway;

        /// <summary>Custom name, whether it came from BWM or from vanilla renaming.</summary>
        public string name;

        /// <summary>Better Workbench Management: extra products counted toward the target.</summary>
        public ThingFilter productFilter;

        /// <summary>loadIDs of the bills this one was linked with.</summary>
        public List<int> linkedTo = new List<int>();

        public void ExposeData()
        {
            Scribe_Values.Look(ref repeatModeDefName, "repeatMode");
            Scribe_Values.Look(ref countAway, "countAway", defaultValue: false);
            Scribe_Values.Look(ref name, "name");
            Scribe_Deep.Look(ref productFilter, "productFilter");
            Scribe_Collections.Look(ref linkedTo, "linkedTo", LookMode.Value);

            if (linkedTo == null) linkedTo = new List<int>();
        }
    }
}
