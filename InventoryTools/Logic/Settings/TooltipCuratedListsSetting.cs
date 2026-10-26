using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayCuratedListsSetting : GenericBooleanSetting
{
    public TooltipDisplayCuratedListsSetting(ILogger<TooltipDisplayCuratedListsSetting> logger,
        ImGuiService imGuiService) : base(
        "TooltipDisplayCuratedLists",
        "精选清单信息",
        "悬停物品时，显示它已包含在你的哪些精选清单中。",
        false,
        "15.0.12",
        logger,
        imGuiService)
    {
    }
}

public class TooltipCuratedListsSetting : MultipleListChoiceSetting
{
    public TooltipCuratedListsSetting(ILogger<TooltipCuratedListsSetting> logger, ImGuiService imGuiService,
        IListService listService) : base(logger, imGuiService, listService)
    {
    }

    public override FilterType? ListFilterType => FilterType.CuratedList;

    public override string Key { get; set; } = "TooltipCuratedLists";
    public override string Name { get; set; } = "要检查的精选清单";

    public override string HelpText { get; set; } =
        "应检查哪些精选清单？若未选择任何清单，则检查全部精选清单。";

    public override string Version => "15.0.12";
}

public class TooltipCuratedListsMatchQualitySetting : GenericBooleanSetting
{
    public TooltipCuratedListsMatchQualitySetting(ILogger<TooltipCuratedListsMatchQualitySetting> logger,
        ImGuiService imGuiService) : base(
        "TooltipCuratedListsMatchQuality",
        "匹配物品品质",
        "悬停物品的品质是否需与精选清单中记录的品质一致？关闭后，NQ 物品可匹配 HQ 条目，反之亦然。",
        false,
        "15.0.12",
        logger,
        imGuiService)
    {
    }
}