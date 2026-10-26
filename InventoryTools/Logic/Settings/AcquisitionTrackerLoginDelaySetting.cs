using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class AcquisitionTrackerLoginDelaySetting : GenericIntegerSetting
{
    public AcquisitionTrackerLoginDelaySetting(ILogger<AcquisitionTrackerLoginDelaySetting> logger, ImGuiService imGuiService) : base("AcquisitionTrackerLoginDelay", "登录延迟", "获取追踪器会在你登录时对角色背包进行初始扫描。由于游戏会推送多次库存更新，扫描器必须等待一段时间才能完成。如果登录后制作清单中的物品出现随机完成的情况，请将此项调高。", 5, "1.12.0.8", logger, imGuiService)
    {
    }
}