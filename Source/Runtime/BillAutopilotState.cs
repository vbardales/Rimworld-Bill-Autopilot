using System.Collections.Generic;
using System.Linq;
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
    ///
    /// **Ce n'est volontairement pas un GameComponent.** Le jeu ecrit un composant sous la forme
    /// `&lt;li Class="BillAutopilot..."&gt;` : retirer le mod rendrait cette classe introuvable et
    /// chaque chargement de la sauvegarde cracherait un "Can't load abstract class Verse.GameComponent".
    /// Nos noeuds sont donc greffes dans `&lt;game&gt;` par un postfix sur Game.ExposeSmallComponents,
    /// sans attribut Class : sans le mod, personne ne les lit et le jeu les ignore en silence.
    /// </summary>
    public class BillAutopilotState
    {
        private static Game owner;
        private static BillAutopilotState instance;

        /// <summary>
        /// L'etat suit l'objet Game : nouvelle partie comme chargement en construisent un neuf, donc
        /// rien ne fuit d'une partie a l'autre sans qu'on ait a s'accrocher au cycle de vie.
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
        /// loadID de la bill -> reglages appliques a sa creation. Deep-saved, mais sans attribut
        /// Class : Scribe_Deep n'en ecrit un que si le type reel differe du type declare. Ne pas
        /// elargir le type de valeur du dictionnaire, sous peine de reintroduire l'attribut.
        /// </summary>
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

        /// <summary>
        /// "DefEtabli/DefRecette" -> ce que les autres mods avaient pose sur la bill retiree. C'est ce
        /// qui permet au cycle retirer/reposer de ne rien perdre.
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

        // --- Memoire des bills retirees ----------------------------------------------------------

        public BillMemory MemoryFor(ThingDef bench, RecipeDef recipe) =>
            memories.TryGetValue(Key(bench, recipe), out var memory) ? memory : null;

        public void Remember(ThingDef bench, RecipeDef recipe, BillMemory memory)
        {
            var key = Key(bench, recipe);
            if (memory == null) memories.Remove(key);
            else memories[key] = memory;
        }

        public void Forget(ThingDef bench, RecipeDef recipe) => memories.Remove(Key(bench, recipe));

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

            var stale = memories.Keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in stale) memories.Remove(key);
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
            if (maps == null) return;

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

        // --- Sauvegarde --------------------------------------------------------------------------

        /// <summary>
        /// Appele depuis un postfix sur Game.ExposeSmallComponents, donc a l'interieur du noeud
        /// &lt;game&gt;, a l'ecriture comme a la lecture. Prefixes explicites : on est chez le jeu,
        /// pas chez nous.
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
