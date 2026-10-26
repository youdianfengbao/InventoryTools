using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuAddToActiveCraftListSetting : BooleanSetting
    {
        public ContextMenuAddToActiveCraftListSetting(ILogger<ContextMenuAddToActiveCraftListSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.AddToActiveCraftListContextMenu;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.AddToActiveCraftListContextMenu = newValue;
        }

        public override string Key { get; set; } = "addToActiveCraftListContextMenu";
        public override string Name { get; set; } = "右键菜单 - 添加到当前制作清单";

        public override string WizardName { get; } = "添加到当前制作清单";

        public override string HelpText { get; set; } =
            "添加一个子菜单，用于将物品加入当前制作清单？";
        
        public override string Version => "1.7.0.5";
    }
}