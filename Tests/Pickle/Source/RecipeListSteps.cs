using System.Collections.Generic;
using RimWorks.Pickle;
using RimWorld;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// A recipe leaving a workbench's list, and coming back, the way Choose Your Recipe does it: it removes the
    /// recipe from <c>ThingDef.AllRecipes</c> (its <c>allRecipesCached</c>) and rebuilds that list from the
    /// original one when the player changes their choice. Done here on the list itself, without that mod, so
    /// that what is under test is this mod's reaction to a recipe disappearing and returning, in any pass.
    ///
    /// The list belongs to the def, which outlives the game: a hook puts every removed recipe back where it
    /// was after each scenario, or the next one would start with a bench missing a recipe.
    /// </summary>
    [PickleSteps]
    public class RecipeListSteps
    {
        private class Removed
        {
            public ThingDef bench;
            public RecipeDef recipe;
            public int index;
        }

        private static readonly List<Removed> Taken = new List<Removed>();

        [When("the recipe {string} is taken off the {string} the way Choose Your Recipe does")]
        public void TakeOff(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = DefDatabase<ThingDef>.GetNamedSilentFail(benchDefName);
            ctx.Require(bench != null, $"no ThingDef named '{benchDefName}'");
            var recipe = DefDatabase<RecipeDef>.GetNamedSilentFail(recipeDefName);
            ctx.Require(recipe != null, $"no RecipeDef named '{recipeDefName}'");

            var list = bench.AllRecipes;
            int index = list.IndexOf(recipe);
            ctx.Require(index >= 0, $"{benchDefName} does not offer {recipeDefName}");

            list.RemoveAt(index);
            Taken.Add(new Removed { bench = bench, recipe = recipe, index = index });
        }

        [When("the recipe {string} is put back on the {string}")]
        public void PutBack(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            int at = Taken.FindIndex(t => t.bench.defName == benchDefName && t.recipe.defName == recipeDefName);
            ctx.Require(at >= 0, $"{recipeDefName} was not taken off {benchDefName} by an earlier step");

            Restore(Taken[at]);
            Taken.RemoveAt(at);
        }

        [AfterScenario]
        public void PutEverythingBack(PickleContext ctx)
        {
            for (int i = Taken.Count - 1; i >= 0; i--) Restore(Taken[i]);
            Taken.Clear();
        }

        private static void Restore(Removed removed)
        {
            var list = removed.bench.AllRecipes;
            if (list.Contains(removed.recipe)) return;
            list.Insert(System.Math.Min(removed.index, list.Count), removed.recipe);
        }
    }
}
