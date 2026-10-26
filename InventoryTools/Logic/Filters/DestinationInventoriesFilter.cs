using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class DestinationInventoriesFilter : InventoryScopeFilter
    {
        public DestinationInventoriesFilter(ILogger<DestinationInventoriesFilter> logger, InventoryScopePicker scopePicker, ImGuiService imGuiService) : base(scopePicker, logger, imGuiService)
        {
        }
        public override int LabelSize { get; set; } = 240;
        public override string Key { get; set; } = "DestinationInventories";
        public override string Name { get; set; } = "目标库存";
        public override string HelpText { get; set; } =
            "定义作为目标的库存，插件会尝试将“来源库存”中找到的物品整理到“目标库存”。可在下方范围配置中查看已找到的库存。";
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;
        public override List<InventorySearchScope>? DefaultValue { get; set; } = null;
        public override FilterType AvailableIn { get; set; } = FilterType.SortingFilter;

        public override List<InventorySearchScope>? GenerateDefaultScope()
        {
            return null;
        }
    }
}