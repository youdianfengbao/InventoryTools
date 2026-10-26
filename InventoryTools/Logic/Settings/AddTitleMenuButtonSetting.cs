using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class AddTitleMenuButtonSetting : BooleanSetting
{
    public AddTitleMenuButtonSetting(ILogger<AddTitleMenuButtonSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.AddTitleMenuButton;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.AddTitleMenuButton = newValue;
    }

    public override string Key { get; set; } = "AddTitleMenuButton";
    public override string Name { get; set; } = "在标题菜单添加按钮？";

    public override string HelpText { get; set; } =
        "在游戏标题菜单中（Dalamud 菜单旁）添加一个按钮，让你在未登录角色时也能打开亚拉戈工具。";
    
    public override string Version => "1.7.0.0"; 
}