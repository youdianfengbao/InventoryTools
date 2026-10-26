using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class HistoryEnabledSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.HistoryEnabled;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.HistoryEnabled = newValue;
    }

    public override string Key { get; set; } = "HistoryEnabled";
    public override string Name { get; set; } = "启用历史记录？";
    public override string WizardName { get; } = "追踪物品历史？";

    public override string HelpText { get; set; } =
        "是否让亚拉戈工具追踪库存中物品的移动、增删情况？";
    
    public override string Version => "1.7.0.0";

    public HistoryEnabledSetting(ILogger<HistoryEnabledSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}