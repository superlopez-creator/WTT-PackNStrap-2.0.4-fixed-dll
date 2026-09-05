// WTT-PackNStrap, Version=2.0.4.0, Culture=neutral, PublicKeyToken=null
// WTTPackNStrap.WTTPackNStrap
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using WTTPackNStrap.Models;
using WTTServerCommonLib;

[Injectable(/*Could not decode attribute arguments.*/)]
public class WTTPackNStrap(global::WTTServerCommonLib.WTTServerCommonLib wttCommon, DatabaseService databaseService, JsonUtil jsonUtil, ModHelper modHelper, ConfigServer configServer) : IOnLoad
{
    private Assembly _assembly;
    private Dictionary<MongoId, TemplateItem> _itemsDb;
    private Dictionary<MongoId, Trader> _traderDb;

    public async Task OnLoad()
    {
        _assembly = Assembly.GetExecutingAssembly();
        _itemsDb = databaseService.GetItems();
        _traderDb = databaseService.GetTraders();
        CreateCustomItemsAndTemplates();
        ConfigureCustomItemsToTraders();
        AddToInventorySlots();
        await wttCommon.CustomItemServiceExtended.CreateCustomItems(_assembly);
        wttCommon.CustomRigLayoutService.CreateRigLayouts(_assembly);
        await wttCommon.CustomLocaleService.CreateCustomLocales(_assembly);
        ApplyConfigSettings();
    }

    private void ApplyConfigSettings()
    {
        string absolutePathToModFolder = modHelper.GetAbsolutePathToModFolder(_assembly);
        string path = System.IO.Path.Join(absolutePathToModFolder, "config", "config.jsonc");
        if (!File.Exists(path)) return;
        string json = File.ReadAllText(path);
        PackNStrapConfig packNStrapConfig = jsonUtil.Deserialize<PackNStrapConfig>(json);
        if (packNStrapConfig != null && !packNStrapConfig.loseArmbandOnDeath)
        {
            foreach (string item in BeltIds.Items)
            {
                if (_itemsDb.TryGetValue(item, out TemplateItem value))
                {
                    TemplateItemProperties? properties = value.Properties;
                    if ((object)properties != null) properties.InsuranceDisabled = true;
                }
            }
        }
        else
        {
            LostOnDeathConfig config = configServer.GetConfig<LostOnDeathConfig>();
            config.Equipment.ArmBand = true;
        }
        if (packNStrapConfig == null || !packNStrapConfig.addCasesToSecureContainers) return;
        foreach (string item2 in ContainerIds.Items)
        {
            foreach (TemplateItem value2 in _itemsDb.Values)
            {
                if ((!(value2.Parent == (MongoId)"5448bf274bdc2dfc2f8b456a") && !(value2.Parent == (MongoId)"68154651f849fb4e7d816738")) || value2.Id == (MongoId)"5c0a794586f77461c458f892") continue;
                List<Grid> list = value2.Properties?.Grids?.ToList();
                if (list == null || list.Count <= 0) continue;
                GridFilter gridFilter = list[0].Properties?.Filters?.FirstOrDefault();
                if (gridFilter != null)
                {
                    GridFilter gridFilter2 = gridFilter;
                    if (gridFilter2.Filter == null) { HashSet<MongoId> hashSet = (gridFilter2.Filter = new HashSet<MongoId>()); }
                    gridFilter.Filter.Add(item2);
                }
            }
        }
    }

    private void AddToInventorySlots()
    {
        TemplateItem templateItem = _itemsDb["55d7217a4bdc2d86028b456d"];
        foreach (Slot slot in templateItem.Properties.Slots)
        {
            if (slot.Name == "SecuredContainer") slot.Properties?.Filters?.First().Filter?.Add("68154651f849fb4e7d816738");
            if (slot.Name == "ArmBand") slot.Properties?.Filters?.First().Filter?.Add("6815465859b8c6ff13f94026");
        }
    }

    private void ConfigureCustomItemsToTraders()
    {
        _traderDb["5ac3b934156ae10c4430e83c"].Base.ItemsBuy?.Category.Add("6815465859b8c6ff13f94026");
        _traderDb["54cb57776803fa99248b456e"].Base.ItemsBuy?.Category.Add("680fd1dae5044e670a092e16");
        _traderDb["54cb57776803fa99248b456e"].Base.ItemsBuy?.Category.Add("68154651f849fb4e7d816738");
    }

    private void CreateCustomItemsAndTemplates()
    {
        _itemsDb["680fce2ec7b9b222270f074c"] = new TemplateItem { Id = "680fce2ec7b9b222270f074c", Name = "CustomContainerTemplate", Parent = "566162e44bdc2d3f298b4573", Type = "Node", Properties = new TemplateItemProperties() };
        _itemsDb["680fd1dae5044e670a092e16"] = new TemplateItem { Id = "680fd1dae5044e670a092e16", Name = "CustomContainerItem", Parent = "680fce2ec7b9b222270f074c", Type = "Node", Properties = new TemplateItemProperties() };
        _itemsDb["68154651f849fb4e7d816738"] = new TemplateItem { Id = "68154651f849fb4e7d816738", Name = "CustomSecureContainerItem", Parent = "680fce2ec7b9b222270f074c", Type = "Node", Properties = new TemplateItemProperties() };
        _itemsDb["6815465859b8c6ff13f94026"] = new TemplateItem { Id = "6815465859b8c6ff13f94026", Name = "CustomBeltItem", Parent = "680fce2ec7b9b222270f074c", Type = "Node", Properties = new TemplateItemProperties() };
    }
}
