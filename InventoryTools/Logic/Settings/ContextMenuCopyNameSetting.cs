using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ContextMenuCopyNameSetting : GenericBooleanSetting
{
    public ContextMenuCopyNameSetting(ILogger<ContextMenuCopyNameSetting> logger,
        ImGuiService imGuiService) : base("CopyNameContextMenu",
        "右键菜单 - 复制名称",
        "将物品名称复制到剪贴板。",
        false,
        "15.0.6",
        logger,
        imGuiService)
    {
    }

    public override string WizardName => "复制名称";
}