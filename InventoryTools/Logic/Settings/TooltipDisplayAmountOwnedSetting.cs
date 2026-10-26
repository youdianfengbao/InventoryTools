using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class TooltipDisplayAmountOwnedSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;

        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.TooltipDisplayAmountOwned;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.TooltipDisplayAmountOwned = newValue;
        }

        public override string Key { get; set; } = "TooltipDisplayOwned";
        public override string Name { get; set; } = "添加物品位置";

        public override string WizardName { get; } = "添加物品位置";

        public override string HelpText { get; set; } =
            "悬停物品时，提示中是否显示你当前持有的该物品所在位置？";
        
        public override string Version => "1.7.0.0";

        public TooltipDisplayAmountOwnedSetting(ILogger<TooltipDisplayAmountOwnedSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}