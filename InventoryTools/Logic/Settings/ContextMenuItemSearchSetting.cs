using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuItemSearchSetting : BooleanSetting
    {
        public ContextMenuItemSearchSetting(ILogger<ContextMenuItemSearchSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.ItemSearchContextMenu;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.ItemSearchContextMenu = newValue;
        }

        public override string Key { get; set; } = "ItemSearchContextMenu";
        public override string Name { get; set; } = "右键菜单 - 搜索";

        public override string WizardName { get; } = "搜索";

        public override string HelpText { get; set; } =
            "对全部库存或下方定义的范围执行搜索。";
        
        public override string Version => "1.7.0.13";
    }
}