using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Our state is grafted into the save's &lt;game&gt; node rather than into a GameComponent: a
    /// component is written with a Class attribute, and removing the mod would then make every load
    /// fail on "Can't load abstract class Verse.GameComponent". Named nodes, on the other hand, are
    /// read by nobody once the mod is gone, and the game ignores them silently.
    ///
    /// ExposeSmallComponents is the only point common to both paths: Game.ExposeData calls it on save,
    /// Game.LoadGame on load (ExposeData refuses LoadingVars).
    /// </summary>
    [HarmonyPatch(typeof(Game), "ExposeSmallComponents")]
    internal static class Patch_Game_ExposeSmallComponents
    {
        private static void Postfix()
        {
            BillAutopilotState.Current?.ExposeData();
        }
    }

    /// <summary>The autopilot's heartbeat, which used to live in GameComponentTick.</summary>
    [HarmonyPatch(typeof(TickManager), nameof(TickManager.DoSingleTick))]
    internal static class Patch_TickManager_DoSingleTick
    {
        private static void Postfix()
        {
            if (Current.ProgramState != ProgramState.Playing) return;
            BillAutopilotState.Current?.Tick();
        }
    }

    /// <summary>A finished research project may unlock recipes, so every workbench is revisited.</summary>
    [HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.FinishProject))]
    internal static class Patch_ResearchManager_FinishProject
    {
        private static void Postfix()
        {
            BillAutopilotState.Current?.MarkDirty();
        }
    }

    /// <summary>
    /// Deleting an automatic bill in the tab means refusing the recipe: without this the autopilot
    /// would put it back on the next pass, and the gesture would have no effect at all.
    /// </summary>
    [HarmonyPatch(typeof(Building_WorkTable), nameof(Building_WorkTable.Notify_BillDeleted))]
    internal static class Patch_BuildingWorkTable_NotifyBillDeleted
    {
        private static void Postfix(Building_WorkTable __instance, Bill bill)
        {
            if (AutoBillSync.SuppressDeleteCapture) return;

            var state = BillAutopilotState.Current;
            if (state == null || bill?.recipe == null || !state.IsAuto(bill)) return;

            state.Disown(bill);
            state.Accept(__instance.def, bill.recipe);

            // An explicit refusal: the memory of what the bill carried has no reason to survive.
            state.Forget(__instance, bill.recipe);

            var profile = BillAutopilotMod.Settings.ProfileFor(__instance.def);
            if (profile == null) return;

            profile.RuleForWriting(bill.recipe).mode = AutoMode.Excluded;
            BillAutopilotMod.Instance.WriteSettings();

            Messages.Message(
                "BillAutopilot.RecipeExcluded".Translate(bill.recipe.LabelCap, __instance.def.LabelCap),
                MessageTypeDefOf.SilentInput, historical: false);
        }
    }

    /// <summary>
    /// Opening the bills tab triggers a sync, so what is on screen is up to date without waiting for
    /// the periodic pass. High priority, to run before the mods that replace the tab wholesale
    /// (Nice Bill Tab) by returning false from a prefix of their own.
    /// </summary>
    [HarmonyPatch(typeof(ITab_Bills), "FillTab")]
    internal static class Patch_ITabBills_FillTab
    {
        private const float ThrottleSeconds = 0.5f;

        private static float lastSyncTime = -999f;

        [HarmonyPriority(Priority.First)]
        private static void Prefix()
        {
            // Once per layout frame, paced in real time: with the game paused the ticks stop advancing and
            // a tick-based lock would let every frame through.
            if (Event.current.type != EventType.Layout) return;

            float now = Time.realtimeSinceStartup;
            if (now - lastSyncTime < ThrottleSeconds) return;
            lastSyncTime = now;

            if (Find.Selector.SingleSelectedThing is Building_WorkTable table && table.Spawned)
            {
                AutoBillSync.Sync(table);
            }
        }
    }

    /// <summary>
    /// Marks an automatic bill in its own label, so it can be told apart from one placed by hand.
    /// Without the mark, "delete it to exclude the recipe" is a surprise, and a bill that came down on
    /// its own leaves no trace of why.
    ///
    /// The label is the one place that reaches every interface at once. Bill_Production.LabelCap is
    /// what the vanilla tab, Nice Bill Tab, Dubs Mint Menus and Better Workbench Management all read
    /// to draw a row, so nothing of theirs has to be patched. Bill_Production is the override the call
    /// actually lands on: Bill.LabelCap is virtual, and Bill_ProductionWithUft and Bill_Autonomous do
    /// not override it again.
    /// </summary>
    [HarmonyPatch(typeof(Bill_Production), nameof(Bill_Production.LabelCap), MethodType.Getter)]
    internal static class Patch_BillProduction_LabelCap
    {
        private static void Postfix(Bill_Production __instance, ref string __result)
        {
            if (!BillAutopilotMod.Settings.markAutomaticBills) return;

            var state = BillAutopilotState.Current;
            if (state == null || !state.IsAuto(__instance)) return;

            __result = __result + " " + "BillAutopilot.AutoMarker".Translate();
        }
    }

    /// <summary>Switch and profile access directly on the selected workbench.</summary>
    [HarmonyPatch(typeof(Building), nameof(Building.GetGizmos))]
    internal static class Patch_Building_GetGizmos
    {
        private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> values, Building __instance)
        {
            foreach (var gizmo in values) yield return gizmo;

            if (!(__instance is Building_WorkTable table) || !table.Faction.IsPlayerSafe()) yield break;

            var settings = BillAutopilotMod.Settings;
            var profile = settings.ProfileFor(table.def);
            bool enabled = profile != null && profile.enabled;

            yield return new Command_Toggle
            {
                defaultLabel = "BillAutopilot.Gizmo.Toggle".Translate(),
                defaultDesc = "BillAutopilot.Gizmo.ToggleDesc".Translate(table.def.LabelCap),
                icon = BillAutopilotTextures.Gizmo,
                isActive = () => settings.IsEnabled(table.def),
                toggleAction = delegate
                {
                    BenchActivation.Toggle(table.def, !settings.IsEnabled(table.def));
                },
            };

            if (!enabled) yield break;

            yield return new Command_Action
            {
                defaultLabel = "BillAutopilot.Gizmo.Configure".Translate(),
                defaultDesc = "BillAutopilot.Gizmo.ConfigureDesc".Translate(table.def.LabelCap),
                icon = BillAutopilotTextures.Gizmo,
                action = delegate
                {
                    Find.WindowStack.Add(new Dialog_BenchProfile(table.def));
                },
            };
        }
    }

    internal static class FactionExtensions
    {
        public static bool IsPlayerSafe(this Faction faction) => faction != null && faction.IsPlayer;
    }
}
