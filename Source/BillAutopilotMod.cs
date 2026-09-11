using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BillAutopilot
{
    public class BillAutopilotMod : Mod
    {
        public const string HarmonyId = "nelim.billautopilot";

        public static BillAutopilotMod Instance { get; private set; }
        public static BillAutopilotSettings Settings { get; private set; }
        public static Harmony HarmonyInstance { get; private set; }

        private Vector2 scrollPosition;
        private float viewHeight = 1000f;
        private List<ThingDef> benchCache;

        public BillAutopilotMod(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<BillAutopilotSettings>();

            HarmonyInstance = new Harmony(HarmonyId);
            HarmonyInstance.PatchAll();
        }

        public override string SettingsCategory() => "Bill Autopilot";

        /// <summary>Every bill-taking workbench in the game, those from mods included.</summary>
        private List<ThingDef> Benches =>
            benchCache ?? (benchCache = DefDatabase<ThingDef>.AllDefsListForReading
                .Where(def => def.IsWorkTable && def.AllRecipes != null && def.AllRecipes.Count > 0)
                .OrderBy(def => def.LabelCap.RawText)
                .ToList());

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var header = new Rect(inRect.x, inRect.y, inRect.width, 204f);
            var listing = new Listing_Standard();
            listing.Begin(header);

            listing.Label("BillAutopilot.Settings.Intro".Translate());

            // What the mod found around it. The log already says so at startup, but nobody reads a log to
            // find out whether an integration took: it shows here.
            var integrations = listing.GetRect(48f);
            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            Widgets.Label(integrations, "BillAutopilot.Settings.Integrations".Translate(
                Detected("Better Workbench Management", BetterWorkbenchesCompat.Active),
                Detected("Dubs Mint Menus", DubsMintMenusCompat.Active),
                Detected("Nice Bill Tab", NiceBillTabCompat.Active),
                Detected("Nice Bill Tab - Expansion", HiddenRecipesCompat.Available)));
            GUI.color = Color.white;
            TooltipHandler.TipRegion(integrations, "BillAutopilot.Settings.IntegrationsDesc".Translate());

            listing.CheckboxLabeled("BillAutopilot.Settings.Notify".Translate(),
                ref Settings.notifyNewRecipes, "BillAutopilot.Settings.NotifyDesc".Translate());

            listing.CheckboxLabeled("BillAutopilot.Settings.MarkBills".Translate(),
                ref Settings.markAutomaticBills, "BillAutopilot.Settings.MarkBillsDesc".Translate());

            // The game's cap: 15, or 125 when Better Workbench Management sees No Max Bills.
            int gameMax = BetterWorkbenchesCompat.MaxBills;

            var capRow = listing.GetRect(28f);
            Widgets.Label(capRow.LeftPart(0.6f),
                "BillAutopilot.Settings.MaxBills".Translate(Settings.maxAutoBillsPerTable, gameMax));
            Settings.maxAutoBillsPerTable = Mathf.RoundToInt(Widgets.HorizontalSlider(
                capRow.RightPart(0.4f), Settings.maxAutoBillsPerTable, 1f, gameMax, middleAlignment: true));

            listing.GapLine(6f);
            listing.End();

            var outRect = new Rect(inRect.x, header.yMax + 4f, inRect.width, inRect.height - header.height - 8f);
            var viewRect = new Rect(0f, 0f, outRect.width - 20f, viewHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

            float y = 0f;
            foreach (var bench in Benches)
            {
                DrawBenchRow(new Rect(0f, y, viewRect.width, 30f), bench);
                y += 32f;
            }

            if (Event.current.type == EventType.Layout) viewHeight = y + 12f;

            Widgets.EndScrollView();
        }

        private void DrawBenchRow(Rect rect, ThingDef bench)
        {
            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);

            var profile = Settings.ProfileFor(bench);
            bool enabled = profile != null && profile.enabled;

            var checkRect = new Rect(rect.x + 4f, rect.y, rect.width * 0.5f, rect.height);
            bool afterClick = enabled;
            Widgets.CheckboxLabeled(checkRect, bench.LabelCap, ref afterClick);
            if (afterClick != enabled) BenchActivation.Toggle(bench, afterClick);

            var summaryRect = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.28f, rect.height);
            GUI.color = new Color(1f, 1f, 1f, 0.6f);
            Widgets.Label(summaryRect, Summary(bench, profile));
            GUI.color = Color.white;

            if (Widgets.ButtonText(new Rect(rect.xMax - 150f, rect.y + 1f, 146f, rect.height - 4f),
                    "BillAutopilot.Settings.Configure".Translate()))
            {
                Find.WindowStack.Add(new Dialog_BenchProfile(bench));
            }
        }

        private static string Detected(string name, bool present)
        {
            return name + " " + (present
                ? "BillAutopilot.Settings.Detected".Translate()
                : "BillAutopilot.Settings.NotDetected".Translate());
        }

        private static string Summary(ThingDef bench, BenchProfile profile)
        {
            int recipeCount = bench.AllRecipes.Count;
            if (profile == null || !profile.enabled)
            {
                return "BillAutopilot.Settings.SummaryOff".Translate(recipeCount);
            }

            return profile.defaultMode == AutoMode.Always
                ? "BillAutopilot.Settings.SummaryAlways".Translate(recipeCount, profile.OverrideCount)
                : "BillAutopilot.Settings.SummaryMaintain".Translate(
                    recipeCount, profile.targetCount, profile.OverrideCount);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }
    }
}
