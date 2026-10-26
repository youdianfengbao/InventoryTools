using AllaganLib.GameSheets.Sheets.Rows;

namespace InventoryTools.Localizers;

public class RoleLocalizer : ILocalizer<RoleType>
{
    public string Format(RoleType instance)
    {
        switch (instance)
        {
            case RoleType.Tank:
                return "坦克";
            case RoleType.DPSMelee:
                return "近战 DPS";
            case RoleType.DPSRanged:
                return "远程 DPS";
            case RoleType.Healer:
                return "治疗";
            case RoleType.Crafting:
                return "能工巧匠";
            case RoleType.Gathering:
                return "大地使者";
            case RoleType.Other:
                return "其他";
        }

        return "未知";
    }
}