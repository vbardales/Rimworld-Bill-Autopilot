using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// Shared lookups for every step class here.
    ///
    /// Two rules run through all of it. First, nothing is cached: a scenario tagged
    /// <c>@same-world</c> and a save reload both replace every object in the game, and a helper
    /// holding yesterday's workbench would assert against an object nothing draws from any more.
    /// Second, every miss names itself. A report keeps no stack trace, so an unguarded hop through
    /// reflection comes back as "Object reference not set to an instance of an object" and the run
    /// that produced it is already over; each lookup below says which one it was instead.
    /// </summary>
    public static class Driver
    {
        internal const BindingFlags StaticAny = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static BillAutopilotMod Mod(PickleContext ctx)
        {
            var mod = LoadedModManager.GetMod<BillAutopilotMod>();
            ctx.Require(mod != null,
                "LoadedModManager.GetMod<BillAutopilotMod>() returned nothing: BillAutopilot.dll is not "
                + "loaded in this session, so no step here can reach its settings or its state");
            return mod;
        }

        /// <summary>
        /// Read through the mod's own static property rather than kept: the settings object survives
        /// a save reload, but <see cref="ResetToDefaults"/> writes over its fields and a stale copy
        /// would be a different object from the one the sync pass reads.
        /// </summary>
        public static BillAutopilotSettings Settings(PickleContext ctx)
        {
            Mod(ctx);
            ctx.Require(BillAutopilotMod.Settings != null,
                "BillAutopilotMod.Settings is null: the mod constructor did not run, which means the "
                + "assembly loaded but its Mod class did not");
            return BillAutopilotMod.Settings;
        }

        /// <summary>
        /// The per-game state. It follows the Game object, so it is null at the main menu and a
        /// different instance after a reload - never hold on to one.
        /// </summary>
        public static BillAutopilotState State(PickleContext ctx)
        {
            var state = BillAutopilotState.Current;
            ctx.Require(state != null,
                "there is no Bill Autopilot state: no game is loaded, so this step needs a "
                + "'the save \"...\" is loaded' step before it");
            return state;
        }

        public static Map Map(PickleContext ctx)
        {
            var map = Find.CurrentMap;
            ctx.Require(map != null, "no current map: load a fixture before this step");
            return map;
        }

        public static ThingDef BenchDef(PickleContext ctx, string defName)
        {
            var def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            ctx.Require(def != null, $"no ThingDef named '{defName}'{Near(defName)}");
            ctx.Require(def.IsWorkTable, $"'{defName}' is a ThingDef but not a workbench, so it has no bill stack");
            return def;
        }

        public static RecipeDef Recipe(PickleContext ctx, string defName)
        {
            var def = DefDatabase<RecipeDef>.GetNamedSilentFail(defName);
            ctx.Require(def != null, $"no RecipeDef named '{defName}'{NearRecipe(defName)}");
            return def;
        }

        /// <summary>
        /// The workbench standing at a cell. Named by cell rather than by def alone because the
        /// scenarios that matter most here - the per-bench memory, the two benches of a kind set
        /// differently - are exactly the ones a def-only lookup cannot tell apart.
        /// </summary>
        public static Building_WorkTable Bench(PickleContext ctx, string defName, int x, int z)
        {
            var map = Map(ctx);
            var cell = new IntVec3(x, 0, z);
            ctx.Require(cell.InBounds(map),
                $"({x}, {z}) is outside this {map.Size.x} x {map.Size.z} map");

            var things = cell.GetThingList(map);
            for (int i = 0; i < things.Count; i++)
            {
                if (things[i] is Building_WorkTable table && table.def.defName == defName) return table;
            }

            var present = things.Count == 0
                ? "the cell is empty"
                : "it holds " + string.Join(", ", things.Select(t => t.def.defName).ToArray());
            ctx.Require(false, $"no '{defName}' at ({x}, {z}): {present}");
            return null;
        }

        /// <summary>The bench's automatic bill for a recipe, or null when the autopilot has none up.</summary>
        public static Bill_Production AutoBill(PickleContext ctx, Building_WorkTable table, RecipeDef recipe)
        {
            var state = State(ctx);
            var bills = table.billStack.Bills;
            for (int i = 0; i < bills.Count; i++)
            {
                if (bills[i]?.recipe == recipe && state.IsAuto(bills[i])) return bills[i] as Bill_Production;
            }
            return null;
        }

        /// <summary>The bench's hand-placed bill for a recipe, or null.</summary>
        public static Bill_Production ManualBill(PickleContext ctx, Building_WorkTable table, RecipeDef recipe)
        {
            var state = State(ctx);
            var bills = table.billStack.Bills;
            for (int i = 0; i < bills.Count; i++)
            {
                if (bills[i]?.recipe == recipe && !state.IsAuto(bills[i])) return bills[i] as Bill_Production;
            }
            return null;
        }

        /// <summary>Every bill on the bench, whoever put it there, for a failure message worth reading.</summary>
        public static string Describe(PickleContext ctx, Building_WorkTable table)
        {
            var state = BillAutopilotState.Current;
            var bills = table.billStack.Bills;
            if (bills.Count == 0) return "the bench carries no bills at all";

            var lines = new List<string>();
            for (int i = 0; i < bills.Count; i++)
            {
                var bill = bills[i];
                string owner = state != null && state.IsAuto(bill) ? "automatic" : "hand-placed";
                string suspended = bill.suspended ? ", suspended" : "";
                lines.Add($"{bill.recipe?.defName ?? "?"} ({owner}{suspended})");
            }
            return "the bench carries " + string.Join(", ", lines.ToArray());
        }

        public static AutoMode Mode(PickleContext ctx, string word)
        {
            switch ((word ?? "").Trim().ToLowerInvariant())
            {
                case "keep in stock":
                case "maintain": return AutoMode.Maintain;
                case "always": return AutoMode.Always;
                case "never":
                case "excluded": return AutoMode.Excluded;
                case "default":
                case "inherit": return AutoMode.Inherit;
                case "another mod":
                case "custom": return AutoMode.Custom;
            }

            ctx.Require(false,
                $"'{word}' is not a Bill Autopilot mode: write 'keep in stock', 'always', 'never', "
                + "'default' or 'another mod'");
            return AutoMode.Inherit;
        }

        public static string ModeName(AutoMode mode)
        {
            switch (mode)
            {
                case AutoMode.Maintain: return "keep in stock";
                case AutoMode.Always: return "always";
                case AutoMode.Excluded: return "never";
                case AutoMode.Custom: return "another mod";
                default: return "default";
            }
        }

        // --- Into the mod's internals ------------------------------------------------------------

        /// <summary>
        /// <c>BenchActivation.Toggle</c> is internal, and it is the ONLY path by which a profile is
        /// switched on: the gizmo, the settings page and the profile window all end up here. A step
        /// that set <c>profile.enabled</c> directly would skip the confirmation this suite exists to
        /// watch, so the internal method is reached by reflection rather than worked around.
        /// </summary>
        public static void Toggle(PickleContext ctx, ThingDef bench, bool turnOn)
        {
            var type = typeof(BillAutopilotMod).Assembly.GetType("BillAutopilot.BenchActivation");
            ctx.Require(type != null,
                "BillAutopilot.BenchActivation no longer exists: the mod renamed the activation path, "
                + "update these steps");

            var method = type.GetMethod("Toggle", StaticAny, null, new[] { typeof(ThingDef), typeof(bool) }, null);
            ctx.Require(method != null,
                "BenchActivation.Toggle(ThingDef, bool) no longer exists: update these steps");

            method.Invoke(null, new object[] { bench, turnOn });
        }

        /// <summary>
        /// How many recipes switching this bench type on would take at once - the number the
        /// confirmation announces. Read out of the mod rather than recomputed: a second
        /// implementation living in the test could only ever agree with itself, and the claim being
        /// checked is precisely that the dialog's number is the real intake.
        /// </summary>
        public static int RecipesTaken(PickleContext ctx, ThingDef bench, BenchProfile profile)
        {
            var type = typeof(BillAutopilotMod).Assembly.GetType("BillAutopilot.BenchActivation");
            ctx.Require(type != null, "BillAutopilot.BenchActivation no longer exists: update these steps");

            var method = type.GetMethod("RecipesTaken", StaticAny, null,
                new[] { typeof(ThingDef), typeof(BenchProfile) }, null);
            ctx.Require(method != null,
                "BenchActivation.RecipesTaken(ThingDef, BenchProfile) no longer exists: update these steps");

            return (int)method.Invoke(null, new object[] { bench, profile });
        }

        /// <summary>
        /// The count the autopilot itself works from, read through its own probe rather than
        /// recomputed here. That is the whole point of the step it serves: TESTING.md scenario 13
        /// asks whether the threshold that fires and the number the bill displays are talking about
        /// the same figure, and a second implementation in the test could only ever agree with
        /// itself.
        /// </summary>
        public static bool TryCount(PickleContext ctx, Building_WorkTable table, RecipeDef recipe, out int count)
        {
            count = 0;
            var type = typeof(BillAutopilotMod).Assembly.GetType("BillAutopilot.RecipeProbe");
            ctx.Require(type != null,
                "BillAutopilot.RecipeProbe no longer exists: the mod renamed its counting path, update these steps");

            var method = type.GetMethod("TryCount", StaticAny);
            ctx.Require(method != null, "RecipeProbe.TryCount no longer exists: update these steps");

            var memory = BillAutopilotState.Current?.MemoryFor(table, recipe);
            var args = new object[] { table, recipe, memory, 0 };
            bool ok = (bool)method.Invoke(null, args);
            count = (int)args[3];
            return ok;
        }

        public static bool CanCount(PickleContext ctx, RecipeDef recipe)
        {
            var type = typeof(BillAutopilotMod).Assembly.GetType("BillAutopilot.RecipeProbe");
            ctx.Require(type != null, "BillAutopilot.RecipeProbe no longer exists: update these steps");

            var method = type.GetMethod("CanCount", StaticAny);
            ctx.Require(method != null, "RecipeProbe.CanCount no longer exists: update these steps");
            return (bool)method.Invoke(null, new object[] { recipe });
        }

        /// <summary>
        /// Is a compatibility bridge live? Read from the mod's own probe, not from the mod list: the
        /// question a scenario asks is whether BILL AUTOPILOT found the neighbour, which is a
        /// different thing from whether the neighbour is loaded. TESTING.md scenario 1 names that
        /// difference as the fastest way to tell a dead integration from a missing mod.
        /// </summary>
        public static bool BridgeActive(PickleContext ctx, string typeName, string memberName)
        {
            var type = typeof(BillAutopilotMod).Assembly.GetType("BillAutopilot." + typeName);
            ctx.Require(type != null, $"BillAutopilot.{typeName} no longer exists: update these steps");

            var property = type.GetProperty(memberName, StaticAny);
            ctx.Require(property != null, $"{typeName}.{memberName} no longer exists: update these steps");
            return (bool)property.GetValue(null);
        }

        // --- Naming things the player would recognise ---------------------------------------------

        /// <summary>
        /// The marker the mod appends to an automatic bill's label, resolved through the same key the
        /// mod resolves. Never spelled out in a scenario: a step that wrote "(auto)" would pass only
        /// on an English game, and every pass of the French half of this suite would fail on a
        /// correctly translated marker.
        /// </summary>
        public static string Marker() => "BillAutopilot.AutoMarker".Translate().Resolve();

        private static string Near(string defName)
        {
            var close = DefDatabase<ThingDef>.AllDefsListForReading
                .Where(d => d.IsWorkTable && d.defName.IndexOf(defName, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(d => d.defName).Take(5).ToArray();
            return close.Length == 0 ? "" : "; workbenches with a similar name: " + string.Join(", ", close);
        }

        private static string NearRecipe(string defName)
        {
            var close = DefDatabase<RecipeDef>.AllDefsListForReading
                .Where(d => d.defName.IndexOf(defName, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(d => d.defName).Take(5).ToArray();
            return close.Length == 0 ? "" : "; recipes with a similar name: " + string.Join(", ", close);
        }
    }
}
