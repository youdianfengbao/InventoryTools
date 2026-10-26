using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class HotkeysFeature : Feature
{
    public HotkeysFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/hotkeys", "快捷键",
            Paragraph("这些快捷键均为可选。留空的字段表示该操作不绑定快捷键。"),
            Section("显示或隐藏窗口",
                Setting<HotKeyListsWindowSetting>("清单"),
                Setting<HotkeyCraftWindowSetting>("制作"),
                Setting<HotkeyConfigWindowSetting>("配置"),
                Setting<HotkeyMobWindowSetting>("怪物"),
                Setting<HotkeyDutiesWindowSetting>("副本"),
                Setting<HotkeyAirshipWindowSetting>("飞艇"),
                Setting<HotkeySubmarinesWindowSetting>("潜水艇"),
                Setting<HotkeyRetainerTasksWindowSetting>("雇员探索")),
            Section("光标悬停在物品上时",
                Paragraph("这些快捷键作用于光标下的物品。若光标下没有物品，则不会触发任何操作。"),
                Setting<HotkeyMoreInfoSetting>("更多信息"),
                Setting<HotkeyOpenItemLogSetting>("与该物品对应的日志"),
                Setting<HotkeyOpenCraftingLogSetting>("制作日志"),
                Setting<HotkeyOpenGatheringLogSetting>("采集日志"),
                Setting<HotkeyOpenFishingLogSetting>("钓鱼日志"))
        );
    }
}
