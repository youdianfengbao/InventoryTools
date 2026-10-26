using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ContextMenuOpenCraftingLogSetting : BooleanSetting
{
    public ContextMenuOpenCraftingLogSetting(ILogger<ContextMenuOpenCraftingLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.OpenCraftingLogContextMenu;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.OpenCraftingLogContextMenu = newValue;
    }

    public override string Key { get; set; } = "OpenCraftingLogContextMenu";
    public override string Name { get; set; } = "右键菜单 - 打开制作笔记";

    public override string WizardName { get; } = "打开制作笔记";

    public override string HelpText { get; set; } =
        "为可制作的物品添加右键菜单项，用于打开制作笔记？";
    
    public override string Version { get; } = "1.11.0.2";
}