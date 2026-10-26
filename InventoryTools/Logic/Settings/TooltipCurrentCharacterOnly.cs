using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class TooltipCurrentCharacterSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = false;

        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.TooltipCurrentCharacter;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.TooltipCurrentCharacter = newValue;
        }

        public override string Key { get; set; } = "TooltipCurrentCharacter";
        public override string Name { get; set; } = "仅限当前角色的物品？";

        public override string HelpText { get; set; } =
            "将提示中显示的信息限制为当前登录角色所拥有的库存。";
        
        public override string Version => "1.7.0.0";

        public TooltipCurrentCharacterSetting(ILogger<TooltipCurrentCharacterSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}