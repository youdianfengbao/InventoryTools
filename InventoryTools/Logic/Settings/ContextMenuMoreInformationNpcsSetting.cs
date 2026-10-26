using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationNpcsSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationNpcsSetting(ILogger<ContextMenuMoreInformationNpcsSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoNpcs" , "右键菜单 - 更多信息（NPC）", "在 NPC 的右键菜单中添加“更多信息”菜单项？", true, "14.0.2", logger, imGuiService)
        {
        }
    }
}