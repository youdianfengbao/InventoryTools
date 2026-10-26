using System;
using LuminaSupplemental.Excel.Model;

namespace InventoryTools.Localizers;

public class RelicWeaponCategoryLocalizer : ILocalizer<RelicWeaponCategory>
{
    public string Format(RelicWeaponCategory instance)
    {
        switch (instance)
        {
            case RelicWeaponCategory.Zodiac:
                return  "黄道武器";
            case RelicWeaponCategory.Anima:
                return  "灵武";
            case RelicWeaponCategory.Eurekan:
                return  "优雷卡武器";
            case RelicWeaponCategory.Resistance:
                return  "复国军武器";
            case RelicWeaponCategory.Manderville:
                return  "曼德维尔武器";
            case RelicWeaponCategory.Phantom:
                return  "幻影武器";
        }
        return instance.ToString();
    }
}