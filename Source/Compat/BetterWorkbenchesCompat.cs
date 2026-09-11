using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Bridge to Better Workbench Management (assembly ImprovedWorkbenches, packageId falconne.BWM).
    /// Soft dependency: everything goes through reflection, and every accessor returns a neutral value
    /// when the mod is absent.
    ///
    /// Why it is needed: BWM attaches an ExtendedBillData to every Bill_Production (name, CountAway,
    /// additional product filter) kept in a WorldComponent, and puts a prefix on BillStack.Delete that
    /// erases it along with the bill. The autopilot removes and re-places bills constantly, so without
    /// this bridge everything the player sets through BWM would vanish the moment a stock filled up.
    /// </summary>
    internal static class BetterWorkbenchesCompat
    {
        private static bool probed;
        private static bool active;

        private static Type extendedStorageType;
        private static Type restrictionStorageType;
        private static Type mainType;

        private static MethodInfo getExtendedDataFor;
        private static MethodInfo getOrCreateExtendedDataFor;
        private static MethodInfo linkBills;
        private static MethodInfo getBillSetContaining;
        private static MethodInfo removeBillFromLinkSets;
        private static PropertyInfo linkedSetBills;

        private static FieldInfo countAwayField;
        private static FieldInfo nameField;
        private static FieldInfo productFilterField;

        private static MethodInfo getRestrictionForTable;
        private static MethodInfo setRestrictionToBill;

        private static PropertyInfo mainInstance;
        private static MethodInfo getMaxBills;

        public static bool Active
        {
            get
            {
                Probe();
                return active;
            }
        }

        private static void Probe()
        {
            if (probed) return;
            probed = true;

            try
            {
                var assembly = FindAssembly();
                if (assembly == null) return;

                extendedStorageType = assembly.GetType("ImprovedWorkbenches.ExtendedBillDataStorage");
                var dataType = assembly.GetType("ImprovedWorkbenches.ExtendedBillData");
                var linkedSetType = assembly.GetType("ImprovedWorkbenches.LinkedBillsSet");
                restrictionStorageType = assembly.GetType("ImprovedWorkbenches.WorktableRestrictionDataStorage");
                var restrictionType = assembly.GetType("ImprovedWorkbenches.WorktableRestrictionData");
                mainType = assembly.GetType("ImprovedWorkbenches.Main");

                if (extendedStorageType == null || dataType == null) return;

                getExtendedDataFor = extendedStorageType.GetMethod("GetExtendedDataFor");
                getOrCreateExtendedDataFor = extendedStorageType.GetMethod("GetOrCreateExtendedDataFor");
                linkBills = extendedStorageType.GetMethod("LinkBills");
                getBillSetContaining = extendedStorageType.GetMethod("GetBillSetContaining");
                removeBillFromLinkSets = extendedStorageType.GetMethod("RemoveBillFromLinkSets");
                linkedSetBills = linkedSetType?.GetProperty("Bills");

                countAwayField = dataType.GetField("CountAway");
                nameField = dataType.GetField("Name");
                productFilterField = dataType.GetField("ProductAdditionalFilter");

                getRestrictionForTable = restrictionStorageType?.GetMethod(
                    "GetWorktableRestrictionData", new[] { typeof(int) });
                setRestrictionToBill = restrictionType?.GetMethod("SetWorktableRestrictionToBill");

                mainInstance = mainType?.GetProperty("Instance",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                getMaxBills = mainType?.GetMethod("GetMaxBills");

                active = getExtendedDataFor != null && getOrCreateExtendedDataFor != null
                         && countAwayField != null && nameField != null && productFilterField != null;

                if (active)
                {
                    Log.Message("[Bill Autopilot] Better Workbench Management found: extended bill "
                                + "data, links and workbench restrictions will be preserved.");
                }
            }
            catch (Exception e)
            {
                active = false;
                Log.Warning("[Bill Autopilot] Could not hook into Better Workbench Management: " + e.Message);
            }
        }

        private static Assembly FindAssembly()
        {
            foreach (var mod in LoadedModManager.RunningModsListForReading)
            {
                foreach (var assembly in mod.assemblies.loadedAssemblies)
                {
                    if (assembly.GetName().Name == "ImprovedWorkbenches") return assembly;
                }
            }
            return null;
        }

        /// <summary>Both stores are WorldComponents, so they are taken at the source, without going through Main.</summary>
        private static object ExtendedStorage =>
            extendedStorageType == null ? null : Find.World?.GetComponent(extendedStorageType);

        private static object RestrictionStorage =>
            restrictionStorageType == null ? null : Find.World?.GetComponent(restrictionStorageType);

        // --- Extended data --------------------------------------------------------------------

        /// <summary>Reads what BWM knows about a bill, just before it is taken down.</summary>
        public static BillMemory Capture(Bill_Production bill)
        {
            if (!Active || bill == null) return null;

            try
            {
                var storage = ExtendedStorage;
                if (storage == null) return null;

                var data = getExtendedDataFor.Invoke(storage, new object[] { bill });
                var memory = new BillMemory();
                bool anything = false;

                if (data != null)
                {
                    memory.countAway = (bool)countAwayField.GetValue(data);
                    memory.name = (string)nameField.GetValue(data);

                    if (productFilterField.GetValue(data) is ThingFilter filter)
                    {
                        memory.productFilter = new ThingFilter();
                        memory.productFilter.CopyAllowancesFrom(filter);
                    }
                    anything = memory.countAway || memory.name != null || memory.productFilter != null;
                }

                CaptureLinks(storage, bill, memory, ref anything);

                return anything ? memory : null;
            }
            catch (Exception e)
            {
                Log.WarningOnce("[Bill Autopilot] Could not read Better Workbench Management data: " + e.Message, 0x5A21);
                return null;
            }
        }

        private static void CaptureLinks(object storage, Bill_Production bill, BillMemory memory, ref bool anything)
        {
            if (getBillSetContaining == null || linkedSetBills == null) return;

            var set = getBillSetContaining.Invoke(storage, new object[] { bill });
            if (!(linkedSetBills.GetValue(set) is IEnumerable bills)) return;

            foreach (var other in bills)
            {
                // The companions, not the bill being removed: they are what we will reattach to.
                if (other is Bill sibling && sibling != bill) memory.linkedTo.Add(sibling.loadID);
            }
            if (memory.linkedTo.Count > 0) anything = true;
        }

        /// <summary>Puts back on a fresh bill what was read from the one it replaces.</summary>
        public static void Restore(Bill_Production bill, BillMemory memory)
        {
            if (!Active || bill == null || memory == null) return;

            try
            {
                var storage = ExtendedStorage;
                if (storage == null) return;

                var data = getOrCreateExtendedDataFor.Invoke(storage, new object[] { bill });
                if (data != null)
                {
                    countAwayField.SetValue(data, memory.countAway);
                    nameField.SetValue(data, memory.name);

                    if (memory.productFilter != null)
                    {
                        var filter = new ThingFilter();
                        filter.CopyAllowancesFrom(memory.productFilter);
                        productFilterField.SetValue(data, filter);
                    }
                }

                RestoreLinks(storage, bill, memory);
            }
            catch (Exception e)
            {
                Log.WarningOnce("[Bill Autopilot] Could not restore Better Workbench Management data: " + e.Message, 0x5A22);
            }
        }

        private static void RestoreLinks(object storage, Bill_Production bill, BillMemory memory)
        {
            if (linkBills == null || memory.linkedTo.Count == 0) return;

            // On se raccroche au premier compagnon encore vivant : LinkBills rattache au groupe
            // existing group if there is one, and only creates a new one otherwise.
            var anchor = FindLiveBill(memory.linkedTo);
            if (anchor != null) linkBills.Invoke(storage, new object[] { anchor, bill });
        }

        private static Bill_Production FindLiveBill(List<int> loadIDs)
        {
            var maps = Find.Maps;
            if (maps == null) return null;

            for (int m = 0; m < maps.Count; m++)
            {
                var buildings = maps[m].listerBuildings.allBuildingsColonist;
                for (int b = 0; b < buildings.Count; b++)
                {
                    if (!(buildings[b] is Building_WorkTable table)) continue;

                    var bills = table.billStack.Bills;
                    for (int i = 0; i < bills.Count; i++)
                    {
                        if (bills[i] is Bill_Production production && loadIDs.Contains(production.loadID))
                        {
                            return production;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>A linked bill is never an orphan: avoid removing one without having read it first.</summary>
        public static bool IsLinked(Bill_Production bill)
        {
            if (!Active || bill == null || getBillSetContaining == null) return false;

            try
            {
                var storage = ExtendedStorage;
                return storage != null && getBillSetContaining.Invoke(storage, new object[] { bill }) != null;
            }
            catch
            {
                return false;
            }
        }

        public static void Unlink(Bill_Production bill)
        {
            if (!Active || bill == null || removeBillFromLinkSets == null) return;

            try
            {
                var storage = ExtendedStorage;
                if (storage != null) removeBillFromLinkSets.Invoke(storage, new object[] { bill });
            }
            catch
            {
                // Harmless: BWM cleans up on deletion anyway.
            }
        }

        // --- Counting ----------------------------------------------------------------------------

        /// <summary>
        /// BWM only widens the count (inventories, off-map, extra products) if the bill has extended
        /// data. Our probe bill has none: it would count the vanilla way while the real bill counts
        /// another way, and the thresholds would no longer be talking about the same thing. So the
        /// same reading is grafted on before measuring.
        /// </summary>
        public static void PrimeProbe(Bill_Production probe, BillMemory memory)
        {
            if (!Active || probe == null) return;

            try
            {
                var storage = ExtendedStorage;
                if (storage == null) return;

                var data = getOrCreateExtendedDataFor.Invoke(storage, new object[] { probe });
                if (data == null) return;

                countAwayField.SetValue(data, memory != null && memory.countAway);
                nameField.SetValue(data, null);
                productFilterField.SetValue(data, memory?.productFilter);
            }
            catch
            {
                // Counting falls back to the vanilla measure: less faithful, never wrong.
            }
        }

        // --- Odds and ends ------------------------------------------------------------------------------

        /// <summary>The restriction set on the workbench, which BWM only applies to a hand-made bill.</summary>
        public static void ApplyWorktableRestriction(Building_WorkTable table, Bill_Production bill)
        {
            if (!Active || getRestrictionForTable == null || setRestrictionToBill == null) return;

            try
            {
                var storage = RestrictionStorage;
                if (storage == null) return;

                var restriction = getRestrictionForTable.Invoke(storage, new object[] { table.thingIDNumber });
                if (restriction == null) return;

                var isRestricted = restriction.GetType().GetField("isRestricted");
                if (isRestricted != null && !(bool)isRestricted.GetValue(restriction)) return;

                setRestrictionToBill.Invoke(restriction, new object[] { bill });
            }
            catch (Exception e)
            {
                Log.WarningOnce("[Bill Autopilot] Workbench restriction not applied: " + e.Message, 0x5A23);
            }
        }

        /// <summary>15 in vanilla; 125 when BWM sees No Max Bills.</summary>
        public static int MaxBills
        {
            get
            {
                if (!Active || getMaxBills == null || mainInstance == null) return BillStack.MaxCount;

                try
                {
                    var main = mainInstance.GetValue(null);
                    if (main == null) return BillStack.MaxCount;
                    return (int)getMaxBills.Invoke(main, null);
                }
                catch
                {
                    return BillStack.MaxCount;
                }
            }
        }
    }
}
