using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemGilShopUseRenderer : ItemGilShopSourceRenderer
{
    private readonly MapSheet _mapSheet;
    private readonly ItemSheet _itemSheet;
    public override string HelpText => "该物品是否可在金币商店消费？";

    public ItemGilShopUseRenderer(MapSheet mapSheet, ItemSheet itemSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(mapSheet, itemSheet, textureProvider, dalamudPluginInterface)
    {
        _mapSheet = mapSheet;
        _itemSheet = itemSheet;
    }

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = AsSource(sources);
        var allGilShops = asSources.ToList();
        var maps = allGilShops.SelectMany(shopSource => shopSource.MapIds == null || shopSource.MapIds.Count == 0
            ? new List<string>()
            : shopSource.MapIds.Select(c => _mapSheet.GetRow(c).FormattedName)).Distinct().ToList();

        ImGui.Text($"{maps.Count} 个区域中共有 {allGilShops.Count} 个物品可用金币购买");
    };

    public override RendererType RendererType => RendererType.Use;
}

public class ItemGilShopSourceRenderer : ItemInfoRenderer<ItemGilShopSource>
{
    private readonly MapSheet _mapSheet;
    private readonly ItemSheet _itemSheet;
    private readonly ITextureProvider _textureProvider;

    public ItemGilShopSourceRenderer(MapSheet mapSheet, ItemSheet itemSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _mapSheet = mapSheet;
        _itemSheet = itemSheet;
        _textureProvider = textureProvider;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.GilShop;
    public override string SingularName => "金币商店";
    public override string PluralName => "金币商店";
    public override string HelpText => "该物品是否可在金币商店购买？";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];

    public override byte MaxColumns => 1;

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = AsSource(sources);
        var firstItem = asSources[0];

        ImGui.Text("消耗：");

        using (ImRaii.PushIndent())
        {
            var itemName = firstItem.CostItem!.NameString;
            var count = firstItem.Cost;
            var costString = $"{itemName} x {count}";
            ImGui.Image(_textureProvider.GetFromGameIcon(new GameIconLookup(firstItem.CostItem.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
            ImGui.SameLine();
            ImGui.Text(costString);

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
        }

        DrawMaps(sources);
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var maps = source.MapIds?.Select(c => _mapSheet.GetRow(c).FormattedName).ToList() ?? new List<string>();

        ImGui.Text("消耗：");

        using (ImRaii.PushIndent())
        {
            var itemName = asSource.CostItem!.NameString;
            var count = asSource.Cost;
            var costString = $"{itemName} x {count}";
            ImGui.Image(_textureProvider.GetFromGameIcon(new GameIconLookup(asSource.CostItem.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
            ImGui.SameLine();
            ImGui.Text(costString);

            if (asSource.GilShopItem.Base.AchievementRequired.RowId != 0)
            {
                ImGui.Text(
                    $"所需成就：{asSource.GilShopItem.Base.AchievementRequired.Value.Name.ExtractText()}");
            }

            foreach (var quest in asSource.GilShopItem.Base.QuestRequired)
            {
                if (quest.RowId != 0 && quest.IsValid)
                {
                    ImGui.Text(
                        $"所需任务：{quest.Value.Name.ExtractText()}");
                }
            }
        }

        DrawMaps(source);
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        if (asSource.MapIds == null || asSource.MapIds.Count == 0)
        {
            return asSource.GilShop.Name;
        }

        var maps = asSource.MapIds.Select(c => _mapSheet.GetRow(c).FormattedName);
        return asSource.GilShop.Name + "(" + string.Join(",", maps) + ")";
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        return _itemSheet.GetRow(1).Icon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var description = $"{asSource.Shop.Name}";
        var rewards = string.Join(", ", asSource.GilShopItem.Rewards.Select(c => c.Item.NameString + " x " + c.Count + ""));
        var costs = string.Join(", ", asSource.GilShopItem.Costs.Select(c => c.Item.NameString + " x " + c.Count + ""));
        return $"{description} 消耗 ({costs}) 获得 ({rewards})";
    };
}