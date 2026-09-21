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

            // GenSpawn rather than GenPlace: GenPlace REFUSES a cell it dislikes and returns false,
            // and the first version of this step ignored that answer. Nothing was spawned, the stock
            // stayed at zero, and the failure surfaced three steps later as "the autopilot counts 0,
            // not 30" - which reads as a broken threshold in the mod. GenSpawn puts the stack where
            // it is told.
            int left = count;
            int placed = 0;
            foreach (var cell in zone.Cells)
            {
                if (left <= 0) break;

                int stack = left < def.stackLimit ? left : def.stackLimit;
                var thing = ThingMaker.MakeThing(def);
                thing.stackCount = stack;
                GenSpawn.Spawn(thing, cell, map);
                placed += stack;
                left -= stack;
            }

            ctx.Require(left <= 0,
                $"the test stockpile holds {zone.Cells.Count} cells, room for "
                + $"{zone.Cells.Count * def.stackLimit} {thingDefName} and not {count}");

            // ResourceCounter recounts on its own every few hundred ticks. A check fired straight
            // after a spawn would otherwise read the old number and blame the mod for it.
            map.resourceCounter.UpdateResourceCounts();

            // And the step proves its own work before handing back. What the scenarios care about is
            // not that stacks exist somewhere but that the game COUNTS them, which is a different
            // claim: a stack outside storage, or on a cell the zone does not really cover, exists
            // and counts for nothing. Checked through the game's own counter, so a stock this step
            // cannot make is reported here, by the step that failed to make it.
            int seen = map.resourceCounter.GetCount(def);
            ctx.Require(seen == count,
                $"{placed} {thingDefName} were spawned into the test stockpile of {zone.Cells.Count} "
                + $"cells, and the game counts {seen}. A stack that is not in storage counts for "
                + "nothing, so no threshold in this scenario would mean what it says");
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

            // Swept outwards from the middle of the map rather than from its corner. A corner is as
            // free as anywhere and works for storage, but the colony is in the middle, and a
            // stockpile the scenarios can also be photographed beside is worth the two extra lines.
            var cells = new List<IntVec3>();
            var centre = map.Center;
            for (int radius = 0; radius < map.Size.x && cells.Count < wanted; radius += 2)
            {
                foreach (var cell in GenRadial.RadialCellsAround(centre, radius, true))
                {
                    if (cells.Count >= wanted) break;
                    if (!cell.InBounds(map) || cells.Contains(cell)) continue;
                    if (map.zoneManager.ZoneAt(cell) != null) continue;
                    if (cell.GetEdifice(map) != null) continue;
                    if (!cell.Standable(map)) continue;
                    cells.Add(cell);
                }
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
