using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayCofferLootSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipDisplayCofferLoot;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TooltipDisplayCofferLoot = newValue;
    }

    public override string Key { get; set; } = "TooltipDisplayCofferLoot";
    public override string Name { get; set; } = "宝箱战利品信息";

    public override string WizardName { get; } = "宝箱战利品信息";

    public override string HelpText { get; set; } =
        "悬停宝箱、卡包或战利品容器时，显示其可能开出的物品中你已拥有的数量。";
    
    public override string Version => "1.12.0.0";

    public TooltipDisplayCofferLootSetting(ILogger<TooltipDisplayCofferLootSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}
