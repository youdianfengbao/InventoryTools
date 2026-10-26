using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Services;
using ImGuiService = InventoryTools.Services.ImGuiService;

namespace InventoryTools.EquipmentSuggest;

public class EquipmentSuggestFilterStatsField : BooleanFormField<EquipmentSuggestConfig>
{
    public EquipmentSuggestFilterStatsField(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override string Key { get; set; } = "FilterStats";
    public override string Name { get; set; } = "按属性筛选";

    public override string HelpText { get; set; } =
        "是否按属性筛选装备：选择能工巧匠职业时仅显示带有制作相关属性的装备，以此类推";
    public override string Version { get; set; } = "1.12.0.10";
}