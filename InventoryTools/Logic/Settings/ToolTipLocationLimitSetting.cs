using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ToolTipLocationLimitSetting : IntegerSetting
{
    public override int DefaultValue { get; set; } = 10;
    public override int CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipLocationLimit;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, int newValue)
    {
        configuration.TooltipLocationLimit = newValue;
    }

    public override string Key { get; set; } = "TooltipLocationLimit";
    public override string Name { get; set; } = "添加物品位置（最大条数）";
    public override string HelpText { get; set; } = "提示中最多列出多少条位置信息。需要开启“显示持有数量？”。";
    public override string Version => "1.7.0.0";

    public ToolTipLocationLimitSetting(ILogger<ToolTipLocationLimitSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}