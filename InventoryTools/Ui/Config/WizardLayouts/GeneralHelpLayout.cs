using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class GeneralHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/general", "常规",
            Paragraph("亚拉戈工具是一款多功能插件，提供 3 项主要功能：追踪并展示你的库存数据、协助规划制作、以及查询物品信息。"),
            Paragraph("如果你用过 Teamcraft 或 Garland Tools，会发现它从两者都有借鉴。"),
            Section("库存追踪",
                Paragraph("插件会尽力追踪你的库存。有些库存只在首次被访问时才会缓存。如果你看不到雇员、部队、投影柜等的内容，请先在游戏中打开它们，否则插件无法缓存。"),
                Paragraph("插件记录物品后，你可以创建清单来缩小特定物品的搜索范围、帮你整理物品，还有诸多其他用途。")),
            Section("制作规划",
                Paragraph("插件有专门的制作窗口，可以创建想要制作的物品清单。它会生成一份计划，将每件物品拆解为各个组成部分，告诉你还缺什么、所需材料在哪、以及去哪里获取或购买缺少的东西。"),
                Paragraph("如果你用过 Teamcraft，会很快上手。")),
            Section("物品信息",
                Paragraph("插件拥有相当全面的物品信息数据库。如果你用过 Garland Tools，会发现提供的信息非常相似。在插件内点击物品图标，总会打开该物品的信息窗口。")),
            Section("高亮",
                Paragraph("使用物品清单或制作清单时，可以开关高亮。它会在游戏中高亮物品，让你准确看到它们的位置。当插件窗口处于活动状态时，勾选「高亮」复选框即可为该清单启用高亮。若想用宏触发，请参阅帮助中的命令部分，并切换「背景」高亮。")),
            Section("更多",
                Paragraph("这只是一份非常基础的指南。更多信息请参阅 Wiki。"),
                Link("打开 Wiki", "https://github.com/Critical-Impact/InventoryTools/wiki/1.-Overview"))
        );
    }
}
