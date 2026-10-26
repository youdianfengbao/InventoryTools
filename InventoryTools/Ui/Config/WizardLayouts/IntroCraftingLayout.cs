using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroCraftingLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/crafting", "制作",
            Paragraph("制作清单会把你想要制作的东西拆解为所有中间产物和原材料，再与你已拥有的部分进行核对。"),
            Section("你能得到什么",
                Bullet("完整的材料树，而不仅仅是直接素材。"),
                Bullet("你已拥有的数量，以及由哪个角色或雇员持有。"),
                Bullet("缺少部分的购买或采集地点。"),
                Bullet("随着你获取材料，进度会不断勾销。")),
            Section("去看一看",
                OpenWindow<CraftsWindow>("打开制作窗口"))
        );
    }
}
