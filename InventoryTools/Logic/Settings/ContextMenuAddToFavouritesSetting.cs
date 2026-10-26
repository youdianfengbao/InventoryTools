using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ContextMenuAddToFavouritesSetting : GenericBooleanSetting
{
    public ContextMenuAddToFavouritesSetting(ILogger<ContextMenuAddToFavouritesSetting> logger,
        ImGuiService imGuiService) : base("AddToFavouritesContextMenu",
        "右键菜单 - 添加/移出收藏",
        "添加一个子菜单，用于将物品加入收藏或从收藏中移除？",
        false,
        "1.13.1.9",
        logger,
        imGuiService)
    {
    }

    public override string WizardName { get; } = "添加/移出收藏";

}