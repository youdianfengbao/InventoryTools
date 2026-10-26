using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class IgnoreHQFilter : BooleanFilter
{
    public override string Key { get; set; } = "IgnoreHQFilter";
    public override string Name { get; set; } = "忽略 HQ 筛选？";

    public override string HelpText { get; set; } =
        "排序时筛选器是否在尝试堆叠时将 HQ 与 NQ 物品视为相同？此筛选器的主要用途是找出可降低品质的物品。";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Advanced;
    public override FilterType AvailableIn { get; set; } = FilterType.SortingFilter;

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
    {
        configuration.IgnoreHQWhenSorting = newValue;
    }

    public override bool? CurrentValue(FilterConfiguration configuration)
    {
        return configuration.IgnoreHQWhenSorting;
    }

    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return true;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return true;
    }

    public IgnoreHQFilter(ILogger<IgnoreHQFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}