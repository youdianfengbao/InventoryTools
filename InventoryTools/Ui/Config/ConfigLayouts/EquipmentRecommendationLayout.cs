using InventoryTools.EquipmentSuggest;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class EquipmentRecommendationLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("equipment-recommendation", "装备推荐",
            Paragraph("将你当前穿戴的装备与可穿的装备进行对比并给出升级建议。以下是推荐界面打开时的默认设置。"),
            Section("默认值",
                Setting<EquipmentSuggestModeSetting>("推荐方式"),
                Setting<EquipmentSuggestViewModeSetting>("布局"))
        );
    }
}
