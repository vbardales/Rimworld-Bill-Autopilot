using System;
using System.IO;
using System.Linq;
using System.Xml;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The one thing a single session cannot check: a game saved WITH the mod, loaded WITHOUT it.
    ///
    /// The mod keeps its state in plain named nodes grafted into the save's game node rather than in a
    /// GameComponent, precisely so that removing it raises no load error (README, "How it works"). That is a
    /// claim about a second process: the mod list is fixed when the game starts, so removing the mod means a
    /// launch of its own. The chain is `-Filter 21-removal-write -Then removal-check -ThenWithout
    /// nelim.billautopilot,nelim.billautopilot.pickletests`: the first launch writes a save and hands it to
    /// the companion mod `nelim.billautopilot.pickleremoval`, which does not depend on Bill Autopilot; the
    /// second launch runs without the mod and loads it. Pattern from Housebroken's TF-18.
    /// </summary>
    [PickleSteps]
    public class RemovalSteps
    {
        [When("Bill Autopilot saves the game as {string}")]
        public void Save(PickleContext ctx, string file)
        {
            GameDataSaveLoader.SaveGame(file);
            ctx.Require(File.Exists(GenFilePaths.FilePathForSavedGame(file)), "no save file was written for " + file);
        }

        /// <summary>
        /// The two halves of the design in one check. The state IS in the save (a control: a save that held
        /// nothing would pass the second half for the wrong reason), and no class of the mod appears in it,
        /// which is what would make a load fail once the mod is gone. The header lists every active mod by
        /// name, so it is set aside first.
        /// </summary>
        [Then("Bill Autopilot save {string} keeps its state as plain nodes, with no class of the mod in it")]
        public void PlainNodes(PickleContext ctx, string file)
        {
            var doc = new XmlDocument();
            doc.Load(GenFilePaths.FilePathForSavedGame(file));
            var meta = doc.DocumentElement.SelectSingleNode("meta");
            ctx.Require(meta != null, "the save has no meta header, so the mod list cannot be set aside");
            doc.DocumentElement.RemoveChild(meta);

            var ours = doc.SelectNodes("//*").Cast<XmlNode>()
                .Where(n => n.Name.StartsWith("billAutopilot", StringComparison.Ordinal))
                .Select(n => n.Name).Distinct().ToList();
            ctx.Assert(ours.Count > 0,
                "the save holds none of the mod's nodes (billAutopilot...): the removal check below would prove "
                + "nothing, since there is no state to survive");

            var text = doc.OuterXml;
            int at = text.IndexOf("Class=\"BillAutopilot", StringComparison.Ordinal);
            ctx.Assert(at < 0,
                "the save names a class of the mod, which a game loaded without it cannot resolve, near: "
                + (at < 0 ? "" : text.Substring(Math.Max(0, at - 80), Math.Min(200, text.Length - Math.Max(0, at - 80)))));
        }

        /// <summary>
        /// Pickle finds a saved game as a fixture: a .rws in the Pickle/Fixtures folder of an active mod. The
        /// game saved in this launch is handed to the mod that the next launch will load it with.
        /// </summary>
        [When("Bill Autopilot hands the saved game {string} to the mod {string}")]
        public void Hand(PickleContext ctx, string file, string packageId)
        {
            var target = LoadedModManager.RunningModsListForReading.FirstOrDefault(m =>
                m.PackageIdPlayerFacing.ToLowerInvariant() == packageId.ToLowerInvariant());
            ctx.Require(target != null, "no active mod has the packageId " + packageId);
            var folder = Path.Combine(target.RootDir, "Pickle", "Fixtures");
            Directory.CreateDirectory(folder);
            var destination = Path.Combine(folder, file + ".rws");
            File.Copy(GenFilePaths.FilePathForSavedGame(file), destination, true);
            ctx.Require(File.Exists(destination), "the saved game was not copied to " + destination);
        }
    }
}
