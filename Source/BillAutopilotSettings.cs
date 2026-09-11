using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// What a recipe should do under the autopilot.
    /// Deliberately independent of <see cref="RimWorld.BillRepeatModeDef"/>: the mod settings are read
    /// in the <see cref="Mod"/> constructor, therefore BEFORE defs are loaded. A Scribe_Defs there
    /// would resolve against an empty DefDatabase.
    /// </summary>
    public enum AutoMode : byte
    {
        /// <summary>Follows the bench default. Only valid on a recipe rule.</summary>
        Inherit = 0,

        /// <summary>Keep a stock: the bill appears on falling to the threshold, and goes once the target is reached.</summary>
        Maintain = 1,

        /// <summary>Always: the bill stays up and runs without end.</summary>
        Always = 2,

        /// <summary>Never: the autopilot ignores this recipe.</summary>
        Excluded = 3,

        /// <summary>
        /// A repeat mode set by another mod, named alongside by its defName. Everybody Gets One adds
        /// three; other mods may add more. The autopilot never tries to work out what they mean: it
        /// sets them, and asks their owner whether there is work to do.
        /// </summary>
        Custom = 4,
    }

    /// <summary>An override set on one recipe, for one workbench type.</summary>
    public class RecipeRule : IExposable
    {
        public AutoMode mode = AutoMode.Inherit;

        /// <summary>defName of the mode when <see cref="mode"/> is Custom. A string, not a Def: the
        /// settings are read before defs are loaded.</summary>
        public string repeatMode;

        /// <summary>-1: inherits from the bench.</summary>
        public int targetCount = -1;

        /// <summary>-1: inherits from the bench.</summary>
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

    /// <summary>The profile of one workbench type. Global: every bench of that ThingDef shares it.</summary>
    public class BenchProfile : IExposable
    {
        public const int DefaultTargetCount = 50;
        public const int DefaultFloorCount = 25;

        public bool enabled;

        /// <summary>Mode applied to recipes with no override. Maintain, Always or Custom.</summary>
        public AutoMode defaultMode = AutoMode.Maintain;

        /// <summary>defName of the mode when <see cref="defaultMode"/> is Custom.</summary>
        public string defaultRepeatMode;

        public int targetCount = DefaultTargetCount;
        public int floorCount = DefaultFloorCount;

        /// <summary>
        /// Mode for recipes the game cannot count (butchering, smelting, cremation, surgery and the like):
        /// RecipeWorkerCounter.CanCountProducts returns false for them, so "keep X" is impossible.
        /// Only Always and Excluded are valid here.
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

        /// <summary>A recipe's effective mode, override and uncountable fallback included.</summary>
        public AutoMode ModeFor(RecipeDef recipe, bool countable)
        {
            var rule = RuleFor(recipe);
            var mode = rule != null && rule.mode != AutoMode.Inherit ? rule.mode : defaultMode;

            // A mode from another mod that has since gone: fall back to one of ours rather than put up a
            // bill with no mode at all.
            if (mode == AutoMode.Custom && RepeatModeFor(recipe) == null) mode = AutoMode.Maintain;

            // The uncountable fallback only applies to "keep a stock": it is the only one of our modes that
            // needs to count. A foreign mode decides for itself.
            if (mode == AutoMode.Maintain && !countable) mode = uncountableMode;
            return mode;
        }

        /// <summary>The repeat mode to set when the effective mode is Custom, or null.</summary>
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
        /// <summary>Whether newly unlocked recipes are announced by letter.</summary>
        public bool notifyNewRecipes = true;

        /// <summary>Ticks between two sync passes over the same workbench.</summary>
        public int syncIntervalTicks = 600;

        /// <summary>
        /// Cap on how many automatic bills may stand on one workbench at a time. The game accepts only 15
        /// bills per bench and hides the "Add" button beyond that, so room is left for hand-placed ones.
        /// Applies to creation only: a bill already up is taken down only once its work is done.
        /// </summary>
        public int maxAutoBillsPerTable = 8;

        private Dictionary<string, BenchProfile> profiles = new Dictionary<string, BenchProfile>();

        public IEnumerable<KeyValuePair<string, BenchProfile>> AllProfiles => profiles;

        /// <summary>An existing profile, or null. Creates nothing: called on every sync tick.</summary>
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
