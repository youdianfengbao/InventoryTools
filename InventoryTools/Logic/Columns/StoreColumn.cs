using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns;

public class StoreColumn : CheckboxColumn
{
    public StoreColumn(ILogger<StoreColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.HasSourcesByType(ItemInfoType.CashShop);
    }
    public override string Name { get; set; } = "在线商店出售？";
    public override string RenderName => "在线商店物品？";
    public override float Width { get; set; } = 80;
    public override string HelpText { get; set; } = "该物品是否在 Square Enix 在线商店出售？";
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
}