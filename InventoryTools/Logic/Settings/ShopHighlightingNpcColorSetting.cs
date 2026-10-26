using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingNpcColorSetting : GenericEnumChoiceSetting<ObjectHighlightColor>
{
    public ShopHighlightingNpcColorSetting(ILogger<ShopHighlightingNpcColorSetting> logger, ImGuiService imGuiService) : base("ShopHighlightingNpcColor", "商店高亮 - NPC 高亮颜色", "在商店物品高亮时，用于在场景中高亮相关 NPC 的颜色。", ObjectHighlightColor.Green, new Dictionary<ObjectHighlightColor, string>
    {
        { ObjectHighlightColor.Red,     "红色"     },
        { ObjectHighlightColor.Green,   "绿色"   },
        { ObjectHighlightColor.Blue,    "蓝色"    },
        { ObjectHighlightColor.Yellow,  "黄色"  },
        { ObjectHighlightColor.Orange,  "橙色"  },
        { ObjectHighlightColor.Magenta, "品红" },
    }, "15.0.4", logger, imGuiService)
    {
    }
}
