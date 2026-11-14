using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorks.Pickle;
using RimWorld;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// What the Workshop page's own captures need and no other scenario does: a profile window with every
    /// group collapsed except one, so that a single overridden recipe is visible without a scroll.
    ///
    /// The collapsed state is a static of the window, kept across openings on purpose (a workshop of sixty
    /// recipes is a punishment to find expanded again), so it would leak into the next scenario; a hook
    /// clears it after every scenario.
    /// </summary>
    [PickleSteps]
    public class GallerySteps
    {
        private const BindingFlags Any = BindingFlags.Static | BindingFlags.Instance
                                         | BindingFlags.Public | BindingFlags.NonPublic;

        private static HashSet<string> CollapsedSet()
        {
            var field = typeof(Dialog_BenchProfile).GetField("Collapsed", Any);
            return field?.GetValue(null) as HashSet<string>;
        }

        [When("Bill Autopilot's profile window keeps open only the group of {string}")]
        public void KeepOnlyGroupOf(PickleContext ctx, string recipeDefName)
        {
            var window = Find.WindowStack.WindowOfType<Dialog_BenchProfile>();
            ctx.Require(window != null, "no Bill Autopilot profile window is open");

            var groupsField = typeof(Dialog_BenchProfile).GetField("groups", Any);
            ctx.Require(groupsField != null, "Dialog_BenchProfile.groups no longer exists: update these steps");
            var collapsed = CollapsedSet();
            ctx.Require(collapsed != null, "Dialog_BenchProfile.Collapsed no longer exists: update these steps");

            var groups = ((IEnumerable)groupsField.GetValue(window)).Cast<object>().ToList();
            string keep = null;
            var labels = new List<string>();
            foreach (var group in groups)
            {
                var type = group.GetType();
                string label = (string)type.GetField("label").GetValue(group);
                var recipes = (List<RecipeDef>)type.GetField("recipes").GetValue(group);
                labels.Add(label);
                if (recipes.Any(r => r.defName == recipeDefName)) keep = label;
            }

            ctx.Require(keep != null,
                $"no group of this window holds {recipeDefName}; the groups are: {string.Join(", ", labels.ToArray())}");

            collapsed.Clear();
            foreach (var label in labels)
            {
                if (label != keep) collapsed.Add(label);
            }
        }

        private static bool? tutorBefore;

        /// <summary>
        /// The owner's remark on the first captures, "in an interior, put lights, otherwise it is too dark", as a check
        /// rather than a judgement after the fact: the light the game itself computes on a cell, ground level, sky
        /// included (a roofed room has none of it, so what is read is the torches). A picture that is too dark then fails
        /// here, saying how dark, instead of costing a run to be found out by eye.
        /// </summary>
        [Then("the cell \\({int}, {int}\\) is lit at least {int} percent")]
        public async System.Threading.Tasks.Task AssertLit(PickleContext ctx, int x, int z, int percent)
        {
            var map = Find.CurrentMap;
            ctx.Require(map != null, "no map is loaded");

            // In 1.6 the glow grid is recomputed from its glowers by the map's own per-frame update
            // (GlowGrid.GlowGridUpdate_First), so a lamp spawned a moment ago is counted after a few frames, and frames
            // advance even with the game paused.
            await ctx.WaitFrames(5);
            float glow = map.glowGrid.GroundGlowAt(new IntVec3(x, 0, z));
            ctx.Assert(glow * 100f >= percent,
                $"the cell ({x}, {z}) is lit at {glow * 100f:0} percent, not {percent}: the room is too dark for a "
                + "picture. Check the torches are placed, filled and inside the roofed room");
        }

        [Given("the Learning helper is switched off")]
        public void TutorOff(PickleContext ctx)
        {
            if (tutorBefore == null) tutorBefore = Prefs.AdaptiveTrainingEnabled;
            Prefs.AdaptiveTrainingEnabled = false;
            Find.ActiveLesson?.Deactivate();
        }

        [AfterScenario]
        public void ForgetCollapsedGroups(PickleContext ctx)
        {
            CollapsedSet()?.Clear();
            if (tutorBefore != null)
            {
                Prefs.AdaptiveTrainingEnabled = tutorBefore.Value;
                tutorBefore = null;
            }
        }
    }
}
