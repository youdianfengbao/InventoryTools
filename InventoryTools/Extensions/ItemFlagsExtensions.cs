using FFXIVClientStructs.FFXIV.Client.Game;

namespace InventoryTools.Extensions;

public static class ItemFlagsExtensions
{
    public static string FormattedName(this InventoryItem.ItemFlags flags)
    {
        return flags switch
        {
            InventoryItem.ItemFlags.None => "普通品质",
            InventoryItem.ItemFlags.HighQuality => "高品质",
            InventoryItem.ItemFlags.CompanyCrestApplied => "已附加部队徽章",
            InventoryItem.ItemFlags.Relic => "元灵",
            InventoryItem.ItemFlags.Collectable => "收藏品",
            _ => "无"
        };
    }
}