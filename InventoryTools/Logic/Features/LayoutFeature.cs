using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class LayoutFeature : Feature
{
    public LayoutFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/layout", "布局",
            Paragraph("物品窗口和制作窗口可以用顶部标签页或侧边栏来显示清单。清单较多时侧边栏更合适，较少时标签页更合适。"),
            Setting<FiltersWindowLayoutSetting>("物品窗口"),
            Setting<CraftWindowLayoutSetting>("制作窗口")
        );
    }
}
