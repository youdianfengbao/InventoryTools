using System;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemJobSoulCrystalUseRenderer : ItemInfoRenderer<ItemJobSoulCrystalUse>
{
    public ItemJobSoulCrystalUseRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.JobSoulCrystal;
    public override string SingularName => "职业灵魂水晶";
    public override string PluralName => "职业灵魂水晶";
    public override string HelpText => "该灵魂水晶是否用于特职转职或由特职装备？";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        if (asSource.ParentClassJob != null)
        {
            ImGui.Text("将 " + asSource.ParentClassJob.Base.Name.ExtractText() + " 转职为 " + asSource.ClassJob.Base.Name.ExtractText());
        }
        else
        {
            ImGui.Text("解锁 " + asSource.ClassJob.Base.Name.ExtractText() + " 特职");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.ClassJob.Base.Name.ExtractText();
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = AsSource(source);
        return asSource.ClassJob.Icon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        if (asSource.ParentClassJob != null)
        {
            return "将 " + asSource.ParentClassJob.Base.Name.ExtractText() + " 转职为 " + asSource.ClassJob.Base.Name.ExtractText();
        }

        return "解锁 " + asSource.ClassJob.Base.Name.ExtractText() + " 特职";
    };
}