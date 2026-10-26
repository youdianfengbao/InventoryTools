using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class AboutHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/about", "关于",
            Paragraph("本插件是作者利用业余时间编写的，纯属兴趣驱动，希望能持续活跃更新一段时间。"),
            Paragraph("如遇到任何问题，请通过插件安装器的反馈按钮提交反馈。"),
            Section("链接",
                Link("打开 Wiki", "https://github.com/Critical-Impact/InventoryTools/wiki/1.-Overview"),
                Link("报告 Bug", "https://github.com/Critical-Impact/InventoryTools/issues"))
        );
    }
}
