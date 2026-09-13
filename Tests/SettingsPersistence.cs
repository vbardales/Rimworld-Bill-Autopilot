using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using BillAutopilot;
using RimWorld;
using Verse;

internal static class SettingsPersistence
{
    // Use the shipped ExposeData and the game's Scribe implementation, not a substitute serializer.
    public static void Run(Action<string, bool> check)
    {
        Console.WriteLine("-- native settings persistence");
        UnityEngine.Debug.unityLogger.logHandler = new TestLogHandler();
        DeepProfiler.enabled = false; // No game preferences/profiler in this console process.
        // The desktop CLR cannot enumerate a few game types with default interface methods.
        // Seed the native discovery cache with real loadable types; serialization stays native.
        var types = new System.Collections.Generic.List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try { types.AddRange(assembly.GetTypes()); }
            catch (System.Reflection.ReflectionTypeLoadException error)
            {
                types.AddRange(error.Types.Where(type => type != null));
                Console.WriteLine("Type-discovery fixture: omitted unloadable types from " + assembly.GetName().Name);
            }
        }
        typeof(GenTypes).GetField("allTypesCached", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            .SetValue(null, types.Where(type => { try { return type.Namespace != null; } catch (TypeLoadException) { return false; } }).ToList());
        // No mod loader runs in this process: register the actual serialized model types.
        var cache = (System.Collections.IDictionary)typeof(GenTypes).GetField("typeCache", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
        var keyType = typeof(GenTypes).GetNestedType("TypeCacheKey", System.Reflection.BindingFlags.NonPublic);
        foreach (var type in new[] { typeof(BillAutopilotSettings), typeof(BenchProfile), typeof(RecipeRule) })
            cache[Activator.CreateInstance(keyType, new object[] { type.FullName, null })] = type;
        string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "persistence-results");
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "settings.xml");
        var bench = (ThingDef)FormatterServices.GetUninitializedObject(typeof(ThingDef));
        bench.defName = "TableMachining";
        var recipe = new RecipeDef { defName = "Make_ComponentIndustrial" };
        var settings = new BillAutopilotSettings
        {
            notifyNewRecipes = false, markAutomaticBills = false,
            maxAutoBillsPerTable = 12, syncIntervalTicks = 900
        };
        var profile = settings.ProfileForWriting(bench);
        profile.enabled = true;
        profile.defaultMode = AutoMode.Custom;
        profile.defaultRepeatMode = "TD_PersonCount";
        profile.targetCount = 120;
        profile.floorCount = 30;
        profile.uncountableMode = AutoMode.Always;
        var rule = profile.RuleForWriting(recipe);
        rule.mode = AutoMode.Custom;
        rule.repeatMode = "TD_PersonCount";
        rule.targetCount = 7;
        rule.floorCount = 3;
        Save(path, settings);
        var loaded = Load(path);
        check("global toggles survive native round-trip", !loaded.notifyNewRecipes && !loaded.markAutomaticBills);
        check("cap and interval survive native round-trip", loaded.maxAutoBillsPerTable == 12 && loaded.syncIntervalTicks == 900);
        var restored = loaded.ProfileFor(bench);
        check("profile restored independently", restored != null && restored != profile && restored.enabled);
        check("profile mode identity survives", restored.defaultMode == AutoMode.Custom && restored.defaultRepeatMode == "TD_PersonCount");
        check("profile counts and uncountable mode survive", restored.targetCount == 120 && restored.floorCount == 30 && restored.uncountableMode == AutoMode.Always);
        var restoredRule = restored.RuleFor(recipe);
        check("recipe mode and counts survive", restoredRule.mode == AutoMode.Custom && restoredRule.repeatMode == "TD_PersonCount" && restoredRule.targetCount == 7 && restoredRule.floorCount == 3);
        check("loaded mode actually resolves", restored.ModeFor(recipe, true) == AutoMode.Custom && restored.RepeatModeFor(recipe).defName == "TD_PersonCount");
        restored.ClearRule(recipe);
        loaded.notifyNewRecipes = true;
        Save(path, loaded);
        loaded = Load(path);
        check("cleared override stays cleared after saving again", loaded.ProfileFor(bench).RuleFor(recipe) == null);
        check("toggle restored to default persists", loaded.notifyNewRecipes);

        string emptyPath = Path.Combine(directory, "empty.xml");
        File.WriteAllText(emptyPath, "<settings />");
        loaded = Load(emptyPath);
        check("missing global fields use documented defaults", loaded.notifyNewRecipes && loaded.markAutomaticBills && loaded.maxAutoBillsPerTable == 8 && loaded.syncIntervalTicks == 600);
        check("missing profiles are safe and empty", !loaded.AllProfiles.Any() && loaded.ProfileFor(bench) == null);

        // Derive a legacy-shaped fixture by removing fields added to the current schema.
        Save(path, settings);
        var xml = new XmlDocument();
        xml.Load(path);
        foreach (XmlNode node in xml.SelectNodes("//markAutomaticBills | //defaultRepeatMode | //repeatMode | //uncountableMode | //floorCount"))
            node.ParentNode.RemoveChild(node);
        string legacyPath = Path.Combine(directory, "legacy-missing-fields.xml");
        xml.Save(legacyPath);
        loaded = Load(legacyPath);
        restored = loaded.ProfileFor(bench);
        check("legacy fields retain stored values", !loaded.notifyNewRecipes && loaded.maxAutoBillsPerTable == 12 && restored.targetCount == 120);
        check("legacy missing marker and uncountable fields default", loaded.markAutomaticBills && restored.uncountableMode == AutoMode.Excluded);
        check("legacy missing floor values default", restored.floorCount == 25 && restored.RuleFor(recipe).floorCount == -1);
        check("legacy custom mode without identity falls back safely", restored.ModeFor(recipe, true) == AutoMode.Maintain);

        string nullPath = Path.Combine(directory, "null-collections.xml");
        File.WriteAllText(nullPath, "<settings><profiles IsNull=\"True\" /></settings>");
        loaded = Load(nullPath);
        check("null profile dictionary becomes writable", loaded.ProfileForWriting(bench) != null);
        loaded.ProfileFor(bench).Rules.Clear();
        Save(path, loaded);
        xml.Load(path);
        var rulesNode = xml.SelectSingleNode("//rules");
        rulesNode.RemoveAll();
        var nullAttribute = xml.CreateAttribute("IsNull");
        nullAttribute.Value = "True";
        rulesNode.Attributes.Append(nullAttribute);
        xml.Save(nullPath);
        loaded = Load(nullPath);
        check("null recipe dictionary becomes writable", loaded.ProfileFor(bench).RuleForWriting(recipe) != null);
        profile = loaded.ProfileFor(bench);
        profile.defaultMode = AutoMode.Custom;
        profile.defaultRepeatMode = "TD_PersonCount";
        profile.ClearRule(recipe);
        profile.AdjustTarget(recipe, 10);
        Save(path, loaded);
        restored = Load(path).ProfileFor(bench);
        check("edited quantity still inherits after native reload", restored.RuleFor(recipe).mode == AutoMode.Inherit && restored.TargetFor(recipe) == 60 && restored.ModeFor(recipe, true) == AutoMode.Custom);
    }

    private static void Save(string path, BillAutopilotSettings settings)
    {
        try
        {
            Scribe.saver.InitSaving(path, "settings");
            settings.ExposeData();
            Scribe.saver.FinalizeSaving();
        }
        finally { Scribe.ForceStop(); }
    }

    private sealed class TestLogHandler : UnityEngine.ILogHandler
    {
        public void LogFormat(UnityEngine.LogType type, UnityEngine.Object context, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Console.WriteLine(type + ": " + message);
            if (!message.StartsWith("An error occurred while logging an error:") && (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception))
                throw new InvalidOperationException(message);
        }
        public void LogException(Exception exception, UnityEngine.Object context) { throw exception; }
    }

    private static BillAutopilotSettings Load(string path)
    {
        try
        {
            Scribe.loader.InitLoading(path);
            var settings = new BillAutopilotSettings();
            settings.ExposeData();
            Scribe.loader.FinalizeLoading();
            return settings;
        }
        finally { Scribe.ForceStop(); }
    }
}
