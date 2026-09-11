using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>What an automatic bill was worth at the moment the autopilot put it up.</summary>
    public class BillStamp : IExposable
    {
        public AutoMode mode = AutoMode.Maintain;

        /// <summary>defName of the repeat mode when <see cref="mode"/> is Custom.</summary>
        public string repeatModeDefName;

        public int targetCount = -1;
        public int floorCount = -1;

        public void ExposeData()
        {
            Scribe_Values.Look(ref mode, "mode", AutoMode.Maintain);
            Scribe_Values.Look(ref repeatModeDefName, "repeatMode");
            Scribe_Values.Look(ref targetCount, "targetCount", -1);
            Scribe_Values.Look(ref floorCount, "floorCount", -1);
        }
    }

    /// <summary>
    /// Per-game state: which bills belong to the autopilot, and which recipes have already been seen.
    /// The configuration itself lives in the mod settings and holds for every game.
    ///
    /// **This is deliberately not a GameComponent.** The game writes a component as
    /// `&lt;li Class="BillAutopilot..."&gt;`: removing the mod would make that class unfindable, and
    /// every load of the save would spit out "Can't load abstract class Verse.GameComponent". Our
    /// nodes are therefore grafted into `&lt;game&gt;` by a postfix on Game.ExposeSmallComponents,
    /// with no Class attribute: without the mod nobody reads them and the game ignores them silently.
    /// </summary>
    public class BillAutopilotState
    {
        private static Game owner;
        private static BillAutopilotState instance;

        /// <summary>
        /// The state follows the Game object: a new game and a load each build a fresh one, so nothing
        /// leaks from one game to the next without having to hook into the lifecycle.
        /// </summary>
        public static BillAutopilotState Current
        {
            get
            {
                var game = Verse.Current.Game;
                if (game == null) return null;

                if (owner != game)
                {
                    owner = game;
                    instance = new BillAutopilotState();
                }
                return instance;
            }
        }

        /// <summary>
        /// Bill loadID -> the settings applied when it was created. Deep-saved, but with no Class
        /// attribute: Scribe_Deep only writes one when the real type differs from the declared type.
        /// Do not widen the dictionary value type, or the attribute comes back.
        /// </summary>
        private Dictionary<int, BillStamp> stamps = new Dictionary<int, BillStamp>();

        /// <summary>"BenchDef/RecipeDef" of recipes already met: anything absent from it is new.</summary>
        private HashSet<string> known = new HashSet<string>();

        /// <summary>
        /// Recipes announced but not yet accepted. While a recipe is listed here, NO workbench of that
        /// type starts it: without this the first bench would put up the suspended bill and the second
        /// would start producing, leaving the question asked but unanswered.
        /// </summary>
        private HashSet<string> pending = new HashSet<string>();

        /// <summary>Workbench types whose opening stock of recipes has already been absorbed.</summary>
        private HashSet<string> seeded = new HashSet<string>();

        /// <summary>
        /// "bench id/RecipeDef" -> what other mods had put on the bill that was taken down. This is
        /// what lets the remove/replace cycle lose nothing. Keyed by the bench, not by its type: see
        /// MemoryKey.
        /// </summary>
        private Dictionary<string, BillMemory> memories = new Dictionary<string, BillMemory>();

        private List<string> pendingNews = new List<string>();

        private readonly Queue<Building_WorkTable> queue = new Queue<Building_WorkTable>();
        private int lastRefillTick = -99999;
        private bool dirty = true;

        public BillAutopilotState()
        {
            RecipeProbe.Reset();
        }

        // --- Ownership ------------------------------------------------------------------------

        public bool IsAuto(Bill bill) => bill != null && stamps.ContainsKey(bill.loadID);

        public BillStamp StampOf(Bill bill) =>
            bill != null && stamps.TryGetValue(bill.loadID, out var stamp) ? stamp : null;

        public void Claim(Bill bill, BillStamp stamp) => stamps[bill.loadID] = stamp;

        public void Disown(Bill bill) => stamps.Remove(bill.loadID);

        // --- Recipes already seen ------------------------------------------------------------------

        private static string Key(ThingDef bench, RecipeDef recipe) => bench.defName + "/" + recipe.defName;

        public bool IsKnown(ThingDef bench, RecipeDef recipe) => known.Contains(Key(bench, recipe));

        public void MarkKnown(ThingDef bench, RecipeDef recipe) => known.Add(Key(bench, recipe));

        public bool IsPending(ThingDef bench, RecipeDef recipe) => pending.Contains(Key(bench, recipe));

        public void MarkPending(ThingDef bench, RecipeDef recipe) => pending.Add(Key(bench, recipe));

        public void Accept(ThingDef bench, RecipeDef recipe) => pending.Remove(Key(bench, recipe));

        // --- Memory of bills taken down ----------------------------------------------------------

        /// <summary>
        /// Keyed by the workbench itself, not by its type. What another mod attaches to a bill (a
        /// name, a link group, a widened count) belongs to that one bill on that one bench: keyed by
        /// type, two benches of the same kind each carrying a differently-set bill for the same recipe
        /// would share one entry, and the last one taken down would overwrite the other.
        /// </summary>
        private static string MemoryKey(Thing table, RecipeDef recipe) =>
            table.thingIDNumber + "/" + recipe.defName;

        public BillMemory MemoryFor(Thing table, RecipeDef recipe) =>
            memories.TryGetValue(MemoryKey(table, recipe), out var memory) ? memory : null;

        public void Remember(Thing table, RecipeDef recipe, BillMemory memory)
        {
            var key = MemoryKey(table, recipe);
            if (memory == null) memories.Remove(key);
            else memories[key] = memory;
        }

        public void Forget(Thing table, RecipeDef recipe) => memories.Remove(MemoryKey(table, recipe));

        /// <summary>Has this workbench type already absorbed its opening stock in this game?</summary>
        public bool IsSeeded(ThingDef bench) => bench != null && seeded.Contains(bench.defName);

        /// <summary>
        /// When a workbench type is switched on, everything already unlocked is absorbed in silence:
        /// "I want all the recipes" means today's, without forty suspended lines at once. Only what is
        /// unlocked AFTERWARDS announces itself.
        /// </summary>
        public void SeedIfNeeded(ThingDef bench)
        {
            if (!seeded.Add(bench.defName)) return;

            var recipes = bench.AllRecipes;
            for (int i = 0; i < recipes.Count; i++)
            {
                if (recipes[i].AvailableNow) known.Add(Key(bench, recipes[i]));
            }
        }

        // --- Notifications -----------------------------------------------------------------------

        public void QueueNewRecipeNotice(ThingDef bench, RecipeDef recipe)
        {
            pendingNews.Add("BillAutopilot.NewRecipeLine".Translate(recipe.LabelCap, bench.LabelCap).Resolve());
        }

        private void FlushNotices()
        {
            if (pendingNews.Count == 0) return;

            if (BillAutopilotMod.Settings.notifyNewRecipes)
            {
                Find.LetterStack.ReceiveLetter(
                    "BillAutopilot.NewRecipeLetterTitle".Translate(pendingNews.Count),
                    "BillAutopilot.NewRecipeLetterBody".Translate(string.Join("\n", pendingNews.ToArray())),
                    LetterDefOf.NeutralEvent);
            }
            pendingNews.Clear();
        }

        // --- Loop ------------------------------------------------------------------------------

        public void MarkDirty() => dirty = true;

        public void Tick()
        {
            var settings = BillAutopilotMod.Settings;
            int interval = settings.syncIntervalTicks < 60 ? 60 : settings.syncIntervalTicks;
            int now = Find.TickManager.TicksGame;

            if (dirty || now - lastRefillTick >= interval)
            {
                Refill();
                lastRefillTick = now;
                dirty = false;
            }

            // One workbench per tick: the interval is a budget, not a burst.
            if (queue.Count > 0)
            {
                var table = queue.Dequeue();
                if (table != null && table.Spawned && table.Map != null)
                {
                    AutoBillSync.Sync(table);
                }
            }
            else if (pendingNews.Count > 0)
            {
                FlushNotices();
            }
        }

        private void Refill()
        {
            queue.Clear();
            var maps = Find.Maps;
            if (maps == null) return;

            // Every living workbench is counted, including those on maps not synced this pass: a bench
            // must not lose its memory merely because nobody is home on its map.
            var live = new HashSet<int>();

            for (int m = 0; m < maps.Count; m++)
            {
                var map = maps[m];
                bool sync = map.IsPlayerHome || map.mapPawns.FreeColonistsSpawnedCount > 0;

                var buildings = map.listerBuildings.allBuildingsColonist;
                for (int b = 0; b < buildings.Count; b++)
                {
                    if (!(buildings[b] is Building_WorkTable table)) continue;

                    live.Add(table.thingIDNumber);
                    if (sync) queue.Enqueue(table);
                }
            }

            PruneMemories(live);
        }

        /// <summary>
        /// A bench that no longer exists keeps no memory. thingIDNumber is never reused, so an entry
        /// whose bench is gone would sit there for the rest of the game. Entries written before the
        /// memory was keyed per bench go the same way: their key names a def, never a live id.
        /// </summary>
        private void PruneMemories(HashSet<int> liveBenches)
        {
            // No bench in sight means we are not looking at a settled game, not that every bench died.
            if (memories.Count == 0 || liveBenches.Count == 0) return;

            List<string> stale = null;
            foreach (var key in memories.Keys)
            {
                int slash = key.IndexOf('/');
                if (slash > 0
                    && int.TryParse(key.Substring(0, slash), out int id)
                    && liveBenches.Contains(id))
                {
                    continue;
                }

                (stale ?? (stale = new List<string>())).Add(key);
            }

            if (stale == null) return;
            foreach (var key in stale) memories.Remove(key);
        }

        // --- Saving --------------------------------------------------------------------------

        /// <summary>
        /// Called from a postfix on Game.ExposeSmallComponents, therefore inside the &lt;game&gt; node, on
        /// write as on read. Explicit prefixes on the node names: this is the game's house, not ours.
        /// </summary>
        public void ExposeData()
        {
            Scribe_Collections.Look(ref stamps, "billAutopilotStamps", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref known, "billAutopilotKnown", LookMode.Value);
            Scribe_Collections.Look(ref pending, "billAutopilotPending", LookMode.Value);
            Scribe_Collections.Look(ref seeded, "billAutopilotSeeded", LookMode.Value);
            Scribe_Collections.Look(ref memories, "billAutopilotMemories", LookMode.Value, LookMode.Deep);

            if (memories == null) memories = new Dictionary<string, BillMemory>();
            if (stamps == null) stamps = new Dictionary<int, BillStamp>();
            if (known == null) known = new HashSet<string>();
            if (pending == null) pending = new HashSet<string>();
            if (seeded == null) seeded = new HashSet<string>();
            if (pendingNews == null) pendingNews = new List<string>();
        }
    }
}
