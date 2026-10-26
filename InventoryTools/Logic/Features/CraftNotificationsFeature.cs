using System.Collections.Generic;
using System.Linq;
using InventoryTools.Logic.Filters;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.WizardSettings;
using InventoryTools.Services.Interfaces;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class CraftNotificationsFeature : Feature
{
    private readonly CraftReportCompletionOnlyFilter _completionOnlyFilter;
    private readonly CraftNotificationsCompletionOnlySetting _completionOnlySetting;
    private readonly InventoryToolsConfiguration _configuration;
    private readonly IListService _listService;
    private readonly CraftReportPlaySoundFilter _playSoundFilter;
    private readonly CraftNotificationsPlaySoundSetting _playSoundSetting;
    private readonly CraftReportProgressFilter _progressFilter;
    private readonly CraftNotificationsReportToChatSetting _reportToChatSetting;

    public CraftNotificationsFeature(IEnumerable<ISetting> settings,
        CraftNotificationsReportToChatSetting reportToChatSetting,
        CraftNotificationsPlaySoundSetting playSoundSetting,
        CraftNotificationsCompletionOnlySetting completionOnlySetting,
        CraftReportProgressFilter progressFilter,
        CraftReportPlaySoundFilter playSoundFilter,
        CraftReportCompletionOnlyFilter completionOnlyFilter,
        IListService listService,
        InventoryToolsConfiguration configuration) : base(settings)
    {
        _reportToChatSetting = reportToChatSetting;
        _playSoundSetting = playSoundSetting;
        _completionOnlySetting = completionOnlySetting;
        _progressFilter = progressFilter;
        _playSoundFilter = playSoundFilter;
        _completionOnlyFilter = completionOnlyFilter;
        _listService = listService;
        _configuration = configuration;
    }

    public override PageLayout Build()
    {
        return Page("feature/craft-notifications", "制作通知",
            Paragraph("插件可以在你为制作清单收集物品时报告进度。这些通知仅在制作清单处于启用状态时出现。"),
            Setting<CraftNotificationsReportToChatSetting>("在聊天窗口报告进度"),
            Setting<CraftNotificationsPlaySoundSetting>("物品齐备时播放提示音"),
            Setting<CraftNotificationsCompletionOnlySetting>("仅报告已完成的物品，而非每收集一件就报告"),
            Paragraph("完成向导后，插件会将这些设置应用到你的各个制作清单和默认制作清单。每份清单会保留自己的副本，可在该清单的设置中修改。")
        );
    }

    public override void OnFinish()
    {
        var reportToChat = _reportToChatSetting.CurrentValue(_configuration);
        var playSound = _playSoundSetting.CurrentValue(_configuration);
        var completionOnly = _completionOnlySetting.CurrentValue(_configuration);

        var lists = _listService.Lists
            .Where(c => c.FilterType == FilterType.CraftFilter)
            .ToList();
        var defaultList = _listService.GetDefaultCraftList();
        if (!lists.Contains(defaultList))
        {
            lists.Add(defaultList);
        }

        foreach (var list in lists)
        {
            _progressFilter.UpdateFilterConfiguration(list, reportToChat);
            _playSoundFilter.UpdateFilterConfiguration(list, playSound);
            _completionOnlyFilter.UpdateFilterConfiguration(list, completionOnly);
        }
    }
}