using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class DisplayFilterInRetainersFilter : ChoiceFilter<FilterItemsRetainerEnum>
    {
        public override string Key { get; set; } = "FilterInRetainers";
        public override string Name { get; set; } = "在雇员处筛选物品？";

        public override string HelpText { get; set; } =
            "与雇员对话时，筛选器是否自动调整为仅显示应从你的库存放入雇员处的物品？若设为“仅”，则只有在雇员铃处及雇员界面内才会高亮。";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Display;
        public override void ResetFilter(FilterConfiguration configuration)
        {
            UpdateFilterConfiguration(configuration, DefaultValue);
        }

        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return null;
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            return null;
        }

        public override FilterItemsRetainerEnum CurrentValue(FilterConfiguration configuration)
        {
            return configuration.FilterItemsInRetainersEnum;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, FilterItemsRetainerEnum newValue)
        {
            configuration.FilterItemsInRetainersEnum = newValue;
        }

        public override FilterItemsRetainerEnum DefaultValue { get; set; } = FilterItemsRetainerEnum.No;

        public override FilterType AvailableIn { get; set; } =
            FilterType.SearchFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter | FilterType.CuratedList | FilterType.CraftFilter;

        public override List<FilterItemsRetainerEnum> GetChoices(FilterConfiguration configuration)
        {
            return Enum.GetValues<FilterItemsRetainerEnum>().ToList();
        }

        public override string GetFormattedChoice(FilterConfiguration filterConfiguration,
            FilterItemsRetainerEnum choice)
        {
            if (choice == FilterItemsRetainerEnum.No)
            {
                return "否";
            }

            if (choice == FilterItemsRetainerEnum.Yes)
            {
                return "是";
            }

            if (choice == FilterItemsRetainerEnum.Only)
            {
                return "仅";
            }

            return choice.ToString();
        }

        public DisplayFilterInRetainersFilter(ILogger<DisplayFilterInRetainersFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}