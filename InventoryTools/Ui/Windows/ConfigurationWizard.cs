using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Logic.Features;
using InventoryTools.Mediator;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Blocks;
using InventoryTools.Ui.Config.Layouts;
using Microsoft.Extensions.Logging;
using OtterGui.Raii;

namespace InventoryTools.Ui;

public class ConfigurationWizard : GenericWindow
{
    private readonly ConfigurationWizardService _configurationWizardService;
    private readonly InventoryToolsConfiguration _configuration;

    public ConfigurationWizard(ILogger<ConfigurationWizard> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, ConfigurationWizardService configurationWizardService, IEnumerable<IContentLayout> contentLayouts, IEnumerable<ISetting> settings, ConfigNavigationState navigationState, string name = "配置向导") : base(logger, mediator, imGuiService, configuration, name)
    {
        _configurationWizardService = configurationWizardService;
        _configuration = configuration;
        _navigationState = navigationState;

        var byType = new Dictionary<Type, ISetting>();
        foreach (var setting in settings) byType[setting.GetType()] = setting;
        _settings = byType;

        _introPages = contentLayouts.Select(c => c.Build())
            .Where(c => c.Key.StartsWith("intro/"))
            .OrderBy(c => IntroPageOrder.IndexOf(c.Key) == -1 ? int.MaxValue : IntroPageOrder.IndexOf(c.Key))
            .ToList();
    }

    private readonly ConfigNavigationState _navigationState;
    private readonly IReadOnlyDictionary<Type, ISetting> _settings;
    private readonly List<PageLayout> _introPages;

    private static readonly List<string> IntroPageOrder =
    [
        "intro/welcome", "intro/inventories", "intro/lists", "intro/crafting", "intro/compendium",
        "intro/windows", "intro/defaults",
    ];

    private static readonly List<string> FeatureOrder =
    [
        "feature/basic", "feature/layout", "feature/sample-lists", "feature/tooltips",
        "feature/marketboard", "feature/context-menu", "feature/hotkeys", "feature/craft-notifications",
    ];

    private List<IFeature> _availableFeatures = new();
    private int _currentFeature;

    private bool _showIntro;

    private List<PageLayout> ActiveIntroPages => _showIntro ? _introPages : [];

    public override void Initialize()
    {
        WindowName = "配置向导";
        Key = "wizard";
        _availableFeatures = _configurationWizardService.GetNewFeatures()
            .OrderBy(c => FeatureOrder.IndexOf(c.Content.Key) == -1
                ? int.MaxValue
                : FeatureOrder.IndexOf(c.Content.Key))
            .ToList();
        _showIntro = !_configurationWizardService.ConfiguredOnce;
    }

    private int StepCount => ActiveIntroPages.Count + _availableFeatures.Count;

    private PageLayout? IntroPageForStep(int step)
    {
        var index = step - 1;
        var intro = ActiveIntroPages;
        return index >= 0 && index < intro.Count ? intro[index] : null;
    }

    private IFeature? FeatureForStep(int step)
    {
        var index = step - 1 - ActiveIntroPages.Count;
        return index >= 0 && index < _availableFeatures.Count ? _availableFeatures[index] : null;
    }

    public override string GenericKey => "wizard";
    public override string GenericName => "配置向导";
    public override bool DestroyOnClose => true;
    public override bool SaveState => false;
    public override Vector2? DefaultSize { get; } = new(800, 650);
    public override Vector2? MaxSize { get; } = new(1000, 1000);
    public override Vector2? MinSize { get; } = new(750, 350);

    private bool CanGoPrevious => _currentFeature != 0;
    private bool CanGoNext => StepCount != 0 && _currentFeature != StepCount;

    private int _furthestStep;

    private void GoToStep(int step)
    {
        _currentFeature = Math.Clamp(step, 0, StepCount);
        _furthestStep = Math.Max(_furthestStep, _currentFeature);
        ScrollToFirstNewSetting();
    }

    private void ScrollToFirstNewSetting()
    {
        var feature = FeatureForStep(_currentFeature);
        if (feature == null)
        {
            return;
        }

        var newSettings = _configurationWizardService.GetNewSettingTypes(feature);
        if (newSettings.Count == 0)
        {
            return;
        }

        var first = FirstSettingIn(feature.Content, newSettings);
        if (first != null)
        {
            _navigationState.RequestScrollTo(first, false);
        }
    }

    private Type? FirstSettingIn(IConfigBlock block, IReadOnlySet<Type> wanted)
    {
        if (block is SettingBlock settingNode && wanted.Contains(settingNode.SettingType))
        {
            return settingNode.SettingType;
        }

        foreach (var child in block.Children)
        {
            var found = FirstSettingIn(child, wanted);
            if (found != null)
            {
                return found;
            }
        }

        if (block is PageLayout pageNode)
        {
            foreach (var subPage in pageNode.SubPages)
            {
                var found = FirstSettingIn(subPage, wanted);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    private void NextStep()
    {
        GoToStep(_currentFeature + 1);
    }

    private void PreviousStep()
    {
        GoToStep(_currentFeature - 1);
    }

    private void DrawPageHeader(string title)
    {
        ImGui.TextUnformatted(title);
        ImGui.SameLine();
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
        {
            var label = $"第 {_currentFeature} 步，共 {StepCount} 步";
            ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize(label).X);
            ImGui.TextUnformatted(label);
        }

        ImGui.Separator();
        ImGui.Spacing();
    }

    private void DrawSideBarGroup(string label)
    {
        ImGui.Spacing();
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
        {
            ImGui.TextUnformatted(label);
        }

        ImGui.Separator();
    }

    private void DrawSideBarItem(string label, int step)
    {
        var isCurrent = step == _currentFeature;
        var isVisited = step <= _furthestStep;
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen, isCurrent))
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3, !isCurrent && !isVisited))
        {
            if (ImGui.Selectable(label, isCurrent)) GoToStep(step);
        }
    }

    public override void DrawWindow()
    {
        using (var sideBar = ImRaii.Child("sideBar", new Vector2(185, 0) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (sideBar)
            {
                using (var sideBarMenu = ImRaii.Child("sideBarMenu",
                           new Vector2(185, -120) * ImGui.GetIO().FontGlobalScale, false))
                {
                    if (sideBarMenu)
                    {
                        DrawSideBarItem("欢迎", 0);

                        var step = 0;
                        var intro = ActiveIntroPages;
                        if (intro.Count != 0)
                        {
                            DrawSideBarGroup("介绍");
                            foreach (var introPage in intro) DrawSideBarItem(introPage.Name, ++step);
                        }

                        if (_availableFeatures.Count != 0)
                        {
                            DrawSideBarGroup("配置");
                            foreach (var feature in _availableFeatures) DrawSideBarItem(feature.Name, ++step);
                        }
                    }
                }
                using (var sideBarImage = ImRaii.Child("sideBarImage",
                           new Vector2(185, 0) * ImGui.GetIO().FontGlobalScale, false))
                {
                    if (sideBarImage)
                    {
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 30f);
                        ImGui.Image(ImGuiService.GetImageTexture("icon").Handle, new (100,100));
                    }
                }
            }
        }
        ImGui.SameLine();
        using (var mainWindow = ImRaii.Child("mainWindow", new Vector2(0, 0)))
        {
            if (mainWindow)
            {
                using (var mainContainer = ImRaii.Child("mainContainer", new Vector2(-1, -40) * ImGui.GetIO().FontGlobalScale, true))
                {
                    if (mainContainer)
                    {
                        if (_currentFeature == 0) DrawWelcome();
                        else DrawStep();
                    }
                }

                using (var nextPrevBar = ImRaii.Child("nextPrevBar", new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, true, ImGuiWindowFlags.NoScrollbar))
                {
                    if (nextPrevBar) DrawFooter();
                }
            }
        }
    }

    private void DrawWelcome()
    {
        if (_configurationWizardService.ConfiguredOnce)
        {
            ImGui.TextWrapped("欢迎回到 Allagan Tools 配置向导。");
            ImGui.Separator();
            ImGui.TextWrapped(
                "有新功能可供配置，而你选择了在出现这种情况时显示此窗口。");
            ImGui.NewLine();

            if (!_showIntro && _introPages.Count != 0)
            {
                ImGui.TextWrapped("你也可以重新浏览介绍，了解本插件的功能。");
                if (ImGui.Button("重新浏览介绍"))
                {
                    _showIntro = true;
                    NextStep();
                }
            }

            return;
        }

        ImGui.TextWrapped("欢迎使用 Allagan Tools 配置向导。");
        ImGui.Separator();
        ImGui.TextWrapped(
            "它将引导你了解本插件的功能，并帮助你设置最常用的特性。只需几分钟，之后所有内容都可以在设置窗口中修改。");
        ImGui.NewLine();
        ImGui.TextWrapped("如果你是老用户，可以直接关闭此窗口。");
        ImGui.NewLine();
        if (ImGui.Button("打开帮助")) MediatorService.Publish(new ToggleGenericWindowMessage(typeof(HelpWindow)));
    }

    private void DrawStep()
    {
        var feature = FeatureForStep(_currentFeature);
        var page = IntroPageForStep(_currentFeature) ?? feature?.Content;
        if (page == null) return;

        DrawPageHeader(page.Name);

        var newSettings = feature == null ? null : _configurationWizardService.GetNewSettingTypes(feature);
        var context = new ConfigDrawContext(_configuration, _settings, _navigationState, ImGuiService, newSettings);
        page.Draw(context);
        while (context.Messages.Count != 0)
        {
            MediatorService.Publish(context.Messages.Dequeue());
        }
    }

    private void DrawFooter()
    {
        var isWelcome = _currentFeature == 0;
        var isLast = _currentFeature == StepCount;

        // Shown where the choice is actually being made, and once more before it takes effect.
        if (isWelcome || isLast)
        {
            var showOnNewFeatures = _configuration.ShowWizardNewFeatures;
            if (ImGui.Checkbox("有新功能发布时显示此向导", ref showOnNewFeatures))
            {
                _configuration.ShowWizardNewFeatures = showOnNewFeatures;
            }

            ImGui.SameLine();
        }

        var buttons = new List<(string Label, Action OnClick, bool Enabled)>();
        if (isWelcome)
        {
            buttons.Add(("关闭", Close, true));
            if (StepCount == 0)
            {
                buttons.Add(("完成", Finish, true));
            }
            else
            {
                buttons.Add(("继续", NextStep, true));
            }
        }
        else
        {
            buttons.Add(("上一步", PreviousStep, CanGoPrevious));
            if (CanGoNext)
            {
                buttons.Add(("下一步", NextStep, true));
            }
            else
            {
                buttons.Add(("完成", Finish, true));
            }
        }

        DrawRightAligned(buttons);
    }

    private void DrawRightAligned(List<(string Label, Action OnClick, bool Enabled)> buttons)
    {
        var style = ImGui.GetStyle();
        var total = buttons.Sum(c => ImGui.CalcTextSize(c.Label).X + (style.FramePadding.X * 2))
                    + (style.ItemSpacing.X * (buttons.Count - 1));

        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - total);

        for (var index = 0; index < buttons.Count; index++)
        {
            if (index != 0)
            {
                ImGui.SameLine();
            }

            var button = buttons[index];
            using (ImRaii.Disabled(!button.Enabled))
            {
                if (ImGui.Button(button.Label))
                {
                    button.OnClick();
                }
            }
        }
    }

    private void Finish()
    {
        this.Close();
        _currentFeature = 0;
        foreach (var feature in _availableFeatures)
        {
            feature.OnFinish();
        }

        if (!_configurationWizardService.ConfiguredOnce)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(FiltersWindow)));
        }
        _configurationWizardService.MarkFeaturesSeen();

    }

    public override void Invalidate()
    {
    }

    public override FilterConfiguration? SelectedConfiguration => null;
}