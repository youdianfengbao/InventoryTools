using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class LeveIsCraftLevelColumn : CheckboxColumn
    {

        public LeveIsCraftLevelColumn(ILogger<LeveIsCraftLevelColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasSourcesByType(ItemInfoType.CraftLeve);
        }
        public override string Name { get; set; } = "制作理符物品？";
        public override string RenderName => "理符（制作）";
        public override float Width { get; set; } = 100.0f;
        public override string HelpText { get; set; } = "该物品是否用于制作类理符？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}