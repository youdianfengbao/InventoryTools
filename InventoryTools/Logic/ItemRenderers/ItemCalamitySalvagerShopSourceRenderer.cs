using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemCalamitySalvagerShopUseRenderer : ItemCalamitySalvagerShopSourceRenderer
{
    private readonly MapSheet _mapSheet;
    private readonly ItemSheet _itemSheet;
    private readonly ITextureProvider _textureProvider;

    public override string HelpText => "该物品是否可在灾厄回收商人处消费？";

    public ItemCalamitySalvagerShopUseRenderer(MapSheet mapSheet, ItemSheet itemSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(mapSheet, itemSheet, textureProvider, dalamudPluginInterface)
    {
        _mapSheet = mapSheet;
        _itemSheet = itemSheet;
        _textureProvider = textureProvider;
    }

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = AsSource(sources);
        var allGilShops = asSources.ToList();
        var maps = allGilShops.SelectMany(shopSource => shopSource.MapIds == null || shopSource.MapIds.Count == 0
            ? new List<string>()
            : shopSource.MapIds.Select(c => _mapSheet.GetRow(c).FormattedName)).Distinct().ToList();

        ImGui.Text($"{maps.Count} 个区域中共有 {allGilShops.Count} 个物品可购买");
    };


    public override RendererType RendererType => RendererType.Use;
}

public class ItemCalamitySalvagerShopSourceRenderer : ItemInfoRenderer<ItemCalamitySalvagerShopSource>
{
    public ItemCalamitySalvagerShopSourceRenderer(MapSheet mapSheet, ItemSheet itemSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CalamitySalvagerShop;
    public override string SingularName => "灾厄回收商人";
    public override string PluralName => "灾厄回收商人";
    public override bool ShouldGroup => true;
    public override string HelpText => "该物品是否可在灾厄回收商人处购买？";

    public override byte MaxColumns => 1;

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = AsSource(sources);
        var firstItem = asSources[0];

        var costItems = asSources.SelectMany(c => c.CostItems).DistinctBy(d => d.ItemId).ToList();
        DrawItems("消耗：", costItems);
        var rewardItems = asSources.SelectMany(c => c.RewardItems).DistinctBy(d => d.ItemId).ToList();
        DrawItems("奖励：", rewardItems);

        if (firstItem.GilShopItem.Base.AchievementRequired.RowId != 0)
        {
            ImGui.Text(
                $"所需成就：{firstItem.GilShopItem.Base.AchievementRequired.Value.Name.ExtractText()}");
        }

        foreach (var quest in firstItem.GilShopItem.Base.QuestRequired)
        {
            if (quest.RowId != 0)
            {
                ImGui.Text(
                    $"所需任务：{quest.Value.Name.ExtractText()}");
            }
        }

        DrawMaps(sources);
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);

        DrawItems("消耗：", asSource.CostItems);
        DrawItems("奖励：", asSource.RewardItems);

        if (asSource.GilShopItem.Base.AchievementRequired.RowId != 0)
        {
            ImGui.Text(
                $"所需成就：{asSource.GilShopItem.Base.AchievementRequired.Value.Name.ExtractText()}");
        }

        foreach (var quest in asSource.GilShopItem.Base.QuestRequired)
        {
            if (quest.RowId != 0)
            {
                ImGui.Text(
                    $"所需任务：{quest.Value.Name.ExtractText()}");
            }
        }

        DrawMaps(asSource);
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        if (asSource.MapIds == null || asSource.MapIds.Count == 0)
        {
            return asSource.GilShop.Name;
        }

        var maps = asSource.MapIds.Select(c => MapSheet.GetRow(c).FormattedName);
        return asSource.GilShop.Name + "(" + maps + ")";
    };

    public override Func<ItemSource, int> GetIcon => source => Icons.CalamitySalvagerBag;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var itemName = asSource.CostItem!.NameString;
        var count = asSource.Cost;
        var description = $"{itemName} x {count}";

        if (asSource.GilShopItem.Base.AchievementRequired.RowId != 0)
        {
            description += $"（需要成就：{asSource.GilShopItem.Base.AchievementRequired.Value.Name.ExtractText()}）";
        }

        foreach (var quest in asSource.GilShopItem.Base.QuestRequired)
        {
            if (quest.RowId != 0)
            {
                description += ($"（需要任务：{quest.Value.Name.ExtractText()}）");
            }
        }

        return description;
    };
}