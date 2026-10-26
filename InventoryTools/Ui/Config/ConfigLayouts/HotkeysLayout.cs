using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class HotkeysLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("hotkeys", "快捷键",
            Section("切换窗口",
                Paragraph("在任何地方都有效，无论鼠标是否悬停在对象上。"),
                Setting<HotKeyListsWindowSetting>("清单"),
                Setting<HotkeyCraftWindowSetting>("制作"),
                Setting<HotkeyConfigWindowSetting>("配置"),
                Setting<HotkeyMobWindowSetting>("怪物"),
                Setting<HotkeyDutiesWindowSetting>("副本"),
                Setting<HotkeyAirshipWindowSetting>("飞艇"),
                Setting<HotkeySubmarinesWindowSetting>("潜艇"),
                Setting<HotkeyRetainerTasksWindowSetting>("雇员探索")),
            Section("悬停物品时",
                Paragraph("这些快捷键作用于光标下的物品，因此只在悬停物品时才会生效。"),
                Setting<HotkeyMoreInfoSetting>("更多信息"),
                Setting<HotkeyOpenItemLogSetting>("打开对应的日志"),
                Setting<HotkeyOpenCraftingLogSetting>("制作手册"),
                Setting<HotkeyOpenGatheringLogSetting>("采集手册"),
                Setting<HotkeyOpenFishingLogSetting>("钓鱼手册"))
        );
    }
}
