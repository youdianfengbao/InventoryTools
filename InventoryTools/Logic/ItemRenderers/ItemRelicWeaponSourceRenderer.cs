using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Misc;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using InventoryTools.Localizers;
using LuminaSupplemental.Excel.Model;

namespace InventoryTools.Logic.ItemRenderers;

public abstract class ItemRelicWeaponSourceRenderer<T> : ItemInfoRenderer<T> where T : ItemRelicWeaponSource
{
    private readonly ILocalizer<RelicWeaponType> _relicWeaponTypeLocalizer;

    public ItemRelicWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _relicWeaponTypeLocalizer = relicWeaponTypeLocalizer;
    }

    public override RendererType RendererType => RendererType.Use;
    public override bool ShouldGroup => false;

    public override IReadOnlyList<ItemInfoRenderCategory>? Categories => [ItemInfoRenderCategory.RelicWeapon];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.TextUnformatted("职业：" + asSource.RelicWeapon.ClassJob.Value.Name.ToImGuiString().ToTitleCase());
        this.DrawForms("形态：", asSource.Forms);
    };

    public void DrawForms(string sectionName, IReadOnlyList<RelicWeapon> items)
    {
        if (items.Count == 0)
        {
            return;
        }
        ImGui.TextUnformatted(sectionName);
        using (ImRaii.PushIndent())
        {
            using (var table = ImRaii.Table("forms", items.Any(c => c.OffhandItemId != 0) ? 3 : 2))
            {
                if (table)
                {
                    foreach (var relicWeapon in items)
                    {
                        if (relicWeapon.ItemId == 0)
                            continue;

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{_relicWeaponTypeLocalizer.Format(relicWeapon.Type)}");
                        ImGui.TableNextColumn();
                        var item = ItemSheet.GetRow(relicWeapon.ItemId);
                        ImGui.Image(
                            TextureProvider
                                .GetFromGameIcon(new GameIconLookup(item.Icon))
                                .GetWrapOrEmpty().Handle,
                            new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale
                        );
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{item.NameString}");
                        if (relicWeapon.OffhandItemId != 0)
                        {
                            ImGui.TableNextColumn();
                            item = ItemSheet.GetRow(relicWeapon.OffhandItemId);
                            ImGui.Image(
                                TextureProvider
                                    .GetFromGameIcon(new GameIconLookup(item.Icon))
                                    .GetWrapOrEmpty().Handle,
                                new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale
                            );
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{item.NameString}");
                        }
                    }
                }
            }
        }
    }

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return string.Join(", ", asSource.Items.Select(c => c.NameString));
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.WeaponIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return string.Join(", ", asSource.Items.Select(c => c.NameString));
    };

    public override Func<ItemSource, (Type, uint)>? RelatedType => source =>
    {
        var asSource = AsSource(source);
        return (asSource.RelicWeapon.GetType(), asSource.RelicWeapon.RowId);
    };
}


public class ItemZodiacWeaponSourceRenderer : ItemRelicWeaponSourceRenderer<ItemZodiacWeaponSource>
{
    public ItemZodiacWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet, relicWeaponTypeLocalizer)
    {
    }

    public override ItemInfoType Type => ItemInfoType.ZodiacWeapon;
    public override string SingularName => "黄道武器";
    public override string HelpText => "该物品是否为黄道武器？";
}

public class ItemAnimaWeaponSourceRenderer : ItemRelicWeaponSourceRenderer<ItemAnimaWeaponSource>
{
    public ItemAnimaWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet, relicWeaponTypeLocalizer)
    {
    }

    public override ItemInfoType Type => ItemInfoType.AnimaWeapon;
    public override string SingularName => "阿尼玛武器";
    public override string HelpText => "该物品是否为阿尼玛武器？";
}

public class ItemEurekanWeaponSourceRenderer : ItemRelicWeaponSourceRenderer<ItemEurekanWeaponSource>
{
    public ItemEurekanWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet, relicWeaponTypeLocalizer)
    {
    }

    public override ItemInfoType Type => ItemInfoType.EurekanWeapon;
    public override string SingularName => "优雷卡武器";
    public override string HelpText => "该物品是否为优雷卡武器？";
}

public class ItemResistanceWeaponSourceRenderer : ItemRelicWeaponSourceRenderer<ItemResistanceWeaponSource>
{
    public ItemResistanceWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet, relicWeaponTypeLocalizer)
    {
    }

    public override ItemInfoType Type => ItemInfoType.ResistanceWeapon;
    public override string SingularName => "抗争武器";
    public override string HelpText => "该物品是否为抗争武器？";
}

public class ItemMandervilleWeaponSourceRenderer : ItemRelicWeaponSourceRenderer<ItemMandervilleWeaponSource>
{
    public ItemMandervilleWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet, relicWeaponTypeLocalizer)
    {
    }

    public override ItemInfoType Type => ItemInfoType.MandervilleWeapon;
    public override string SingularName => "曼德维尔武器";
    public override string HelpText => "该物品是否为曼德维尔武器？";
}

public class ItemPhantomWeaponSourceRenderer : ItemRelicWeaponSourceRenderer<ItemPhantomWeaponSource>
{
    public ItemPhantomWeaponSourceRenderer(ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface, ItemSheet itemSheet, MapSheet mapSheet, ILocalizer<RelicWeaponType> relicWeaponTypeLocalizer) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet, relicWeaponTypeLocalizer)
    {
    }

    public override ItemInfoType Type => ItemInfoType.PhantomWeapon;
    public override string SingularName => "幻影武器";
    public override string HelpText => "该物品是否为幻影武器？";
}
