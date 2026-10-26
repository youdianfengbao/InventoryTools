using System;
using InventoryTools.Logic.Filters;

namespace InventoryTools.Extensions;

public static class FilterCategoryExtensions
{
    public static string FormattedName(this FilterCategory filterCategory)
    {
        return filterCategory switch
        {
            FilterCategory.SourceCategories => "来源（分类）",
            FilterCategory.UseCategories => "用途（分类）",
            _ => filterCategory.ToString().ToSentence()
        };
    }
}