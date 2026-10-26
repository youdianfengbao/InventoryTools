using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipFooterLinesSetting : IntegerSetting
{
    public override int DefaultValue { get; set; } = 0;
    public override int CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipFooterLines;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, int newValue)
    {
        configuration.TooltipFooterLines = newValue;
    }

    public override string Key { get; set; } = "TooltipFooterLines";
    public override string Name { get; set; } = "底部空行数";

    public override string HelpText { get; set; } =
        "在本插件添加的提示内容下方追加多少个空行？";
    
    public override string Version => "1.7.0.0";

    public TooltipFooterLinesSetting(ILogger<TooltipFooterLinesSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}