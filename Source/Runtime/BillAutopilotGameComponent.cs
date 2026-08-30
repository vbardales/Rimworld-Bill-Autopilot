using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>Ce qu'une bill automatique valait au moment ou le pilote l'a posee.</summary>
    public class BillStamp : IExposable
    {
        public AutoMode mode = AutoMode.Maintain;
        public int targetCount = -1;
        public int floorCount = -1;

        public void ExposeData()
        {
            Scribe_Values.Look(ref mode, "mode", AutoMode.Maintain);
            Scribe_Values.Look(ref targetCount, "targetCount", -1);
            Scribe_Values.Look(ref floorCount, "floorCount", -1);
        }
    }

    /// <summary>
    /// Etat propre a la partie : quelles bills appartiennent au pilote, et quelles recettes ont deja
    /// ete vues. La configuration, elle, vit dans les reglages du mod et vaut pour toutes les parties.
    /// </summary>
    public class BillAutopilotGameComponent : GameComponent
    {
        public static BillAutopilotGameComponent Current =>
            Verse.Current.Game?.GetComponent<BillAutopilotGameComponent>();

        /// <summary>loadID de la bill -> reglages appliques a sa creation.</summary>
        private Dictionary<int, BillStamp> stamps = new Dictionary<int, BillStamp>();

        /// <summary>"DefEtabli/DefRecette" des recettes deja rencontrees : ce qui n'y est pas est neuf.</summary>
        private HashSet<string> known = new HashSet<string>();

        /// <summary>
        /// Recettes signalees mais pas encore acceptees. Tant qu'une recette y figure, AUCUN etabli du
        /// type ne la lance : sans cela le premier etabli poserait la bill suspendue et le deuxieme se
        /// mettrait a produire, la question posee restant sans reponse.
        /// </summary>
        private HashSet<string> pending = new HashSet<string>();

        /// <summary>Types d'etabli dont le stock initial de recettes a deja ete absorbe.</summary>
        private HashSet<string> seeded = new HashSet<string>();

        private List<string> pendingNews = new List<string>();

        private readonly Queue<Building_WorkTable> queue = new Queue<Building_WorkTable>();
        private int lastRefillTick = -99999;
        private bool dirty = true;

        public BillAutopilotGameComponent(Game game)
        {
            RecipeProbe.Reset();
        }

        // --- Appartenance ------------------------------------------------------------------------

        public bool IsAuto(Bill bill) => bill != null && stamps.ContainsKey(bill.loadID);

        public BillStamp StampOf(Bill bill) =>
            bill != null && stamps.TryGetValue(bill.loadID, out var stamp) ? stamp : null;

        public void Claim(Bill bill, BillStamp stamp) => stamps[bill.loadID] = stamp;

        public void Disown(Bill bill) => stamps.Remove(bill.loadID);

        // --- Recettes deja vues ------------------------------------------------------------------

        private static string Key(ThingDef bench, RecipeDef recipe) => bench.defName + "/" + recipe.defName;

        public bool IsKnown(ThingDef bench, RecipeDef recipe) => known.Contains(Key(bench, recipe));

        public void MarkKnown(ThingDef bench, RecipeDef recipe) => known.Add(Key(bench, recipe));

        public bool IsPending(ThingDef bench, RecipeDef recipe) => pending.Contains(Key(bench, recipe));

        public void MarkPending(ThingDef bench, RecipeDef recipe) => pending.Add(Key(bench, recipe));

        public void Accept(ThingDef bench, RecipeDef recipe) => pending.Remove(Key(bench, recipe));

        /// <summary>
        /// A l'activation d'un type d'etabli, tout ce qui est deja debloque est absorbe en silence :
        /// "je veux toutes les recettes" veut dire celles d'aujourd'hui, sans quarante lignes suspendues
        /// d'un coup. Seul ce qui se debloque ENSUITE se signale.
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

        /// <summary>Oublie tout d'un type d'etabli : le prochain passage refera une absorption complete.</summary>
        public void ForgetBench(ThingDef bench)
        {
            seeded.Remove(bench.defName);
            var prefix = bench.defName + "/";
            known.RemoveWhere(k => k.StartsWith(prefix));
            pending.RemoveWhere(k => k.StartsWith(prefix));
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

        // --- Boucle ------------------------------------------------------------------------------

        public void MarkDirty() => dirty = true;

        public override void GameComponentTick()
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

            // Un etabli par tick : l'intervalle est un budget, pas une salve.
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
            for (int m = 0; m < maps.Count; m++)
            {
                var map = maps[m];
                if (!map.IsPlayerHome && map.mapPawns.FreeColonistsSpawnedCount == 0) continue;

                var buildings = map.listerBuildings.allBuildingsColonist;
                for (int b = 0; b < buildings.Count; b++)
                {
                    if (buildings[b] is Building_WorkTable table) queue.Enqueue(table);
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref stamps, "stamps", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref known, "known", LookMode.Value);
            Scribe_Collections.Look(ref pending, "pending", LookMode.Value);
            Scribe_Collections.Look(ref seeded, "seeded", LookMode.Value);

            if (stamps == null) stamps = new Dictionary<int, BillStamp>();
            if (known == null) known = new HashSet<string>();
            if (pending == null) pending = new HashSet<string>();
            if (seeded == null) seeded = new HashSet<string>();
            if (pendingNews == null) pendingNews = new List<string>();
        }
    }
}
