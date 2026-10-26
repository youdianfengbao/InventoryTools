using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ContextMenuAddToCuratedListSetting : GenericBooleanSetting
{
    public ContextMenuAddToCuratedListSetting(ILogger<ContextMenuAddToCuratedListSetting> logger,
        ImGuiService imGuiService) : base("AddToCuratedListContextMenu",
        "右键菜单 - 添加到精选清单",
        "添加一个子菜单，用于将物品加入精选清单？",
        false,
        "1.7.0.21",
        logger,
        imGuiService)
    {
    }

    public override string WizardName { get; } = "添加到精选清单";

}