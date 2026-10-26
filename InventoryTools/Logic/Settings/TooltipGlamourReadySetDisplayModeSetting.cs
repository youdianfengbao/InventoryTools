using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipGlamourReadySetDisplayModeSetting : ChoiceSetting<GlamourReadySetDisplayMode>
{
    public override GlamourReadySetDisplayMode DefaultValue { get; set; } = GlamourReadySetDisplayMode.Compact;

    public override GlamourReadySetDisplayMode CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipGlamourReadySetDisplayMode;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, GlamourReadySetDisplayMode newValue)
    {
        configuration.TooltipGlamourReadySetDisplayMode = newValue;
    }

    public override string Key { get; set; } = "TooltipGlamourReadySetDisplayMode";
    public override string Name { get; set; } = "显示模式";

    public override string WizardName { get; } = "显示模式";

    public override string HelpText { get; set; } =
        "套装投影信息在提示中的显示方式。详细模式逐一列出各部件及持有状态；紧凑模式仅显示汇总数量。";

    public override Dictionary<GlamourReadySetDisplayMode, string> Choices => new()
    {
        { GlamourReadySetDisplayMode.Detailed, "详细（列出全部部件）" },
        { GlamourReadySetDisplayMode.Compact, "紧凑（汇总数量）" },
    };

    public override string Version => "1.12.0.0";

    public TooltipGlamourReadySetDisplayModeSetting(ILogger<TooltipGlamourReadySetDisplayModeSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}
