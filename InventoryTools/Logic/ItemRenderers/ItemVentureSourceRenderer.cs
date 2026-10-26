using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Humanizer;
using Dalamud.Bindings.ImGui;
using InventoryTools.Extensions;
using InventoryTools.Mediator;
using InventoryTools.Ui;
using OtterGui.Raii;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemWoodlandExplorationVentureSourceRenderer : ItemVentureSourceRenderer<ItemWoodlandExplorationVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.ExplorationVenture];
    public ItemWoodlandExplorationVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.BotanyExplorationVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "林地探索（园艺工）";
    public override string HelpText => "该物品是否可由雇员通过园艺探索带回？";
}
public class ItemWatersideExplorationVentureSourceRenderer : ItemVentureSourceRenderer<ItemWatersideExplorationVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.ExplorationVenture];
    public ItemWatersideExplorationVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.FishingExplorationVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "水域探索（捕鱼人）";
    public override string HelpText => "该物品是否可由雇员通过捕鱼探索带回？";
}
public class ItemHighlandExplorationVentureSourceRenderer : ItemVentureSourceRenderer<ItemHighlandExplorationVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.ExplorationVenture];
    public ItemHighlandExplorationVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.MiningExplorationVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "高地探索（采矿工）";
    public override string HelpText => "该物品是否可由雇员通过采矿探索带回？";
}

public class ItemFieldExplorationVentureSourceRenderer : ItemVentureSourceRenderer<ItemFieldExplorationVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.ExplorationVenture];
    public ItemFieldExplorationVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.CombatExplorationVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "野外探索（战斗精英）";
    public override string HelpText => "该物品是否可由雇员通过战斗探索带回？";
}

public class ItemBotanistVentureSourceRenderer : ItemVentureSourceRenderer<ItemBotanistVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Venture];
    public ItemBotanistVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.BotanyVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "园艺探索";
    public override string HelpText => "该物品是否可由雇员通过园艺探索带回？";
}
public class ItemFishingVentureSourceRenderer : ItemVentureSourceRenderer<ItemFishingVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Venture];
    public ItemFishingVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.FishingVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "捕鱼探索";
    public override string HelpText => "该物品是否可由雇员通过捕鱼探索带回？";
}
public class ItemMiningVentureSourceRenderer : ItemVentureSourceRenderer<ItemMiningVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Venture];
    public ItemMiningVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.MiningVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采矿探索";
    public override string HelpText => "该物品是否可由雇员通过采矿探索带回？";
}

public class ItemHuntingVentureSourceRenderer : ItemVentureSourceRenderer<ItemHuntingVentureSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Venture];
    public ItemHuntingVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, ItemInfoType.CombatVenture, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "狩猎探索";
    public override string HelpText => "该物品是否可由雇员通过狩猎探索带回？";
}

public abstract class ItemVentureSourceRenderer<T> : ItemInfoRenderer<T> where T : ItemVentureSource
{
    private readonly ItemInfoType _itemInfoType;

    public ItemVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ItemInfoType itemInfoType,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _itemInfoType = itemInfoType;
    }
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => _itemInfoType;
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);

        ImGui.Text($"{asSource.RetainerTaskRow.FormattedName}");
        using (ImRaii.PushIndent())
        {
            ImGui.Text($"探索消耗：{asSource.RetainerTaskRow.Base.VentureCost}");
            ImGui.Text($"所需等级：{asSource.RetainerTaskRow.Base.RetainerLevel}");
            if (asSource.RetainerTaskRow.Base.RequiredGathering != 0)
            {
                ImGui.Text(
                    $"所需采集力：{asSource.RetainerTaskRow.Base.RequiredGathering}");
            }

            if (asSource.RetainerTaskRow.Base.RequiredItemLevel != 0)
            {
                ImGui.Text(
                    $"所需物品等级：{asSource.RetainerTaskRow.Base.RequiredItemLevel}");
            }

            ImGui.Text($"经验值：{asSource.RetainerTaskRow.Base.Experience}");
            ImGui.Text(
                $"时长：{asSource.RetainerTaskRow.Base.MaxTimemin.Minutes().ToHumanReadableString()}");
        }
    };

    public override Func<ItemSource, List<MessageBase>?>? OnClick => source =>
    {
        var asSource = AsSource(source);

        return new List<MessageBase>()
            { new OpenUintWindowMessage(typeof(RetainerTaskWindow), asSource.RetainerTaskRow.RowId) };
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.RetainerTaskRow.FormattedName;
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.VentureIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return
            $"{asSource.RetainerTaskRow.FormattedName}（消耗 {asSource.RetainerTaskRow.Base.VentureCost} 次探索，{asSource.RetainerTaskRow.Base.MaxTimemin.Minutes().ToHumanReadableString()}）";
    };
}