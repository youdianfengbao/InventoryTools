using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayUnlockHideUnlockedSetting : GenericBooleanSetting
{
    public TooltipDisplayUnlockHideUnlockedSetting(ILogger<TooltipDisplayUnlockHideUnlockedSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayUnlockHideUnlocked", "添加物品解锁状态（隐藏已解锁角色）", "是否隐藏已解锁该物品的角色？分组模式下将隐藏“已获得”分组。", false, "1.11.1.1", logger, imGuiService)
    {
    }
}