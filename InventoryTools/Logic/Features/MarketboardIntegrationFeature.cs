using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class MarketboardIntegrationFeature : Feature
{
    public MarketboardIntegrationFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/marketboard", "市场板",
            Paragraph("价格来自 Universalis 而非游戏本体。插件会下载价格并保存在本地。开启后自动下载价格；若关闭，则需要手动点击“刷新市场价格”才能更新物品价格。"),
            Setting<AutomaticallyDownloadPricesSetting>("自动下载价格"),
            Setting<MarketRefreshTimeHoursSetting>("价格保留时长（小时）"),
            Setting<MarketBoardSaleCountLimitSetting>("纳入统计的出售历史天数")
        );
    }
}
