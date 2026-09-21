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
        ///
        /// No cell is named, and that is the fix for the first real run of this suite: seven
        /// scenarios died on "no free cell for a stockpile at (134, 150)" because the fixture's
        /// colony already occupies that square. Where the stock sits is not something any scenario
        /// here has an opinion about, so the step finds its own room instead of making the feature
        /// files carry a map layout they would have to be corrected against every fixture.
        /// </summary>
        [Given("the Bill Autopilot test stockpile holds {int} {string}")]
        public void SetStock(PickleContext ctx, int count, string thingDefName)
        {
            var map = Driver.Map(ctx);
            var def = DefDatabase<ThingDef>.GetNamedSilentFail(thingDefName);
            ctx.Require(def != null, $"no ThingDef named '{thingDefName}'");
            ctx.Require(!def.MadeFromStuff,
                $"{thingDefName} is made from stuff, so 'how many' is not a whole answer: pick a "
                + "product with no stuff, or extend this step to name one");

            var zone = TestStockpile(ctx, map, count, def);

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
                $"the test stockpile holds {zone.Cells.Count} cells, room for "
                + $"{zone.Cells.Count * def.stackLimit} {thingDefName} and not {count}");

            // ResourceCounter recounts on its own every few hundred ticks. A check fired straight
            // after a spawn would otherwise read the old number and blame the mod for it.
            map.resourceCounter.UpdateResourceCounts();
        }

        /// <summary>The label the test stockpile carries, so a scenario can find the one it made.</summary>
        private const string ZoneLabel = "BillAutopilot test stock";

        /// <summary>
        /// The scenario's own stockpile, made once and reused. It is found again by its label rather
        /// than by a remembered reference: a save reload replaces every object in the game, and a
        /// scenario that moves its stock across a round trip would otherwise be filling a zone that
        /// no longer belongs to the live map.
        ///
        /// Room is taken wherever the map has it. The whole map is swept in reading order for enough
        /// free, unzoned, walkable cells; a colony's buildings, existing zones and walls are simply
        /// skipped. Only a map with no room at all fails, and it says how many cells it wanted.
        /// </summary>
        private static Zone_Stockpile TestStockpile(PickleContext ctx, Map map, int count, ThingDef def)
        {
            foreach (var existing in map.zoneManager.AllZones)
            {
                if (existing is Zone_Stockpile mine && mine.label == ZoneLabel) return mine;
            }

            // One cell per stack, plus a margin, so a scenario asking for a large stock is not
            // refused by a zone sized for a small one.
            int wanted = count / System.Math.Max(1, def.stackLimit) + 2;

            var cells = new List<IntVec3>();
            foreach (var cell in map.AllCells)
            {
                if (cells.Count >= wanted) break;
                if (map.zoneManager.ZoneAt(cell) != null) continue;
                if (cell.GetEdifice(map) != null) continue;
                if (!cell.Standable(map)) continue;
                cells.Add(cell);
            }

            ctx.Require(cells.Count >= wanted,
                $"this map has {cells.Count} free unzoned cells and {wanted} are needed to hold "
                + $"{count} {def.defName}: everything else is built on, zoned or impassable");

            var zone = new Zone_Stockpile(StorageSettingsPreset.DefaultStockpile, map.zoneManager);
            map.zoneManager.RegisterZone(zone);
            zone.label = ZoneLabel;
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
