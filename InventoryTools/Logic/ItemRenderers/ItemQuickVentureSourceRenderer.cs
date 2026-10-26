using System;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Humanizer;
using Dalamud.Bindings.ImGui;
using InventoryTools.Extensions;
using OtterGui.Raii;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemQuickVentureSourceRenderer : ItemInfoRenderer<ItemQuickVentureSource>
{
    public ItemQuickVentureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.QuickVenture;
    public override string SingularName => "快速探索";
    public override bool ShouldGroup => true;
    public override string HelpText => "该物品是否可由雇员通过快速探索带回？";
    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);

         ImGui.Text($"{asSource.RetainerTaskRow.FormattedName}");
         using (ImRaii.PushIndent())
         {
             ImGui.Text($"探索消耗：{asSource.RetainerTaskRow.Base.VentureCost}");
             ImGui.Text(
                 $"时长：{asSource.RetainerTaskRow.Base.MaxTimemin.Minutes().ToHumanReadableString()}");
         }
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