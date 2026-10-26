using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns.Stats;

public class DyeCountColumn : IntegerColumn
{
    public DyeCountColumn(ILogger<DyeCountColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override string Name { get; set; } = "染剂数量";
    public override float Width { get; set; } = 90;
    public override string HelpText { get; set; } = "物品拥有或支持的染料数量。";
    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Stats;
    public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.InventoryItem?.DyeCount ?? searchResult.Item.Base.DyeCount;
    }

    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}