using System;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class IsCollectableFilter : GenericBooleanFilter
{
    public IsCollectableFilter(ILogger<IsCollectableFilter> logger, ImGuiService imGuiService) : base("IsCollectable", "可收藏？", "该物品是否可收藏？", FilterCategory.Basic, item => item.Item.IsCollectable, item => item.IsCollectable, logger, imGuiService)
    {
    }
}