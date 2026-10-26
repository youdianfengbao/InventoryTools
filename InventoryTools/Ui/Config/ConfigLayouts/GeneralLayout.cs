using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class GeneralLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("general", "常规",
            Section("保存",
                Paragraph("亚拉戈工具会自行记录你的库存数据。以下选项控制该记录写入磁盘的频率。"),
                Setting<AutoSaveSetting>("自动保存"),
                EnabledBy<AutoSaveSetting>(
                    Setting<AutoSaveTimeSetting>("保存频率")),
                Setting<PersistDataSetting>()),
            Section("集成",
                Paragraph("从插件窗口之外访问亚拉戈工具的途径。"),
                Setting<AddTitleMenuButtonSetting>("在主菜单添加按钮"),
                Setting<CompendiumWotsitSetting>("在 Wotsit 中列出插件窗口")),
            Section("集成",
                Paragraph("从插件窗口之外访问亚拉戈工具的途径。"),
                Setting<ActiveCraftListSetting>())
        );
    }
}
