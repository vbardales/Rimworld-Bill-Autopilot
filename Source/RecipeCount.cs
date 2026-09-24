using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// A number of recipes, written in the noun form the language wants: "1 recipe", "3 recipes".
    ///
    /// The count used to sit inside sentences as "{0} recipes", so a workbench with one recipe read "1
    /// recipes" on the settings page and in the confirmation. The plural cannot be built by adding an "s":
    /// French says "0 recette" and "1 recette", English "0 recipes" and "1 recipe". So the noun phrase is
    /// three keys the translator owns, and the sentences take the finished phrase as one argument.
    /// </summary>
    public static class RecipeCount
    {
        public static TaggedString Phrase(int count)
        {
            string key = count == 1
                ? "BillAutopilot.Recipes.One"
                : count == 0 ? "BillAutopilot.Recipes.Zero" : "BillAutopilot.Recipes.Many";
            return key.Translate(count);
        }
    }
}
