using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayIngredientPatchSetting : GenericBooleanSetting
{
    public TooltipDisplayIngredientPatchSetting(ILogger<TooltipDisplayIngredientPatchSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayIngredientPatch", "添加材料版本", "显示材料最后出现的版本。计算方式是遍历该物品参与的所有配方及其下游配方，直到确定最高版本。", false, "1.12.0.11", logger, imGuiService)
    {
    }
}