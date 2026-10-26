using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class CraftOverlayLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("craft-overlay", "制作悬浮层",
            Paragraph("一个跟随你当前制作清单的紧凑面板，在游戏中即可看到还缺什么，无需打开窗口。"),
            Section("显示",
                Setting<CraftOverlayMaxExpandedItemsSetting>("展开时显示的物品数"),
                Setting<CraftOverlayHideSetting>("在副本和过场动画中隐藏")),
            Section("行为",
                Setting<CraftOverlayRememberStateSetting>("重载后保持展开状态"))
        );
    }
}
