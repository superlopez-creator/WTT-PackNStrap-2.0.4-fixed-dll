using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Services;
public class HandleInsuredItemLostEventPatch : AbstractPatch
{
    protected override MethodBase? GetTargetMethod() => typeof(LocationLifecycleService).GetMethod("HandleInsuredItemLostEvent", BindingFlags.Instance | BindingFlags.NonPublic);
    [PatchPrefix]
    public static void Prefix(MongoId sessionId, PmcData preRaidPmcProfile, EndLocalRaidRequestData request, string locationName)
    {
        if (request.LostInsuredItems == null || !request.LostInsuredItems.Any()) return;
        List<Item> list = preRaidPmcProfile.Inventory?.Items ?? new List<Item>();
        Item item = list.FirstOrDefault((Item i) => i.SlotId == "ArmBand");
        if (!(item == null))
        {
            List<string> armBandDescendants = GetAllDescendants(item.Id, list).ToList();
            request.LostInsuredItems = request.LostInsuredItems.Where((Item item2) => !armBandDescendants.Contains(item2.Id)).ToList();
        }
    }
    private static IEnumerable<string> GetAllDescendants(string parentId, IEnumerable<Item> allItems)
    {
        List<Item> items = allItems.ToList();
        IEnumerable<Item> enumerable = items.Where((Item i) => i.ParentId == parentId);
        foreach (Item child in enumerable)
        {
            yield return child.Id;
            foreach (string allDescendant in GetAllDescendants(child.Id, items)) yield return allDescendant;
        }
    }
    public HandleInsuredItemLostEventPatch() : base((string)null) { }
}
