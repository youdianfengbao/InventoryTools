using System;
using LuminaSupplemental.Excel.Model;

namespace InventoryTools.Localizers;

public class RelicToolCategoryLocalizer : ILocalizer<RelicToolCategory>
{
    public string Format(RelicToolCategory instance)
    {
        switch (instance)
        {
            case RelicToolCategory.Mastercraft:
                return "名匠工具";
            case RelicToolCategory.Skysteel:
                return "天钢工具";
            case RelicToolCategory.Resplendent:
                return "曜晶工具";
            case RelicToolCategory.Splendorous:
                return "耀晶工具";
            case RelicToolCategory.Cosmic:
                return "星海工具";
        }
        return instance.ToString();
    }
}