using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipAmountOwnedSortSetting : ChoiceSetting<TooltipAmountOwnedSort>
{
    public TooltipAmountOwnedSortSetting(ILogger<TooltipAmountOwnedSortSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override TooltipAmountOwnedSort DefaultValue { get; set; } = TooltipAmountOwnedSort.Alphabetically;
    public override TooltipAmountOwnedSort CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipAmountOwnedSort;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, TooltipAmountOwnedSort newValue)
    {
        configuration.TooltipAmountOwnedSort = newValue;
    }

    public override string Key { get; set; } = "TooltipAmountOwnedSort";
    public override string Name { get; set; } = "添加物品位置（排序）";

    public override string HelpText { get; set; } =
        "提示中你持有的物品应如何排序？若排序影响性能可选“无排序”。";

    public override string Version { get; } = "1.7.0.17";

    public override Dictionary<TooltipAmountOwnedSort, string> Choices { get; } =
        new Dictionary<TooltipAmountOwnedSort, string>()
        {
            { TooltipAmountOwnedSort.Alphabetically, "按字母顺序（角色/雇员等）" },
            { TooltipAmountOwnedSort.Categorically, "按字母顺序（分类）" },
            { TooltipAmountOwnedSort.Quantity, "按物品数量" },
            { TooltipAmountOwnedSort.None, "无排序" },
        };
}

public enum TooltipAmountOwnedSort
{
    Alphabetically,
    Categorically,
    Quantity,
    None
}