using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class CraftDefaultHQRequiredFilter : BooleanFilter
{
    public override bool? DefaultValue { get; set; } = false;
    public override string Key { get; set; } = "CraftDefaultHqRequired";
    public override string Name { get; set; } = "需要 HQ";
    public override string HelpText { get; set; } = "清单中的每个物品是否都需要 HQ 版本（如适用）？";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Settings;
    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return null;
    }

    public override string[] GetChoices()
    {
        return ["是", "否"];
    }

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
    {
        configuration.CraftList.HQRequired = newValue ?? false;
        configuration.NotifyConfigurationChange();
    }

    public override bool? CurrentValue(FilterConfiguration configuration)
    {
        return configuration.CraftList.HQRequired;
    }

    public CraftDefaultHQRequiredFilter(ILogger<CraftDefaultHQRequiredFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}