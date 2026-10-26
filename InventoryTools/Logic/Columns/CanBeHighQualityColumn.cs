using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class CanHighQualityColumn : CheckboxColumn
    {
        public CanHighQualityColumn(ILogger<CanHighQualityColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.Base.CanBeHq;
        }
        public override string Name { get; set; } = "可有优质品？";
        public override float Width { get; set; } = 90.0f;
        public override string HelpText { get; set; } = "该物品是否存在高品质（HQ）版本？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}