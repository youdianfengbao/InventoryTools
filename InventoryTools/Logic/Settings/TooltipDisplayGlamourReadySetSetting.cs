using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayGlamourReadySetSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipDisplayGlamourReadySet;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TooltipDisplayGlamourReadySet = newValue;
    }

    public override string Key { get; set; } = "TooltipDisplayGlamourReadySet";
    public override string Name { get; set; } = "套装投影信息";

    public override string WizardName { get; } = "套装投影信息";

    public override string HelpText { get; set; } =
        "悬停物品时，显示该物品是否为套装投影或属于某套装投影，以及其组成部件中你已在军械库或武器柜中拥有的部分。";
    
    public override string Version => "1.12.0.0";

    public TooltipDisplayGlamourReadySetSetting(ILogger<TooltipDisplayGlamourReadySetSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}
