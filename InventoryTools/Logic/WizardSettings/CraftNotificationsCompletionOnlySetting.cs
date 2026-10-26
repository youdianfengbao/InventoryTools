using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.WizardSettings;

public class CraftNotificationsCompletionOnlySetting : GenericBooleanSetting
{
    public override bool AppearsInConfigWindow => false;

    private bool _shouldAdd;

    public CraftNotificationsCompletionOnlySetting(ILogger<CraftNotificationsCompletionOnlySetting> logger,
        ImGuiService imGuiService) : base("CraftNotificationsCompletionOnly",
        "仅物品齐备时报告",
        "不逐次报告每次获取，只在物品达到所需数量时输出一条消息。通知仅在制作清单处于启用状态时出现。",
        false,
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