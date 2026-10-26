using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class ShowTooltipsSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.DisplayTooltip;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.DisplayTooltip = newValue;
        }

        public override string Key { get; set; } = "ShowTooltips";
        public override string Name { get; set; } = "启用提示增强？";

        public override string HelpText { get; set; } =
            "启用/停用本插件的整套提示修改系统。关闭后，物品提示将保持原样不做任何修改。";
        
        public override string Version => "1.7.0.0";

        public ShowTooltipsSetting(ILogger<ShowTooltipsSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}