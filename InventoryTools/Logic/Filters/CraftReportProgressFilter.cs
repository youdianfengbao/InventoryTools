using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class CraftReportProgressFilter : GenericBooleanFilter
{
    public CraftReportProgressFilter(ILogger<CraftReportProgressFilter> logger, ImGuiService imGuiService) : base(
        "CraftReportProgress", "在聊天栏报告获取进度？",
        "获取此制作清单包含的物品时，在聊天栏输出进度（如“铁木原木 剩余 30”）。制作清单须处于激活状态。",
        FilterCategory.Notifications, null, null, logger, imGuiService)
    {
        DefaultValue = true;
        AvailableIn = FilterType.CraftFilter;
    }
}