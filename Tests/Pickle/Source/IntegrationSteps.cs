using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace BillAutopilot.PickleSteps
{
    /// <summary>
    /// The three neighbours whose interfaces can actually be driven from here: Better Workbench
    /// Management, Nice Bill Tab and Dubs Mint Menus.
    ///
    /// Everything is reached by reflection, exactly as the mod reaches it, and nothing here is
    /// referenced at compile time. That is not caution for its own sake: the minimal pass loads this
    /// assembly with none of those mods present, and a real reference would fail to load the whole
    /// suite in the very pass meant to prove the mod stands alone. Each scenario using these steps
    /// carries a `@requires:` tag, so it is skipped rather than failed when its mod is absent.
    ///
    /// The member names below are the ones the mod's own bridges use. When a neighbour renames one,
    /// this fails with the name it looked for - which is the same failure the mod itself would have,
    /// reported somewhere a person reads.
    /// </summary>
    [PickleSteps]
    public class IntegrationSteps
    {
        private const BindingFlags Any = BindingFlags.Static | BindingFlags.Instance
                                         | BindingFlags.Public | BindingFlags.NonPublic;

        // --- Better Workbench Management ----------------------------------------------------------

        /// <summary>
        /// BWM keeps an ExtendedBillData per bill in a WorldComponent, and prefixes BillStack.Delete
        /// to erase it with the bill. The autopilot removes and re-places bills constantly, so
        /// without its bridge everything set through BWM would vanish the moment a stock filled up.
        ///
        /// These steps write and read through BWM's own store, not through the mod's copy of it: a
        /// check that read the mod's own memory back would agree with itself whether or not anything
        /// ever reached BWM.
        /// </summary>
        [Given("Better Workbench Management counts Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) away from the home map")]
        public void SetCountAway(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            SetExtendedField(ctx, recipeDefName, benchDefName, x, z, "CountAway", true);
        }

        [Then("Better Workbench Management still counts Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) away from the home map")]
        public void AssertCountAway(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z)
        {
            var value = GetExtendedField(ctx, recipeDefName, benchDefName, x, z, "CountAway");
            ctx.Assert(value is bool flag && flag,
                $"the bill that came back is not counting away from the home map. Whatever the player "
                + "set through Better Workbench Management was lost on the down-and-up cycle, in "
                + "silence: nothing crashes, the feature is simply gone");
        }

        [Given("Better Workbench Management names Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) {string}")]
        public void SetBwmName(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z, string name)
        {
            SetExtendedField(ctx, recipeDefName, benchDefName, x, z, "Name", name);
        }

        [Then("Better Workbench Management still names Bill Autopilot's bill for {string} on the {string} at \\({int}, {int}\\) {string}")]
        public void AssertBwmName(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z, string name)
        {
            var value = GetExtendedField(ctx, recipeDefName, benchDefName, x, z, "Name");
            ctx.Assert((value as string) == name,
                $"the bill that came back is named '{value as string ?? "nothing"}' in Better Workbench "
                + $"Management's own store, not '{name}'");
        }

        /// <summary>
        /// A link group. What has to survive is membership: the returning bill must REJOIN the group
        /// its predecessor belonged to rather than start a new one of its own, which looks identical
        /// on a single bill and is a different thing entirely on two.
        /// </summary>
        [Given("Better Workbench Management links Bill Autopilot's bills for {string} and {string} on the {string} at \\({int}, {int}\\)")]
        public void LinkBills(PickleContext ctx, string firstRecipe, string secondRecipe,
            string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var a = AutoBill(ctx, table, firstRecipe);
            var b = AutoBill(ctx, table, secondRecipe);

            var storage = ExtendedStorage(ctx);
            var link = storage.GetType().GetMethod("LinkBills");
            ctx.Require(link != null,
                "ImprovedWorkbenches.ExtendedBillDataStorage.LinkBills no longer exists: update these steps");

            link.Invoke(storage, new object[] { a, b });
        }

        [Then("Better Workbench Management still links Bill Autopilot's bills for {string} and {string} on the {string} at \\({int}, {int}\\)")]
        public void AssertLinked(PickleContext ctx, string firstRecipe, string secondRecipe,
            string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var a = AutoBill(ctx, table, firstRecipe);
            var b = AutoBill(ctx, table, secondRecipe);

            var storage = ExtendedStorage(ctx);
            var getSet = storage.GetType().GetMethod("GetBillSetContaining");
            ctx.Require(getSet != null,
                "ImprovedWorkbenches.ExtendedBillDataStorage.GetBillSetContaining no longer exists: "
                + "update these steps");

            var set = getSet.Invoke(storage, new object[] { a });
            ctx.Assert(set != null,
                $"the returning bill for {firstRecipe} belongs to no link set at all: it started a new "
                + "group of its own instead of rejoining the one it was in");

            var billsProperty = set.GetType().GetProperty("Bills");
            ctx.Require(billsProperty != null,
                "ImprovedWorkbenches.LinkedBillsSet.Bills no longer exists: update these steps");

            bool together = (billsProperty.GetValue(set) as IEnumerable)?.Cast<object>()
                .Any(o => ReferenceEquals(o, b)) == true;
            ctx.Assert(together,
                $"the bills for {firstRecipe} and {secondRecipe} are no longer in the same link set");
        }

        // --- Nice Bill Tab -------------------------------------------------------------------------

        /// <summary>
        /// Nice Bill Tab redraws the whole tab from a cached list of the rows it shows, and reorders
        /// from that list before writing back into the stack. A stale entry there is not cosmetic: a
        /// drag can put a deleted bill back.
        ///
        /// The drag itself is a gesture a person performs and stays in the manual table. What IS
        /// assertable, and what these two steps pin, is its cause: the flag that tells that list to
        /// rebuild is set every time the autopilot puts a bill up or takes one down. Photographing
        /// the result without naming the cause would leave a failure with nowhere to point.
        /// </summary>
        [Given("Nice Bill Tab's row cache is marked as up to date")]
        public void ClearRefreshFlag(PickleContext ctx) => RefreshField(ctx).SetValue(null, false);

        [Then("Nice Bill Tab's row cache has been told to rebuild")]
        public void AssertRefreshFlag(PickleContext ctx)
        {
            ctx.Assert((bool)RefreshField(ctx).GetValue(null),
                "Nice Bill Tab's row cache was not told to rebuild. Its list goes on drawing a bill "
                + "that no longer exists, and dragging the rows can put the deleted one back - the "
                + "single most dangerous interaction in this mod");
        }

        private static FieldInfo RefreshField(PickleContext ctx)
        {
            var type = Assembly(ctx, "NiceBillTab")?.GetType("NiceBillTab.TabBillsDrawer");
            ctx.Require(type != null,
                "NiceBillTab.TabBillsDrawer not found: this scenario needs Nice Bill Tab staged, which "
                + "its @requires tag should have ensured");

            var field = type.GetField("shouldRefreshFilter", BindingFlags.Static | BindingFlags.Public);
            ctx.Require(field != null,
                "NiceBillTab.TabBillsDrawer.shouldRefreshFilter no longer exists: Nice Bill Tab renamed "
                + "its cache flag, so this mod's bridge is dead too and its list will show bills that "
                + "are gone");
            return field;
        }

        // --- Nice Bill Tab - Expansion, the hidden-recipe store -------------------------------------

        /// <summary>
        /// Hiding and unhiding a recipe on a bench, through that mod's own store.
        ///
        /// The mod's bridge only ever READS the store - <c>IsHidden(table, recipe)</c> is the whole
        /// of its interest in it - so the write side is not documented anywhere on this side of the
        /// fence, and Nice Bill Tab - Expansion is not installed on the machine these steps were
        /// written on. Rather than guess at one name and lose a whole run to an unbound method, a
        /// short list of plausible ones is tried and the failure prints every public static method
        /// the store really has. The first run of this pass therefore either works or says exactly
        /// what to write instead.
        /// </summary>
        [When("{string} is hidden on the Bill Autopilot bench {string} at \\({int}, {int}\\)")]
        public void Hide(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z) =>
            SetHidden(ctx, recipeDefName, benchDefName, x, z, true);

        [When("{string} is unhidden on the Bill Autopilot bench {string} at \\({int}, {int}\\)")]
        public void Unhide(PickleContext ctx, string recipeDefName, string benchDefName, int x, int z) =>
            SetHidden(ctx, recipeDefName, benchDefName, x, z, false);

        private static void SetHidden(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z, bool hidden)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var recipe = Driver.Recipe(ctx, recipeDefName);

            var assembly = Assembly(ctx, "NiceBillTabExpansion");
            var store = assembly.GetType("NiceBillTabExpansion.HiddenRecipeStore")
                        ?? assembly.GetTypes().FirstOrDefault(t => t.Name == "HiddenRecipeStore");
            ctx.Require(store != null,
                "NiceBillTabExpansion.HiddenRecipeStore not found: this scenario needs Nice Bill Tab - "
                + "Expansion staged, which its @requires tag should have ensured");

            var statics = store.GetMethods(BindingFlags.Static | BindingFlags.Public);

            var toggle = statics.FirstOrDefault(m => m.Name == "SetHidden" && Takes(m, typeof(bool)));
            if (toggle != null)
            {
                toggle.Invoke(null, new object[] { table, recipe, hidden });
                return;
            }

            var single = statics.FirstOrDefault(m => m.Name == (hidden ? "Hide" : "Unhide") && Takes(m, null))
                         ?? statics.FirstOrDefault(m => m.Name == (hidden ? "AddHidden" : "RemoveHidden") && Takes(m, null));
            if (single != null)
            {
                single.Invoke(null, new object[] { table, recipe });
                return;
            }

            ctx.Require(false,
                "no way to hide a recipe was found on NiceBillTabExpansion.HiddenRecipeStore. Tried "
                + "SetHidden(table, recipe, bool), Hide/Unhide(table, recipe) and "
                + "AddHidden/RemoveHidden(table, recipe). The type really carries: "
                + string.Join(", ", statics.Select(m =>
                    m.Name + "(" + string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name).ToArray()) + ")")
                    .ToArray())
                + ". Put the right one in IntegrationSteps.SetHidden");
        }

        /// <summary>
        /// A (Building_WorkTable, RecipeDef) method, optionally followed by one more parameter -
        /// which is how a setter and a pair of one-way methods both get recognised.
        /// </summary>
        private static bool Takes(MethodInfo method, Type extra)
        {
            var parameters = method.GetParameters();
            int wanted = extra == null ? 2 : 3;
            if (parameters.Length != wanted) return false;
            if (parameters[0].ParameterType != typeof(Building_WorkTable)) return false;
            if (parameters[1].ParameterType != typeof(RecipeDef)) return false;
            return extra == null || parameters[2].ParameterType == extra;
        }

        // --- Dubs Mint Menus -----------------------------------------------------------------------

        /// <summary>
        /// Making a bench template photographs every bill on the bench. A template taken from an
        /// autopiloted bench would capture whatever the autopilot happened to have up at that moment,
        /// and re-applying it later would turn those recipes into hand-placed bills for good -
        /// retiring the autopilot from them without a word.
        ///
        /// The real method is called, so the mod's own postfix on it runs. Building the template by
        /// hand here would test nothing at all.
        /// </summary>
        [When("a Dubs Mint Menus bench template is made from the {string} at \\({int}, {int}\\)")]
        public void MakeTemplate(PickleContext ctx, string benchDefName, int x, int z)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var type = Assembly(ctx, "DubsMintMenus")?.GetType("DubsMintMenus.Patch_BillStack_DoListing");
            ctx.Require(type != null,
                "DubsMintMenus.Patch_BillStack_DoListing not found: this scenario needs Dubs Mint Menus "
                + "staged, which its @requires tag should have ensured");

            var method = type.GetMethod("MakeBenchTemplate", Any);
            ctx.Require(method != null,
                "DubsMintMenus.Patch_BillStack_DoListing.MakeBenchTemplate no longer exists: update "
                + "these steps, and the mod's own bridge with them");

            method.Invoke(null, new object[] { table });
        }

        [Then("the Dubs Mint Menus template holds no bill for {string}")]
        public void AssertTemplateExcludes(PickleContext ctx, string recipeDefName)
        {
            var recipes = TemplateRecipes(ctx);
            ctx.Assert(!recipes.Contains(recipeDefName),
                $"the template captured {recipeDefName}, which the autopilot had put up. Applying that "
                + "template later would turn it into a hand-placed bill for good. The template holds: "
                + Join(recipes));
        }

        [Then("the Dubs Mint Menus template holds a bill for {string}")]
        public void AssertTemplateIncludes(PickleContext ctx, string recipeDefName)
        {
            var recipes = TemplateRecipes(ctx);
            ctx.Assert(recipes.Contains(recipeDefName),
                $"the template does not hold {recipeDefName}, which the player placed by hand. Keeping "
                + "autopilot bills out must not take the player's own with them. The template holds: "
                + Join(recipes));
        }

        private static System.Collections.Generic.List<string> TemplateRecipes(PickleContext ctx)
        {
            var settings = Assembly(ctx, "DubsMintMenus")?.GetType("DubsMintMenus.Settings");
            ctx.Require(settings != null, "DubsMintMenus.Settings not found: update these steps");

            var property = settings.GetProperty("fbenchTemplates", Any);
            ctx.Require(property != null,
                "DubsMintMenus.Settings.fbenchTemplates no longer exists: update these steps");

            var templates = property.GetValue(null) as IList;
            ctx.Require(templates != null && templates.Count > 0,
                "Dubs Mint Menus holds no bench template: the step that makes one did not, or its "
                + "template list lives elsewhere now");

            var bills = Assembly(ctx, "DubsMintMenus").GetType("DubsMintMenus.FBenchTemplate")
                ?.GetField("FBills")?.GetValue(templates[templates.Count - 1]) as IList;
            ctx.Require(bills != null,
                "DubsMintMenus.FBenchTemplate.FBills no longer exists: update these steps");

            var recipes = new System.Collections.Generic.List<string>();
            foreach (var entry in bills)
            {
                if (entry is Bill bill && bill.recipe != null) recipes.Add(bill.recipe.defName);
            }
            return recipes;
        }

        // --- Shared ------------------------------------------------------------------------------

        private static Bill_Production AutoBill(PickleContext ctx, Building_WorkTable table, string recipeDefName)
        {
            var recipe = Driver.Recipe(ctx, recipeDefName);
            var bill = Driver.AutoBill(ctx, table, recipe);
            ctx.Require(bill != null,
                $"the autopilot has no bill up for {recipeDefName}: {Driver.Describe(ctx, table)}");
            return bill;
        }

        private static Assembly Assembly(PickleContext ctx, string name)
        {
            foreach (var mod in LoadedModManager.RunningModsListForReading)
            {
                foreach (var assembly in mod.assemblies.loadedAssemblies)
                {
                    if (assembly.GetName().Name == name) return assembly;
                }
            }
            ctx.Require(false, $"no assembly named '{name}' is loaded in this pass");
            return null;
        }

        private static object ExtendedStorage(PickleContext ctx)
        {
            var type = Assembly(ctx, "ImprovedWorkbenches")?.GetType("ImprovedWorkbenches.ExtendedBillDataStorage");
            ctx.Require(type != null,
                "ImprovedWorkbenches.ExtendedBillDataStorage not found: this scenario needs Better "
                + "Workbench Management staged, which its @requires tag should have ensured");

            var storage = Find.World?.GetComponent(type);
            ctx.Require(storage != null,
                "Better Workbench Management's ExtendedBillDataStorage is not on the world: it is a "
                + "WorldComponent, so this means no world is loaded");
            return storage;
        }

        private static object ExtendedData(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z, bool create)
        {
            var table = Driver.Bench(ctx, benchDefName, x, z);
            var bill = AutoBill(ctx, table, recipeDefName);
            var storage = ExtendedStorage(ctx);

            string name = create ? "GetOrCreateExtendedDataFor" : "GetExtendedDataFor";
            var method = storage.GetType().GetMethod(name);
            ctx.Require(method != null,
                $"ImprovedWorkbenches.ExtendedBillDataStorage.{name} no longer exists: update these steps");

            return method.Invoke(storage, new object[] { bill });
        }

        private static void SetExtendedField(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z, string fieldName, object value)
        {
            var data = ExtendedData(ctx, recipeDefName, benchDefName, x, z, create: true);
            ctx.Require(data != null,
                "Better Workbench Management would not create extended data for this bill");

            var field = data.GetType().GetField(fieldName);
            ctx.Require(field != null,
                $"ImprovedWorkbenches.ExtendedBillData.{fieldName} no longer exists: update these steps");
            field.SetValue(data, value);
        }

        private static object GetExtendedField(PickleContext ctx, string recipeDefName, string benchDefName,
            int x, int z, string fieldName)
        {
            var data = ExtendedData(ctx, recipeDefName, benchDefName, x, z, create: false);
            if (data == null) return null;

            var field = data.GetType().GetField(fieldName);
            ctx.Require(field != null,
                $"ImprovedWorkbenches.ExtendedBillData.{fieldName} no longer exists: update these steps");
            return field.GetValue(data);
        }

        private static string Join(System.Collections.Generic.List<string> values) =>
            values.Count == 0 ? "nothing" : string.Join(", ", values.ToArray());
    }
}
