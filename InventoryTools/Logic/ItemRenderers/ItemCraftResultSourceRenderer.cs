using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Textures;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using OtterGui.Raii;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemCraftResultSourceRenderer : ItemInfoRenderer<ItemCraftResultSource>
{
    private readonly ItemSheet _itemSheet;
    private readonly IGameInterface _gameInterface;
    private readonly ITextureProvider _textureProvider;

    public ItemCraftResultSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, IGameInterface gameInterface,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _itemSheet = itemSheet;
        _gameInterface = gameInterface;
        _textureProvider = textureProvider;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CraftRecipe;
    public override string SingularName => "制作配方";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Crafting];
    public override string HelpText => "该物品是否可通过制作配方制作？";
    public override Func<ItemSource, List<MessageBase>?>? OnClick => source =>
    {
        var asSource = AsSource(source);
        _gameInterface.OpenCraftingLog(asSource.Item.RowId, asSource.Recipe.RowId);
        return null;
    };

    public override Func<List<ItemSource>, List<List<ItemSource>>>? CustomGroup => sources =>
    {
        return sources.GroupBy(c => AsSource(c).Recipe.Base.CraftType.RowId).Select(c => c.ToList()).ToList();
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text($"制作类型：{asSource.Recipe.Base.CraftType.Value.Name}");
        ImGui.Text($"产出数量：{asSource.Recipe.Base.AmountResult}");
        ImGui.Text($"制作难度：{asSource.Recipe.Base.DifficultyFactor}");
        ImGui.Text($"所需作业精度：{asSource.Recipe.Base.RequiredCraftsmanship}");

        ImGui.Text("材料：");
        using (ImRaii.PushIndent())
        {
            foreach (var ingredient in asSource.Recipe.IngredientCounts)
            {
                var item = _itemSheet.GetRow(ingredient.Key);
                ImGui.Image(_textureProvider.GetFromGameIcon(new GameIconLookup(item.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                ImGui.SameLine();
                ImGui.Text($"{item.NameString} x {ingredient.Value}");
            }
        }
    };
    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.Item.NameString + " (" + (asSource.Recipe.CraftType?.FormattedName ?? "未知") + ")";
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = AsSource(source);
        if (asSource.Recipe.CraftType != null)
        {
            return asSource.Recipe.CraftType.Icon;
        }

        return Icons.CraftIcon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return $"{asSource.Recipe.Base.CraftType.Value.Name}（产出 {asSource.Recipe.Base.AmountResult} 个）（{string.Join("、", asSource.Recipe.IngredientCounts.Select(c => _itemSheet.GetRow(c.Key).NameString + " x " + c.Value))}）";
    };
}