using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// A number of recipes, written in the noun form the language wants: "1 recipe", "3 recipes".
    ///
    /// The count used to sit inside sentences as "{0} recipes", so a workbench with one recipe read "1
    /// recipes" on the settings page and in the confirmation. The noun phrase is a family of keys the
    /// translator owns (see <see cref="CountForm"/>), and the sentences take the finished phrase as one
    /// argument.
    /// </summary>
    public static class RecipeCount
    {
        public static TaggedString Phrase(int count)
        {
            return CountForm.Text("BillAutopilot.Recipes", count);
        }

        /// <summary>"3 overridden" / "3 surchargées": the adjective agrees with the count.</summary>
        public static TaggedString Overridden(int count)
        {
            return CountForm.Text("BillAutopilot.Overridden", count);
        }
    }
}
