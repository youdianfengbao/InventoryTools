using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class HighlightingLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("highlighting", "高亮",
            Paragraph("当清单匹配到物品时，高亮功能会为库存格子和背包标签着色。以下为默认设置，各个清单可以覆盖其中大部分选项。"),
            Section("启用中的清单",
                Paragraph("高亮通常通过各窗口中的「高亮」按钮和斜杠命令来控制，也可以在这里开关高亮。"),
                Setting<WindowFilterSetting>(),
                Setting<BackgroundFilterSetting>(),
                Setting<SaveBackgroundFilterSetting>("跨会话记住该选择")
            ),
            Section("何时高亮",
                Setting<HighlightWhenSetting>(),
                Setting<InvertHighlightingSetting>(),
                Setting<InvertTabHighlightingSetting>()),
            Section("颜色",
                Setting<HighlightColourSetting>("匹配物品颜色"),
                Setting<TabHighlightColourSetting>("匹配标签颜色")),
            Section("去向",
                Paragraph("整理清单知道物品最终要放到哪里。可以同时高亮目标背包和来源背包，让一次移动的两端都一目了然。"),
                Setting<HighlightDestinationSetting>(),
                EnabledBy<HighlightDestinationSetting>(
                    Setting<HighlightDestinationEmptySetting>(),
                    Setting<InvertDestinationHighlightingSetting>(),
                    Setting<HighlightDestinationColourSetting>("去向颜色"))),
            Section("雇员列表",
                Paragraph("当雇员持有你的清单所需的物品时，可以对召唤铃列表进行标注，无需逐个打开即可知道该查看哪个雇员。"),
                Setting<ColourRetainerListSetting>("为雇员名称着色"),
                EnabledBy<ColourRetainerListSetting>(
                    Setting<RetainerListColourSetting>("名称颜色")),
                Setting<ShowItemNumberRetainerListSetting>("显示物品数量")),
            Section("商店高亮",
                Paragraph("打开商人窗口时，被清单匹配到的商店物品可以高亮显示，售卖这些物品的商人也会在世界中被标记出来。"),
                Setting<ShopHighlightingDisableItemsSetting>("调暗不匹配的物品"),
                Setting<ShopHighlightingNpcSetting>("在世界中高亮商人 NPC"),
                EnabledBy<ShopHighlightingNpcSetting>(
                    Setting<ShopHighlightingNpcColorSetting>("高亮颜色"),
                    Setting<ShopHighlightingNpcNameplateIconSetting>("在其名牌上显示图标")))
        );
    }
}
