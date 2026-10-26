using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.WizardSettings;

public class CraftNotificationsReportToChatSetting : GenericBooleanSetting
{
    public override bool AppearsInConfigWindow => false;

    private bool _shouldAdd;

    public CraftNotificationsReportToChatSetting(ILogger<CraftNotificationsReportToChatSetting> logger,
        ImGuiService imGuiService) : base("CraftNotificationsReportToChat",
        "在聊天中报告获取进度",
        "获取制作清单所需物品时，在聊天中输出进度。通知仅在制作清单处于启用状态时出现。",
        true,
        "15.0.8",
        logger,
        imGuiService)
    {
    }

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return _shouldAdd;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        _shouldAdd = newValue;
    }
}