using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroInventoriesLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/inventories", "你的物品",
            Paragraph("只有游戏至少向你展示过一次的库存，插件才能看到。这是游戏发送数据方式的限制，不是你能打开的设置。"),
            Section("如果缺了点什么",
                Paragraph("在游戏中打开一次，之后就会被记住。最常见的有："),
                Bullet("雇员：与召唤铃互动并打开每个雇员的库存。"),
                Bullet("部队金库：打开你关心的每个标签页。"),
                Bullet("投影柜和武器柜。"),
                Bullet("陆行鸟鞍袋。")),
            Section("之后",
                Paragraph("插件会保留自己的副本，因此你在任何地方都能搜索雇员的内容，无需再跑回召唤铃。"))
        );
    }
}
