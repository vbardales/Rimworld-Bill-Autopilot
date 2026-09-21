using System.Collections.Generic;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The colony's stock of a thing, set to an exact number.
    ///
    /// This is the instrument the base loop needs. A bill goes up while the stock is at or below the
    /// restart threshold and comes down once it reaches the target, so a scenario that cannot place
    /// the stock exactly on either side of those two numbers cannot show the gap between them - and
    /// the gap is the whole reason the bill does not flicker on every unit made.
    ///
    /// Everything lands in a stockpile on purpose. RimWorld's own <c>ResourceCounter</c>, which is
    /// what the recipe's counter reads for a countable product, only counts what sits in storage; a
    /// stack dropped on open ground would leave the count at zero and read as a broken threshold.
    /// </summary>
    [PickleSteps]
    public class StockSteps
    {
        /// <summary>
        /// Exactly N, not "at least N": every existing stack of that def on the map is destroyed
        /// first. A fixture arrives with a colony's worth of odds and ends, and a scenario that only
        /// added to it would be asserting against a number it did not choose.
        /// </summary>
        [Given("the Bill Autopilot test stockpile at ({int}, {int}) holds {int} {string}")]
        public void SetStock(PickleContext ctx, int x, int z, int count, string thingDefName)
        {
            var map = Driver.Map(ctx);
            var def = DefDatabase<ThingDef>.GetNamedSilentFail(thingDefName);
            ctx.Require(def != null, $"no ThingDef named '{thingDefName}'");
            ctx.Require(!def.MadeFromStuff,
                $"{thingDefName} is made from stuff, so 'how many' is not a whole answer: pick a "
                + "product with no stuff, or extend this step to name one");

            var zone = StockpileAt(ctx, map, x, z);

            foreach (var existing in map.listerThings.ThingsOfDef(def).ToList())
            {
                existing.Destroy(DestroyMode.Vanish);
            }

            int left = count;
            foreach (var cell in zone.Cells)
            {
                if (left <= 0) break;

                int stack = left < def.stackLimit ? left : def.stackLimit;
                var thing = ThingMaker.MakeThing(def);
                thing.stackCount = stack;
                GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Direct);
                left -= stack;
            }

            ctx.Require(left <= 0,
                $"the stockpile at ({x}, {z}) holds {zone.Cells.Count} cells, room for "
                + $"{zone.Cells.Count * def.stackLimit} {thingDefName} and not {count}: make it bigger");

            // ResourceCounter recounts on its own every few hundred ticks. A check fired straight
            // after a spawn would otherwise read the old number and blame the mod for it.
            map.resourceCounter.UpdateResourceCounts();
        }

        /// <summary>
        /// A stockpile the scenario owns. Reused when the same cell is asked for twice, so a scenario
        /// can move its stock up and down across several steps without stacking zone on zone.
        /// </summary>
        private static Zone_Stockpile StockpileAt(PickleContext ctx, Map map, int x, int z)
        {
            var origin = new IntVec3(x, 0, z);
            ctx.Require(origin.InBounds(map),
                $"({x}, {z}) is outside this {map.Size.x} x {map.Size.z} map");

            if (map.zoneManager.ZoneAt(origin) is Zone_Stockpile existing) return existing;

            var zone = new Zone_Stockpile(StorageSettingsPreset.DefaultStockpile, map.zoneManager);
            map.zoneManager.RegisterZone(zone);

            var cells = new List<IntVec3>();
            for (int dx = 0; dx < 5; dx++)
            {
                for (int dz = 0; dz < 5; dz++)
                {
                    var cell = new IntVec3(x + dx, 0, z + dz);

                    // A cell already zoned, or one a zone cannot cover, is skipped rather than fought
                    // over: RimWorld's own designator does the same, and a stockpile of 23 cells
                    // serves this suite exactly as well as one of 25.
                    if (!cell.InBounds(map) || map.zoneManager.ZoneAt(cell) != null) continue;
                    if (cell.GetEdifice(map) != null) continue;
                    cells.Add(cell);
                }
            }

            ctx.Require(cells.Count > 0,
                $"no free cell for a stockpile at ({x}, {z}): every cell in the 5 x 5 square is "
                + "already zoned or built on");

            foreach (var cell in cells) zone.AddCell(cell);
            return zone;
        }
    }

    internal static class ThingListExtensions
    {
        /// <summary>
        /// A copy, because destroying a thing removes it from the list being walked. The obvious
        /// foreach over the live list drops every other stack and leaves a count nobody asked for.
        /// </summary>
        public static List<Thing> ToList(this IEnumerable<Thing> things)
        {
            var copy = new List<Thing>();
            foreach (var thing in things) copy.Add(thing);
            return copy;
        }
    }
}
