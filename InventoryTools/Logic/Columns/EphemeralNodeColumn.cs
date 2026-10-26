using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class EphemeralNodeColumn : CheckboxColumn
    {

        public EphemeralNodeColumn(ILogger<EphemeralNodeColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasSourcesByCategory(ItemInfoCategory.EphemeralGathering);
        }

        public override string Name { get; set; } = "来自临存节点？";
        public override string RenderName => "临存节点？";
        public override float Width { get; set; } = 125.0f;
        public override string HelpText { get; set; } = "该物品是否可从临存节点采集？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}