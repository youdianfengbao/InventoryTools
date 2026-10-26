using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class CompendiumRowHeightSetting : GenericIntegerSetting
{
    public CompendiumRowHeightSetting(ILogger<CompendiumRowHeightSetting> logger, ImGuiService imGuiService) : base("CompendiumRowHeight", "行高", "图鉴清单中行的最小高度应为多少？", 32, "14.0.3", logger, imGuiService)
    {
    }
}