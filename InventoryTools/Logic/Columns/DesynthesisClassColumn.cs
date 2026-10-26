using AllaganLib.GameSheets.Extensions;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns;

public class DesynthesisClassColumn : TextColumn
{
    public DesynthesisClassColumn(ILogger<DesynthesisClassColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Desynthesis;
    public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (!searchResult.Item.CanBeDesynthed || searchResult.Item.Base.ClassJobRepair.RowId == 0)
        {
            return null;
        }

        return searchResult.Item.Base.ClassJobRepair.ValueNullable?.Name.ToString().ToTitleCase() ?? "未知";
    }
    public override string Name { get; set; } = "分解职业";
    public override string RenderName  => "分解职业";
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = "分解该物品与哪种职业相关？";
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}