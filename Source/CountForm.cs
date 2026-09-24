using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Picks the wording for a count, so no sentence ever writes "1 recipes" or "1 surchargées".
    ///
    /// A plural cannot be built by adding an "s": French says "0 recette" and "1 recette", the adjective
    /// "surchargée" agrees too, and "Clear the 1 overrides" has to become "Clear the override". So each
    /// counted thing is a family of keys the translator owns: <c>base.One</c> for a count of one,
    /// <c>base.Many</c> for the rest, and an optional <c>base.Zero</c> for a language where zero is
    /// singular. The count is always the first argument, {0}, followed by whatever the sentence needs.
    /// </summary>
    public static class CountForm
    {
        public static TaggedString Text(string keyBase, int count, params NamedArgument[] rest)
        {
            string form = ".Many";
            if (count == 1) form = ".One";
            else if (count == 0 && (keyBase + ".Zero").CanTranslate()) form = ".Zero";

            var args = new NamedArgument[rest.Length + 1];
            args[0] = count;
            for (int i = 0; i < rest.Length; i++) args[i + 1] = rest[i];
            return (keyBase + form).Translate(args);
        }
    }
}
