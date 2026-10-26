using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class TooltipAddCharacterNameSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = false;

        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.TooltipAddCharacterNameOwned;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.TooltipAddCharacterNameOwned = newValue;
        }

        public override string Key { get; set; } = "TooltipCharacterName";
        public override string Name { get; set; } = "添加物品位置（附带角色名）";

        public override string HelpText { get; set; } =
            "悬停物品时，若持有者中包含雇员，是否在位置信息后附上雇员主人的角色名？";
        
        public override string Version => "1.7.0.0";

        public TooltipAddCharacterNameSetting(ILogger<TooltipAddCharacterNameSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}