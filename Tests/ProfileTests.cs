using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BillAutopilot;
using RimWorld;
using Verse;

// Exercises the shipped BillAutopilot.dll's decision layer against the real RimWorld assemblies,
// with no game running.
//
// BenchProfile answers three questions for every recipe on every sync pass: which mode applies,
// how many to keep, and when to start again. Nothing on that path touches a workbench, a bill
// stack or a map, so it can be answered here - and a wrong answer there is the kind that never
// announces itself: the mod goes on working, and quietly makes the wrong amount of the wrong
// thing.
//
// Everything else is in TESTING.md, and needs the game.
internal static class Program
{
    private static int failures;
    private static int checks;

    private static void Main(string[] args)
    {
        string managed = args.Length > 0 ? args[0] : Metadata("RimWorldManaged");

        // Assembly-CSharp pulls in Unity assemblies that are not beside this executable.
        AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
        {
            string path = Path.Combine(managed, new AssemblyName(e.Name).Name + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };

        Console.WriteLine("RimWorld:      " + managed);

        // Main must not name a type from those assemblies: the JIT resolves them as it compiles the
        // method, which happens before the handler above is ever installed. Hence the separate,
        // never-inlined entry point below, and the "under test" line inside it rather than here.
        Run();

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? checks + " CHECKS PASSED"
            : failures + " OF " + checks + " CHECKS FAILED");
        Environment.Exit(failures == 0 ? 0 : 1);
    }

    private static string Metadata(string key)
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .First(a => a.Key == key)
            .Value;
    }

    // A repeat mode belonging to another mod, as Everybody Gets One supplies three of. Registering
    // it is what separates "the mod is installed" from "the mod has gone", which the profile has to
    // tell apart.
    private static BillRepeatModeDef Register(string defName)
    {
        var mode = new BillRepeatModeDef { defName = defName, label = defName };
        DefDatabase<BillRepeatModeDef>.Add(mode);
        return mode;
    }

    private static RecipeDef Recipe(string defName) => new RecipeDef { defName = defName };

    /// <summary>
    /// A ThingDef cannot simply be constructed here. BuildableDef's constructor reaches
    /// Verse.BaseContent, whose static initialiser loads shaders through UnityEngine.Resources,
    /// which throws outside a running player loop: "ECall methods must be packaged into a system
    /// module". RecipeDef is a plain Def and has no such ancestry, which is why those are built
    /// normally above.
    ///
    /// Allocating without running a constructor is enough, because the only thing read from a
    /// bench on this path is its defName. If a test ever needs more of a ThingDef than that, it
    /// has left the layer this harness can reach and belongs in TESTING.md.
    /// </summary>
    private static ThingDef Bench(string defName)
    {
        var bench = (ThingDef)FormatterServices.GetUninitializedObject(typeof(ThingDef));
        bench.defName = defName;
        return bench;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Run()
    {
        Console.WriteLine("under test:    " + typeof(BenchProfile).Assembly.Location);

        var foreign = Register("TD_PersonCount");
        var steel = Recipe("Make_ComponentIndustrial");
        var meat = Recipe("ButcherCorpseFlesh");

        Defaults();
        Countability(steel, meat);
        Overrides(steel);
        ForeignModes(steel, meat, foreign);
        Counts(steel);
        Bookkeeping(steel);
        Settings();
    }

    // --- The shipped defaults -------------------------------------------------------------

    private static void Defaults()
    {
        Section("defaults");
        var profile = new BenchProfile();

        Check("off until switched on", !profile.enabled);
        Check("default mode is keep a stock", profile.defaultMode == AutoMode.Maintain);
        Check("target is 50", profile.targetCount == 50);
        Check("floor is 25", profile.floorCount == 25);
        Check("uncountable recipes are left alone", profile.uncountableMode == AutoMode.Excluded);
        Check("no overrides", profile.OverrideCount == 0);
    }

    // --- Countability, and the fallback that only applies to one mode ------------------------

    private static void Countability(RecipeDef countable, RecipeDef uncountable)
    {
        Section("countability");
        var profile = new BenchProfile();

        Check("a countable recipe keeps a stock",
            profile.ModeFor(countable, countable: true) == AutoMode.Maintain);

        // This is why a butcher table on autopilot produces nothing until it is configured: "keep 50
        // in stock" cannot be expressed for a recipe the game will not count.
        Check("an uncountable recipe falls back, and the default fallback is never",
            profile.ModeFor(uncountable, countable: false) == AutoMode.Excluded);

        profile.uncountableMode = AutoMode.Always;
        Check("the fallback is the one configured",
            profile.ModeFor(uncountable, countable: false) == AutoMode.Always);

        // The fallback exists because "keep a stock" needs to count. Nothing else does, so nothing
        // else may be diverted by it.
        profile.defaultMode = AutoMode.Always;
        profile.uncountableMode = AutoMode.Excluded;
        Check("always is not diverted by countability",
            profile.ModeFor(uncountable, countable: false) == AutoMode.Always);
    }

    // --- A recipe's own rule against the bench default ----------------------------------------

    private static void Overrides(RecipeDef recipe)
    {
        Section("overrides");
        var profile = new BenchProfile();

        profile.RuleForWriting(recipe).mode = AutoMode.Always;
        Check("a rule beats the default", profile.ModeFor(recipe, countable: true) == AutoMode.Always);

        profile.RuleForWriting(recipe).mode = AutoMode.Excluded;
        Check("a rule can refuse a recipe outright",
            profile.ModeFor(recipe, countable: true) == AutoMode.Excluded);

        profile.RuleForWriting(recipe).mode = AutoMode.Inherit;
        profile.defaultMode = AutoMode.Always;
        Check("inherit really inherits, it is not a mode of its own",
            profile.ModeFor(recipe, countable: true) == AutoMode.Always);
    }

    // --- Repeat modes belonging to other mods ------------------------------------------------

    private static void ForeignModes(RecipeDef countable, RecipeDef uncountable, BillRepeatModeDef mode)
    {
        Section("modes from other mods");

        var profile = new BenchProfile { defaultMode = AutoMode.Custom, defaultRepeatMode = mode.defName };
        Check("a known foreign mode is kept",
            profile.ModeFor(countable, countable: true) == AutoMode.Custom);
        Check("and it is resolved to the def", profile.RepeatModeFor(countable) == mode);

        // A foreign mode decides for itself whether there is work: counting is its owner's business,
        // so countability must not divert it either.
        Check("a foreign mode is not diverted by countability",
            profile.ModeFor(uncountable, countable: false) == AutoMode.Custom);

        // The mod that supplied the mode has been disabled. A bill must not go up with no mode at
        // all, so the profile falls back to one of ours.
        profile.defaultRepeatMode = "TD_ModeFromAModThatIsGone";
        Check("a mode whose mod has gone falls back to keep a stock",
            profile.ModeFor(countable, countable: true) == AutoMode.Maintain);
        Check("and resolves to nothing", profile.RepeatModeFor(countable) == null);

        // Both fallbacks at once, and their order matters: Custom becomes Maintain, and Maintain is
        // then diverted by countability. Applied the other way round, an uncountable recipe would
        // end up keeping a stock it cannot count.
        Check("the two fallbacks chain, in that order",
            profile.ModeFor(uncountable, countable: false) == AutoMode.Excluded);

        // A rule naming its own foreign mode, over a bench default that names none.
        var byRule = new BenchProfile();
        var rule = byRule.RuleForWriting(countable);
        rule.mode = AutoMode.Custom;
        rule.repeatMode = mode.defName;
        Check("a rule can carry a foreign mode of its own",
            byRule.ModeFor(countable, countable: true) == AutoMode.Custom);
        Check("read from the rule, not from the bench", byRule.RepeatModeFor(countable) == mode);
    }

    // --- The two numbers ----------------------------------------------------------------------

    private static void Counts(RecipeDef recipe)
    {
        Section("target and floor");
        var profile = new BenchProfile();

        Check("target comes from the bench", profile.TargetFor(recipe) == 50);
        Check("floor comes from the bench", profile.FloorFor(recipe) == 25);

        var rule = profile.RuleForWriting(recipe);
        rule.targetCount = 200;
        Check("a rule overrides the target", profile.TargetFor(recipe) == 200);

        // Zero is a real target, meaning "make none of this". Only -1 means "inherit", so a test
        // written as "greater than zero" would silently turn it back into 50.
        rule.targetCount = 0;
        Check("zero is a target, not an absence", profile.TargetFor(recipe) == 0);

        rule.targetCount = -1;
        Check("minus one inherits again", profile.TargetFor(recipe) == 50);

        // A floor above the target would mean a bill that fires the moment it stops, for ever.
        rule.floorCount = 90;
        Check("a floor above the target is clamped to it", profile.FloorFor(recipe) == 50);

        rule.floorCount = -1;
        profile.targetCount = 10;
        Check("the bench's own floor is clamped too", profile.FloorFor(recipe) == 10);
    }

    // --- Keeping the rule table honest ---------------------------------------------------------

    private static void Bookkeeping(RecipeDef recipe)
    {
        Section("rule bookkeeping");
        var profile = new BenchProfile();

        // The profile window asks for a writable rule just to draw a row. An empty one must not
        // count, or the settings screen would report overrides nobody made.
        var rule = profile.RuleForWriting(recipe);
        Check("a freshly created rule is default", rule.IsDefault);
        Check("and is not counted as an override", profile.OverrideCount == 0);

        rule.mode = AutoMode.Excluded;
        Check("a rule that says something is counted", profile.OverrideCount == 1);
        Check("and is not default any more", !rule.IsDefault);

        Check("the same rule comes back", profile.RuleFor(recipe) == rule);

        profile.ClearRule(recipe);
        Check("clearing removes it", profile.RuleFor(recipe) == null);
        Check("and the count follows", profile.OverrideCount == 0);
        Check("the recipe returns to the default",
            profile.ModeFor(recipe, countable: true) == AutoMode.Maintain);
    }

    // --- Settings -------------------------------------------------------------------------------

    private static void Settings()
    {
        Section("settings");
        var settings = new BillAutopilotSettings();
        var bench = Bench("TableMachining");

        // ProfileFor is called for every workbench on every pass. If it created as it read, an
        // untouched game would end up with a profile for every bench type in the mod list.
        Check("an unknown bench has no profile", settings.ProfileFor(bench) == null);
        Check("and reading created nothing", !settings.AllProfiles.Any());
        Check("an unknown bench is not enabled", !settings.IsEnabled(bench));

        var profile = settings.ProfileForWriting(bench);
        Check("writing creates one", profile != null);
        Check("and it is the one read back", settings.ProfileFor(bench) == profile);
        Check("still not enabled until it is", !settings.IsEnabled(bench));

        profile.enabled = true;
        Check("enabled once the profile says so", settings.IsEnabled(bench));

        Check("defaults carried over from the mod settings",
            settings.maxAutoBillsPerTable == 8 && settings.syncIntervalTicks == 600
            && settings.markAutomaticBills && settings.notifyNewRecipes);
    }

    // --- Plumbing ---------------------------------------------------------------------------------

    private static void Section(string name)
    {
        Console.WriteLine();
        Console.WriteLine("-- " + name);
    }

    private static void Check(string what, bool ok)
    {
        checks++;
        if (!ok) failures++;
        Console.WriteLine((ok ? "   ok   " : "  FAIL  ") + what);
    }
}
