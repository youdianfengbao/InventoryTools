using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationMonstersSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationMonstersSetting(ILogger<ContextMenuMoreInformationMonstersSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoMonsters" , "右键菜单 - 更多信息（魔物）", "在魔物的右键菜单中添加“更多信息”菜单项？", true, "14.0.2", logger, imGuiService)
        {
        }
    }
}