using InventoryTools.Boot;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class PersistDataSetting : GenericBooleanSetting
{
    private readonly BootConfiguration _bootConfiguration;

    public PersistDataSetting(ILogger<PersistDataSetting> logger, BootConfiguration bootConfiguration, ImGuiService imGuiService) : base("PersistData", "持久化缓存数据", "亚拉戈工具首次启动时需要计算一些信息，根据电脑性能可能耗时 5-10 秒。开启此项后，这些数据会在更新之间保留，从而加快插件启动速度。", true, "14.0.5", logger, imGuiService)
    {
        _bootConfiguration = bootConfiguration;
    }

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return _bootConfiguration.PersistLuminaCache;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        _bootConfiguration.PersistLuminaCache = newValue;
    }
}