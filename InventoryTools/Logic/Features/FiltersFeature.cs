using System.Collections.Generic;
using System.Linq;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class FiltersFeature : Feature
{
    public FiltersFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/sample-lists", "示例清单",
            Paragraph("这些清单展示了插件的部分功能。勾选后，完成向导时会自动安装。"),
            Setting<SampleFilter100GillOrLess>("价值不超过 100 金币的物品"),
            Setting<SampleFilterDuplicateItems>("库存中的重复物品"),
            Setting<SampleFilterMaterialCleanup>("需移入仓库的制作材料")
        );
    }

    public override void OnFinish()
    {
        foreach (var setting in RelatedSettings.Select(c => c as ISampleFilter))
        {
            if (setting is { ShouldAdd: true })
            {
                setting.AddFilter();
            }
        }
    }
}