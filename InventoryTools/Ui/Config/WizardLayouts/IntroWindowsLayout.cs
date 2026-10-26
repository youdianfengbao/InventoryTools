using InventoryTools.EquipmentSuggest;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroWindowsLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("intro/windows", "其他工具",
            Paragraph("清单、制作和图鉴之外还有几个计算器。两者都位于任意亚拉戈工具窗口底部的菜单按钮中。"),
            Section("陆行鸟颜色计算器",
                Paragraph("选择你想要的陆行鸟颜色，它会算出该喂哪种果子、按什么顺序。锁定顺序后它会追踪进度，逐一勾销已喂的果子。"),
                OpenWindow<ChocoboColourWindow>("打开陆行鸟颜色计算器")),
            Section("装备推荐",
                Paragraph("选择一个职业，它会检视你在任何角色和雇员处拥有的全部装备，找出优于当前穿戴的部件，并在物品品级与该职业真正看重的属性之间权衡。"),
                OpenWindow<EquipmentSuggestWindow>("打开装备推荐")),
            Section("两个小贴士",
                Paragraph("右键点击物品或表格行，几乎总能发现超出预期的选项；在插件任意位置点击物品图标，则会打开它的完整信息页。"))
        );
    }
}
