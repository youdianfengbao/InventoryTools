using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class IsArmoireItem : CheckboxColumn
    {
        public IsArmoireItem(ILogger<IsArmoireItem> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.CabinetCategory != null;
        }
        public override string Name { get; set; } = "武器柜物品？";
        public override string RenderName => "武器柜物品？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "显示该物品是否可存入武器柜。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}