using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroWelcomeLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/welcome", "概览",
            Paragraph("亚拉戈工具主要有三大功能。接下来几页会逐一简要介绍。只需一分钟，之后如果想深入了解，帮助窗口里有全部细节。"),
            Section("追踪你的库存",
                Paragraph("涵盖角色、雇员、部队、陆行鸟鞍袋和投影柜中的所有物品，一处即可搜索。")),
            Section("规划你的制作",
                Paragraph("将一项制作拆解为所有中间产物，查看你已拥有的部分，并找出剩余材料可购买或采集的位置。")),
            Section("查询资料",
                Paragraph("一个涵盖物品、怪物、副本、NPC、飞艇等的数据库，告诉你东西从哪来、有什么用。"))
        );
    }
}
