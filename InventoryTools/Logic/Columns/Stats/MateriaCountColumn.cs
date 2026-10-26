using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns.Stats;

public class MateriaCountColumn : IntegerColumn
{
    public MateriaCountColumn(ILogger<MateriaCountColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override string Name { get; set; } = "魔晶石数量";
    public override float Width { get; set; } = 90;
    public override string HelpText { get; set; } = "该物品已镶嵌或可镶嵌的魔晶石数量。";
    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Stats;
    public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.InventoryItem?.MateriaCount ?? searchResult.Item.Base.MateriaSlotCount;
    }

    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}