using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class TooltipsFeature : Feature
{
    public TooltipsFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/tooltips", "物品提示",
            Paragraph("Allagan Tools 可以在游戏的物品提示中添加额外信息行。"),
            Setting<TooltipDisplayAmountOwnedSetting>("你在何处持有该物品"),
            Setting<TooltipDisplayRetrieveAmountSetting>("当前制作清单还需要多少"),
            Setting<TooltipMinimumMarketPriceSetting>("市场价格"),
            Setting<TooltipDisplayUnlockSetting>("是否已学习/解锁该物品"),
            Setting<TooltipSourceInformationEnabledSetting>("物品获取途径"),
            Setting<TooltipUseInformationEnabledSetting>("物品用途"),
            Setting<TooltipDisplayIngredientPatchSetting>("材料来自哪个版本"),
            Setting<TooltipDisplayCofferLootSetting>("宝箱可能开出的物品"),
            Setting<TooltipDisplayCuratedListsSetting>("物品是否在精选清单中"),
            Setting<TooltipDisplayGlamourReadySetSetting>("物品是否可完成一套投影"),
            Paragraph("每项都有更多选项，位于设置窗口的“物品提示”下，包括颜色、搜索位置和显示模式。")
        );
    }
}
