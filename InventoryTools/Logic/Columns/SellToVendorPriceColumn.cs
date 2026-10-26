using System;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class SellToVendorPriceColumn : GilColumn
    {
        public SellToVendorPriceColumn(ILogger<SellToVendorPriceColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem != null)
            {
                return (int)searchResult.InventoryItem.SellToVendorPrice;
            }
            return (int)searchResult.Item.Base.PriceLow;
        }
        public override string Name { get; set; } = "商人收购价";
        public override float Width { get; set; } = 100.0f;
        public override string HelpText { get; set; } = "将该物品卖给商人可获得的价格（金币）。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override event IColumn.ButtonPressedDelegate? ButtonPressed
        {
            add { throw new NotSupportedException(); }
            remove { }
        }
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}