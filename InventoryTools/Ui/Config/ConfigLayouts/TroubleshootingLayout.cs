using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class TroubleshootingLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("troubleshooting", "故障排查",
            Paragraph("这些时长只有在你遇到特定问题时才需要调整。默认值适合绝大多数人。"),
            Section("获取追踪器",
                Paragraph("获取追踪器会记录你制作、采集和拾取的物品，让制作清单能自动勾销。两个数值单位均为秒：调大它们会在网络不佳或机器繁忙时，用响应速度换取可靠性。"),
                Setting<AcquisitionTrackerLoginDelaySetting>("登录后等待该时长再扫描"),
                Setting<AcquisitionTrackerPersistStateSetting>("停止后继续追踪该时长"))
        );
    }
}
