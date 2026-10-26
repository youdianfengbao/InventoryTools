using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class HistoryLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("history", "历史记录",
            Paragraph("记录物品进出及在你的库存之间移动的情况，方便回顾发生了哪些变化。历史类清单会从这份记录中读取数据。"),
            Paragraph("不启用此项，历史类清单将无法工作。"),
            Section("追踪",
                Setting<HistoryEnabledSetting>("追踪库存变动"),
                EnabledBy<HistoryEnabledSetting>(
                    Setting<HistoryTrackEventsSetting>("值得记录的事件")))
        );
    }
}
