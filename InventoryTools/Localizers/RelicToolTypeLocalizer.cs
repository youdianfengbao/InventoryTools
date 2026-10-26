using System;
using LuminaSupplemental.Excel.Model;

namespace InventoryTools.Localizers;

public class RelicToolTypeLocalizer : ILocalizer<RelicToolType>
{
    public string Format(RelicToolType relicToolType)
    {
        switch (relicToolType)
        {
            case RelicToolType.Unknown:
                return "未知";
            case RelicToolType.MastercraftBase:
                return "名匠";
            case RelicToolType.MastercraftSupra:
                return "Supra";
            case RelicToolType.MastercraftLucis:
                return "Lucis";
            case RelicToolType.SkysteelBase:
                return "天钢";
            case RelicToolType.SkysteelBase1:
                return "天钢 + 1";
            case RelicToolType.SkysteelDragonsung:
                return "Dragonsung";
            case RelicToolType.SkysteelAugmentedDragonsung:
                return "Augmented Dragonsung";
            case RelicToolType.SkysteelSkysung:
                return "Skysung";
            case RelicToolType.SkysteelSkybuilders:
                return "Skybuilders";
            case RelicToolType.Resplendent:
                return "曜晶";
            case RelicToolType.SplendorousBase:
                return "耀晶";
            case RelicToolType.SplendorousAugmented:
                return "改良耀晶";
            case RelicToolType.SplendorousCrystalline:
                return "Crystalline";
            case RelicToolType.SplendorousChoraZoiCrystalline:
                return "Chora-Zoi's Crystalline";
            case RelicToolType.SplendorousBrilliant:
                return "Brilliant";
            case RelicToolType.SplendorousVrandticVisionary:
                return "Vrandtic Visionary's";
            case RelicToolType.SplendorousLodestar:
                return "Lodestar";
            case RelicToolType.CosmicPrototype01:
                return "原型 v0.1";
            case RelicToolType.CosmicPrototype02:
                return "原型 v0.2";
            case RelicToolType.CosmicPrototype03:
                return "原型 v0.3";
            case RelicToolType.CosmicPrototype04:
                return "原型 v0.4";
            case RelicToolType.CosmicPrototype05:
                return "原型 v0.5";
            case RelicToolType.CosmicPrototype06:
                return "原型 v0.6";
            case RelicToolType.CosmicPrototype07:
                return "原型 v0.7";
            case RelicToolType.CosmicPrototype08:
                return "原型 v0.8";
            case RelicToolType.CosmicCosmic:
                return "星海";
            case RelicToolType.CosmicCosmic11:
                return "星海 v1.1";
            case RelicToolType.CosmicCosmic12:
                return "星海 v1.2";
            case RelicToolType.CosmicCosmic13:
                return "星海 v1.3";
            case RelicToolType.CosmicCosmic14:
                return "星海 v1.4";
            case RelicToolType.CosmicStellar:
                return "Stellar";
            case RelicToolType.CosmicStellar11:
                return "Stellar v1.1";
            case RelicToolType.CosmicStellar12:
                return "Stellar v1.2";
            case RelicToolType.CosmicHypertools:
                return "Hypertools";
        }

        return relicToolType.ToString();
    }
}