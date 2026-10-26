using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.WizardSettings;

public class CraftNotificationsPlaySoundSetting : GenericBooleanSetting
{
    public override bool AppearsInConfigWindow => false;

    private bool _shouldAdd;

    public CraftNotificationsPlaySoundSetting(ILogger<CraftNotificationsPlaySoundSetting> logger,
        ImGuiService imGuiService) : base("CraftNotificationsPlaySound",
        "物品齐备时播放提示音",
        "制作清单中的物品达到所需数量时播放音效。通知仅在制作清单处于启用状态时出现。",
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