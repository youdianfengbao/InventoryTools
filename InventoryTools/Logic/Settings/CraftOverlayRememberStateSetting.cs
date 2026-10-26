using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayRememberStateSetting : GenericBooleanSetting
{
    public CraftOverlayRememberStateSetting(ILogger<CraftOverlayRememberStateSetting> logger,
        ImGuiService imGuiService) : base("CraftOverlayRememberState",
        "记住状态",
        "制作悬浮层是否在插件重载/游戏重启后保持打开？",
        true,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}