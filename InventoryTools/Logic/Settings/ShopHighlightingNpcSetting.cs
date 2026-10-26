using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingNpcSetting : GenericBooleanSetting
{
    public ShopHighlightingNpcSetting(ILogger<ShopHighlightingNpcSetting> logger, ImGuiService imGuiService) : base("ShopHighlightingNpc", "商店高亮 - 高亮 NPC", "在商店中高亮物品时，是否在场景中高亮相关 NPC？", true, "15.0.4", logger, imGuiService)
    {
    }
}
