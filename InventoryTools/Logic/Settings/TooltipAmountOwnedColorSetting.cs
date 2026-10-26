using System.Numerics;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipAmountOwnedColorSetting : GenericGameColorSetting
{
    public TooltipAmountOwnedColorSetting(ILogger<TooltipAmountOwnedColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipAmountOwnedColor", "文字颜色", "启用后，“添加物品位置”提示文字使用什么颜色？", null, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 8;
    }
}


public class TooltipMarketPricingColorSetting : GenericGameColorSetting
{
    public TooltipMarketPricingColorSetting(ILogger<TooltipMarketPricingColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipMarketPricingColor", "文字颜色", "启用后，“市场价格”提示文字使用什么颜色？", null, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 559;
    }
}
public class TooltipAmountToRetrieveColorSetting : GenericGameColorSetting
{
    public TooltipAmountToRetrieveColorSetting(ILogger<TooltipAmountToRetrieveColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipAmountToRetrieveColor", "文字颜色", "启用后，“待取出数量”提示文字使用什么颜色？", null, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 37;
    }
}
public class TooltipItemUnlockStatusColorSetting : GenericGameColorSetting
{
    public TooltipItemUnlockStatusColorSetting(ILogger<TooltipItemUnlockStatusColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipItemUnlockStatusColor", "文字颜色", "启用后，“物品解锁状态”提示文字使用什么颜色？", null, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 555;
    }
}
public class TooltipSourceInformationColorSetting : GenericGameColorSetting
{
    public TooltipSourceInformationColorSetting(ILogger<TooltipSourceInformationColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipSourceInformationColor", "文字颜色", "启用后，“来源信息”提示文字使用什么颜色？", null, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 502;
    }
}
public class TooltipUseInformationColorSetting : GenericGameColorSetting
{
    public TooltipUseInformationColorSetting(ILogger<TooltipUseInformationColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipUseInformationColor", "文字颜色", "启用后，“用途信息”提示文字使用什么颜色？", null, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 60;
    }
}
public class TooltipIngredientPatchTooltipColorSetting : GenericGameColorSetting
{
    public TooltipIngredientPatchTooltipColorSetting(ILogger<TooltipIngredientPatchTooltipColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipIngredientPatchTooltipColor", "文字颜色", "启用后，“材料获取区域”提示文字使用什么颜色？", null, "1.12.0.12", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 540;
    }
}
public class TooltipGlamourReadySetColorSetting : GenericGameColorSetting
{
    public TooltipGlamourReadySetColorSetting(ILogger<TooltipGlamourReadySetColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipGlamourReadySetColor", "文字颜色", "启用后，“套装投影”提示文字使用什么颜色？", null, "1.12.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 500;
    }
}

public class TooltipGlamourReadySetAcquiredColorSetting : GenericGameColorSetting
{
    public TooltipGlamourReadySetAcquiredColorSetting(ILogger<TooltipGlamourReadySetAcquiredColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipGlamourReadySetAcquiredColor", "已获得物品颜色", "详细模式下，已获得的物品以什么颜色显示？", null, "1.13.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 45;
    }
}
public class TooltipGlamourReadySetNotAcquiredColorSetting : GenericGameColorSetting
{
    public TooltipGlamourReadySetNotAcquiredColorSetting(ILogger<TooltipGlamourReadySetNotAcquiredColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipGlamourReadySetNotAcquiredColor", "未获得物品颜色", "详细模式下，尚未获得的物品以什么颜色显示？", null, "1.13.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 17;
    }
}
public class TooltipCofferLootColorSetting : GenericGameColorSetting
{
    public TooltipCofferLootColorSetting(ILogger<TooltipCofferLootColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipCofferLootColor", "文字颜色", "启用后，“宝箱战利品”提示文字使用什么颜色？", null, "1.12.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 541;
    }
}
public class TooltipCuratedListsColorSetting : GenericGameColorSetting
{
    public TooltipCuratedListsColorSetting(ILogger<TooltipCuratedListsColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipCuratedListsColor", "文字颜色", "启用后，“精选清单”提示文字使用什么颜色？", null, "15.0.12", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 535;
    }
}