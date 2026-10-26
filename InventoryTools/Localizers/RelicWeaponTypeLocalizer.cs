using System;
using LuminaSupplemental.Excel.Model;

namespace InventoryTools.Localizers;

public class RelicWeaponTypeLocalizer : ILocalizer<RelicWeaponType>
{
    public string Format(RelicWeaponType relicWeaponType)
    {
        switch (relicWeaponType)
        {
            case RelicWeaponType.Unknown:
                return "未知";
            case RelicWeaponType.ZodiacBase:
                return "基础";
            case RelicWeaponType.ZodiacZenith:
                return "天极";
            case RelicWeaponType.ZodiacAtma:
                return "阿特玛";
            case RelicWeaponType.ZodiacAnimus:
                return "阿尼玛斯";
            case RelicWeaponType.ZodiacNovus:
                return "诺乌斯";
            case RelicWeaponType.ZodiacNexus:
                return "尼克斯";
            case RelicWeaponType.ZodiacZodiac:
                return "黄道";
            case RelicWeaponType.ZodiacZeta:
                return "泽塔";
            case RelicWeaponType.AnimaAnimated:
                return "跃动";
            case RelicWeaponType.AnimaAwoken:
                return "觉醒";
            case RelicWeaponType.AnimaAnima:
                return "阿尼玛";
            case RelicWeaponType.AnimaHyperconductive:
                return "超导";
            case RelicWeaponType.AnimaReconditioned:
                return "修整";
            case RelicWeaponType.AnimaSharpened:
                return "磨砺";
            case RelicWeaponType.AnimaComplete:
                return "完结";
            case RelicWeaponType.AnimaLux:
                return "流光";
            case RelicWeaponType.EurekanAntiquated:
                return "古旧";
            case RelicWeaponType.EurekanBase:
                return "基础";
            case RelicWeaponType.EurekanBase1:
                return "基础 + 1";
            case RelicWeaponType.EurekanBase2:
                return "基础 + 2";
            case RelicWeaponType.EurekanAnemos:
                return "阿尼莫斯";
            case RelicWeaponType.EurekanPagos:
                return "帕戈斯";
            case RelicWeaponType.EurekanPagos1:
                return "帕戈斯 + 1";
            case RelicWeaponType.EurekanElemental:
                return "元素";
            case RelicWeaponType.EurekanElemental1:
                return "元素 + 1";
            case RelicWeaponType.EurekanElemental2:
                return "元素 + 2";
            case RelicWeaponType.EurekanPyros:
                return "皮罗斯";
            case RelicWeaponType.EurekanHydatos:
                return "希达托斯";
            case RelicWeaponType.EurekanHydatos1:
                return "希达托斯 + 1";
            case RelicWeaponType.EurekanBaseEureka:
                return "基础";
            case RelicWeaponType.EurekanEureka:
                return "优雷卡";
            case RelicWeaponType.EurekanPhyseos:
                return "Physeos";
            case RelicWeaponType.ResistanceResistance:
                return "复国军";
            case RelicWeaponType.ResistanceAugmentedResistance:
                return "改良复国军";
            case RelicWeaponType.ResistanceRecollection:
                return "追忆";
            case RelicWeaponType.ResistanceLawsOrder:
                return "法则秩序";
            case RelicWeaponType.ResistanceAugmentedLawsOrder:
                return "改良法则秩序";
            case RelicWeaponType.ResistanceBlades:
                return "剑刃";
            case RelicWeaponType.MandervilleManderville:
                return "曼德维尔";
            case RelicWeaponType.MandervilleAmazing:
                return "惊奇";
            case RelicWeaponType.MandervilleMajestic:
                return "威严";
            case RelicWeaponType.MandervilleMandervillous:
                return  "Mandervillous";
            case RelicWeaponType.PhantomPenumbrae:
                return "Penumbrae";
            case RelicWeaponType.PhantomUmbrae:
                return "Umbrae";
            case RelicWeaponType.PhantomObscurum:
                return "Obscurum";
        }

        return relicWeaponType.ToString();
    }
}