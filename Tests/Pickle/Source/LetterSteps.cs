using System.Linq;
using System.Threading.Tasks;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The letter that announces a recipe unlocked later, and the toast messages the mod shows when
    /// it records something the player did in the bills tab.
    ///
    /// None of these is matched on English text. Every one of them is rebuilt from the mod's own
    /// translation key with the same arguments the mod passes, so the check holds in whatever
    /// language the pass runs in - and a French pass in developer mode, where a missing key comes
    /// back as accented gibberish rather than clean English, fails here rather than passing quietly.
    /// </summary>
    [PickleSteps]
    public class LetterSteps
    {
        /// <summary>
        /// The letter is queued as the bench is synced and sent only once the sync queue empties, so
        /// it can be several seconds behind the bill. Waiting is done here rather than left to a tick
        /// count in the scenario: a tick count is a guess about how long the game needs, and the
        /// guess breaks on a faster machine.
        /// </summary>
        [Then("Bill Autopilot has announced {string} on {string}")]
        public async Task AssertAnnounced(PickleContext ctx, string recipeDefName, string benchDefName)
        {
            var bench = Driver.BenchDef(ctx, benchDefName);
            var recipe = Driver.Recipe(ctx, recipeDefName);
            string line = "BillAutopilot.NewRecipeLine".Translate(recipe.LabelCap, bench.LabelCap).Resolve();

            await ctx.WaitUntil(() => Letters().Any(l => Text(l).Contains(line)), 30f);

            ctx.Assert(Letters().Any(l => Text(l).Contains(line)),
                $"no letter names {recipeDefName} on {benchDefName}. The letter stack holds: "
                + Describe());
        }

        [Then("Bill Autopilot has announced nothing")]
        public void AssertSilent(PickleContext ctx)
        {
            var titles = Letters().Select(l => l.Label).ToArray();
            ctx.Assert(titles.Length == 0,
                "the letter stack is not empty: " + Describe()
                + ". A recipe already unlocked when the bench was switched on is absorbed in silence; "
                + "only what is unlocked afterwards announces itself");
        }

        private static System.Collections.Generic.List<Letter> Letters() =>
            Find.LetterStack?.LettersListForReading ?? new System.Collections.Generic.List<Letter>();

        private static string Text(Letter letter) =>
            letter is ChoiceLetter choice ? choice.Text.Resolve() : letter.Label.Resolve();

        private static string Describe()
        {
            var letters = Letters();
            return letters.Count == 0
                ? "nothing"
                : string.Join(" | ", letters.Select(l => l.Label.Resolve()).ToArray());
        }
    }
}
