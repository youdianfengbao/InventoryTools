using System;
using System.Globalization;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemCashShopSourceRenderer : ItemInfoRenderer<ItemCashShopSource>
{
    public ItemCashShopSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CashShop;
    public override string SingularName => "商城购买（真实货币）";
    public override bool ShouldGroup => true;
    public override string HelpText => "该物品是否可通过商城（Mog Station）购买？";

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var priceUsd = asSource.PriceUsd.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
        ImGui.TextUnformatted($"价格（美元）：{priceUsd}");
        if (asSource.FittingShopItemSetRow?.Items.Count > 1)
        {
            ImGui.TextUnformatted($"套装：{asSource.FittingShopItemSetRow.Base.Name.ExtractText()}");
            ImGui.TextUnformatted($"包含：");
            using (ImRaii.PushIndent())
            {
                foreach (var item in asSource.FittingShopItemSetRow.Items)
                {
                    ImGui.TextUnformatted(item.NameString);
                }
            }
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return (asSource.FittingShopItemSetRow?.Base.Name.ExtractText() ?? "不属于任何套装");
    };

    public override Func<ItemSource, int> GetIcon => source => Icons.BagStar;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var priceUsd = asSource.PriceUsd.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
        var description = $"价格（美元）：{priceUsd}";
        if (asSource.FittingShopItemSetRow != null)
        {
            description += $" （属于 {asSource.FittingShopItemSetRow.Base.Name.ExtractText()} 套装）";
            description += $" （包含 {String.Join("、", asSource.FittingShopItemSetRow.Items.Select(c => c.NameString))}";
        }
        return description;
    };
}