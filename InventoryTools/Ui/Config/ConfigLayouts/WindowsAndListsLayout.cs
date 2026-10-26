using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class WindowsAndListsLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("windows-lists", "窗口与清单",
            Section("布局",
                Paragraph("控制各个窗口的布局方式。"),
                Setting<CraftWindowLayoutSetting>("制作窗口"),
                Setting<FiltersWindowLayoutSetting>("物品窗口"),
                Setting<ShowFiltersTabSetting>(),
                Setting<CompendiumRowHeightSetting>("插件列表行高")),
            Section("自动切换",
                Paragraph("在界面中切换清单时，如果高亮已开启，是否自动将高亮切换到该清单？"),
                Setting<SwitchFiltersAutomaticallySetting>(),
                Setting<SwitchCraftListsAutomaticallySetting>()),
            Section("忽略 Esc 键",
                Paragraph("按下 Esc 键时保持打开的窗口。"),
                Setting<CraftWindowIgnoreEscapeSetting>("制作窗口"),
                Setting<FiltersWindowIgnoreEscapeSetting>("物品窗口"),
                Setting<ItemWindowIgnoreEscapeSetting>("物品详情窗口"),
                Setting<FilterWindowIgnoreEscapeSetting>("清单窗口"))
        );
    }
}
