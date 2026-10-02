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

        // Nelim's Pickle Tools' zen meadow studio (fixture nelim-zen-meadow-studio) is built around cell (125, 125). Its
        // "display" pavilion, the large empty interior meant for mod demonstrations, is the wall rectangle below, with its
        // door side to the north (z = -24, opened over x = -2..2). The studio leaves every roof off on purpose, which is
        // right for its own pictures and wrong for a bench: an unroofed room is "outdoors" for work (the malus that read
        // "Work speed factor: 40% (outdoors)" on the first gallery attempt) and, once roofed, is dark without a light.
        private const int StudioX = 125, StudioZ = 125;
        private const int PavilionX0 = -9, PavilionZ0 = -36, PavilionWidth = 19, PavilionHeight = 13;

        /// <summary>
        /// Closes the studio's display pavilion (a door and four wall segments where it is open), roofs it and lights it
        /// with four standing torches, filled: a torch burns wood, needs no power grid, and is the warm light the studio
        /// already uses for its own lanterns.
        ///
        /// Built with the region updater held off, then rebuilt once. Doing this cell by cell lets a spawn's own region
        /// notification and the roof notifications interleave, and RimWorld's dirty-cell set is a HashSet with no
        /// re-entrancy guard: on 2026-09-27, run `88b3` hit "Collection was modified; enumeration operation may not
        /// execute" inside Verse.RegionAndRoomUpdater.RegenerateNewRegionsFromDirtyCells, not deterministically, and a
        /// later scenario timed out loading its save with the region graph left inconsistent. Disabling the updater for
        /// the whole build and rebuilding once afterwards is the pattern RimWorld's own bulk map edits use.
        /// </summary>
        [When("the studio's display pavilion is closed, roofed and lit")]
        public void ClosePavilion(PickleContext ctx)
        {
            var map = Find.CurrentMap;
            ctx.Require(map != null, "no map is loaded");

            var rect = new CellRect(StudioX + PavilionX0, StudioZ + PavilionZ0, PavilionWidth, PavilionHeight);
            ctx.Require(rect.InBounds(map), "the pavilion would leave the map: this is not the studio's map");
            var corner = new IntVec3(rect.minX, 0, rect.minZ);
            ctx.Require(corner.GetEdifice(map)?.def == ThingDefOf.Wall,
                "no wall at the display pavilion's corner: load the save \"nelim-zen-meadow-studio\", with the "
                + "nelim.pickletools.screenshotstudio mod in the pass");

            var updater = map.regionAndRoomUpdater;
            bool wasEnabled = updater.Enabled;
            updater.Enabled = false;
            try
            {
                var wood = ThingDefOf.WoodLog;

                // The door side: a door in the middle, wall on either side of it.
                int doorZ = rect.maxZ;
                for (int dx = -2; dx <= 2; dx++)
                {
                    var cell = new IntVec3(StudioX + dx, 0, doorZ);
                    foreach (var thing in cell.GetThingList(map).ToList())
                    {
                        if (!(thing is Pawn) && thing.def.destroyable) thing.Destroy();
                    }

                    var def = dx == 0 ? ThingDefOf.Door : ThingDefOf.Wall;
                    var built = ThingMaker.MakeThing(def, wood);
                    built.SetFaction(Faction.OfPlayer);
                    GenSpawn.Spawn(built, cell, map);
                }

                foreach (var cell in rect) map.roofGrid.SetRoof(cell, RoofDefOf.RoofConstructed);

                // The game's own wood plank floor over the whole interior: the studio's dark green ground swallowed the
                // picture (owner, 2026-10-02).
                var parquet = DefDatabase<TerrainDef>.GetNamed("WoodPlankFloor");
                foreach (var cell in rect.ContractedBy(1)) map.terrainGrid.SetTerrain(cell, parquet);

                // Four flowering plant pots beside the bench (at x 0, z -29), none on a torch cell nor in front of the
                // bills tab. A pot grows nothing by itself, so each gets a grown flower on its cell.
                var flowers = new[] { "Plant_Rose", "Plant_Daylily" };
                int flower = 0;
                foreach (var spot in new[] { new IntVec2(-3, -30), new IntVec2(3, -30), new IntVec2(-3, -28), new IntVec2(3, -28) })
                {
                    var cell = new IntVec3(StudioX + spot.x, 0, StudioZ + spot.z);
                    foreach (var thing in cell.GetThingList(map).ToList())
                    {
                        if (!(thing is Pawn) && thing.def.destroyable) thing.Destroy();
                    }

                    var pot = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("PlantPot"), ThingDefOf.WoodLog);
                    pot.SetFaction(Faction.OfPlayer);
                    GenSpawn.Spawn(pot, cell, map);

                    var plant = (Plant)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(flowers[flower++ % flowers.Length]));
                    plant.Growth = 1f;
                    GenSpawn.Spawn(plant, cell, map);
                }

                // Four in the corners, four close to the bench: the corners alone lit the bench cell at 25 percent (run c1fe,
                // 2026-09-28), a torch's light falling off well before the 8 cells that separate them from the middle.
                foreach (var spot in new[]
                {
                    new IntVec2(-7, -34), new IntVec2(7, -34), new IntVec2(-7, -26), new IntVec2(7, -26),
                    new IntVec2(-4, -32), new IntVec2(4, -32), new IntVec2(-4, -26), new IntVec2(4, -26),
                })
                {
                    var cell = new IntVec3(StudioX + spot.x, 0, StudioZ + spot.z);
                    var lamp = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("TorchLamp"));
                    lamp.SetFaction(Faction.OfPlayer);
                    GenSpawn.Spawn(lamp, cell, map);
                    var fuel = lamp.TryGetComp<CompRefuelable>();
                    fuel?.Refuel(fuel.Props.fuelCapacity);
                }
            }
            finally
            {
                updater.Enabled = wasEnabled;
            }

            updater.RebuildAllRegionsAndRooms();
        }
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

        [When("the camera looks at \\({int}, {int}\\) from a distance of {int}")]
        public void FrameCamera(PickleContext ctx, int x, int z, int size)
        {
            var driver = Find.CameraDriver;
            ctx.Require(driver != null, "no camera driver");
            driver.JumpToCurrentMapLoc(new IntVec3(x, 0, z));
            foreach (var name in new[] { "rootSize", "desiredSize" })
            {
                var field = typeof(CameraDriver).GetField(name, Any);
                if (field != null && field.FieldType == typeof(float)) field.SetValue(driver, (float)size);
            }
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
