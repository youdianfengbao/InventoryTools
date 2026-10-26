using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroListsLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/lists", "清单",
            Paragraph("插件中几乎所有功能都围绕清单展开。清单是对你的物品保存下来的搜索，创建后即可在游戏中高亮其结果、整理或追踪它们。"),
            Section("三种类型",
                Bullet("搜索清单：在所有库存中查找物品。"),
                Bullet("整理清单：查找物品并算出它们应该移动到哪里。"),
                Bullet("游戏物品清单：搜索游戏中的所有物品，不只是你拥有的。")),
            Section("去看一看",
                Paragraph("物品窗口是清单的所在地。还有一些示例清单可以安装。"),
                OpenWindow<FiltersWindow>("打开物品窗口"))
        );
    }
}
