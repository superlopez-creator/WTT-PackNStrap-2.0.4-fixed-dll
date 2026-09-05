using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
public class IsItemKeptAfterDeathPatch : AbstractPatch
{
    protected override MethodBase? GetTargetMethod() => typeof(InRaidHelper).GetMethod("IsItemKeptAfterDeath", BindingFlags.Instance | BindingFlags.NonPublic);
    [PatchPostfix]
    public static void Postfix(PmcData pmcData, Item itemToCheck, ref bool __result)
    {
        if (!__result && IsItemInArmBand(pmcData, itemToCheck)) __result = true;
    }
    private static bool IsItemInArmBand(PmcData pmcData, Item item)
    {
        List<Item> list = pmcData.Inventory?.Items ?? new List<Item>();
        Item item2 = list.FirstOrDefault((Item i) => i.SlotId == "ArmBand");
        if (item2 == null) return false;
        if (!(item.Id == item2.Id)) return list.GetItemWithChildren(item2.Id).Any((Item i) => i.Id == item.Id);
        return true;
    }
    public IsItemKeptAfterDeathPatch() : base((string)null) { }
}
