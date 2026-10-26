using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using InventoryTools.Ui;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayWindowStateSetting : GenericEnumChoiceSetting<CraftOverlayWindowState>
{
    public override bool AppearsInConfigWindow => false;

    public CraftOverlayWindowStateSetting(ILogger<CraftOverlayWindowStateSetting> logger, ImGuiService imGuiService) : base("CraftOverlayWindowState", "窗口状态", "制作悬浮层窗口的当前状态。", CraftOverlayWindowState.Single, new Dictionary<CraftOverlayWindowState, string>()
    {
        { CraftOverlayWindowState.Collapsed, "折叠"},
        { CraftOverlayWindowState.Single, "单个"},
        { CraftOverlayWindowState.List, "展开"},
    }, "1.11.0.8", logger, imGuiService)
    {
    }
}