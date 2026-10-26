using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using Autofac;
using CriticalCommonLib;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Helpers;
using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures;
using Dalamud.Bindings.ImGui;
using InventoryTools.Extensions;
using InventoryTools.Groupers;
using InventoryTools.Logic;
using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Widgets;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Services;
using Lumina.Excel;
using InventoryTools.Compendium.Windows;
using InventoryTools.Lists;
using InventoryTools.Logic.Columns;
using InventoryTools.Logic.Filters;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.GameCraftSources;
using InventoryTools.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using ImGuiUtil = OtterGui.ImGuiUtil;
using InventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;
using PopupMenu = InventoryTools.Ui.Widgets.PopupMenu;
using StringExtensions = InventoryTools.Extensions.StringExtensions;

namespace InventoryTools.Ui
{
    public class CraftsWindow : GenericWindow, IMenuWindow
    {
        private readonly TableService _tableService;
        private readonly InventoryToolsConfiguration _configuration;
        private readonly IListService _listService;
        private readonly IFilterService _filterService;
        private readonly PluginLogic _pluginLogic;
        private readonly IUniversalis _universalis;
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IFileDialogManager _fileDialogManager;
        private readonly GameCraftSourceService _gameCraftSourceService;
        private readonly IChatUtilities _chatUtilities;
        private readonly ListImportExportService _importExportService;
        private readonly CraftWindowLayoutSetting _layoutSetting;
        private readonly IComponentContext _context;
        private readonly PopupService _popupService;
        private readonly CraftWindowViewSetting _craftWindowViewSetting;
        private readonly ITextureProvider _textureProvider;
        private readonly CraftSettingsColumn _craftSettingsColumn;
        private readonly IFont _font;
        private readonly ImGuiTooltipService _tooltipService;
        private readonly ImGuiMenuService _menuService;
        private readonly IClipboardService _clipboardService;
        private readonly IKeyState _keyState;
        private readonly ItemSheet _itemSheet;
        private readonly IFramework _framework;
        private readonly IEnumerable<ICompendiumType> _compendiumTypes;
        private readonly ICompendiumTypeFactory _compendiumTypeFactory;
        private readonly ICalloutService _calloutService;
        private readonly MissingRequirementsGrouper _missingRequirementsGrouper;
        private readonly IPluginLog _pluginLog;
        private IEnumerable<IMenuWindow> _menuWindows;
        private ThrottleDispatcher? _throttleDispatcher;

        public CraftsWindow(ILogger<CraftsWindow> logger,
            MediatorService mediator,
            ImGuiService imGuiService,
            InventoryToolsConfiguration configuration,
            TableService tableService,
            IListService listService,
            IFilterService filterService,
            PluginLogic pluginLogic,
            IUniversalis universalis,
            ICharacterMonitor characterMonitor,
            IFileDialogManager fileDialogManager,
            GameCraftSourceService gameCraftSourceService,
            IChatUtilities chatUtilities,
            ListImportExportService importExportService,
            CraftWindowLayoutSetting layoutSetting,
            IComponentContext context,
            PopupService popupService,
            CraftWindowViewSetting craftWindowViewSetting,
            ITextureProvider textureProvider,
            CraftSettingsColumn craftSettingsColumn,
            IFont font,
            ImGuiTooltipService tooltipService,
            ImGuiMenuService menuService,
            IClipboardService clipboardService,
            IKeyState keyState,
            ItemSheet itemSheet,
            IFramework framework,
            IEnumerable<ICompendiumType> compendiumTypes,
            ICompendiumTypeFactory compendiumTypeFactory,
            ICalloutService calloutService,
            MissingRequirementsGrouper missingRequirementsGrouper,
            IPluginLog pluginLog) : base(logger, mediator, imGuiService, configuration, "制作窗口")
        {
            _tableService = tableService;
            _configuration = configuration;
            _listService = listService;
            _filterService = filterService;
            _pluginLogic = pluginLogic;
            _universalis = universalis;
            _characterMonitor = characterMonitor;
            _fileDialogManager = fileDialogManager;
            _gameCraftSourceService = gameCraftSourceService;
            _chatUtilities = chatUtilities;
            _importExportService = importExportService;
            _layoutSetting = layoutSetting;
            _context = context;
            _popupService = popupService;
            _craftWindowViewSetting = craftWindowViewSetting;
            _textureProvider = textureProvider;
            _craftSettingsColumn = craftSettingsColumn;
            _font = font;
            _tooltipService = tooltipService;
            _menuService = menuService;
            _clipboardService = clipboardService;
            _keyState = keyState;
            _itemSheet = itemSheet;
            _framework = framework;
            _compendiumTypes = compendiumTypes.Where(c => c.ShowInListing).OrderBy(c => c.Plural);
            _compendiumTypeFactory = compendiumTypeFactory;
            _calloutService = calloutService;
            _missingRequirementsGrouper = missingRequirementsGrouper;
            _pluginLog = pluginLog;
            Flags = ImGuiWindowFlags.MenuBar;
            MediatorService.Subscribe<ListUpdatedMessage>(this, ListUpdatedMessage);
        }

        private void ListUpdatedMessage(ListUpdatedMessage obj)
        {
            if (obj.FilterConfiguration.FilterType == FilterType.CraftFilter)
            {
                _missingRequirementsDirty = true;
            }
        }

        public override void Initialize()
        {
            WindowName = "制作";
            Key = "crafts";
            _throttleDispatcher = new ThrottleDispatcher(5000, true);
            _splitter = new(_configuration.CraftWindowSplitterPosition, new(100, 100), true);
            _menuWindows = _context.Resolve<IEnumerable<IMenuWindow>>().OrderBy(c => c.GenericName).Where(c => c.GetType() != this.GetType());
            MediatorService.Subscribe<ListInvalidatedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRepositionedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListAddedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRemovedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<MarketCacheUpdatedMessage>(this, _ => RefreshCraftList());
            MediatorService.Subscribe<TeamCraftDataImported>(this, ImportTeamcraftData);
            MediatorService.Subscribe<FocusListMessage>(this, FocusList);
        }

        private void FocusList(FocusListMessage message)
        {
            if (message.windowType == this.GetType())
            {
                FocusFilter(message.FilterConfiguration);
            }
        }

        private void ImportTeamcraftData(TeamCraftDataImported data)
        {
            if (SelectedConfiguration != null)
            {
                foreach (var item in data.listData)
                {
                    bool isHq = item.Item1 > 1000000;
                    var itemId = item.Item1 % 500000;
                    SelectedConfiguration.CraftList.AddCraftItem(itemId, item.Item2, isHq ? InventoryItem.ItemFlags.HighQuality : InventoryItem.ItemFlags.None);
                }
                SelectedConfiguration.NeedsRefresh = true;
            }
        }

        public override bool SaveState => true;


        public override Vector2? DefaultSize { get; } = new(600, 600);
        public override Vector2? MaxSize => new Vector2(5000, 5000);
        public override Vector2? MinSize => new Vector2(300, 300);
        public override string GenericKey => "crafts";
        public override string GenericName => "制作";
        public override bool DestroyOnClose => false;
        private int _selectedFilterTab;
        private bool _addItemBarOpen;
        private bool _missingRequirementsBarOpen;
        private bool _missingRequirementsDirty = true;
        private string? _lastMissingReqConfigKey;
        private IReadOnlyList<MissingRequirementGroup> _missingRequirements = Array.Empty<MissingRequirementGroup>();




        private TeamCraftImportWindow? _teamCraftImportWindow;
        private List<FilterConfiguration>? _filters;
        private FilterConfiguration? _defaultFilter;
        private Dictionary<FilterConfiguration, Widgets.PopupMenu> _popupMenus = new();

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

        private void RefreshCraftList()
        {
            _throttleDispatcher?.ThrottleAsync(RequestRefresh);
        }

        private Task RequestRefresh()
        {
            if (SelectedConfiguration != null)
            {
                MediatorService.Publish(new RequestListUpdateMessage(SelectedConfiguration));
            }

            return Task.CompletedTask;
        }

        public Widgets.PopupMenu GetFilterMenu(FilterConfiguration configuration, WindowLayout layout)
        {
            if (!_popupMenus.ContainsKey(configuration))
            {
                _popupMenus[configuration] = new Widgets.PopupMenu("fm" + configuration.Key, Widgets.PopupMenu.PopupMenuButtons.Right,
                    new List<Widgets.PopupMenu.IPopupMenuItem>()
                    {
                        new Widgets.PopupMenu.PopupMenuItemSelectable("编辑", "ef_" + configuration.Key, EditFilter, "编辑该制作清单。"),
                        new Widgets.PopupMenu.PopupMenuItemSelectableAskName("创建副本", "df_" + configuration.Key, configuration.Name, DuplicateFilter, "复制该制作清单。"),
                        new Widgets.PopupMenu.PopupMenuItemSelectable(layout == WindowLayout.Tabs ? "左移" : "上移", "mu_" + configuration.Key, MoveFilterUp, layout == WindowLayout.Tabs ? "将该制作清单左移。" : "将该制作清单上移。"),
                        new Widgets.PopupMenu.PopupMenuItemSelectable(layout == WindowLayout.Tabs ? "右移" : "下移", "md_" + configuration.Key, MoveFilterDown, layout == WindowLayout.Tabs ? "将该制作清单右移。" : "将该制作清单下移。"),
                        new Widgets.PopupMenu.PopupMenuItemSelectableConfirm("移除", "rf_" + configuration.Key, "确定要移除此制作清单吗？", RemoveFilter, "移除该制作清单。"),
                    }
                );
            }

            return _popupMenus[configuration];
        }

        private void EditFilter(string id)
        {
            id = id.Replace("ef_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                FocusFilter(existingFilter, true);
            }
        }


        private void RemoveFilter(string id, bool confirmed)
        {
            if (confirmed)
            {
                id = id.Replace("rf_", "");
                var existingFilter = _listService.GetListByKey(id);
                if (existingFilter != null)
                {
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
                var currentFilter = this.SelectedConfiguration;
                _listService.MoveListDown(existingFilter);
                if (currentFilter != null)
                {
                    FocusFilter(currentFilter);
                }
            }
        }

        private void MoveFilterUp(string id)
        {
            id = id.Replace("mu_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                var currentFilter = this.SelectedConfiguration;
                _listService.MoveListUp(existingFilter);
                if (currentFilter != null)
                {
                    FocusFilter(currentFilter);
                }
            }
        }

        private void DuplicateFilter(string filterName, string id)
        {
            id = id.Replace("df_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                var newFilter = _listService.DuplicateList(existingFilter, filterName);
                FocusFilter(newFilter);
            }
        }


        private List<FilterConfiguration> Filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = _listService.Lists.Where(c => c.FilterType == FilterType.CraftFilter && c.CraftListDefault == false).ToList();
                }

                return _filters;
            }
        }

        private FilterConfiguration DefaultConfiguration
        {
            get
            {
                if (_defaultFilter == null)
                {
                    _defaultFilter = _listService.GetDefaultCraftList();
                }

                return _defaultFilter;
            }
        }

        public void FocusFilter(FilterConfiguration filterConfiguration, bool showSettings = false)
        {
            var filterConfigurations = Filters;
            if (filterConfigurations.Contains(filterConfiguration))
            {
                _selectedFilterTab = filterConfigurations.IndexOf(filterConfiguration);
                var filterIndex = Filters.Contains(filterConfiguration) ? Filters.IndexOf(filterConfiguration) : -1;
                if (filterIndex != -1)
                {
                    _newTab = filterIndex;
                }

                _applyNewTabTime = DateTime.Now + TimeSpan.FromMilliseconds(10);
                if (showSettings)
                {
                    _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Configuration);
                }
            }
        }

        private void DrawMenuBar()
        {
            using(var menuBar = ImRaii.MenuBar())
            {
                if (menuBar)
                {
                    using (var menu = ImRaii.Menu("文件"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("配置"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
                            }

                            if (ImGui.MenuItem("更新日志"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(ChangelogWindow)));
                            }

                            if (ImGui.MenuItem("帮助"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow)));
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

                            if (ImGui.MenuItem("报告问题"))
                            {
                                "https://github.com/Critical-Impact/InventoryTools".OpenBrowser();
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

                    if (this.SelectedConfiguration != null)
                    {
                        using(var editMenu = ImRaii.Menu("编辑"))
                        {
                            if (editMenu)
                            {
                                if (ImGui.MenuItem("清空搜索"))
                                {
                                    _tableService.GetListTable(SelectedConfiguration).ClearFilters();
                                }

                                ImGui.Separator();

                                using (var menu = ImRaii.Menu("复制清单内容"))
                                {
                                    if (menu)
                                    {
                                        if (ImGui.MenuItem("制作清单（全部）"))
                                        {
                                            var searchResults = SelectedConfiguration.CraftList
                                                .GetFlattenedMergedMaterials()
                                                .ToList();
                                            var tcString = _importExportService.ToTCString(searchResults);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print(
                                                "制作清单内容已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（成品）"))
                                        {
                                            var searchResults = SelectedConfiguration.CraftList
                                                .GetFlattenedMergedMaterials()
                                                .Where(c => c.IsOutputItem)
                                                .ToList();

                                            var tcString = _importExportService.ToTCString(searchResults);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print(
                                                "制作清单成品已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（半成品）"))
                                        {
                                            var searchResults = SelectedConfiguration.CraftList
                                                .GetFlattenedMergedMaterials()
                                                .Where(c => c is
                                                {
                                                    IsOutputItem: false,
                                                    IngredientPreference.Type: IngredientPreferenceType.Crafting
                                                })
                                                .ToList();

                                            var tcString = _importExportService.ToTCString(searchResults);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print(
                                                "制作清单成品已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（采集物）"))
                                        {
                                            var searchResults = SelectedConfiguration.CraftList
                                                .GetFlattenedMergedMaterials()
                                                .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                .ToList();

                                            var tcString = _importExportService.ToTCString(searchResults);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print(
                                                "制作清单采集物已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（缺少的采集物）"))
                                        {
                                            var searchResults = SelectedConfiguration.CraftList
                                                .GetFlattenedMergedMaterials()
                                                .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                .ToList();

                                            var tcString =
                                                _importExportService.ToTCString(searchResults, TCExportMode.Missing);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print(
                                                "制作清单采集物已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("雇员/背包清单"))
                                        {
                                            var searchResults = _tableService.GetListTable(SelectedConfiguration)
                                                .SearchResults
                                                .ToList();
                                            var tcString = _importExportService.ToTCString(searchResults);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print("雇员/背包清单已复制到剪贴板。");
                                        }
                                    }
                                }

                                using (var menu = ImRaii.Menu("复制清单内容（JSON）"))
                                {
                                    if (menu)
                                    {
                                        if (ImGui.MenuItem("制作清单（全部）"))
                                        {
                                            var craftTable = _tableService.GetCraftTable(SelectedConfiguration);
                                            var searchResults = craftTable.CraftItems
                                                .ToList();
                                            _clipboardService.CopyToClipboard(craftTable.ExportToJson(searchResults));
                                            _chatUtilities.Print(
                                                "制作清单内容已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（成品）"))
                                        {
                                            var craftTable = _tableService.GetCraftTable(SelectedConfiguration);
                                            var searchResults = craftTable.CraftItems
                                                .Where(c => c.CraftItem?.IsOutputItem ?? false)
                                                .ToList();
                                            _clipboardService.CopyToClipboard(craftTable.ExportToJson(searchResults));
                                            _chatUtilities.Print(
                                                "制作清单成品已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（半成品）"))
                                        {
                                            var craftTable = _tableService.GetCraftTable(SelectedConfiguration);
                                            var searchResults = craftTable.CraftItems
                                                .Where(c => c.CraftItem is
                                                {
                                                    IsOutputItem: false,
                                                    IngredientPreference.Type: IngredientPreferenceType.Crafting
                                                })
                                                .ToList();
                                            _clipboardService.CopyToClipboard(craftTable.ExportToJson(searchResults));
                                            _chatUtilities.Print(
                                                "制作清单成品已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("制作清单（采集物）"))
                                        {
                                            var craftTable = _tableService.GetCraftTable(SelectedConfiguration);
                                            var searchResults = craftTable.CraftItems
                                                .Where(c => c.Item.ObtainedGathering &&
                                                            (c.CraftItem?.IsOutputItem ?? false))
                                                .ToList();
                                            _clipboardService.CopyToClipboard(craftTable.ExportToJson(searchResults));
                                            _chatUtilities.Print(
                                                "制作清单采集物已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("雇员/背包清单"))
                                        {
                                            var itemTable = _tableService.GetListTable(SelectedConfiguration);
                                            _clipboardService.CopyToClipboard(itemTable.ExportToJson());
                                        }
                                    }
                                }

                                if (ImGui.MenuItem("粘贴清单内容"))
                                {
                                    var pasteFromClipboard = _clipboardService.PasteFromClipboard();
                                    var importedList = _importExportService.FromTCString(pasteFromClipboard, false);
                                    if (importedList == null)
                                    {
                                        importedList =
                                            _importExportService.FromGarlandToolsUrl(pasteFromClipboard);
                                        if (importedList == null)
                                        {
                                            _chatUtilities.PrintError(
                                                "无法解析剪贴板内容。");
                                        }
                                        else
                                        {
                                            _chatUtilities.Print("剪贴板内容已导入。");
                                            this.SelectedConfiguration.AddItemsToList(importedList);
                                        }
                                    }
                                    else
                                    {
                                        _chatUtilities.Print("剪贴板内容已导入。");
                                        this.SelectedConfiguration.AddItemsToList(importedList);
                                    }
                                }

                                if (ImGui.IsItemHovered())
                                {
                                    using (ImRaii.Tooltip())
                                    {
                                        ImGui.TextUnformatted(
                                            "这将粘贴上方“复制清单内容”菜单所复制的物品内容，也会尝试解析剪贴板中的 Teamcraft 清单。如果剪贴板中有指向分组的 Garland Tools 链接，也会尝试解析并将其加入制作清单。");
                                    }
                                }

                                if (ImGui.MenuItem("清空清单"))
                                {
                                    _popupService.AddPopup(new ConfirmPopup(GetType(), "craftListDelete",
                                        "确定要清空你的制作清单吗？",
                                        result =>
                                        {
                                            if (result)
                                            {
                                                this.SelectedConfiguration.CraftList.CraftItems.Clear();
                                                this.SelectedConfiguration.CraftList.NeedsRefresh = true;
                                            }
                                        }));

                                }

                                ImGui.Separator();
                                using (var addToCraftListMenu = ImRaii.Menu("添加到制作清单"))
                                {
                                    if (addToCraftListMenu)
                                    {
                                        var craftLists = _listService.Lists
                                            .Where(c => c.FilterType == FilterType.CraftFilter &&
                                                        c.CraftListDefault == false)
                                            .OrderBy(c => c.Order)
                                            .ToList();

                                        foreach (var craft in craftLists)
                                        {
                                            using (var menu = ImRaii.Menu(craft.Name))
                                            {
                                                if (menu)
                                                {
                                                    if (ImGui.MenuItem("制作清单（全部）"))
                                                    {
                                                        var searchResults = SelectedConfiguration.CraftList
                                                            .GetFlattenedMergedMaterials()
                                                            .ToList();

                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.QuantityRequired,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craft));
                                                    }

                                                    if (ImGui.MenuItem("制作清单（成品）"))
                                                    {
                                                        var searchResults = SelectedConfiguration.CraftList
                                                            .GetFlattenedMergedMaterials()
                                                            .Where(c => c.IsOutputItem)
                                                            .ToList();

                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.QuantityRequired,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craft));
                                                    }

                                                    if (ImGui.MenuItem("制作清单（半成品）"))
                                                    {
                                                        var searchResults = SelectedConfiguration.CraftList
                                                            .GetFlattenedMergedMaterials()
                                                            .Where(c => c is
                                                            {
                                                                IsOutputItem: false,
                                                                IngredientPreference.Type: IngredientPreferenceType
                                                                    .Crafting
                                                            })
                                                            .ToList();

                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.QuantityRequired,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craft));
                                                    }

                                                    if (ImGui.MenuItem("制作清单（采集物）"))
                                                    {
                                                        var searchResults = SelectedConfiguration.CraftList
                                                            .GetFlattenedMergedMaterials()
                                                            .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                            .ToList();

                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.QuantityRequired,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craft));
                                                    }

                                                    if (ImGui.MenuItem("制作清单（缺少的采集物）"))
                                                    {
                                                        var searchResults = SelectedConfiguration.CraftList
                                                            .GetFlattenedMergedMaterials()
                                                            .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                            .ToList();

                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.QuantityMissingOverall,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craft));
                                                    }

                                                    if (ImGui.MenuItem("雇员/背包清单"))
                                                    {
                                                        var searchResults = _tableService
                                                            .GetListTable(SelectedConfiguration)
                                                            .SearchResults
                                                            .ToList();
                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.Quantity,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craft));
                                                    }
                                                }
                                            }
                                        }

                                        if (craftLists.Count != 0)
                                        {
                                            ImGui.Separator();
                                        }

                                        using (var menu = ImRaii.Menu("新建制作清单"))
                                        {
                                            if (menu)
                                            {
                                                if (ImGui.MenuItem("制作清单（全部）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .ToList();

                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.IsOutputItem)
                                                        .ToList();

                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（半成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c is
                                                        {
                                                            IsOutputItem: false,
                                                            IngredientPreference.Type: IngredientPreferenceType.Crafting
                                                        })
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（缺少的采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityMissingOverall,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("雇员/背包清单"))
                                                {
                                                    var searchResults = _tableService
                                                        .GetListTable(SelectedConfiguration)
                                                        .SearchResults
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.Quantity,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                            }
                                        }

                                        using (var menu = ImRaii.Menu("新建制作清单（临时）"))
                                        {
                                            if (menu)
                                            {
                                                if (ImGui.MenuItem("制作清单（全部）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .ToList();

                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2, true);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.IsOutputItem)
                                                        .ToList();

                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2, true);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（半成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c is
                                                        {
                                                            IsOutputItem: false,
                                                            IngredientPreference.Type: IngredientPreferenceType.Crafting
                                                        })
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2, true);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2, true);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（缺少的采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2, true);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityMissingOverall,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("雇员/背包清单"))
                                                {
                                                    var searchResults = _tableService
                                                        .GetListTable(SelectedConfiguration)
                                                        .SearchResults
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCraftList",
                                                        "新制作清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var craftList =
                                                                    _listService.AddNewCraftList(result.Item2, true);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    craftList.CraftList.AddCraftItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.Quantity,
                                                                        searchResult.Flags);
                                                                }
                                                            }
                                                        }));
                                                }
                                            }
                                        }

                                    }
                                }

                                using (var menu = ImRaii.Menu("添加到精选清单"))
                                {
                                    if (menu)
                                    {
                                        var curatedLists = _listService.Lists
                                            .Where(c => c.FilterType == FilterType.CuratedList)
                                            .OrderBy(c => c.Order)
                                            .ToList();

                                        foreach (var curatedList in curatedLists)
                                        {
                                            if (ImGui.MenuItem(curatedList.Name))
                                            {
                                                if (ImGui.MenuItem("制作清单（全部）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .ToList();

                                                    foreach (var searchResult in searchResults)
                                                    {
                                                        curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                            searchResult.QuantityRequired,
                                                            searchResult.Flags));
                                                    }
                                                }

                                                if (ImGui.MenuItem("制作清单（成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.IsOutputItem)
                                                        .ToList();

                                                    foreach (var searchResult in searchResults)
                                                    {
                                                        curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                            searchResult.QuantityRequired,
                                                            searchResult.Flags));
                                                    }
                                                }

                                                if (ImGui.MenuItem("制作清单（半成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c is
                                                        {
                                                            IsOutputItem: false,
                                                            IngredientPreference.Type: IngredientPreferenceType.Crafting
                                                        })
                                                        .ToList();

                                                    foreach (var searchResult in searchResults)
                                                    {
                                                        curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                            searchResult.QuantityRequired,
                                                            searchResult.Flags));
                                                    }
                                                }

                                                if (ImGui.MenuItem("制作清单（采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();

                                                    foreach (var searchResult in searchResults)
                                                    {
                                                        curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                            searchResult.QuantityRequired,
                                                            searchResult.Flags));
                                                    }
                                                }

                                                if (ImGui.MenuItem("制作清单（缺少的采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();

                                                    foreach (var searchResult in searchResults)
                                                    {
                                                        curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                            searchResult.QuantityRequired,
                                                            searchResult.Flags));
                                                    }
                                                }

                                                if (ImGui.MenuItem("雇员/背包清单"))
                                                {
                                                    var searchResults = _tableService
                                                        .GetListTable(SelectedConfiguration)
                                                        .SearchResults
                                                        .ToList();
                                                    foreach (var searchResult in searchResults)
                                                    {
                                                        curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                            searchResult.Quantity,
                                                            searchResult.Flags));
                                                    }
                                                }

                                            }
                                        }

                                        if (curatedLists.Count != 0)
                                        {
                                            ImGui.Separator();
                                        }

                                        using (var newCuratedListMenu = ImRaii.Menu("新建精选清单"))
                                        {
                                            if (newCuratedListMenu)
                                            {
                                                if (ImGui.MenuItem("制作清单（全部）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .ToList();

                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCuratedList",
                                                        "新精选清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var curatedList =
                                                                    _listService.AddNewCuratedList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    curatedList.AddCuratedItem(new CuratedItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags));
                                                                }

                                                                this.MediatorService.Publish(
                                                                    new FocusListMessage(typeof(FiltersWindow),
                                                                        curatedList));
                                                                curatedList.NeedsRefresh = true;
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.IsOutputItem)
                                                        .ToList();

                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCuratedList",
                                                        "新精选清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var curatedList =
                                                                    _listService.AddNewCuratedList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    curatedList.AddCuratedItem(new CuratedItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags));
                                                                }

                                                                this.MediatorService.Publish(
                                                                    new FocusListMessage(typeof(FiltersWindow),
                                                                        curatedList));
                                                                curatedList.NeedsRefresh = true;
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（半成品）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c is
                                                        {
                                                            IsOutputItem: false,
                                                            IngredientPreference.Type: IngredientPreferenceType.Crafting
                                                        })
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCuratedList",
                                                        "新精选清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var curatedList =
                                                                    _listService.AddNewCuratedList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    curatedList.AddCuratedItem(new CuratedItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags));
                                                                }

                                                                this.MediatorService.Publish(
                                                                    new FocusListMessage(typeof(FiltersWindow),
                                                                        curatedList));
                                                                curatedList.NeedsRefresh = true;
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCuratedList",
                                                        "新精选清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var curatedList =
                                                                    _listService.AddNewCuratedList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    curatedList.AddCuratedItem(new CuratedItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityRequired,
                                                                        searchResult.Flags));
                                                                }

                                                                this.MediatorService.Publish(
                                                                    new FocusListMessage(typeof(FiltersWindow),
                                                                        curatedList));
                                                                curatedList.NeedsRefresh = true;
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("制作清单（缺少的采集物）"))
                                                {
                                                    var searchResults = SelectedConfiguration.CraftList
                                                        .GetFlattenedMergedMaterials()
                                                        .Where(c => c.Item.ObtainedGathering && !c.IsOutputItem)
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCuratedList",
                                                        "新精选清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var curatedList =
                                                                    _listService.AddNewCuratedList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    curatedList.AddCuratedItem(new CuratedItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.QuantityMissingOverall,
                                                                        searchResult.Flags));
                                                                }

                                                                this.MediatorService.Publish(
                                                                    new FocusListMessage(typeof(FiltersWindow),
                                                                        curatedList));
                                                                curatedList.NeedsRefresh = true;
                                                            }
                                                        }));
                                                }

                                                if (ImGui.MenuItem("雇员/背包清单"))
                                                {
                                                    var searchResults = _tableService
                                                        .GetListTable(SelectedConfiguration)
                                                        .SearchResults
                                                        .ToList();
                                                    _popupService.AddPopup(new NamePopup(typeof(CraftsWindow),
                                                        "newCuratedList",
                                                        "新精选清单",
                                                        result =>
                                                        {
                                                            if (result.Item1)
                                                            {
                                                                var curatedList =
                                                                    _listService.AddNewCuratedList(result.Item2);
                                                                foreach (var searchResult in searchResults)
                                                                {
                                                                    curatedList.AddCuratedItem(new CuratedItem(
                                                                        searchResult.ItemId,
                                                                        searchResult.Quantity,
                                                                        searchResult.Flags));
                                                                }

                                                                this.MediatorService.Publish(
                                                                    new FocusListMessage(typeof(FiltersWindow),
                                                                        curatedList));
                                                                curatedList.NeedsRefresh = true;
                                                            }
                                                        }));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                    using (var menu = ImRaii.Menu("视图"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("标签页", "",
                                    _layoutSetting.CurrentValue(_configuration) == WindowLayout.Tabs))
                            {
                                _layoutSetting.UpdateFilterConfiguration(_configuration, WindowLayout.Tabs);
                            }

                            if (ImGui.MenuItem("侧边栏", "",
                                    _layoutSetting.CurrentValue(_configuration) == WindowLayout.Sidebar))
                            {
                                _layoutSetting.UpdateFilterConfiguration(_configuration, WindowLayout.Sidebar);
                            }

                            if (ImGui.MenuItem("单栏", "",
                                    _layoutSetting.CurrentValue(_configuration) == WindowLayout.Single))
                            {
                                _layoutSetting.UpdateFilterConfiguration(_configuration, WindowLayout.Single);
                            }

                            ImGui.Separator();

                            if (ImGui.MenuItem("制作", "",
                                    _craftWindowViewSetting.CurrentValue(_configuration) == CraftWindowView.Crafts))
                            {
                                _craftWindowViewSetting.UpdateFilterConfiguration(_configuration,
                                    CraftWindowView.Crafts);
                            }

                            if (ImGui.MenuItem("树状视图", "",
                                    _craftWindowViewSetting.CurrentValue(_configuration) == CraftWindowView.Tree))
                            {
                                _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Tree);
                            }

                            if (ImGui.MenuItem("配置", "",
                                    _craftWindowViewSetting.CurrentValue(_configuration) ==
                                    CraftWindowView.Configuration))
                            {
                                _craftWindowViewSetting.UpdateFilterConfiguration(_configuration,
                                    CraftWindowView.Configuration);
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("导出"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("制作清单 (CSV)"))
                            {
                                if (SelectedConfiguration != null)
                                {
                                    _fileDialogManager.SaveFileDialog("保存为 CSV", "*.csv",
                                        "export-craft-list.csv", ".csv",
                                        (b, s) =>
                                        {
                                            var craftTable = _tableService.GetCraftTable(SelectedConfiguration);
                                            SaveCraftCallback(craftTable, b, s);
                                        }, null, true);
                                }
                            }

                            if (ImGui.MenuItem("雇员/背包清单 (CSV)"))
                            {
                                if (SelectedConfiguration != null)
                                {
                                    var itemTable = _tableService.GetListTable(SelectedConfiguration);
                                    _fileDialogManager.SaveFileDialog("保存为 CSV", "*.csv", "export.csv", ".csv",
                                        (b, s) => { SaveCallback(itemTable, b, s); }, null, true);
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("市场"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("刷新所有价格（制作清单）"))
                            {
                                var activeCharacter = _characterMonitor.ActiveCharacter;
                                if (activeCharacter != null && SelectedConfiguration != null)
                                {
                                    var itemTable = _tableService.GetCraftTable(SelectedConfiguration);
                                    foreach (var item in itemTable.CraftItems)
                                    {
                                        _universalis.QueuePriceCheck(item.Item.RowId, activeCharacter.WorldId);
                                    }
                                }
                            }

                            if (ImGui.MenuItem("刷新所有价格（雇员/背包）"))
                            {
                                var activeCharacter = _characterMonitor.ActiveCharacter;
                                if (activeCharacter != null && SelectedConfiguration != null)
                                {
                                    var itemTable = _tableService.GetListTable(SelectedConfiguration);
                                    foreach (var item in itemTable.RenderSearchResults)
                                    {
                                        _universalis.QueuePriceCheck(item.Item.RowId, activeCharacter.WorldId);
                                    }
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("清单"))
                    {
                        if (menu)
                        {
                            using (var addMenu = ImRaii.Menu("添加"))
                            {
                                if (addMenu)
                                {
                                    if (ImGui.MenuItem("制作清单"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addCraftList", "", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                AddCraftFilter(result.Item2);
                                            }
                                        }));
                                    }

                                    if (ImGui.MenuItem("制作清单（临时）"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addCraftListEphemeral", "",
                                            result =>
                                            {
                                                if (result.Item1)
                                                {
                                                    AddCraftFilter(result.Item2);
                                                }
                                            }));
                                    }
                                }
                            }

                            ImGui.NewLine();

                            var windowGroups = _listService.Lists.GroupBy(c => c.FilterType).OrderBySequence(
                            [
                                FilterType.CraftFilter, FilterType.SearchFilter, FilterType.SortingFilter,
                                FilterType.GameItemFilter, FilterType.HistoryFilter, FilterType.CuratedList
                            ], grouping => grouping.Key).ToList();
                            for (var index = 0; index < windowGroups.Count; index++)
                            {
                                var windowGroup = windowGroups[index];
                                ImGui.Text(windowGroup.Key.FormattedName());
                                ImGui.Separator();
                                foreach (var window in windowGroup.OrderBy(c => c.CraftListDefault)
                                             .ThenBy(c => c.Order))
                                {
                                    if (ImGui.MenuItem(window.Name, "",
                                            SelectedConfiguration == window ||
                                            (SelectedConfiguration == null && window.CraftListDefault)))
                                    {
                                        if (window.FilterType == FilterType.CraftFilter)
                                        {
                                            if (_keyState[VirtualKey.CONTROL])
                                            {
                                                this.MediatorService.Publish(
                                                    new OpenStringWindowMessage(typeof(FilterWindow), window.Key));
                                            }
                                            else
                                            {
                                                if (window.CraftListDefault)
                                                {
                                                    _selectedFilterTab = Filters.Count + 1;
                                                }
                                                else
                                                {
                                                    MediatorService.Publish(
                                                        new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                    MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow),
                                                        window));
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (_keyState[VirtualKey.CONTROL])
                                            {
                                                this.MediatorService.Publish(
                                                    new OpenStringWindowMessage(typeof(FilterWindow), window.Key));
                                            }
                                            else
                                            {
                                                MediatorService.Publish(
                                                    new OpenGenericWindowMessage(typeof(FiltersWindow)));
                                                MediatorService.Publish(new FocusListMessage(typeof(FiltersWindow),
                                                    window));
                                            }
                                        }
                                    }

                                    ImGuiUtil.HoverTooltip("[CTRL] 在新窗口中打开。");
                                }

                                if (index != windowGroups.Count - 1)
                                {
                                    ImGui.NewLine();
                                }
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
                                        this.MediatorService.Publish(new OpenGenericWindowMessage(window.GetType()));
                                    }
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("图鉴"))
                    {
                        if (menu)
                        {
                            if (ImGui.Selectable("图鉴查看器"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(CompendiumTypesWindow)));
                            }
                            ImGui.Separator();
                            foreach (var compendiumType in _compendiumTypes)
                            {
                                if (compendiumType.ShowInListing && ImGui.MenuItem(compendiumType.Plural))
                                {
                                    this.MediatorService.Publish(new ToggleCompendiumListMessage(compendiumType));
                                }
                            }
                        }
                    }

                    if (ImGui.IsItemHovered())
                    {
                        using (ImRaii.Tooltip())
                        {
                            ImGui.Text("图鉴是开发中的功能，敬请期待更多内容！");
                        }
                    }

                    if (ImGui.MenuItem("切换制作悬浮层"))
                    {
                        this.MediatorService.Publish(new ToggleGenericWindowMessage(typeof(CraftOverlayWindow)));
                    }

                    DrawAddFromGameMenu();
                }
            }
        }

        private void DrawAddFromGameMenu()
        {
            if (SelectedConfiguration == null)
            {
                return;
            }

            var categories = _gameCraftSourceService.GetAvailableCategories();
            if (categories.Count == 0)
            {
                return;
            }

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 0.6f, 1f, 1f));
            using (var menu = ImRaii.Menu("从游戏添加"))
            {
                ImGui.PopStyleColor();
                if (menu)
                {
                    foreach (var category in categories)
                    {
                        if (ImGui.MenuItem(category.Name))
                        {
                            var configuration = SelectedConfiguration;
                            var items = category.GetItems();
                            _framework.RunOnFrameworkThread(() =>
                            {
                                foreach (var item in items)
                                {
                                    configuration.CraftList.AddCraftItem(item.ItemId, item.Quantity, item.Flags);
                                }
                                configuration.NeedsRefresh = true;
                            });
                        }
                    }
                }
            }

            if (ImGui.IsItemHovered())
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.Text("出现此菜单是因为 Allagan Tools 检测到打开了受支持的游戏窗口");
                    ImGui.Text("（例如部队工房或军团补给清单）。");
                    ImGui.Text("每个选项都会将其物品添加到当前选中的制作清单。");
                }
            }
        }

        public override unsafe void DrawWindow()
        {
            DrawMenuBar();
            _popupService.Draw(GetType());
            if (!_configuration.HasSeenNotification(NotificationPopup.CraftNotice) && ImGui.IsWindowFocused())
            {
                ImGui.OpenPopup("notification");
                _configuration.MarkNotificationSeen(NotificationPopup.CraftNotice);
            }

            ImGuiUtil.HelpPopup("notification", new Vector2(750,340) * ImGui.GetIO().FontGlobalScale, () =>
            {
                ImGui.TextUnformatted("制作系统通知");
                ImGui.Separator();
                ImGui.NewLine();
                ImGui.PushTextWrapPos();
                ImGui.Bullet();
                ImGui.Text("制作系统已更新，你的默认配置已被重置。请根据偏好重新调整。");
                ImGui.PopTextWrapPos();

                ImGui.BulletText("现在可以在制作清单之间复制配置。");

                ImGui.BulletText("制作清单新增了两列：Next Step（下一步）和 Settings（设置）。");

                ImGui.Indent();
                ImGui.BulletText("Next Step（下一步）列会提示你接下来该做什么。");
                ImGui.Unindent();

                ImGui.Indent();
                ImGui.BulletText("Settings（设置）列允许你配置物品获取途径、雇员设置和配方。");
                ImGui.Unindent();

                ImGui.BulletText("本次更新包含以下变更：");

                ImGui.Indent();
                ImGui.BulletText("现在可以按职业或所需制作顺序更改制作分组。");
                ImGui.BulletText("可取出的物品可以单独成组并优先显示。");
                ImGui.BulletText("可采集和可购买的物品可以按区域分组。");
                ImGui.BulletText("改进了可用军票、诗学神典石和票据购买的物品的处理。");
                ImGui.Unindent();

                ImGui.BulletText("你可以点击清单右上角的铅笔图标进一步自定义这些选项。");

            });

            if (_configuration.CraftWindowLayout == WindowLayout.Sidebar)
            {
                DrawSidebar();
                DrawMainWindow();
            }
            else if (_configuration.CraftWindowLayout == WindowLayout.Tabs)
            {
                DrawTabBar();
            }
            else
            {
                DrawMainWindow();
            }
        }

        private string _newCraftName = "";
        private bool openNewFilterNamePopup;
        private bool openNewTypePopup;
        private bool _ephemeralList;
        private unsafe void DrawTabBar()
        {
            if (openNewFilterNamePopup)
            {
                ImGui.OpenPopup("addCraftFilterName");
                openNewFilterNamePopup = false;
            }
            if (ImGuiUtil.OpenNameField("addCraftFilterName", ref _newCraftName))
            {
                _framework.RunOnFrameworkThread(() =>
                {
                    AddCraftFilter(_newCraftName, _ephemeralList);
                    _newCraftName = "";
                });
            }
            if (openNewTypePopup)
            {
                ImGui.OpenPopup("addCraftFilterType");
                openNewTypePopup = false;
            }
            using(var popup = ImRaii.Popup("addCraftFilterType"))
            {
                if (popup.Success)
                {
                    if (ImGui.Selectable("普通清单"))
                    {
                        _ephemeralList = false;
                        openNewFilterNamePopup = true;
                    }
                    ImGuiUtil.HoverTooltip("添加新制作清单。");

                    if (ImGui.Selectable("临时清单"))
                    {
                        _ephemeralList = true;
                        openNewFilterNamePopup = true;
                    }
                    ImGuiUtil.HoverTooltip("添加新的临时制作清单，其中的物品全部完成后将被删除。");
                }
            }

            using (var tabbar = ImRaii.TabBar("CraftTabs", ImGuiTabBarFlags.FittingPolicyScroll | ImGuiTabBarFlags.ListPopupButton))
            {
                if (tabbar.Success)
                {
                    var filterConfigurations = Filters;
                    for (var index = 0; index < filterConfigurations.Count; index++)
                    {
                        var filterConfiguration = filterConfigurations[index];
                        using var id = ImRaii.PushId(index);
                        var imGuiTabItemFlags = _newTab == index && SwitchNewTab ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None;
                        using (var tabItem = ImRaii.TabItem(filterConfiguration.NameFormatted, imGuiTabItemFlags))
                        {
                            if (SwitchNewTab && _newTab != null && _newTab == index)
                            {
                                _newTab = null;
                                _applyNewTabTime = null;
                                _selectedFilterTab = index;
                            }
                            GetFilterMenu(filterConfiguration, WindowLayout.Tabs).Draw();

                            if (tabItem.Success)
                            {
                                _selectedFilterTab = index;
                                DrawMainWindow();
                            }
                        }
                    }
                    using (var tabItem = ImRaii.TabItem("默认配置"))
                    {
                        if (_filters != null && tabItem.Success)
                        {
                            _selectedFilterTab = filterConfigurations.Count + 1;
                            DrawMainWindow();
                        }
                    }
                    if (ImGui.TabItemButton("+", ImGuiTabItemFlags.Trailing | ImGuiTabItemFlags.NoTooltip))
                    {
                        openNewTypePopup = true;
                    }
                    ImGuiUtil.HoverTooltip("添加新制作清单");
                }
            }
        }

        private void AddCraftFilter(string newName, bool ephemeralList = false)
        {
            var filterConfiguration = _listService.AddNewCraftList(newName, ephemeralList);
            Invalidate();
            this.FocusFilter(filterConfiguration);
        }

        private int? _newTab;
        private DateTime? _applyNewTabTime;

        private bool SwitchNewTab => _newTab != null && _applyNewTabTime != null && _applyNewTabTime.Value <= DateTime.Now;

        private void DrawMainWindow()
        {
            var isWindowFocused = ImGui.IsWindowFocused();
            var filterConfigurations = Filters;
            using (var child = ImRaii.Child("Main",
                       new Vector2(_addItemBarOpen || _missingRequirementsBarOpen ? -250 : -1, -1) * ImGui.GetIO().FontGlobalScale, false,
                       ImGuiWindowFlags.HorizontalScrollbar))
            {
                if (child.Success)
                {
                    if (filterConfigurations.Count == 0 && _selectedFilterTab == 0)
                    {
                        using (var contentChild = ImRaii.Child("Content", new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale, true))
                        {
                            if (contentChild.Success)
                            {
                                ImGui.TextUnformatted(
                                    "点击左下角的 + 按钮添加制作清单即可开始使用。");
                            }
                        }
                    }

                    for (var index = 0; index < filterConfigurations.Count; index++)
                    {
                        var filterConfiguration = filterConfigurations[index];

                        if (_selectedFilterTab == index)
                        {

                            if (isWindowFocused)
                            {
                                if (filterConfiguration.Active != true)
                                {
                                    filterConfiguration.NeedsRefresh = true;
                                    filterConfiguration.Active = true;
                                }
                                if (_configuration.SwitchFiltersAutomatically &&
                                    _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                    _configuration.ActiveUiFilter != null)
                                {
                                    _framework.RunOnFrameworkThread(() =>
                                    {
                                        _listService.ToggleActiveUiList(filterConfiguration);
                                    });
                                }
                                if (_configuration.SwitchCraftListsAutomatically &&
                                    _configuration.ActiveCraftList != filterConfiguration.Key &&
                                    _configuration.ActiveCraftList != null && filterConfiguration.FilterType == FilterType.CraftFilter)
                                {
                                    _framework.RunOnFrameworkThread(() =>
                                    {
                                        _listService.ToggleActiveCraftList(filterConfiguration);
                                    });
                                }
                            }

                            var currentViewMode = _craftWindowViewSetting.CurrentValue(_configuration);

                            if (currentViewMode == CraftWindowView.Crafts || currentViewMode == CraftWindowView.Tree)
                            {
                                DrawCraftPanel(filterConfiguration);
                            }
                            else if(currentViewMode == CraftWindowView.Configuration)
                            {
                                DrawSettingsPanel(filterConfiguration);
                            }
                        }
                        else
                        {
                            if (isWindowFocused)
                            {
                                filterConfiguration.Active = false;
                            }
                        }
                    }

                    if (_selectedFilterTab == filterConfigurations.Count + 1)
                    {
                        DrawSettingsPanel(DefaultConfiguration);
                    }
                }
            }

            ImGui.SameLine();
            if (_missingRequirementsBarOpen)
            {
                DrawMissingRequirementsBar();
            }

            if (_addItemBarOpen)
            {
                using (var addItemChild = ImRaii.Child("AddItem", new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, true))
                {
                    if (addItemChild.Success)
                    {
                        for (var index = 0; index < filterConfigurations.Count; index++)
                        {
                            if (_selectedFilterTab == index)
                            {
                                var filterConfiguration = filterConfigurations[index];
                                if (filterConfiguration.FilterType == FilterType.CraftFilter)
                                {
                                    ImGui.TextUnformatted("添加新物品");
                                    var searchString = SearchString;
                                    ImGui.InputText("##ItemSearch", ref searchString, 50);
                                    if (_searchString != searchString)
                                    {
                                        SearchString = searchString;
                                    }

                                    ImGui.SameLine();
                                    var clearSearchCursorX = ImGui.GetCursorPosX();
                                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Times, ref clearSearchCursorX))
                                    {
                                        SearchString = "";
                                    }

                                    ImGuiUtil.HoverTooltip("清空当前搜索。");

                                    var craftableOnly = _searchCraftableOnly;
                                    if (ImGui.Checkbox("仅可制作", ref craftableOnly) && craftableOnly != _searchCraftableOnly)
                                    {
                                        _searchCraftableOnly = craftableOnly;
                                        _searchItems = null;
                                    }

                                    ImGui.Separator();
                                    if (_searchString == "")
                                    {
                                        ImGui.TextUnformatted("输入以开始搜索...");
                                    }

                                    using var table = ImRaii.Table("", 2, ImGuiTableFlags.SizingStretchProp);
                                    if (!table || !table.Success)
                                        return;

                                    ImGui.TableSetupColumn("名称", ImGuiTableColumnFlags.None, 200);
                                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 16);

                                    foreach (var datum in SearchItems)
                                    {
                                        ImGui.TableNextRow();
                                        DrawSearchRow(filterConfiguration, datum);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawMissingRequirementsBar()
        {
            using var child = ImRaii.Child("MissingReqs", new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, true);
            if (!child) return;

            ImGui.TextUnformatted("缺少的需求");
            ImGui.Separator();
            ImGui.Spacing();

            if (_missingRequirements.Count == 0)
            {
                ImGui.TextUnformatted("未检测到缺少的需求。");
                return;
            }

            foreach (var group in _missingRequirements)
            {
                var rowRef = group.RowRef;

                var compendiumType = ResolveCompendiumType(rowRef);

                if (compendiumType == null)
                {
                    continue;
                }

                var hasLink = rowRef.RowId != 0;
                if (hasLink)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.4f, 0.7f, 1f, 1f));
                }

                var icon = compendiumType.GetIcon(rowRef.RowId);
                ImGuiService.DrawIcon(icon, new FFXIVClientStructs.FFXIV.Common.Math.Vector2(16, 16));
                ImGui.SameLine();
                var clicked = ImGui.Selectable($"{group.Description}##req_{rowRef.RowId}", false, ImGuiSelectableFlags.SpanAllColumns);

                if (hasLink)
                {
                    ImGui.PopStyleColor();
                }

                if (clicked)
                {
                    MediatorService.Publish(new OpenCompendiumViewMessage(compendiumType, rowRef.RowId));
                }

                ImGui.Indent();
                foreach (var itemName in group.AffectedItems)
                {
                    ImGui.TextUnformatted($"- {itemName}");
                }
                ImGui.Unindent();
                ImGui.Spacing();
            }
        }

        private ICompendiumType? ResolveCompendiumType(RowRef rowRef)
        {
            return _compendiumTypeFactory.GetByRowRef(rowRef, out _);
        }

        private int selectedTreeViewIndex = 0;



        private void DrawTreeView(FilterConfiguration filterConfiguration)
        {
            if (filterConfiguration.CraftList.CraftItems.Count == 0)
            {
                ImGui.TextUnformatted("没有制作数据。");
                return;
            }

            using (var sideBar = ImRaii.Child("SideBar", new Vector2(32 + ImGui.GetStyle().ScrollbarSize, 0), false,
                       ImGuiWindowFlags.AlwaysVerticalScrollbar))
            {
                if (sideBar)
                {
                    for (var index = 0; index < filterConfiguration.CraftList.CraftItems.Count; index++)
                    {
                        var rootItem = filterConfiguration.CraftList.CraftItems[index];
                        var iconTex = _textureProvider.GetFromGameIcon(new GameIconLookup(rootItem.Item.Icon, rootItem.Flags == InventoryItem.ItemFlags.HighQuality));
                        using var tsbPushId = ImRaii.PushId("tsb_" + index);
                        if (ImGui.ImageButton(iconTex.GetWrapOrEmpty().Handle, new Vector2(32, 32) * ImGui.GetIO().FontGlobalScale))
                        {
                            selectedTreeViewIndex = index;
                        }

                        if (ImGui.IsItemHovered())
                        {
                            _tooltipService.DrawItemTooltip(new SearchResult(rootItem));
                        }

                        if (ImGui.IsItemHovered() && ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup("tsb_" + index);
                        }

                        using (var popup = ImRaii.Popup("tsb_" + index))
                        {
                            if (popup)
                            {
                                MediatorService.Publish(_menuService.DrawRightClickPopup(rootItem.Item));
                            }
                        }

                    }
                }
            }
            ImGui.SameLine();
            using (var main = ImRaii.Child("Main", new Vector2(0, 0)))
            {
                if (!main)
                {
                    return;
                }
                if (filterConfiguration.CraftList.CraftItems.Count == 0)
                {
                    return;
                }

                if (selectedTreeViewIndex < 0 || selectedTreeViewIndex >= filterConfiguration.CraftList.CraftItems.Count)
                {
                    selectedTreeViewIndex = 0;
                }
                var rootItem = filterConfiguration.CraftList.CraftItems[selectedTreeViewIndex];
                DrawTreeCraftItem(rootItem, selectedTreeViewIndex.ToString(), selectedTreeViewIndex);
            }
        }

        private Dictionary<string, bool> _nodeStates = new();
        private Dictionary<string, bool> _nextState = new();

        private void DrawTreeCraftItem(CraftItem item, string itemId, int index = 0, float indentWidth = 0, bool? nextState = null)
        {
            if (SelectedConfiguration == null)
            {
                return;
            }
            using (var popup = ImRaii.Popup("ConfigureItemSettings" + index + item.ItemId + (item.IsOutputItem ? "o" : "")))
            {
                if (popup.Success)
                {
                    ImGui.Text("配置获取方式:");
                    ImGui.Separator();

                    _craftSettingsColumn.DrawRecipeSelector(SelectedConfiguration, item, index);
                    _craftSettingsColumn.DrawHqSelector(SelectedConfiguration, item, index);
                    _craftSettingsColumn.DrawRetainerRetrievalSelector(SelectedConfiguration, item, index);
                    _craftSettingsColumn.DrawSourceSelector(SelectedConfiguration, item, index);
                    _craftSettingsColumn.DrawZoneSelector(SelectedConfiguration, item, index);
                    _craftSettingsColumn.DrawMarketWorldSelector(SelectedConfiguration, item, index);
                    _craftSettingsColumn.DrawMarketPriceSelector(SelectedConfiguration, item, index);
                }
            }

            if (!_nodeStates.TryGetValue(itemId, out bool isOpen))
                _nodeStates[itemId] = isOpen = true;

            var hasSubcrafts = item.ChildCrafts.Any(c => c.ChildCrafts.Count != 0);

            if (indentWidth != 0)
            {
                ImGui.Indent(indentWidth);
            }

            // Unique ID for this line so buttons don't collide
            using var id = ImRaii.PushId(itemId);

            if (item.ChildCrafts.Count > 0)
            {
                var posX = ImGui.GetCursorPosX();
                if (ImGuiService.DrawIconButton(_font, isOpen? FontAwesomeIcon.ChevronDown : FontAwesomeIcon.ChevronRight, ref posX, minWidth: 20 * ImGui.GetIO().FontGlobalScale))
                {
                    _nodeStates[itemId] = !isOpen;
                    isOpen = !isOpen;
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }


            }
            else
            {
                ImGui.Dummy(new Vector2(20,20));
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                // Toggle all descendant nodes based on current state
                bool newState = !isOpen;
                _nextState[itemId] = newState;
            }

            nextState = _nextState.TryGetValue(itemId, out bool actualState) ? actualState : null;

            if (nextState != null)
            {
                _nodeStates[itemId] = nextState.Value;
                isOpen = nextState.Value;
            }


            ImGui.SameLine();

            using var tciPushId = ImRaii.PushId("tci_" + itemId);
            if (ImGui.ImageButton(ImGuiService.GetIconTexture(item.Item.Icon, item.Flags == InventoryItem.ItemFlags.HighQuality).Handle, new Vector2(32, 32) * ImGui.GetIO().FontGlobalScale))
            {

            }

            if (ImGui.IsItemHovered())
            {
                _tooltipService.DrawItemTooltip(new SearchResult(item));
            }

            if (ImGui.IsItemHovered() && ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("tci_" + index);
            }

            using (var popup = ImRaii.Popup("tci_" + index))
            {
                if (popup)
                {
                    MediatorService.Publish(_menuService.DrawRightClickPopup(item.Item));
                }
            }

            ImGui.SameLine();

            ImGui.TextUnformatted(item.FormattedName + "\n" + item.IngredientPreference.Type.FormattedName());

            ImGui.SameLine();

            var perItemRetainerRetrieval = SelectedConfiguration.CraftList.GetCraftRetainerRetrieval(item.ItemId);
            var retainerRetrievalDefault = item.IsOutputItem ? SelectedConfiguration.CraftList.CraftRetainerRetrievalOutput : SelectedConfiguration.CraftList.CraftRetainerRetrieval;
            var originalPos = ImGui.GetCursorPosY();
            _craftSettingsColumn.DrawRecipeIcon(SelectedConfiguration,index, item);
            ImGui.SetCursorPosY(originalPos);
            _craftSettingsColumn.DrawHqIcon(SelectedConfiguration, index, item);
            ImGui.SetCursorPosY(originalPos);
            _craftSettingsColumn.DrawRetainerIcon(SelectedConfiguration, index, item, perItemRetainerRetrieval, retainerRetrievalDefault);
            ImGui.SetCursorPosY(originalPos);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + SelectedConfiguration.TableHeight / 2.0f - 9);
            id.Pop();
            var settingsCursorX = ImGui.GetCursorPosX();
            if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Cog, ref settingsCursorX))
            {
                ImGui.OpenPopup("ConfigureItemSettings" + index + item.ItemId + (item.IsOutputItem ? "o" : ""));
            }

            ImGui.NewLine();



            // Draw children if open
            if (isOpen && item.ChildCrafts is { Count: > 0 })
            {
                for (var i = 0; i < item.ChildCrafts.Count; i++)
                {
                    var child = item.ChildCrafts[i];
                    using var childId = ImRaii.PushId(i);
                    DrawTreeCraftItem(child, itemId + "_" + i, index + 1, indentWidth + (hasSubcrafts ? 20f : 10f), nextState);
                }
            }

            _nextState.Remove(itemId);

            if (indentWidth != 0)
            {
                ImGui.Unindent(indentWidth);
            }
        }

        private void DrawSidebar()
        {
            var filterConfigurations = Filters;
            using (var sideMenuChild = ImRaii.Child("SideMenu", new Vector2(180, -1) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (sideMenuChild.Success)
                {
                    using (var craftListChild = ImRaii.Child("CraftList", new Vector2(0, -28) * ImGui.GetIO().FontGlobalScale, false))
                    {
                        if (craftListChild.Success)
                        {
                            for (var index = 0; index < filterConfigurations.Count; index++)
                            {
                                var filterConfiguration = filterConfigurations[index];
                                var actualName = filterConfiguration.Name;
                                if (filterConfiguration.IsEphemeralCraftList)
                                {
                                    actualName += " (*)";
                                }
                                if (ImGui.Selectable(actualName + "###fl" + filterConfiguration.Key,
                                        index == _selectedFilterTab))
                                {
                                    _selectedFilterTab = index;
                                    if (_configuration.SwitchFiltersAutomatically &&
                                        _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                        _configuration.ActiveUiFilter != null)
                                    {
                                        _framework.RunOnFrameworkThread(() =>
                                        {
                                            _listService.ToggleActiveUiList(filterConfiguration);
                                        });
                                    }
                                    if (_configuration.SwitchCraftListsAutomatically &&
                                        _configuration.ActiveCraftList != filterConfiguration.Key &&
                                        _configuration.ActiveCraftList != null && filterConfiguration.FilterType == FilterType.CraftFilter)
                                    {
                                        _framework.RunOnFrameworkThread(() =>
                                        {
                                            _listService.ToggleActiveCraftList(filterConfiguration);
                                        });
                                    }
                                }

                                GetFilterMenu(filterConfiguration, WindowLayout.Sidebar).Draw();
                            }

                            if (filterConfigurations.Count == 0)
                            {
                                ImGui.TextUnformatted("尚未创建制作清单。");
                            }

                            ImGui.Separator();
                            if (_filters != null && ImGui.Selectable("默认配置",
                                    filterConfigurations.Count + 1 == _selectedFilterTab))
                            {
                                _selectedFilterTab = filterConfigurations.Count + 1;
                            }
                        }
                    }

                    using (var commandBarChild = ImRaii.Child("CommandBar", new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale, false))
                    {
                        if (commandBarChild.Success)
                        {
                            float height = ImGui.GetWindowSize().Y;
                            ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                            var addCursorX = ImGui.GetCursorPosX();
                            if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Plus, ref addCursorX))
                            {
                                _pluginLogic.AddNewCraftFilter();
                            }

                            ImGuiUtil.HoverTooltip("添加新制作清单。");
                        }
                    }
                }
            }

            ImGui.SameLine();
        }

        private HorizontalSplitter _splitter;

        private unsafe void DrawCraftPanel(FilterConfiguration filterConfiguration)
        {
            if (_missingRequirementsDirty || filterConfiguration.Key != _lastMissingReqConfigKey)
            {
                _missingRequirements = _missingRequirementsGrouper.GetMissingRequirements(filterConfiguration.CraftList);
                _missingRequirementsDirty = false;
                _lastMissingReqConfigKey = filterConfiguration.Key;
            }
            var itemTable = _tableService.GetListTable(filterConfiguration);
            var craftTable = _tableService.GetCraftTable(filterConfiguration);
            using (var topBarChild = ImRaii.Child("TopBar", new Vector2(0, 40) * ImGui.GetIO().FontGlobalScale, true, ImGuiWindowFlags.NoScrollbar))
            {
                if (topBarChild.Success)
                {
                    var highlightItems = itemTable.HighlightItems;
                    ImGuiService.CenterElement(22 * ImGui.GetIO().FontGlobalScale);
                    ImGui.Checkbox("高亮？" + "###" + itemTable.Key + "VisibilityCheckbox", ref highlightItems);
                    if (highlightItems != itemTable.HighlightItems)
                    {
                        _framework.RunOnFrameworkThread(() =>
                        {
                            _listService.ToggleActiveUiList(itemTable.FilterConfiguration);
                        });
                    }
                    ImGuiUtil.HoverTooltip("勾选后，需要从外部来源取出的物品将被高亮。");

                    ImGui.SameLine();
                    var clearCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Times, ref clearCursorX))
                    {
                        itemTable.ClearFilters();
                    }

                    ImGuiUtil.HoverTooltip("清空当前搜索。");

                    ImGui.SameLine();
                    ImGuiService.CenterElement(22 * ImGui.GetIO().FontGlobalScale);
                    var hideCompleted = filterConfiguration.CraftList.HideComplete;
                    ImGui.Checkbox("隐藏已完成？" + "###" + itemTable.Key + "HideCompleted", ref hideCompleted);
                    if (hideCompleted != filterConfiguration.CraftList.HideComplete)
                    {
                        filterConfiguration.CraftList.HideComplete = hideCompleted;
                        filterConfiguration.NeedsRefresh = true;
                    }

                    ImGuiUtil.HoverTooltip("完成后隐藏半成品/采集/购买物品？");

                    ImGui.SameLine();
                    float width = ImGui.GetWindowSize().X;
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Search, ref width))
                    {
                        _addItemBarOpen = !_addItemBarOpen;
                        if (_addItemBarOpen) _missingRequirementsBarOpen = false;
                    }

                    ImGuiUtil.HoverTooltip("切换添加物品侧栏。");

                    ImGui.SameLine();
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    var hasMissingReqs = _missingRequirements.Count > 0;
                    if (hasMissingReqs)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.6f, 0.1f, 0.1f, 0.8f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.15f, 0.15f, 0.9f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.1f, 0.1f, 1f));
                    }

                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.ExclamationTriangle, ref width))
                    {
                        _missingRequirementsBarOpen = !_missingRequirementsBarOpen;
                        if (_missingRequirementsBarOpen) _addItemBarOpen = false;
                    }

                    if (hasMissingReqs)
                    {
                        ImGui.PopStyleColor(3);
                    }

                    var reqButtonPos = ImGui.GetItemRectMin();
                    _calloutService.DrawCallout(
                        NotificationPopup.MissingRequirementsButton,
                        "新功能：缺少的需求",
                        "当制作清单包含需要你尚未解锁内容的物品（配方书、采集秘籍、职业等级）时，此按钮会变红。点击它可查看具体缺少什么。",
                        reqButtonPos);

                    ImGuiUtil.HoverTooltip("显示当前制作清单缺少的需求。");

                    ImGui.SameLine();
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Edit, ref width))
                    {
                        var currentViewMode = _craftWindowViewSetting.CurrentValue(_configuration);
                        if (currentViewMode != CraftWindowView.Configuration)
                        {
                            _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Configuration);
                        }
                        else
                        {
                            _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Crafts);
                        }
                    }

                    ImGuiUtil.HoverTooltip("编辑制作清单的配置。");

                    ImGui.SameLine();
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    var isActiveList = _configuration.ActiveCraftList == filterConfiguration.Key;
                    if (ImGuiService.DrawIconButton(_font, isActiveList ? FontAwesomeIcon.ToggleOn : FontAwesomeIcon.ToggleOff, ref width))
                    {
                        _listService.ToggleActiveCraftList(filterConfiguration);
                    }
                    ImGuiUtil.HoverTooltip("切换当前制作清单。");

                    ImGui.SameLine();
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    var isTreeView = _craftWindowViewSetting.CurrentValue(_configuration) == CraftWindowView.Tree;
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.FolderTree, ref width))
                    {
                        if (_craftWindowViewSetting.CurrentValue(_configuration) == CraftWindowView.Tree)
                        {
                            _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Crafts);
                        }
                        else
                        {
                            _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Tree);
                        }
                    }
                    ImGuiUtil.HoverTooltip("打开制作清单的树状视图。");

                    ImGui.SameLine();
                    width -= 156 * ImGui.GetIO().FontGlobalScale;
                    ImGui.SetCursorPosX(width);
                    ImGui.SetNextItemWidth(150);
                    var activeCraftList = _listService.GetActiveCraftList();
                    using (var combo = ImRaii.Combo("##ActiveCraftList",activeCraftList != null ? activeCraftList.Name : "无"))
                    {
                        if (combo.Success)
                        {
                            if (ImGui.Selectable("无"))
                            {
                                _listService.ClearActiveCraftList();
                            }
                            foreach (var filter in _listService.Lists.Where(c =>
                                         c.FilterType == FilterType.CraftFilter && !c.CraftListDefault))
                            {
                                if (ImGui.Selectable(filter.Name + "##" + filter.Key))
                                {
                                    _listService.SetActiveCraftList(filter);
                                }
                            }
                        }
                    }
                    ImGuiUtil.HoverTooltip("已完成的制作将计入此清单。");
                    ImGui.SameLine();
                    var textSize = ImGui.CalcTextSize("活动: ");
                    width -= textSize.X * ImGui.GetIO().FontGlobalScale;
                    ImGui.SetCursorPosX(width);
                    ImGui.Text("活动: ");
                    if (SelectedConfiguration?.IsEphemeralCraftList ?? false)
                    {
                        ImGui.SameLine();
                        width -= 28 * ImGui.GetIO().FontGlobalScale;
                        ImGui.SetCursorPosX(width);
                        ImGui.Image(ImGuiService.GetImageTexture("recycle").Handle,
                            new Vector2(22, 22));
                        ImGuiUtil.HoverTooltip("这是临时制作清单，其中的物品全部完成后清单将自行删除。");
                    }
                }
            }

            using (var contentChild = ImRaii.Child("Content", new Vector2(0, -44) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (contentChild.Success)
                {
                    var craftWindowView = _craftWindowViewSetting.CurrentValue(_configuration);
                    if (craftWindowView == CraftWindowView.Crafts)
                    {
                        var result = _splitter.Draw(
                            (shouldDraw) =>
                            {
                                MediatorService.Publish(craftTable.Draw(new Vector2(0, 0), shouldDraw));
                            },
                            (shouldDraw) => { MediatorService.Publish(itemTable.Draw(new Vector2(0, 0), shouldDraw)); },
                            "待制作", "雇员/背包中的物品");
                        if (result != null)
                        {
                            _configuration.CraftWindowSplitterPosition = (int)result.Value;
                            _configuration.IsDirty = true;
                        }
                    }
                    else if (craftWindowView == CraftWindowView.Tree)
                    {
                        this.DrawTreeView(filterConfiguration);
                    }
                }
            }


            //Need to have these buttons be determined dynamically or moved elsewhere
            using (var bottomBarChild = ImRaii.Child("BottomBar", new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale,
                       true, ImGuiWindowFlags.NoScrollbar))
            {
                if (bottomBarChild.Success)
                {
                    var marketCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Sync, ref marketCursorX, verticalCenter: true, resetCursorY: true))
                    {
                        var activeCharacter = _characterMonitor.ActiveCharacter;
                        foreach (var item in itemTable.RenderSearchResults)
                        {
                            if (activeCharacter != null)
                            {
                                _universalis.QueuePriceCheck(item.Item.RowId, activeCharacter.WorldId);
                            }
                        }

                        foreach (var item in filterConfiguration.CraftList.GetFlattenedMergedMaterials())
                        {
                            var useActiveWorld = filterConfiguration.GetBooleanFilter("CraftWorldPriceUseActiveWorld");
                            var useHomeWorld = filterConfiguration.GetBooleanFilter("CraftWorldPriceUseHomeWorld");
                            var character = _characterMonitor.ActiveCharacter;
                            HashSet<uint> worldIds = new HashSet<uint>();

                            var marketItemWorldPreference = filterConfiguration.CraftList.GetMarketItemWorldPreference(item.ItemId);
                            if (marketItemWorldPreference != null)
                            {
                                worldIds.Add(marketItemWorldPreference.Value);
                            }

                            if (character != null)
                            {
                                if (useActiveWorld == true)
                                {
                                    worldIds.Add(character.ActiveWorldId);
                                }
                                if (useHomeWorld == true)
                                {
                                    worldIds.Add(character.WorldId);
                                }
                            }

                            foreach (var worldId in filterConfiguration.CraftList.WorldPricePreference)
                            {
                                worldIds.Add(worldId);
                            }

                            foreach (var worldId in worldIds)
                            {
                                _universalis.QueuePriceCheck(item.ItemId, worldId);
                            }
                        }
                    }

                    ImGuiUtil.HoverTooltip("刷新市场价格");
                    ImGui.SameLine();

                    ImGuiService.VerticalCenter("待处理市场查询: " + _universalis.QueuedCount);

                    if (_universalis.LastFailure != null)
                    {
                        ImGui.SameLine();
                        ImGui.Image(ImGuiService.GetIconTexture(Icons.ExclamationIcon).Handle,
                            new Vector2(22, 22));
                        ImGuiUtil.HoverTooltip($"联系 Universalis 时在 {_universalis.LastFailure.Value.ToString(CultureInfo.CurrentCulture)} 发生错误。这很可能意味着 Universalis 出现了问题。发生这种情况时，Allagan Tools 将暂停请求 30 秒。");
                    }

                    if (_universalis.TooManyRequests)
                    {
                        ImGui.SameLine();
                        ImGui.Image(ImGuiService.GetIconTexture(Icons.ExclamationIcon).Handle,
                            new Vector2(22, 22));
                        ImGuiUtil.HoverTooltip($"你似乎向 Universalis 发送了过多请求。如果你有多个插件在请求市场板数据，这就是最可能的原因。");
                    }

                    craftTable?.DrawFooterItems();
                    itemTable.DrawFooterItems();
                    ImGui.SameLine();


                    var width = ImGui.GetWindowSize().X;

                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Cog, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(ConfigurationWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开配置窗口。");

                    ImGui.SetCursorPosY(0);
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.List, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(FiltersWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开物品窗口。");

                    if (craftTable != null)
                    {

                        var totalItems =  itemTable.RenderSearchResults.Count + " 个物品 / " + craftTable.GetCraftListCount() + " 个制作物品";
                        var calcTextSize = ImGui.CalcTextSize(totalItems);
                        width -= calcTextSize.X + 15;
                        ImGui.SameLine();
                        ImGui.SetCursorPosX(width);
                        ImGuiService.VerticalCenter(totalItems);
                    }
                }
            }
        }

        private string? _newName;
        private void DrawSettingsPanel(FilterConfiguration filterConfiguration)
        {
            using (var contentChild = ImRaii.Child("Content", new Vector2(0, -44) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (contentChild.Success)
                {
                    var filterName = _newName ?? filterConfiguration.Name;
                    var labelName = "##" + filterConfiguration.Key;
                    if (ImGui.CollapsingHeader("常规",
                            ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.CollapsingHeader))
                    {
                        if (!filterConfiguration.CraftListDefault)
                        {
                            ImGui.SetNextItemWidth(100);
                            ImGui.LabelText(labelName + "FilterNameLabel", "名称: ");
                            ImGui.SameLine();
                            ImGui.InputText(labelName + "FilterName", ref filterName, 100);
                            if (filterName != _newName && filterName != filterConfiguration.Name)
                            {
                                _newName = filterName;
                            }

                            if (_newName != null)
                            {
                                ImGui.SameLine();
                                if (ImGui.Button("保存"))
                                {
                                    filterConfiguration.Name = _newName;
                                    Invalidate();
                                    _newName = null;
                                }
                            }

                            ImGui.NewLine();
                            if (ImGui.Button("导出配置到剪贴板"))
                            {
                                var base64 = _importExportService.ToBase64(filterConfiguration);
                                _clipboardService.CopyToClipboard(base64);
                                _chatUtilities.PrintClipboardMessage("[导出] ", "筛选器配置");
                            }
                        }
                        else
                        {
                            ImGui.TextWrapped(
                                "这是新制作清单的默认配置。任何新建的制作清单都将继承此清单的设置。");
                        }

                        var filterType = filterConfiguration.FormattedFilterType;
                        ImGui.SetNextItemWidth(100);
                        ImGui.LabelText(labelName + "FilterTypeLabel", "清单类型: ");
                        ImGui.SameLine();
                        ImGui.TextDisabled(filterType);

                    }

                    using (var tabBar = ImRaii.TabBar("###FilterConfigTabs", ImGuiTabBarFlags.FittingPolicyScroll))
                    {
                        if (tabBar.Success)
                        {
                            foreach (var group in _filterService.GroupedFilters)
                            {
                                var hasValuesSet = false;
                                foreach (var filter in group.Value)
                                {
                                    if (filter.HasValueSet(filterConfiguration) && filter.AvailableIn.HasFlag(filterConfiguration.FilterType))
                                    {
                                        hasValuesSet = true;
                                        break;
                                    }
                                }

                                using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen,
                                    hasValuesSet);

                                var hasValues = group.Value.Any(filter =>
                                    filter.AvailableIn.HasFlag(FilterType.SearchFilter) &&
                                    filterConfiguration.FilterType.HasFlag(
                                        FilterType.SearchFilter)
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.SortingFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .SortingFilter))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.CraftFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .CraftFilter))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.HistoryFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .HistoryFilter))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.CuratedList) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .CuratedList))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.GameItemFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .GameItemFilter)));
                                if (hasValues)
                                {
                                    using (var tabItem = ImRaii.TabItem(group.Key.ToString().ToSentence(), ImGuiTabItemFlags.NoReorder))
                                    {
                                        if (!tabItem.Success) continue;
                                        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite))
                                        {
                                            if (group.Key is FilterCategory.CraftColumns or FilterCategory.Columns)
                                            {
                                                using (var craftColumns = ImRaii.Child("craftColumns", new (0, -100 * ImGui.GetIO().FontGlobalScale)))
                                                {
                                                    if (craftColumns.Success)
                                                    {
                                                        group.Value.Single(c => c is CraftColumnsFilter or ColumnsFilter).Draw(filterConfiguration);
                                                    }
                                                }
                                                using (var otherFilters = ImRaii.Child("otherFilters", new (0, 0)))
                                                {
                                                    if (otherFilters.Success)
                                                    {
                                                        foreach (var filter in group.Value.Where(c => c is not CraftColumnsFilter && c is not ColumnsFilter))
                                                        {
                                                            if ((filter.AvailableIn.HasFlag(FilterType.SearchFilter) &&
                                                                 filterConfiguration.FilterType.HasFlag(FilterType
                                                                     .SearchFilter)
                                                                 ||
                                                                 (filter.AvailableIn.HasFlag(FilterType
                                                                      .SortingFilter) &&
                                                                  filterConfiguration.FilterType.HasFlag(FilterType
                                                                      .SortingFilter))
                                                                 ||
                                                                 (filter.AvailableIn.HasFlag(FilterType.CraftFilter) &&
                                                                  filterConfiguration.FilterType
                                                                      .HasFlag(FilterType.CraftFilter))
                                                                 ||
                                                                 (filter.AvailableIn.HasFlag(FilterType
                                                                      .HistoryFilter) &&
                                                                  filterConfiguration.FilterType.HasFlag(FilterType
                                                                      .HistoryFilter))
                                                                 ||
                                                                 (filter.AvailableIn.HasFlag(FilterType.CuratedList) &&
                                                                  filterConfiguration.FilterType.HasFlag(FilterType
                                                                      .CuratedList))
                                                                 ||
                                                                 (filter.AvailableIn.HasFlag(FilterType
                                                                      .GameItemFilter) &&
                                                                  filterConfiguration.FilterType.HasFlag(FilterType
                                                                      .GameItemFilter))
                                                                ))
                                                            {
                                                                filter.Draw(filterConfiguration);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (var filter in group.Value)
                                                {
                                                    if ((filter.AvailableIn.HasFlag(FilterType.SearchFilter) &&
                                                         filterConfiguration.FilterType.HasFlag(FilterType.SearchFilter)
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.SortingFilter) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .SortingFilter))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.CraftFilter) &&
                                                          filterConfiguration.FilterType
                                                              .HasFlag(FilterType.CraftFilter))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.HistoryFilter) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .HistoryFilter))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.CuratedList) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .CuratedList))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.GameItemFilter) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .GameItemFilter))
                                                        ))
                                                    {
                                                        filter.Draw(filterConfiguration);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            using (var bottomBarChild = ImRaii.Child("BottomBar", new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar))
            {
                if (bottomBarChild.Success)
                {
                    if (filterConfiguration.CraftListDefault)
                    {
                        ImGuiService.VerticalCenter(
                            "你正在编辑默认制作清单配置。");
                    }
                    else
                    {
                        ImGuiService.VerticalCenter(
                            "你正在编辑制作清单的配置。点击右侧的对勾以保存配置。");
                    }
                    float width = ImGui.GetWindowSize().X;

                    if (!filterConfiguration.CraftListDefault)
                    {
                        ImGui.SameLine();
                        width -= 28 * ImGui.GetIO().FontGlobalScale;
                        ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                        if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Check, ref width))
                        {
                            var currentViewMode = _craftWindowViewSetting.CurrentValue(_configuration);
                            _craftWindowViewSetting.UpdateFilterConfiguration(_configuration, CraftWindowView.Crafts);
                        }
                        ImGuiUtil.HoverTooltip("返回制作清单。");

                        ImGui.SameLine();
                        width -= 28 * ImGui.GetIO().FontGlobalScale;
                        ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                        if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Bomb, ref width))
                        {
                            ImGui.OpenPopup("confirmReset");
                        }

                        var result = InventoryTools.Ui.Widgets.ImGuiUtil.ConfirmPopup("confirmReset", new Vector2(400, 100), () =>
                        {
                            ImGui.TextWrapped("确定要将配置重置为默认值吗？");
                        });
                        if (result == true)
                        {
                            _listService.ResetFilter(_filterService.AvailableFilters, filterConfiguration);
                        }
                        ImGuiUtil.HoverTooltip("将制作清单重置为默认配置（保留物品）。");
                    }
                    else
                    {
                        ImGui.SameLine();
                        width -= 28 * ImGui.GetIO().FontGlobalScale;
                        ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                        if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Bomb, ref width))
                        {
                            ImGui.OpenPopup("重置默认制作清单？##defaultReset");
                        }

                        ImGuiUtil.HoverTooltip("重置为默认设置。");

                        using (var popup = ImRaii.Popup("重置默认制作清单？##defaultReset"))
                        {
                            if (popup.Success)
                            {
                                ImGui.TextUnformatted(
                                    "确定要重置默认制作清单吗？\n此操作无法撤销！\n\n");
                                ImGui.Separator();

                                if (ImGui.Button("确定", new Vector2(120, 0) * ImGui.GetIO().FontGlobalScale))
                                {
                                    _listService.ResetFilter(_filterService.AvailableFilters, DefaultConfiguration);
                                    ImGui.CloseCurrentPopup();
                                }

                                ImGui.SetItemDefaultFocus();
                                ImGui.SameLine();
                                if (ImGui.Button("取消", new Vector2(120, 0) * ImGui.GetIO().FontGlobalScale))
                                {
                                    ImGui.CloseCurrentPopup();
                                }
                            }
                        }
                    }
                    ImGui.SameLine();
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Clipboard, ref width))
                    {
                        ImGui.OpenPopup("copyFilter");
                    }
                    ImGuiUtil.HoverTooltip("复制现有筛选器的设置");

                    using (var popup = ImRaii.ContextPopup("copyFilter"))
                    {
                        if (popup.Success)
                        {
                            var filterConfigurations = Filters.Where(c => c != SelectedConfiguration).ToList();
                            foreach (var filter in filterConfigurations)
                            {
                                if (ImGui.Selectable("从“" + filter.Name + "”复制配置"))
                                {
                                    _listService.ResetFilter(_filterService.AvailableFilters, filterConfiguration, filter);
                                }
                            }

                            if (filterConfigurations.Count == 0)
                            {
                                ImGui.Text("没有可供复制的其他配置。");
                            }
                        }
                    }
                }
            }
        }



        private void DrawSearchRow(FilterConfiguration filterConfiguration, ItemRow item)
        {
            ImGui.TableNextColumn();
            ImGui.TextWrapped( item.NameString);
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled & ImGuiHoveredFlags.AllowWhenOverlapped & ImGuiHoveredFlags.AllowWhenBlockedByPopup & ImGuiHoveredFlags.AllowWhenBlockedByActiveItem & ImGuiHoveredFlags.AnyWindow) && ImGui.IsMouseReleased(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("RightClick" + item.RowId);
            }

            using (var popup = ImRaii.Popup("RightClick"+ item.RowId))
            {
                if (popup.Success)
                {
                    MediatorService.Publish(ImGuiService.ImGuiMenuService.DrawRightClickPopup(item));
                }
            }
            ImGui.TableNextColumn();
            using (ImRaii.PushId("s_" + item.RowId))
            {
                var addItemCursorX = ImGui.GetCursorPosX();
                if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Plus, ref addItemCursorX))
                {
                    _framework.RunOnFrameworkThread(() =>
                    {
                        filterConfiguration.CraftList.AddCraftItem(item.RowId, 1, InventoryItem.ItemFlags.None);
                        filterConfiguration.NeedsRefresh = true;
                    });
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }
            }
        }

        private string _searchString = "";
        private bool _searchCraftableOnly;
        private List<ItemRow>? _searchItems;
        public List<ItemRow> SearchItems
        {
            get
            {
                if (SearchString == "")
                {
                    _searchItems = new List<ItemRow>();
                    return _searchItems;
                }
                if (_searchItems == null)
                {
                    var query = _itemSheet.Where(c => c.NameString.ToLower().PassesFilter(SearchString.ToLower()));
                    if (_searchCraftableOnly)
                    {
                        query = query.Where(c => c.CanBeCrafted);
                    }
                    _searchItems = query.Take(100).Select(c => _itemSheet.GetRow(c.RowId)).ToList();
                }

                return _searchItems;
            }
        }

        public override FilterConfiguration? SelectedConfiguration
        {
            get
            {
                if (_selectedFilterTab >= 0 && _selectedFilterTab < Filters.Count) return Filters[_selectedFilterTab];
                return null;
            }
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                _searchItems = null;
            }
        }

        private void SaveCallback(FilterTable filterTable, bool arg1, string arg2)
        {
            if (arg1)
            {
                filterTable.ExportToCsv(arg2);
            }
        }

        private void SaveCraftCallback(CraftItemTable craftItemTable, bool arg1, string arg2)
        {
            if (arg1)
            {
                craftItemTable.ExportToCsv(arg2);
            }
        }

        public override void Invalidate()
        {
            var selectedConfiguration = SelectedConfiguration;
            _filters = null;
            if (selectedConfiguration != null)
            {
                FocusFilter(selectedConfiguration);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _throttleDispatcher?.Dispose();
        }

        public override void OnClose()
        {
            if (SelectedConfiguration != null)
            {
                SelectedConfiguration.Active = false;
            }
            foreach (var filter in Filters)
            {
                if (SelectedConfiguration == filter)
                {
                    filter.Active = false;
                }
            }
            base.OnClose();
        }
    }
}