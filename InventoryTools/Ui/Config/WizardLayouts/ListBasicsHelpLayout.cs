using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class ListBasicsHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/list-basics", "清单基础",
            Paragraph("清单是插件查看你要找的物品或整理物品的核心方式。目前可以创建 3 种类型的清单。"),
            Section("搜索清单",
                Paragraph("在所有库存中搜索特定物品。如果你只需要找到某个物品、不需要整理帮助，这就是你要的清单类型。"),
                Paragraph("示例用途："),
                Bullet("寻找制作所需的材料。"),
                Bullet("找出你随手放置的房屋家具。"),
                Bullet("查看刚捡到的物品值多少钱。"),
                Bullet("查看某件物品是否已在投影柜或武器柜中。"),
                Bullet("不去召唤铃也能查看雇员的装备。"),
                Bullet("检查你持有的物品中哪些可以存入武器柜。")),
            Section("整理清单",
                Paragraph("在搜索清单的基础上，还可以指定物品要整理到哪里。它会尝试为你生成最优的存放计划，把物品放进你选定的目标位置。"),
                Paragraph("示例用途："),
                Bullet("制作完成后收纳材料，避免重复堆叠。"),
                Bullet("把超过一定品级的物品存进陆行鸟鞍袋备用。"),
                Bullet("找出部队金库中独有的物品并放进去。")),
            Section("游戏物品清单",
                Paragraph("在游戏物品目录的全部物品中进行搜索。"),
                Paragraph("示例用途："),
                Bullet("搜索投影。"),
                Bullet("查看还有哪些坐骑和宠物未获得。"),
                Bullet("追踪游戏内所有物品的价格。"))
        );
    }
}
