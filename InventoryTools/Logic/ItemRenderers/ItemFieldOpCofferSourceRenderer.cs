using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Interface.Textures;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace InventoryTools.Logic.ItemRenderers;

public abstract class ItemFieldOpCofferSourceRenderer<T> : ItemInfoRenderer<T> where T : ItemFieldOpCofferSource
{
    private readonly ItemInfoType _itemInfoType;

    public ItemFieldOpCofferSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface, ItemInfoType itemInfoType) : base(textureProvider, pluginInterface, itemSheet, mapSheet)
    {
        _itemInfoType = itemInfoType;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => _itemInfoType;
    public override bool ShouldGroup => true;
    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text("由 " + asSource.CofferType + " 宝物库掉落");
        if (asSource.Min != null && asSource.Max != null)
        {
            ImGui.SameLine();
            if (asSource.Min == asSource.Max)
            {
                ImGui.Text("(掉落 1)");
            }
            else
            {
                ImGui.Text("(掉落 " + asSource.Min.Value + " - " + asSource.Max.Value + ")");
            }
        }

        if (asSource.Probability != null)
        {
            ImGui.SameLine();
            ImGui.TextUnformatted($"{asSource.Probability.Value}%");
        }
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.GoldChest2;

    public override Func<ItemSource, string> GetName => _ => "";

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return asSource.CofferType + " 宝物库";
    };
}

public class ItemPagosTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemPagosTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.Pagos];

    public ItemPagosTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.PagosTreasure)
    {
    }

    public override string SingularName => "优雷卡 恒冰之地（宝物库）";
    public override string HelpText => "该物品是否由恒冰之地的宝物库掉落？";
}

public class ItemPyrosTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemPyrosTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.Pyros];

    public ItemPyrosTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.PyrosTreasure)
    {
    }

    public override string SingularName => "优雷卡 涌火之地（宝物库）";
    public override string HelpText => "该物品是否由涌火之地的宝物库掉落？";
}

public class ItemHydatosTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemHydatosTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.Hydatos];

    public ItemHydatosTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.HydatosTreasure)
    {
    }

    public override string SingularName => "优雷卡 丰水之地（宝物库）";
    public override string HelpText => "该物品是否由丰水之地的宝物库掉落？";
}

public class ItemOccultTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemOccultTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.OccultCrescent];

    public ItemOccultTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.OccultTreasure)
    {
    }

    public override string SingularName => "隐秘月牙（宝物库）";
    public override string HelpText => "该物品是否由隐秘月牙的宝物库掉落？";
}

public class ItemOccultPotSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemOccultPotSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.OccultCrescent];

    public ItemOccultPotSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.OccultPot)
    {
    }

    public override string SingularName => "隐秘月牙（陶壶）";
    public override string HelpText => "该物品是否由隐秘月牙的陶壶掉落？";
}

public class ItemOccultGoldenCofferSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemOccultGoldenCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.OccultCrescent];

    public ItemOccultGoldenCofferSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.OccultGoldenCoffer)
    {
    }

    public override string SingularName => "隐秘月牙（黄金宝物库）";
    public override string HelpText => "该物品是否由隐秘月牙的黄金宝物库掉落？";
}