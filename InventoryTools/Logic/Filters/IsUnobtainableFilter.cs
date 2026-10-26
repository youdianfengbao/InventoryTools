using System;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class IsUnobtainableFilter : GenericBooleanFilter
{
    public IsUnobtainableFilter(ILogger<IsUnobtainableFilter> logger, ImGuiService imGuiService) : base("IsUnobtainable", "无法获取？", "该物品是否已变得无法获取？", FilterCategory.Basic, item => item.Item.IsUnobtainable, item => item.IsUnobtainable,  logger, imGuiService)
    {
    }
}