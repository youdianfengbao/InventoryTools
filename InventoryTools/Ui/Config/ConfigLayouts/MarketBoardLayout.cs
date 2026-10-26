using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class MarketBoardLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("marketboard", "市场板",
            Paragraph("市场价格来自 Universalis 而非游戏本身，因此需要抓取并缓存，而不是实时读取。"),
            Section("计价服务器",
                Paragraph("这些选项合并为一个列表，下方额外的服务器会叠加到你的主服务器和当前服务器上，而非替换它们。"),
                Setting<MarketBoardUseHomeWorldSetting>("主世界"),
                Setting<MarketBoardUseActiveWorldSetting>("当前世界"),
                Setting<MarketBoardExtraWorldsSetting>("额外世界")),
            Section("下载",
            Paragraph("是否自动下载价格数据？若不启用，则需点击「刷新市场价格」按钮来下载物品价格。"),
                Setting<AutomaticallyDownloadPricesSetting>("自动下载价格"),
                Setting<MarketRefreshTimeHoursSetting>("价格保留时长（小时）"),
                Setting<MarketBoardSaleCountLimitSetting>("成交历史范围（天）"))
        );
    }
}
