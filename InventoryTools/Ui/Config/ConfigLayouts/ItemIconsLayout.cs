using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class ItemIconsLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("item-icons", "物品图标",
            Paragraph("当一件物品可以通过多种相似方式获取或使用时，这些图标可以折叠为单个分组图标。下方每一类来源和用途都可以覆盖此默认值。"),
            Section("来源",
                Scrollable("sourceIconGrouping", 320,
                    Setting<SourceIconGroupingSetting>())),
            Section("用途",
                Scrollable("useIconGrouping", 200,
                    Setting<UseIconGroupingSetting>()))
        );
    }
}
