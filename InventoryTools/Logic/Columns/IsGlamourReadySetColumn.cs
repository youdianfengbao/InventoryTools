using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns;

public class IsGlamourReadySetColumn : CheckboxColumn
{
    public IsGlamourReadySetColumn(ILogger<IsGlamourReadySetColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.HasUsesByType(ItemInfoType.GlamourReadySet);
    }

    public override string Name { get; set; } = "成套投影套装？";
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = "该物品是否为成套投影（套装组合形态）？";
}