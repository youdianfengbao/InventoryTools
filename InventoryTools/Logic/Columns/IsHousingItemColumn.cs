using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Misc;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class IsHousingItemColumn : CheckboxColumn
    {
        public IsHousingItemColumn(ILogger<IsHousingItemColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return Helpers.HousingCategoryIds.Contains(searchResult.Item.Base.ItemUICategory.RowId);
        }
        public override string Name { get; set; } = "房屋物品？";
        public override string RenderName => "房屋物品？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否为房屋家具？目前判定可能略有偏差。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}