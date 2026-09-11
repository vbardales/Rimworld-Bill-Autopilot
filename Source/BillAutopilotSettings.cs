using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Ce qu'une recette doit faire sous pilote automatique.
    /// Volontairement independant de <see cref="RimWorld.BillRepeatModeDef"/> : les reglages du mod sont
    /// lus dans le constructeur de <see cref="Mod"/>, donc AVANT le chargement des defs. Un
    /// Scribe_Defs y resoudrait sur une DefDatabase vide.
    /// </summary>
    public enum AutoMode : byte
    {
        /// <summary>Suit le mode par defaut de l'etabli. Valable uniquement pour une regle de recette.</summary>
        Inherit = 0,

        /// <summary>Maintenir un stock : la bill apparait quand on descend au plancher, disparait une fois la cible atteinte.</summary>
        Maintain = 1,

        /// <summary>Toujours : la bill reste en place et tourne sans fin.</summary>
        Always = 2,

        /// <summary>Jamais : le pilote automatique ignore cette recette.</summary>
        Excluded = 3,

        /// <summary>
        /// Un mode de repetition pose par un autre mod, nomme a cote par son defName. Everybody Gets
        /// One en ajoute trois ; d'autres mods peuvent en ajouter. Le pilote ne cherche jamais a
        /// savoir ce qu'ils veulent dire : il les pose, et demande a leur proprietaire s'il y a du
        /// travail.
        /// </summary>
        Custom = 4,
    }

    /// <summary>Surcharge posee sur une recette precise, pour un type d'etabli donne.</summary>
    public class RecipeRule : IExposable
    {
        public AutoMode mode = AutoMode.Inherit;

        /// <summary>defName du mode quand <see cref="mode"/> vaut Custom. Une chaine, pas un Def :
        /// les reglages sont lus avant le chargement des defs.</summary>
        public string repeatMode;

        /// <summary>-1 : herite de l'etabli.</summary>
        public int targetCount = -1;

        /// <summary>-1 : herite de l'etabli.</summary>
        public int floorCount = -1;

        public bool IsDefault => mode == AutoMode.Inherit && targetCount < 0 && floorCount < 0;

        public void ExposeData()
        {
            Scribe_Values.Look(ref mode, "mode", AutoMode.Inherit);
            Scribe_Values.Look(ref repeatMode, "repeatMode");
            Scribe_Values.Look(ref targetCount, "targetCount", -1);
            Scribe_Values.Look(ref floorCount, "floorCount", -1);
        }
    }

    /// <summary>Profil d'un type d'etabli. Global : tous les etablis de ce ThingDef le partagent.</summary>
    public class BenchProfile : IExposable
    {
        public const int DefaultTargetCount = 50;
        public const int DefaultFloorCount = 25;

        public bool enabled;

        /// <summary>Mode applique aux recettes sans surcharge. Maintain, Always ou Custom.</summary>
        public AutoMode defaultMode = AutoMode.Maintain;

        /// <summary>defName du mode quand <see cref="defaultMode"/> vaut Custom.</summary>
        public string defaultRepeatMode;

        public int targetCount = DefaultTargetCount;
        public int floorCount = DefaultFloorCount;

        /// <summary>
        /// Mode des recettes que le jeu ne sait pas compter (decoupe, fonte, cremation, chirurgie...) :
        /// RecipeWorkerCounter.CanCountProducts y renvoie false, donc "maintenir X" leur est interdit.
        /// Seuls Always et Excluded sont valables.
        /// </summary>
        public AutoMode uncountableMode = AutoMode.Excluded;

        private Dictionary<string, RecipeRule> rules = new Dictionary<string, RecipeRule>();

        public Dictionary<string, RecipeRule> Rules => rules;

        public RecipeRule RuleFor(RecipeDef recipe)
        {
            if (recipe == null) return null;
            return rules.TryGetValue(recipe.defName, out var rule) ? rule : null;
        }

        public RecipeRule RuleForWriting(RecipeDef recipe)
        {
            if (!rules.TryGetValue(recipe.defName, out var rule))
            {
                rule = new RecipeRule();
                rules[recipe.defName] = rule;
            }
            return rule;
        }

        public void ClearRule(RecipeDef recipe)
        {
            if (recipe != null) rules.Remove(recipe.defName);
        }

        /// <summary>Mode effectif d'une recette, surcharge et repli des recettes non comptables compris.</summary>
        public AutoMode ModeFor(RecipeDef recipe, bool countable)
        {
            var rule = RuleFor(recipe);
            var mode = rule != null && rule.mode != AutoMode.Inherit ? rule.mode : defaultMode;

            // Un mode d'un autre mod dont le mod est parti : on retombe sur le notre plutot que de
            // poser une bill sans mode.
            if (mode == AutoMode.Custom && RepeatModeFor(recipe) == null) mode = AutoMode.Maintain;

            // Le repli des recettes non comptables ne vaut que pour "maintenir un stock" : c'est le
            // seul de nos modes qui exige de savoir compter. Un mode etranger decide pour lui-meme.
            if (mode == AutoMode.Maintain && !countable) mode = uncountableMode;
            return mode;
        }

        /// <summary>Le mode de repetition a poser quand le mode effectif est Custom, ou null.</summary>
        public BillRepeatModeDef RepeatModeFor(RecipeDef recipe)
        {
            var rule = RuleFor(recipe);
            var name = rule != null && rule.mode != AutoMode.Inherit ? rule.repeatMode : defaultRepeatMode;

            return string.IsNullOrEmpty(name)
                ? null
                : DefDatabase<BillRepeatModeDef>.GetNamedSilentFail(name);
        }

        public int TargetFor(RecipeDef recipe)
        {
            var rule = RuleFor(recipe);
            return rule != null && rule.targetCount >= 0 ? rule.targetCount : targetCount;
        }

        public int FloorFor(RecipeDef recipe)
        {
            var rule = RuleFor(recipe);
            var floor = rule != null && rule.floorCount >= 0 ? rule.floorCount : floorCount;
            return Mathf_Min(floor, TargetFor(recipe));
        }

        private static int Mathf_Min(int a, int b) => a < b ? a : b;

        public int OverrideCount => rules.Count(pair => !pair.Value.IsDefault);

        public void ExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled", defaultValue: false);
            Scribe_Values.Look(ref defaultMode, "defaultMode", AutoMode.Maintain);
            Scribe_Values.Look(ref defaultRepeatMode, "defaultRepeatMode");
            Scribe_Values.Look(ref targetCount, "targetCount", DefaultTargetCount);
            Scribe_Values.Look(ref floorCount, "floorCount", DefaultFloorCount);
            Scribe_Values.Look(ref uncountableMode, "uncountableMode", AutoMode.Excluded);
            Scribe_Collections.Look(ref rules, "rules", LookMode.Value, LookMode.Deep);
            if (rules == null) rules = new Dictionary<string, RecipeRule>();
        }
    }

    public class BillAutopilotSettings : ModSettings
    {
        /// <summary>Une notification de nouvelle recette par etabli, ou une seule pour tous.</summary>
        public bool notifyNewRecipes = true;

        /// <summary>Nombre de ticks entre deux passages de synchronisation d'un meme etabli.</summary>
        public int syncIntervalTicks = 600;

        /// <summary>
        /// Plafond de bills automatiques simultanees sur un meme etabli. Le jeu n'accepte que 15 bills
        /// par etabli et masque le bouton "Ajouter" au-dela : on garde de la place pour celles posees a
        /// la main. Ne s'applique qu'a la creation - une bill deja en place n'est retiree qu'une fois
        /// son travail fait.
        /// </summary>
        public int maxAutoBillsPerTable = 8;

        private Dictionary<string, BenchProfile> profiles = new Dictionary<string, BenchProfile>();

        public IEnumerable<KeyValuePair<string, BenchProfile>> AllProfiles => profiles;

        /// <summary>Profil existant, ou null. Ne cree rien : appele a chaque tick de synchro.</summary>
        public BenchProfile ProfileFor(ThingDef benchDef)
        {
            if (benchDef == null) return null;
            return profiles.TryGetValue(benchDef.defName, out var profile) ? profile : null;
        }

        public BenchProfile ProfileForWriting(ThingDef benchDef)
        {
            if (!profiles.TryGetValue(benchDef.defName, out var profile))
            {
                profile = new BenchProfile();
                profiles[benchDef.defName] = profile;
            }
            return profile;
        }

        public bool IsEnabled(ThingDef benchDef)
        {
            var profile = ProfileFor(benchDef);
            return profile != null && profile.enabled;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref notifyNewRecipes, "notifyNewRecipes", defaultValue: true);
            Scribe_Values.Look(ref syncIntervalTicks, "syncIntervalTicks", 600);
            Scribe_Values.Look(ref maxAutoBillsPerTable, "maxAutoBillsPerTable", 8);
            Scribe_Collections.Look(ref profiles, "profiles", LookMode.Value, LookMode.Deep);
            if (profiles == null) profiles = new Dictionary<string, BenchProfile>();
        }
    }
}
