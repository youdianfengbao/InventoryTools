using InventoryTools.Logic;

namespace InventoryTools.Extensions;

public static class FilterTypeExtensions
{
    public static string FormattedName(this FilterType filterType)
    {
        return filterType switch
        {
            FilterType.None => "无",
            FilterType.SearchFilter => "搜索清单",
            FilterType.SortingFilter => "整理清单",
            FilterType.GameItemFilter => "游戏物品清单",
            FilterType.CraftFilter => "制作清单",
            FilterType.HistoryFilter => "历史清单",
            FilterType.CuratedList => "精选清单",
            _ => "未知"
        };
    }
}