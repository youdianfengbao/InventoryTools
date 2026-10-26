using System;
using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.EquipmentSuggest;

public enum EquipmentSuggestViewMode
{
    Expanded,
    Normal,
    Compact
}

public class EquipmentSuggestViewModeSetting : GenericEnumChoiceSetting<EquipmentSuggestViewMode>
{
    public EquipmentSuggestViewModeSetting(ILogger<EquipmentSuggestViewModeSetting> logger, ImGuiService imGuiService) : base("EquipSuggestViewMode", "视图模式", "装备推荐界面应使用紧凑还是普通视图？", EquipmentSuggestViewMode.Normal, new(){{EquipmentSuggestViewMode.Normal, "普通"}, {EquipmentSuggestViewMode.Expanded, "展开"}, {EquipmentSuggestViewMode.Compact, "紧凑"}}, "12.0.10", logger, imGuiService)
    {
    }

    public int GetIconContainerSize(InventoryToolsConfiguration configuration)
    {
        switch (CurrentValue(configuration))
        {
            case EquipmentSuggestViewMode.Expanded:
                return 64;
            case EquipmentSuggestViewMode.Normal:
                return 32;
            case EquipmentSuggestViewMode.Compact:
                return 16;
        }

        return 32;
    }

    public int GetIconSize(InventoryToolsConfiguration configuration)
    {
        switch (CurrentValue(configuration))
        {
            case EquipmentSuggestViewMode.Expanded:
                return 32;
            case EquipmentSuggestViewMode.Normal:
                return 32;
            case EquipmentSuggestViewMode.Compact:
                return 16;
        }

        return 32;
    }
}