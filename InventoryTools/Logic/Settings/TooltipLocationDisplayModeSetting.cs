using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipLocationDisplayModeSetting : ChoiceSetting<TooltipLocationDisplayMode>
{
    public override TooltipLocationDisplayMode DefaultValue { get; set; } =
        TooltipLocationDisplayMode.CharacterCategoryQuantityQuality;
    public override TooltipLocationDisplayMode CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipLocationDisplayMode;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, TooltipLocationDisplayMode newValue)
    {
        configuration.TooltipLocationDisplayMode = newValue;
    }

    public override string Key { get; set; } = "TooltipLocationDisplayMode";
    public override string Name { get; set; } = "添加物品位置（显示模式）";

    public override string WizardName { get; } = "显示模式";

    public override string HelpText { get; set; } =
        "物品位置在提示中的显示格式。需开启“添加物品位置”。";

    public override Dictionary<TooltipLocationDisplayMode, string> Choices
    {
        get
        {
            return new Dictionary<TooltipLocationDisplayMode, string>()
            {
                { TooltipLocationDisplayMode.CharacterQuantityQuality, "角色/雇员 - 数量 - 品质" },
                { TooltipLocationDisplayMode.CharacterBagSlotQuality, "角色/雇员 - 背包 - 格子 - 品质" },
                { TooltipLocationDisplayMode.CharacterBagSlotQuantity, "角色/雇员 - 背包 - 格子 - 数量" },
                {
                    TooltipLocationDisplayMode.CharacterCategoryQuantityQuality,
                    "角色/雇员 - 分类 - 数量 - 品质"
                },
                {
                    TooltipLocationDisplayMode.CharacterWorldCategoryQuantityQuality,
                    "角色/雇员 - 服务器 - 分类 - 数量 - 品质"
                },
            };
        }
    }
    public override string Version => "1.7.0.0";

    public TooltipLocationDisplayModeSetting(ILogger<TooltipLocationDisplayModeSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}