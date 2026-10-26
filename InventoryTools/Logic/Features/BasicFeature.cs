using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class BasicFeature : Feature
{
    public BasicFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/basic", "基础设置",
            Paragraph("以下设置对整个插件生效，之后随时可以在设置窗口中修改。"),
            Setting<AutoSaveSetting>("自动保存库存"),
            Setting<HistoryEnabledSetting>("启用物品历史记录"),
            Setting<AddTitleMenuButtonSetting>("在游戏主菜单中添加按钮")
        );
    }
}