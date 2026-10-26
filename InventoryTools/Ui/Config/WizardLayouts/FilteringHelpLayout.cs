using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class FilteringHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/filtering", "筛选",
            Section("高级搜索语法",
                Paragraph("创建清单或搜索清单结果时，可以使用一系列运算符让搜索更精确。可用运算符取决于搜索的对象类型。"),
                Bullet("!  - 排除包含所输入内容的条目 - 适用于文本和数字。"),
                Bullet("<  - 值小于所输入内容的条目 - 适用于数字。"),
                Bullet(">  - 值大于所输入内容的条目 - 适用于数字。"),
                Bullet(">= - 值大于或等于所输入内容的条目 - 适用于数字。"),
                Bullet("<= - 值小于或等于所输入内容的条目 - 适用于数字。"),
                Bullet("=  - 值恰好等于所输入内容的条目 - 适用于文本和数字。"),
                Bullet("&& 和 || - 分别表示“与”和“或”，用于串联多个运算符。"))
        );
    }
}
