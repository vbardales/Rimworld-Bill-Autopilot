using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BillAutopilot
{
    /// <summary>
    /// Le profil d'un type d'etabli. Tout se fait au pointeur, sans saisie de texte : la partie se
    /// joue sur Steam Deck, ou un champ de texte impose le clavier virtuel.
    /// </summary>
    public class Dialog_BenchProfile : Window
    {
        private const float RowHeight = 30f;
        private const float ModeButtonWidth = 150f;
        private const float CountButtonWidth = 36f;

        /// <summary>Un groupe de recettes : la categorie du produit, et ce qu'elle contient.</summary>
        private sealed class RecipeGroup
        {
            public string label;
            public List<RecipeDef> recipes;
        }

        /// <summary>
        /// Categories repliees, partagees entre les ouvertures de la fenetre : sur un atelier de
        /// soixante recettes, retrouver tout deplie a chaque fois serait une punition.
        /// </summary>
        private static readonly HashSet<string> Collapsed = new HashSet<string>();

        private readonly ThingDef bench;
        private readonly List<RecipeGroup> groups;

        private Vector2 scrollPosition;
        private float viewHeight = 1000f;
        private bool showOverridesOnly;

        public Dialog_BenchProfile(ThingDef bench)
        {
            this.bench = bench;
            groups = BuildGroups(bench);

            doCloseX = true;
            doCloseButton = true;
            forcePause = false;
            absorbInputAroundWindow = false;
            draggable = true;
        }

        public override Vector2 InitialSize => new Vector2(
            Mathf.Min(880f, UI.screenWidth - 40f),
            Mathf.Min(720f, UI.screenHeight - 80f));

        /// <summary>
        /// Regroupe par categorie du produit. Les recettes sans produit - decoupe, cremation,
        /// chirurgie - tombent dans un groupe a part, place en dernier.
        /// </summary>
        private static List<RecipeGroup> BuildGroups(ThingDef bench)
        {
            string other = "BillAutopilot.Profile.Uncategorised".Translate();

            return bench.AllRecipes
                .Where(r => r != null)
                .Distinct()
                .GroupBy(r => r.ProducedThingDef?.FirstThingCategory?.LabelCap.RawText ?? other)
                .Select(g => new RecipeGroup
                {
                    label = g.Key,
                    recipes = g.OrderBy(r => r.LabelCap.RawText).ToList(),
                })
                // Le groupe fourre-tout ferme la marche, les autres par ordre alphabetique.
                .OrderBy(g => g.label == other ? 1 : 0)
                .ThenBy(g => g.label)
                .ToList();
        }

        private BenchProfile Profile => BillAutopilotMod.Settings.ProfileForWriting(bench);

        private static void Save()
        {
            BillAutopilotMod.Instance.WriteSettings();
            BillAutopilotState.Current?.MarkDirty();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var profile = Profile;

            Text.Font = GameFont.Medium;
            var titleRect = new Rect(inRect.x, inRect.y, inRect.width, 34f);
            Widgets.Label(titleRect, bench.LabelCap);
            Text.Font = GameFont.Small;

            var header = new Rect(inRect.x, titleRect.yMax + 4f, inRect.width, 176f);
            DrawHeader(header, profile);

            var listRect = new Rect(
                inRect.x,
                header.yMax + 8f,
                inRect.width,
                inRect.height - header.yMax - 8f - CloseButSize.y - 10f);

            DrawRecipeList(listRect, profile);
        }

        private void DrawHeader(Rect rect, BenchProfile profile)
        {
            var listing = new Listing_Standard();
            listing.Begin(rect);

            bool enabled = profile.enabled;
            listing.CheckboxLabeled("BillAutopilot.Profile.Enabled".Translate(), ref enabled,
                "BillAutopilot.Profile.EnabledDesc".Translate());
            if (enabled != profile.enabled)
            {
                profile.enabled = enabled;
                Save();
            }

            listing.GapLine(4f);

            // Mode par defaut.
            var modeRow = listing.GetRect(RowHeight);
            Widgets.Label(modeRow.LeftPart(0.45f), "BillAutopilot.Profile.DefaultMode".Translate());
            if (Widgets.ButtonText(
                    new Rect(modeRow.x + modeRow.width * 0.45f, modeRow.y, ModeButtonWidth, RowHeight - 4f),
                    ModeLabel(profile.defaultMode)))
            {
                OpenModeMenu(new[] { AutoMode.Maintain, AutoMode.Always }, mode =>
                {
                    profile.defaultMode = mode;
                    Save();
                });
            }

            if (profile.defaultMode == AutoMode.Maintain)
            {
                DrawCountRow(listing, "BillAutopilot.Profile.Target".Translate(), profile.targetCount,
                    value =>
                    {
                        profile.targetCount = Mathf.Max(1, value);
                        if (profile.floorCount > profile.targetCount) profile.floorCount = profile.targetCount;
                        Save();
                    });

                DrawCountRow(listing, "BillAutopilot.Profile.Floor".Translate(), profile.floorCount,
                    value =>
                    {
                        profile.floorCount = Mathf.Clamp(value, 0, profile.targetCount);
                        Save();
                    });
            }
            else
            {
                listing.Gap(RowHeight * 2f);
            }

            // Recettes que le jeu ne sait pas compter.
            var uncountableRow = listing.GetRect(RowHeight);
            Widgets.Label(uncountableRow.LeftPart(0.45f), "BillAutopilot.Profile.Uncountable".Translate());
            TooltipHandler.TipRegion(uncountableRow, "BillAutopilot.Profile.UncountableDesc".Translate());
            if (Widgets.ButtonText(
                    new Rect(uncountableRow.x + uncountableRow.width * 0.45f, uncountableRow.y,
                        ModeButtonWidth, RowHeight - 4f),
                    ModeLabel(profile.uncountableMode)))
            {
                OpenModeMenu(new[] { AutoMode.Excluded, AutoMode.Always }, mode =>
                {
                    profile.uncountableMode = mode;
                    Save();
                });
            }

            listing.End();
        }

        private void DrawCountRow(Listing_Standard listing, string label, int value, System.Action<int> setter)
        {
            var row = listing.GetRect(RowHeight);
            Widgets.Label(row.LeftPart(0.45f), label + " : " + value);

            float x = row.x + row.width * 0.45f;
            foreach (int step in new[] { -10, -1, 1, 10 })
            {
                var buttonRect = new Rect(x, row.y, CountButtonWidth, RowHeight - 4f);
                if (Widgets.ButtonText(buttonRect, step > 0 ? "+" + step : step.ToString()))
                {
                    setter(value + step);
                }
                x += CountButtonWidth + 4f;
            }
        }

        private void DrawRecipeList(Rect rect, BenchProfile profile)
        {
            var toolbar = new Rect(rect.x, rect.y, rect.width, RowHeight);
            Widgets.CheckboxLabeled(toolbar.LeftPart(0.32f),
                "BillAutopilot.Profile.OverridesOnly".Translate(), ref showOverridesOnly);

            // Tout plier / tout deplier : sans clavier ni recherche, c'est la seule facon de
            // traverser vite un etabli de soixante recettes.
            bool anyOpen = groups.Any(g => !Collapsed.Contains(g.label));
            if (Widgets.ButtonText(
                    new Rect(toolbar.xMax - 380f, toolbar.y, 150f, RowHeight - 4f),
                    anyOpen
                        ? "BillAutopilot.Profile.CollapseAll".Translate()
                        : "BillAutopilot.Profile.ExpandAll".Translate()))
            {
                Collapsed.Clear();
                if (anyOpen)
                {
                    foreach (var group in groups) Collapsed.Add(group.label);
                }
            }

            if (Widgets.ButtonText(
                    new Rect(toolbar.xMax - 220f, toolbar.y, 220f, RowHeight - 4f),
                    "BillAutopilot.Profile.ClearOverrides".Translate(profile.OverrideCount)))
            {
                profile.Rules.Clear();
                Save();
            }

            var outRect = new Rect(rect.x, toolbar.yMax + 4f, rect.width, rect.height - toolbar.height - 4f);
            var viewRect = new Rect(0f, 0f, outRect.width - 20f, viewHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

            float y = 0f;
            foreach (var group in groups)
            {
                var shown = group.recipes;
                if (showOverridesOnly)
                {
                    shown = shown.Where(r =>
                    {
                        var rule = profile.RuleFor(r);
                        return rule != null && !rule.IsDefault;
                    }).ToList();

                    if (shown.Count == 0) continue;
                }

                bool collapsed = Collapsed.Contains(group.label);
                DrawGroupHeader(new Rect(0f, y, viewRect.width, RowHeight), group, shown.Count, collapsed);
                y += RowHeight + 2f;

                if (collapsed) continue;

                foreach (var recipe in shown)
                {
                    DrawRecipeRow(new Rect(0f, y, viewRect.width, RowHeight), profile, recipe,
                        profile.RuleFor(recipe));
                    y += RowHeight + 2f;
                }

                y += 6f;
            }

            if (Event.current.type == EventType.Layout) viewHeight = y + 12f;

            Widgets.EndScrollView();
        }

        private static void DrawGroupHeader(Rect rect, RecipeGroup group, int count, bool collapsed)
        {
            Widgets.DrawHighlight(rect);
            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);

            var label = (collapsed ? "> " : "v ") + group.label + "  (" + count + ")";

            Text.Font = GameFont.Small;
            GUI.color = new Color(0.85f, 0.85f, 0.7f);
            Widgets.Label(new Rect(rect.x + 6f, rect.y, rect.width - 12f, rect.height), label);
            GUI.color = Color.white;

            if (Widgets.ButtonInvisible(rect))
            {
                if (!Collapsed.Remove(group.label)) Collapsed.Add(group.label);
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            }
        }

        private void DrawRecipeRow(Rect rect, BenchProfile profile, RecipeDef recipe, RecipeRule rule)
        {
            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);

            bool countable = recipe.products != null && recipe.products.Count == 1 && recipe.specialProducts == null;
            var effective = profile.ModeFor(recipe, countable);

            var labelRect = new Rect(rect.x + 4f, rect.y, rect.width * 0.5f, rect.height);
            Widgets.Label(labelRect, recipe.LabelCap);

            if (!countable)
            {
                TooltipHandler.TipRegion(labelRect, "BillAutopilot.Profile.UncountableRecipe".Translate());
            }

            float x = rect.x + rect.width * 0.5f;

            string buttonLabel = rule == null || rule.mode == AutoMode.Inherit
                ? "BillAutopilot.Mode.Inherit".Translate(ModeLabel(effective))
                : ModeLabel(rule.mode);

            if (Widgets.ButtonText(new Rect(x, rect.y + 1f, ModeButtonWidth, rect.height - 4f), buttonLabel))
            {
                OpenModeMenu(new[] { AutoMode.Inherit, AutoMode.Maintain, AutoMode.Always, AutoMode.Excluded },
                    mode =>
                    {
                        if (mode == AutoMode.Inherit) profile.ClearRule(recipe);
                        else profile.RuleForWriting(recipe).mode = mode;
                        Save();
                    },
                    effective);
            }
            x += ModeButtonWidth + 6f;

            bool maintains = (rule != null && rule.mode == AutoMode.Maintain)
                             || ((rule == null || rule.mode == AutoMode.Inherit) && effective == AutoMode.Maintain);

            if (maintains)
            {
                int target = profile.TargetFor(recipe);
                var targetRect = new Rect(x, rect.y, 70f, rect.height);
                Widgets.Label(targetRect, target.ToString());
                x += 74f;

                foreach (int step in new[] { -10, -1, 1, 10 })
                {
                    if (Widgets.ButtonText(new Rect(x, rect.y + 1f, CountButtonWidth, rect.height - 4f),
                            step > 0 ? "+" + step : step.ToString()))
                    {
                        var written = profile.RuleForWriting(recipe);
                        if (written.mode == AutoMode.Inherit) written.mode = AutoMode.Maintain;
                        written.targetCount = Mathf.Max(1, target + step);
                        if (written.floorCount < 0) written.floorCount = Mathf.Max(0, written.targetCount / 2);
                        else if (written.floorCount > written.targetCount) written.floorCount = written.targetCount;
                        Save();
                    }
                    x += CountButtonWidth + 3f;
                }
            }
        }

        private static void OpenModeMenu(IEnumerable<AutoMode> modes, System.Action<AutoMode> setter,
            AutoMode inheritedFrom = AutoMode.Maintain)
        {
            var options = modes.Select(mode => new FloatMenuOption(
                mode == AutoMode.Inherit
                    ? "BillAutopilot.Mode.Inherit".Translate(ModeLabel(inheritedFrom)).Resolve()
                    : ModeLabel(mode),
                () => setter(mode))).ToList();

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private static string ModeLabel(AutoMode mode)
        {
            switch (mode)
            {
                case AutoMode.Maintain: return "BillAutopilot.Mode.Maintain".Translate();
                case AutoMode.Always: return "BillAutopilot.Mode.Always".Translate();
                case AutoMode.Excluded: return "BillAutopilot.Mode.Excluded".Translate();
                default: return "BillAutopilot.Mode.InheritShort".Translate();
            }
        }
    }
}
