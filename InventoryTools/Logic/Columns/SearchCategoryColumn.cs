using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class SearchCategoryColumn : TextColumn
    {
        public SearchCategoryColumn(ILogger<SearchCategoryColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.Item.Base.ItemSearchCategory.IsValid)
            {
                return searchResult.Item.Base.ItemSearchCategory.Value.Name.ExtractText();
            }

            return "";
        }
        public override string Name { get; set; } = "分类（市场板）";
        public override string RenderName => "市场板分类";
        public override float Width { get; set; } = 200.0f;

        public override string HelpText { get; set; } =
            "按市场板搜索分类划分的物品分类。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}