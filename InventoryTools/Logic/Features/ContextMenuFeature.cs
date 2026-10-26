using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class ContextMenuFeature : Feature
{
    public ContextMenuFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/context-menu", "右键菜单",
            Paragraph("插件可以将以下条目添加到物品的右键菜单中。"),
            Section("更多信息",
                Setting<ContextMenuMoreInformationSetting>("物品"),
                Setting<ContextMenuMoreInformationNpcsSetting>("NPC"),
                Setting<ContextMenuMoreInformationMonstersSetting>("怪物")),
            Section("打开游戏日志",
                Setting<ContextMenuOpenCraftingLogSetting>("制作日志"),
                Setting<ContextMenuOpenGatheringLogSetting>("采集日志"),
                Setting<ContextMenuOpenFishingLogSetting>("钓鱼日志")),
            Section("清单",
                Setting<ContextMenuAddToCraftListSetting>("添加到制作清单"),
                Setting<ContextMenuAddToActiveCraftListSetting>("添加到当前制作清单"),
                Setting<ContextMenuAddToCuratedListSetting>("添加到精选清单"),
                Setting<ContextMenuAddToFavouritesSetting>("添加/移出收藏")),
            Section("其他",
                Setting<ContextMenuCopyNameSetting>("复制物品名"))
        );
    }
}
