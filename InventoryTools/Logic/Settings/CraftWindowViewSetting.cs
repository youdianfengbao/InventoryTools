using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public enum CraftWindowView
{
    Crafts,
    Tree,
    Configuration
}

public class CraftWindowViewSetting : GenericEnumChoiceSetting<CraftWindowView>
{
    public override bool AppearsInConfigWindow => false;

    public CraftWindowViewSetting(ILogger<CraftWindowViewSetting> logger, ImGuiService imGuiService) : base("CraftWindowView", "制作窗口视图", "制作清单当前处于哪个视图？",CraftWindowView.Crafts, new Dictionary<CraftWindowView, string>(){
        { CraftWindowView.Crafts , "制作"}, { CraftWindowView.Configuration, "配置"},
        { CraftWindowView.Tree, "树状图"}}, "12.0.20", logger, imGuiService)
    {
    }
}