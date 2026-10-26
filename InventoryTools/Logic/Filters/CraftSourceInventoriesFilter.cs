using System.Collections.Generic;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class CraftSourceInventoriesFilter : InventoryScopeFilter
{
    public CraftSourceInventoriesFilter(InventoryScopePicker scopePicker, ILogger<CraftSourceInventoriesFilter> logger, ImGuiService imGuiService) : base(scopePicker, logger, imGuiService)
    {
    }

    public override string Key { get; set; } = "CraftSourceInventories";
    public override string Name { get; set; } = "取出来源库存";

    public override string HelpText { get; set; } =
        "制作清单应检查哪些库存以取出材料？所选库存中找到的物品会出现在“雇员/背包物品”列表中，需根据清单配置在采集前或采集后取出。";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;

    public override List<InventorySearchScope>? DefaultValue { get; set; } = null;

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override List<InventorySearchScope>? GenerateDefaultScope()
    {
        return new List<InventorySearchScope>()
        {
            new InventorySearchScope() { ActiveCharacter = true, Categories = [InventoryCategory.RetainerBags, InventoryCategory.FreeCompanyBags, InventoryCategory.CharacterSaddleBags, InventoryCategory.CharacterPremiumSaddleBags] }
        };
    }

    public override int Order { get; set; } = -3;
}