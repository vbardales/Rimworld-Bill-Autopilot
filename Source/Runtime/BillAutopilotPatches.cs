using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Notre etat se greffe dans le noeud &lt;game&gt; de la sauvegarde plutot que dans un
    /// GameComponent : un composant s'ecrit avec un attribut Class, et retirer le mod ferait alors
    /// echouer chaque chargement sur "Can't load abstract class Verse.GameComponent". Des noeuds
    /// nommes, eux, ne sont lus par personne une fois le mod parti - le jeu les ignore en silence.
    ///
    /// ExposeSmallComponents est le seul point commun aux deux chemins : Game.ExposeData l'appelle a
    /// la sauvegarde, Game.LoadGame au chargement (ExposeData refuse LoadingVars).
    /// </summary>
    [HarmonyPatch(typeof(Game), "ExposeSmallComponents")]
    internal static class Patch_Game_ExposeSmallComponents
    {
        private static void Postfix()
        {
            BillAutopilotState.Current?.ExposeData();
        }
    }

    /// <summary>Le battement du pilote, qui vivait dans GameComponentTick.</summary>
    [HarmonyPatch(typeof(TickManager), nameof(TickManager.DoSingleTick))]
    internal static class Patch_TickManager_DoSingleTick
    {
        private static void Postfix()
        {
            if (Current.ProgramState != ProgramState.Playing) return;
            BillAutopilotState.Current?.Tick();
        }
    }

    /// <summary>Une recherche terminee peut debloquer des recettes : on repasse sur tous les etablis.</summary>
    [HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.FinishProject))]
    internal static class Patch_ResearchManager_FinishProject
    {
        private static void Postfix()
        {
            BillAutopilotState.Current?.MarkDirty();
        }
    }

    /// <summary>
    /// Supprimer une bill automatique dans l'onglet, c'est refuser la recette : sans cela le pilote
    /// la reposerait au passage suivant, et le geste n'aurait aucun effet.
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

            // Refus explicite : le souvenir de ce que portait la bill n'a plus lieu d'etre.
            state.Forget(__instance.def, bill.recipe);

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
    /// Ouvrir l'onglet des bills declenche une synchronisation : ce qu'on regarde est a jour, sans
    /// attendre le passage periodique. Priorite haute pour passer avant les mods qui remplacent
    /// entierement l'onglet (Nice Bill Tab) en renvoyant false depuis leur propre prefixe.
    /// </summary>
    [HarmonyPatch(typeof(ITab_Bills), "FillTab")]
    internal static class Patch_ITabBills_FillTab
    {
        private const float ThrottleSeconds = 0.5f;

        private static float lastSyncTime = -999f;

        [HarmonyPriority(Priority.First)]
        private static void Prefix()
        {
            // Une fois par frame de mise en page, et cadence en temps reel : jeu en pause, les ticks
            // n'avancent plus et un verrou en ticks laisserait passer chaque frame.
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

    /// <summary>Interrupteur et acces au profil directement sur l'etabli selectionne.</summary>
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
                    var written = settings.ProfileForWriting(table.def);
                    written.enabled = !written.enabled;
                    BillAutopilotMod.Instance.WriteSettings();
                    BillAutopilotState.Current?.MarkDirty();
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
