using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace InventoryTools.Localizers;

public class ContentTypeLocalizer : ILocalizer<ContentType>
{
    public string Format(ContentType instance)
    {
        return instance.RowId switch
        {
            20 => "新人馆",
            22 => "季节活动",
            23 => "无名岛",
            39 => "终焉之诗",
            _ => instance.Name.ToImGuiString()
        };
    }
}
