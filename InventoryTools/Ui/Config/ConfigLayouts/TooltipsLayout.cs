using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class TooltipsLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return PageGroup("tooltips", "提示",
            Page("tooltips/general", "常规",
                Paragraph("亚拉戈工具可以在游戏的物品提示中添加额外行。除非启用提示增强，否则本节中的所有功能均不会生效。"),
                Setting<ShowTooltipsSetting>("启用提示增强"),
                EnabledBy<ShowTooltipsSetting>(
                    Setting<ImGuiTooltipModeSetting>("在亚拉戈工具自己的窗口中显示"),
                    Setting<TooltipDisplayHeaderSetting>("用插件名称标注添加的行"),
                    Setting<TooltipCategoryWhitelistSetting>("仅限这些物品类别"),
                    Setting<TooltipCategoryBlacklistSetting>("将上述列表视为排除项"))),
            Page("tooltips/appearance", "外观",
                Paragraph("亚拉戈工具添加行的默认样式。每项功能都可以覆盖颜色。"),
                Setting<TooltipColorSetting>("默认文字颜色"),
                Setting<TooltipHeaderLinesSetting>("上方空行数"),
                Setting<TooltipFooterLinesSetting>("下方空行数")),
            Page("tooltips/locations", "物品位置",
                Paragraph("你已拥有此物品的位置及数量。"),
                Setting<TooltipDisplayAmountOwnedSetting>("显示我持有此物品的位置"),
                EnabledBy<TooltipDisplayAmountOwnedSetting>(
                    Setting<TooltipLocationScopeLimitSetting>("搜索这些位置"),
                    Setting<TooltipLocationDisplayModeSetting>("显示模式"),
                    Setting<TooltipAmountOwnedSortSetting>("排序"),
                    Setting<ToolTipLocationLimitSetting>("最大结果数"),
                    Setting<TooltipCurrentCharacterSetting>("仅限当前角色"),
                    Setting<TooltipAddCharacterNameSetting>("附加角色名称"),
                    Setting<TooltipAmountOwnedColorSetting>("文字颜色"))),
            Page("tooltips/retrieve", "待获取数量",
                Paragraph("当前清单（筛选清单或制作清单）还需要你获取多少个此物品。"),
                Setting<TooltipDisplayRetrieveAmountSetting>("显示待获取数量"),
                EnabledBy<TooltipDisplayRetrieveAmountSetting>(
                    Setting<TooltipAmountToRetrieveColorSetting>("文字颜色"))),
            Page("tooltips/market", "市场价格",
                Paragraph("来自 Universalis 的物品价格。两行可以单独显示。"),
                Setting<TooltipAverageMarketPriceSetting>("显示平均 NQ/HQ 价格"),
                Setting<TooltipMinimumMarketPriceSetting>("显示最低 NQ/HQ 价格"),
                Setting<TooltipMarketPricingColorSetting>("文字颜色")),
            Page("tooltips/unlock", "物品解锁状态",
                Paragraph("该物品是否已被学习，以及由谁学习。"),
                Setting<TooltipDisplayUnlockSetting>("显示解锁状态"),
                EnabledBy<TooltipDisplayUnlockSetting>(
                    Setting<TooltipDisplayUnlockCharacterSetting>("要检查的角色"),
                    Setting<TooltipDisplayUnlockDisplayModeSetting>("显示模式"),
                    Setting<TooltipDisplayUnlockHideUnlockedSetting>("隐藏已拥有的角色"),
                    Setting<TooltipItemUnlockStatusColorSetting>("文字颜色"))),
            Page("tooltips/coffer", "宝箱战利品",
                Paragraph("对于宝箱和容器，显示其中可能开出的物品。"),
                Setting<TooltipDisplayCofferLootSetting>("显示宝箱内容"),
                EnabledBy<TooltipDisplayCofferLootSetting>(
                    Setting<TooltipCofferLootScopeSetting>("搜索这些位置"),
                    Setting<TooltipCofferLootColorSetting>("文字颜色"))),
            Page("tooltips/curated", "精选清单",
                Paragraph("你的哪些精选清单中已包含此物品。"),
                Setting<TooltipDisplayCuratedListsSetting>("显示精选清单"),
                EnabledBy<TooltipDisplayCuratedListsSetting>(
                    Setting<TooltipCuratedListsSetting>("仅限这些清单"),
                    Setting<TooltipCuratedListsMatchQualitySetting>("匹配物品品质"),
                    Setting<TooltipCuratedListsColorSetting>("文字颜色"))),
            Page("tooltips/glamour", "套装投影",
                Paragraph("此物品是否为你正在收集的套装补齐了缺件。"),
                Setting<TooltipDisplayGlamourReadySetSetting>("显示套装投影信息"),
                EnabledBy<TooltipDisplayGlamourReadySetSetting>(
                    Setting<TooltipGlamourReadySetScopeSetting>("搜索这些位置"),
                    Setting<TooltipGlamourReadySetDisplayModeSetting>("显示模式"),
                    Setting<TooltipGlamourReadySetColorSetting>("文字颜色"),
                    Setting<TooltipGlamourReadySetAcquiredColorSetting>("已获取物品颜色"),
                    Setting<TooltipGlamourReadySetNotAcquiredColorSetting>("未获取物品颜色"))),
            Page("tooltips/patch", "素材版本",
                Paragraph("制作素材在哪个版本加入游戏。"),
                Setting<TooltipDisplayIngredientPatchSetting>("显示素材版本"),
                EnabledBy<TooltipDisplayIngredientPatchSetting>(
                    Setting<TooltipIngredientPatchTooltipColorSetting>("文字颜色"))),
            Page("tooltips/sources", "来源信息",
                Paragraph("该物品可以通过哪些途径获取。"),
                Setting<TooltipSourceInformationEnabledSetting>("显示来源信息"),
                EnabledBy<TooltipSourceInformationEnabledSetting>(
                    Setting<TooltipSourceInformationModifierSetting>("按住此键显示"),
                    Setting<TooltipSourceInformationColorSetting>("文字颜色"),
                    Scrollable("sourceInformation", 260,
                        Setting<TooltipSourceInformationSetting>()))),
            Page("tooltips/uses", "用途信息",
                Paragraph("该物品可以加工成什么或用在何处。"),
                Setting<TooltipUseInformationEnabledSetting>("显示用途信息"),
                EnabledBy<TooltipUseInformationEnabledSetting>(
                    Setting<TooltipUseInformationModifierSetting>("按住此键显示"),
                    Setting<TooltipUseInformationColorSetting>("文字颜色"),
                    Scrollable("useInformation", 260,
                        Setting<TooltipUseInformationSetting>())))
        );
    }
}
