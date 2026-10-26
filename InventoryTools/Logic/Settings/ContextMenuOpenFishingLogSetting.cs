using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ContextMenuOpenFishingLogSetting : BooleanSetting
{
    public ContextMenuOpenFishingLogSetting(ILogger<ContextMenuOpenFishingLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.OpenFishingLogContextMenu;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.OpenFishingLogContextMenu = newValue;
    }

    public override string Key { get; set; } = "OpenFishingLogContextMenu";
    public override string Name { get; set; } = "右键菜单 - 打开钓鱼笔记";

    public override string WizardName { get; } = "打开钓鱼笔记";

    public override string HelpText { get; set; } =
        "为可钓鱼获得的物品添加右键菜单项，用于打开钓鱼笔记？";
    
    public override string Version { get; } = "1.11.0.2";
}