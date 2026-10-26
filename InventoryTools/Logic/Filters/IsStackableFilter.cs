using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class IsStackableFilter : GenericBooleanFilter
{
    public IsStackableFilter(ILogger<IsStackableFilter> logger, ImGuiService imGuiService) : base("IsStackable", "可堆叠？", "该物品是否可堆叠（单格可放多个）？", FilterCategory.Basic, item => item.Item.Base.StackSize > 1, item => item.Base.StackSize > 1, logger, imGuiService)
    {
    }
}
