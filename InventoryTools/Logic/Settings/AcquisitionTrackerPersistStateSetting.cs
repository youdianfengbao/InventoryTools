using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class AcquisitionTrackerPersistStateSetting : GenericIntegerSetting
{
    public AcquisitionTrackerPersistStateSetting(ILogger<AcquisitionTrackerPersistStateSetting> logger, ImGuiService imGuiService) : base("AcquisitionTrackerPersistState", "状态保持时间（秒）", "获取追踪器会追踪你的当前状态（制作、采集等）。状态切回空闲后，默认会再保持 2 秒，以捕捉追踪器的延迟。如果发现制作/采集的物品未被追踪到，请将此数值调高。", 2, "1.12.0.8", logger, imGuiService)
    {
    }
}