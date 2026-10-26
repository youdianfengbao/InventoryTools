using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroDefaultsLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/defaults", "默认设置",
            Paragraph("默认情况下，插件已启用一套默认功能。"),
            Paragraph("接下来的几页会展示最常用功能的设置。"),
            Paragraph("将鼠标悬停在 ? 图标上，可查看每个设置项的详细说明。")
        );
    }
}
