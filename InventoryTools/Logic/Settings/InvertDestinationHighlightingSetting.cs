using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class InvertDestinationHighlightingSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.InvertDestinationHighlighting;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.InvertDestinationHighlighting = newValue;
        }

        public override string Key { get; set; } = "InvertDestinationHighlighting";
        public override string Name { get; set; } = "反转目标高亮？";

        public override string HelpText { get; set; } =
            "高亮目标物品时，是否反转物品的颜色？";
        
        public override string Version => "1.7.0.0";

        public InvertDestinationHighlightingSetting(ILogger<InvertDestinationHighlightingSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}