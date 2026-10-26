using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public enum ImGuiTooltipMode
{
    Never,
    Icons,
    Everywhere
}

public class ImGuiTooltipModeSetting : GenericEnumChoiceSetting<ImGuiTooltipMode>
{
    public ImGuiTooltipModeSetting(ILogger<ImGuiTooltipModeSetting> logger,
        ImGuiService imGuiService) : base("ImGuiTooltipMode",
        "物品提示模式",
        "是否显示物品提示：从不显示、悬停物品图标时显示，还是悬停物品表格中任意行时都显示？",
        ImGuiTooltipMode.Icons,
        new Dictionary<ImGuiTooltipMode, string>()
        {
            {ImGuiTooltipMode.Never, "从不"},
            {ImGuiTooltipMode.Icons, "仅图标"},
            {ImGuiTooltipMode.Everywhere, "处处显示"},
        },
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}