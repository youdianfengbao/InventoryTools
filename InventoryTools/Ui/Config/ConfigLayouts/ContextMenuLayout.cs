using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class ContextMenuLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("context-menu", "右键菜单",
            Paragraph("亚拉戈工具添加到游戏自带右键菜单中的条目。每一条都会占用你常用菜单中的一行，因此均可单独开关。"),
            Section("清单",
                Setting<ContextMenuAddToCraftListSetting>("添加到制作清单"),
                Setting<ContextMenuAddToActiveCraftListSetting>("添加到当前制作清单"),
                Setting<ContextMenuAddToCuratedListSetting>("添加到精选清单"),
                Setting<ContextMenuAddToFavouritesSetting>("添加或移出收藏")),
            Section("更多信息",
                Paragraph("打开亚拉戈工具的信息窗口，查看右键点击的对象。"),
                Setting<ContextMenuMoreInformationSetting>("物品"),
                Setting<ContextMenuMoreInformationNpcsSetting>("NPC"),
                Setting<ContextMenuMoreInformationMonstersSetting>("怪物")),
            Section("在游戏中打开",
                Paragraph("打开游戏内各类日志的快捷方式。"),
                Setting<ContextMenuOpenCraftingLogSetting>("制作手册"),
                Setting<ContextMenuOpenGatheringLogSetting>("采集手册"),
                Setting<ContextMenuOpenFishingLogSetting>("钓鱼手册")),
            Section("搜索",
                Paragraph("这些搜索位置同样适用于从亚拉戈工具自身菜单发起的搜索，而不仅仅是这个右键菜单条目。"),
                Paragraph("此搜索比游戏默认的搜索功能范围更广，因为它会跨插件已知的所有角色进行搜索。"),
                Setting<ContextMenuItemSearchSetting>("搜索此物品"),
                Paragraph("若想限制搜索的位置，请在下方配置搜索范围。"),
                Setting<ContextMenuItemSearchScopeSetting>("搜索这些位置")),
            Section("其他",
                Setting<ContextMenuCopyNameSetting>("复制物品名称"))
        );
    }
}
