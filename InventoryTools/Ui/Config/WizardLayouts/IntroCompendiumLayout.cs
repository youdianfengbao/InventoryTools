using InventoryTools.Compendium.Windows;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroCompendiumLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/compendium", "图鉴",
            Paragraph("图鉴是游戏内容的可浏览参考资料，基于插件其他地方使用的同一份数据构建。想知道某个东西是什么、从哪来、由什么掉落，就在这里查。"),
            Section("收录内容",
                Bullet("物品、任务和成就。"),
                Bullet("坐骑、宠物和投影套装。"),
                Bullet("副本、理符、蛮族和定制交货。"),
                Bullet("魔器武器和工具、大师配方书、天书和灵魂水晶。"),
                Bullet("NPC、地图区域、飞艇和潜艇航线。"),
                Bullet("职业、装备方案、陆行鸟物品等等。")),
            Section("去看一看",
                Paragraph("每个条目都链接到相关的一切，你可以顺着一条链从物品找到掉落它的副本，再找到出售剩余材料的 NPC。"),
                OpenWindow<CompendiumTypesWindow>("打开图鉴"),
                Paragraph("它也位于物品窗口和制作窗口的图鉴菜单下，或使用 /compendium 命令。"))
        );
    }
}
