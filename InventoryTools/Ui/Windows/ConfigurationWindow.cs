using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.Interface.Widgets;
using AllaganLib.Shared.Extensions;
using Autofac;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.MenuItems;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using InventoryTools.Ui.Widgets;
using OtterGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using InventoryTools.Extensions;
using InventoryTools.Logic.Features;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.ConfigLayouts;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Ui.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using ImGuiUtil = OtterGui.ImGuiUtil;

namespace InventoryTools.Ui
{
    public class ConfigurationWindow : GenericWindow, IMenuWindow
    {
        private readonly IPluginLog _pluginLog;
        private readonly ConfigurationWizardService _configurationWizardService;
        private readonly IChatUtilities _chatUtilities;
        private readonly PluginLogic _pluginLogic;
        private readonly IListService _listService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly LayoutConfigPage.Factory _layoutPageFactory;
        private readonly SettingCoverageService _settingCoverageService;
        private readonly ConfigNavigationState _configNavigationState;
        private readonly ConfigSearchService _configSearchService = new();
        private string _searchQuery = string.Empty;
        private readonly FilterConfiguration.Factory _filterConfigurationFactory;
        private readonly IEnumerable<ISampleFilter> _sampleFilters;
        private readonly Func<Type, IConfigPage> _configPageFactory;
        private readonly Func<FilterConfiguration, FilterPage> _filterPageFactory;
        private readonly IComponentContext _context;
        private readonly InventoryToolsConfiguration _configuration;
        private readonly VerticalSplitter _verticalSplitter;
        private readonly IFont _font;
        private IEnumerable<IMenuWindow>? _menuWindows;
        private FilterConfiguration? _nextFilter;

        public ConfigurationWindow(ILogger<ConfigurationWindow> logger,
            IPluginLog pluginLog,
            MediatorService mediator,
            ImGuiService imGuiService,
            InventoryToolsConfiguration configuration,
            ConfigurationWizardService configurationWizardService,
            IChatUtilities chatUtilities,
            PluginLogic pluginLogic,
            IListService listService,
            IServiceScopeFactory serviceScopeFactory,
            Func<Type, IConfigPage> configPageFactory,
            Func<FilterConfiguration, FilterPage> filterPageFactory,
            LayoutConfigPage.Factory layoutPageFactory,
            SettingCoverageService settingCoverageService,
            ConfigNavigationState configNavigationState,
            FilterConfiguration.Factory filterConfigurationFactory,
            IEnumerable<ISampleFilter> sampleFilters,
            IComponentContext context,
            IFont font) : base(logger,
            mediator,
            imGuiService,
            configuration,
            "配置窗口")
        {
            _pluginLog = pluginLog;
            _configurationWizardService = configurationWizardService;
            _chatUtilities = chatUtilities;
            _pluginLogic = pluginLogic;
            _listService = listService;
            _serviceScopeFactory = serviceScopeFactory;
            _layoutPageFactory = layoutPageFactory;
            _settingCoverageService = settingCoverageService;
            _configNavigationState = configNavigationState;
            _filterConfigurationFactory = filterConfigurationFactory;
            _sampleFilters = sampleFilters;
            _configPageFactory = configPageFactory;
            _filterPageFactory = filterPageFactory;
            _context = context;
            _configuration = configuration;
            _verticalSplitter = new VerticalSplitter(250, new Vector2(200, 400));
            _font = font;
            this.Flags = ImGuiWindowFlags.MenuBar;
        }

        public override void Initialize()
        {
            WindowName = "配置";
            Key = "configuration";
            #if DEBUG
            _settingCoverageService.Report();
            #endif
            _configPages = new List<IConfigPage>();
            _configPages.Add(new SeparatorPageItem("设置"));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<GeneralLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<WindowsAndListsLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<HighlightingLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<ItemIconsLayout>()));
            _configPages.Add(new SeparatorPageItem("模块", true));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<MarketBoardLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<TooltipsLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<ContextMenuLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<HotkeysLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<CraftOverlayLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<EquipmentRecommendationLayout>()));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<HistoryLayout>()));
            _configPages.Add(new SeparatorPageItem(null, true));
            _configPages.Add(_layoutPageFactory.Invoke(_context.Resolve<TroubleshootingLayout>()));
            _configPages.Add(new SeparatorPageItem("数据", true));
            _configPages.Add(_configPageFactory.Invoke(typeof(ListsPage)));
            _configPages.Add(_configPageFactory.Invoke(typeof(CharacterRetainerPage)));

            _addFilterMenu = new PopupMenu("addFilter", PopupMenu.PopupMenuButtons.LeftRight,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectableAskName("搜索清单", "adf1", "新搜索清单", AddSearchFilter, "创建一个新清单，用于在角色和雇员库存中搜索特定物品。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("排序清单", "af2", "新排序筛选器", AddSortFilter, "创建一个新清单，用于在角色和雇员库存中搜索特定物品，并确定它们应被移动到哪里。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("游戏物品清单", "af3", "新游戏物品清单", AddGameItemFilter, "创建一个清单，用于搜索游戏中的所有物品。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("历史清单", "af4", "新历史记录清单", AddHistoryFilter, "创建一个清单，用于查看库存变化的历史数据。"),
                });

            _addSampleMenu = new PopupMenu("addSampleFilter", PopupMenu.PopupMenuButtons.LeftRight, []);

            var sampleId = 0;
            foreach (var sampleFilter in _sampleFilters)
            {
                if (sampleFilter.SampleFilterType == SampleFilterType.Default)
                {
                    _addSampleMenu.Items.Add(new PopupMenu.PopupMenuItemSelectableAskName(sampleFilter.Name,
                        $"sf{sampleId}", sampleFilter.SampleDefaultName, (newName, id) =>
                        {
                            var createdFilter = sampleFilter.AddFilter();
                            createdFilter.Name = newName;
                        }, sampleFilter.SampleDescription));
                    sampleId++;
                }
            }

            _addSampleMenu.Items.Add(new PopupMenu.PopupMenuItemSeparator());

            foreach (var sampleFilter in _sampleFilters)
            {
                if (sampleFilter.SampleFilterType == SampleFilterType.Sample)
                {
                    _addSampleMenu.Items.Add(new PopupMenu.PopupMenuItemSelectableAskName(sampleFilter.Name,
                        $"sf{sampleId}", sampleFilter.SampleDefaultName, (newName, id) =>
                        {
                            var createdFilter = sampleFilter.AddFilter();
                            createdFilter.Name = newName;
                        }, sampleFilter.SampleDescription));
                    sampleId++;
                }
            }

            _settingsMenu = new PopupMenu("configMenu", PopupMenu.PopupMenuButtons.All,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectable("物品窗口", "filters", OpenFiltersWindow,"打开物品窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("制作窗口", "crafts", OpenCraftsWindow,"打开制作窗口。"),
                    new PopupMenu.PopupMenuItemSeparator(),
                    new PopupMenu.PopupMenuItemSelectable("魔物窗口", "mobs", OpenMobsWindow,"打开魔物窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("NPC 窗口", "npcs", OpenNpcsWindow,"打开 NPC 窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("副本窗口", "duties", OpenDutiesWindow,"打开副本窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("飞空艇窗口", "airships", OpenAirshipsWindow,"打开飞空艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("潜水艇窗口", "submarines", OpenSubmarinesWindow,"打开潜水艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("雇员探索窗口", "ventures", OpenRetainerVenturesWindow,"打开雇员探索窗口。"),
                    new PopupMenu.PopupMenuItemSeparator(),
                    new PopupMenu.PopupMenuItemSelectable("帮助", "help", OpenHelpWindow,"打开帮助窗口。"),
                });

            _wizardMenu = new PopupMenu("wizardMenu", PopupMenu.PopupMenuButtons.All,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectable("配置新设置", "configureNew", ConfigureNewSettings,"配置新设置。"),
                    new PopupMenu.PopupMenuItemSelectable("配置全部设置", "configureAll", ConfigureAllSettings,"配置全部设置。"),
                });
            _menuWindows = _context.Resolve<IEnumerable<IMenuWindow>>().OrderBy(c => c.GenericName).Where(c => c.GetType() != this.GetType());

            GenerateFilterPages();
            CheckDuplicateKeys();
            RebuildSearchIndex();
            MediatorService.Subscribe<ListInvalidatedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRepositionedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListAddedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRemovedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ConfigurationWindowEditFilter>(this,  message =>
            {
                Invalidate();
                SetActiveFilter(message.filter);
            });
            MediatorService.Subscribe<ListInvalidatedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRepositionedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListAddedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRemovedMessage>(this, _ => Invalidate());
        }

        private void ListInvalidated(ListInvalidatedMessage obj)
        {
            Invalidate();
        }



        private PopupMenu _wizardMenu = null!;

        private void ConfigureAllSettings(string obj)
        {
            _configurationWizardService.ClearFeaturesSeen();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
        }

        private void ConfigureNewSettings(string obj)
        {
            if (_configurationWizardService.HasNewFeatures)
            {
                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
            }
            else
            {
                _chatUtilities.Print("没有可配置的新设置。");
            }
        }

        private PopupMenu _addFilterMenu = null!;
        private PopupMenu _addSampleMenu = null!;
        private PopupMenu _settingsMenu = null!;

        private void OpenCraftsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(CraftsWindow)));
        }

        private void OpenFiltersWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(FiltersWindow)));
        }

        private void OpenHelpWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow)));
        }

        private void OpenDutiesWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(DutiesWindow)));
        }

        private void OpenAirshipsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(AirshipsWindow)));
        }

        private void OpenSubmarinesWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(SubmarinesWindow)));
        }

        private void OpenRetainerVenturesWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(RetainerTasksWindow)));
        }

        private void OpenMobsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(BNpcsWindow)));
        }

        private void OpenNpcsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ENpcsWindow)));
        }

        private Dictionary<FilterConfiguration, PopupMenu> _popupMenus = new();
        public PopupMenu GetFilterMenu(FilterConfiguration configuration)
        {
            if (!_popupMenus.ContainsKey(configuration))
            {
                _popupMenus[configuration] = new PopupMenu("fm" + configuration.Key, PopupMenu.PopupMenuButtons.Right,
                    new List<PopupMenu.IPopupMenuItem>()
                    {
                        new PopupMenu.PopupMenuItemSelectableAskName("创建副本", "df_" + configuration.Key, configuration.Name, DuplicateFilter, "复制该筛选器。"),
                        new PopupMenu.PopupMenuItemSelectable("上移", "mu_" + configuration.Key, MoveFilterUp, "将该筛选器上移。"),
                        new PopupMenu.PopupMenuItemSelectable("下移", "md_" + configuration.Key, MoveFilterDown, "将该筛选器下移。"),
                        new PopupMenu.PopupMenuItemSelectableConfirm("移除", "rf_" + configuration.Key, "确定要移除此筛选器吗？", RemoveFilter, "移除该筛选器。"),
                    }
                );
            }

            return _popupMenus[configuration];
        }

        private void RemoveFilter(string id, bool confirmed)
        {
            if (confirmed)
            {
                id = id.Replace("rf_", "");
                var existingFilter = _listService.GetListByKey(id);
                if (existingFilter != null)
                {
                    if (_filterPages.TryGetValue(existingFilter.Key, out var removedPage)
                        && ConfigSelectedConfigurationPageKey == removedPage.Key)
                    {
                        ConfigSelectedConfigurationPageKey = null;
                    }

                    _listService.RemoveList(existingFilter);
                }
            }
        }

        private void MoveFilterDown(string id)
        {
            id = id.Replace("md_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                _listService.MoveListDown(existingFilter);
            }
        }

        private void MoveFilterUp(string id)
        {
            id = id.Replace("mu_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                _listService.MoveListUp(existingFilter);
            }
        }

        private void DuplicateFilter(string filterName, string id)
        {
            id = id.Replace("df_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                var duplicatedFilter = _listService.DuplicateList(existingFilter, filterName);
                SetNewFilterActive(duplicatedFilter);
            }
        }

        private void AddSearchFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.SearchFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive(filterConfiguration);
        }

        private void AddHistoryFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.HistoryFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive(filterConfiguration);
        }

        private void AddGameItemFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.GameItemFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive(filterConfiguration);
        }

        private void AddSortFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.SortingFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive(filterConfiguration);
        }

        private void CheckDuplicateKeys()
        {
            var seen = new HashSet<string>();
            foreach (var page in SelectablePages())
            {
                if (!seen.Add(page.Key))
                {
                    Logger.LogError(
                        "Two configuration pages share the key {Key}; the second ({Name}) cannot be navigated to.",
                        page.Key,
                        page.Name);
                }
            }
        }

        private string? ConfigSelectedConfigurationPageKey
        {
            get => _configuration.SelectedConfigurationPageKey;
            set => _configuration.SelectedConfigurationPageKey = value;
        }

        private IEnumerable<IConfigPage> SelectablePages()
        {
            foreach (var configPage in _configPages)
            {
                if (configPage.IsMenuItem)
                {
                    continue;
                }

                if (configPage.ChildPages != null)
                {
                    foreach (var childPage in configPage.ChildPages)
                    {
                        yield return childPage;
                    }
                }
                else
                {
                    yield return configPage;
                }
            }

            foreach (var filterPage in _filterPages.Values)
            {
                yield return filterPage;
            }
        }

        private IConfigPage? SelectedPage()
        {
            IConfigPage? first = null;
            foreach (var page in SelectablePages())
            {
                first ??= page;
                if (page.Key == ConfigSelectedConfigurationPageKey)
                {
                    return page;
                }
            }

            return first;
        }

        public void SetActiveFilter(FilterConfiguration configuration)
        {
            if (_filterPages.ContainsKey(configuration.Key))
            {
                _nextFilter = configuration;
            }
        }

        public void GenerateFilterPages()
        {
            var filterConfigurations = _listService.Lists.Where(c => c.FilterType != FilterType.CraftFilter);
            var filterPages = new Dictionary<string, IConfigPage>();
            foreach (var filter in filterConfigurations)
            {
                if (!filterPages.ContainsKey(filter.Key))
                {
                    filterPages.Add(filter.Key, _filterPageFactory.Invoke(filter));
                }
            }

            _filterPages = filterPages;
        }

        public override bool SaveState => true;
        public override Vector2? DefaultSize { get; } = new(700, 700);
        public override Vector2? MaxSize { get; } = new(2000, 2000);
        public override Vector2? MinSize { get; } = new(200, 200);
        public override string GenericKey => "configuration";
        public override string GenericName => "配置";
        public override bool DestroyOnClose => true;
        private List<IConfigPage> _configPages = null!;
        public Dictionary<string, IConfigPage> _filterPages = new Dictionary<string,IConfigPage>();


        private void SetNewFilterActive(FilterConfiguration filterConfiguration)
        {
            _nextFilter = filterConfiguration;
        }

        private void DrawMenuBar()
        {
            using (var menuBar = ImRaii.MenuBar())
            {
                if (menuBar)
                {
                    using (var menu = ImRaii.Menu("文件"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("报告问题"))
                            {
                                "https://github.com/Critical-Impact/AllaganMarket".OpenBrowser();
                            }

                            if (ImGui.MenuItem("更新日志"))
                            {
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ChangelogWindow)));
                            }

                            if (ImGui.MenuItem("帮助"))
                            {
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow)));
                            }

                            if (ImGui.MenuItem("启用详细日志", "",
                                    this._pluginLog.MinimumLogLevel == LogEventLevel.Verbose))
                            {
                                if (this._pluginLog.MinimumLogLevel == LogEventLevel.Verbose)
                                {
                                    this._pluginLog.MinimumLogLevel = LogEventLevel.Debug;
                                }
                                else
                                {
                                    this._pluginLog.MinimumLogLevel = LogEventLevel.Verbose;
                                }
                            }

                            if (ImGui.MenuItem("生成支持转储"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(SupportDumpWindow)));
                            }

                            if (ImGui.MenuItem("Ko-Fi"))
                            {
                                "https://ko-fi.com/critical_impact".OpenBrowser();
                            }

                            if (ImGui.MenuItem("关闭"))
                            {
                                this.IsOpen = false;
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("向导"))
                    {
                        if (menu)
                        {
                            var hasNewFeatures = this._configurationWizardService.HasNewFeatures;
                            using var disabled = ImRaii.Disabled(!hasNewFeatures);
                            if (ImGui.MenuItem("配置新功能"))
                            {
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
                            }

                            disabled.Dispose();

                            if (ImGui.MenuItem("重新配置所有功能"))
                            {
                                this._configurationWizardService.ClearFeaturesSeen();
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("窗口"))
                    {
                        if (menu)
                        {
                            if (_menuWindows != null)
                            {
                                foreach (var window in _menuWindows)
                                {
                                    if (ImGui.MenuItem(window.GenericName))
                                    {
                                        MediatorService.Publish(new OpenGenericWindowMessage(window.GetType()));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void DrawWindow()
        {
            DrawMenuBar();
            DrawSearchBar();
            _verticalSplitter.Draw(DrawSideBar, DrawMainWindow);
        }

        private void DrawSearchBar()
        {
            ImGui.SetNextItemWidth(-60 * ImGui.GetIO().FontGlobalScale);
            ImGui.InputTextWithHint("##configSearch", "搜索设置...", ref _searchQuery, 100);

            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                ImGui.SameLine();
                if (ImGui.Button("清空##configSearchClear"))
                {
                    _searchQuery = string.Empty;
                }
            }

            ImGui.Separator();
        }

        private void DrawSearchResults()
        {
            var results = _configSearchService.Search(_searchQuery);
            if (results.Count == 0)
            {
                ImGui.TextWrapped($"没有匹配“{_searchQuery}”的设置。");
                return;
            }

            foreach (var result in results)
            {
                using (ImRaii.PushId(result.SettingType.Name))
                {
                    if (ImGui.Selectable(result.DisplayName))
                    {
                        ConfigSelectedConfigurationPageKey = result.PageKey;
                        _configNavigationState.RequestScrollTo(result.SettingType);
                        _searchQuery = string.Empty;
                    }

                    using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
                    {
                        ImGui.TextUnformatted("    " + result.Breadcrumb);
                    }
                }

                ImGui.Separator();
            }
        }

        private void DrawMainWindow()
        {
            if (_nextFilter != null && _filterPages.TryGetValue(_nextFilter.Key, out var nextFilterPage))
            {
                ConfigSelectedConfigurationPageKey = nextFilterPage.Key;
                _nextFilter = null;
            }

            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                DrawSearchResults();
                return;
            }

            var currentConfigPage = SelectedPage();
            if (currentConfigPage != null)
            {
                MediatorService.Publish(currentConfigPage.Draw());
            }
        }

        private void DrawSideBar()
        {
            using (var menuChild = ImRaii.Child("Menu", new Vector2(0, -28) * ImGui.GetIO().FontGlobalScale,
                       false, ImGuiWindowFlags.NoSavedSettings))
            {
                if (menuChild.Success)
                {

                    for (var index = 0; index < _configPages.Count; index++)
                    {
                        var configPage = _configPages[index];
                        if (configPage.IsMenuItem)
                        {
                            MediatorService.Publish(configPage.Draw());
                        }
                        else
                        {
                            var hasChildren = configPage.ChildPages != null;
                            var isSelected = ConfigSelectedConfigurationPageKey == configPage.Key;
                            using (var node = ImRaii.TreeNode(configPage.Name, hasChildren ?  ImGuiTreeNodeFlags.None : isSelected ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.Leaf))
                            {
                                if (node)
                                {
                                    if (configPage.ChildPages != null)
                                    {
                                        foreach (var childPage in configPage.ChildPages)
                                        {
                                            isSelected = ConfigSelectedConfigurationPageKey == childPage.Key;

                                            using (var subNode = ImRaii.TreeNode(childPage.Name,
                                                       isSelected
                                                           ? ImGuiTreeNodeFlags.Selected |
                                                             ImGuiTreeNodeFlags.Bullet
                                                           : ImGuiTreeNodeFlags.Bullet))
                                            {
                                                if (subNode)
                                                {
                                                }
                                            }

                                            if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                                            {
                                                ConfigSelectedConfigurationPageKey = childPage.Key;
                                            }
                                        }
                                    }
                                }
                            }

                            if (!hasChildren)
                            {
                                if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                                {
                                    ConfigSelectedConfigurationPageKey = configPage.Key;
                                }
                            }
                        }
                    }

                    ImGui.NewLine();
                    ImGui.TextUnformatted("物品清单");
                    ImGui.Separator();

                    foreach (var item in _filterPages)
                    {
                        using (var subNode = ImRaii.TreeNode(item.Value.Name,
                                   ConfigSelectedConfigurationPageKey == item.Value.Key
                                       ? ImGuiTreeNodeFlags.Selected |
                                         ImGuiTreeNodeFlags.Leaf
                                       : ImGuiTreeNodeFlags.Leaf))
                        {
                            if (subNode)
                            {
                            }
                        }

                        if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                        {
                            ConfigSelectedConfigurationPageKey = item.Value.Key;
                        }

                        var filter = _listService.GetListByKey(item.Key);
                        if (filter != null)
                        {
                            GetFilterMenu(filter).Draw();
                        }

                    }
                }
            }

            using (var commandBarChild = ImRaii.Child("CommandBar",
                       new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale, false))
            {
                if (commandBarChild.Success)
                {

                    float height = ImGui.GetWindowSize().Y;
                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);

                    var cursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Plus, ref cursorX))
                    {

                    }

                    _addFilterMenu.Draw();
                    ImGuiUtil.HoverTooltip("添加新清单");

                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                    ImGui.SetCursorPosX(26 * ImGui.GetIO().FontGlobalScale);

                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Lightbulb, ref cursorX))
                    {

                    }

                    _addSampleMenu.Draw();
                    ImGuiUtil.HoverTooltip("添加示例筛选器");

                    var width = ImGui.GetCursorPosX();
                    width -= 24 * ImGui.GetIO().FontGlobalScale;

                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Bars, ref width))
                    {

                    }

                    _settingsMenu.Draw();


                    width -= 26 * ImGui.GetIO().FontGlobalScale;

                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.WandMagicSparkles, ref width))
                    {
                        _wizardMenu.Open();
                    }
                    _wizardMenu.Draw();


                    ImGuiUtil.HoverTooltip("启动配置向导。");
                }
            }
        }

        public override void Invalidate()
        {
            GenerateFilterPages();
            RebuildSearchIndex();
        }

        private void RebuildSearchIndex()
        {
            _configSearchService.BuildIndex(SelectablePages());
        }

        public override FilterConfiguration? SelectedConfiguration => null;
    }
}