using System.Linq;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// The only path by which a bench profile is switched on or off: settings, profile window, gizmo on
    /// the workbench.
    ///
    /// Why the detour: ticking a workbench type silently accepts ALL of its already-unlocked recipes.
    /// On a machining table that starts twenty-five productions at once. It is written in the mod's
    /// description, but nobody reads that before ticking a box, so it is said at the moment it is
    /// decided, with the exact count and the target.
    /// </summary>
    internal static class BenchActivation
    {
        public static void Toggle(ThingDef bench, bool turnOn)
        {
            var profile = BillAutopilotMod.Settings.ProfileForWriting(bench);

            if (!turnOn)
            {
                Commit(profile, enabled: false);
                return;
            }

            // Already been through this in this game: the opening stock is absorbed, nothing more can be
            // taken silently. Switching back on after a pause does not deserve a question.
            var state = BillAutopilotState.Current;
            if (state != null && state.IsSeeded(bench))
            {
                Commit(profile, enabled: true);
                return;
            }

            int count = RecipesTaken(bench, profile);

            TaggedString text;
            if (profile.defaultMode == AutoMode.Always)
            {
                text = "BillAutopilot.Confirm.BodyAlways".Translate(count, bench.LabelCap);
            }
            else if (profile.defaultMode == AutoMode.Custom)
            {
                // Under a mode from another mod, announcing a target would be wrong, so the mode is named.
                var mode = DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(profile.defaultRepeatMode ?? "");
                text = mode != null
                    ? "BillAutopilot.Confirm.BodyCustom".Translate(count, bench.LabelCap, mode.LabelCap)
                    : "BillAutopilot.Confirm.BodyMaintain".Translate(count, bench.LabelCap, profile.targetCount);
            }
            else
            {
                text = "BillAutopilot.Confirm.BodyMaintain".Translate(count, bench.LabelCap, profile.targetCount);
            }

            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                text,
                () => Commit(profile, enabled: true),
                destructive: false,
                title: "BillAutopilot.Confirm.Title".Translate(bench.LabelCap)));
        }

        private static void Commit(BenchProfile profile, bool enabled)
        {
            profile.enabled = enabled;
            BillAutopilotMod.Instance.WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        /// <summary>
        /// How many recipes the autopilot would take charge of. The countability test is done by hand
        /// rather than through RecipeProbe: this window also opens from the main menu, where there is
        /// no game in which to build a probe bill.
        /// </summary>
        private static int RecipesTaken(ThingDef bench, BenchProfile profile)
        {
            return bench.AllRecipes
                .Where(r => r != null)
                .Distinct()
                .Count(r =>
                {
                    if (!r.AvailableNow) return false;

                    bool countable = r.products != null && r.products.Count == 1 && r.specialProducts == null;
                    return profile.ModeFor(r, countable) != AutoMode.Excluded;
                });
        }
    }
}
