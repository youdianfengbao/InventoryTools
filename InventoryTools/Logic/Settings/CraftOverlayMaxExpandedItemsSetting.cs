using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayMaxExpandedItemsSetting : GenericIntegerSetting
{
    public CraftOverlayMaxExpandedItemsSetting(ILogger<CraftOverlayMaxExpandedItemsSetting> logger,
        ImGuiService imGuiService) : base("CraftOverlayMaxItems",
        "展开时最大物品数",
        "制作悬浮层展开时，应显示多少个物品？",
        5,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}