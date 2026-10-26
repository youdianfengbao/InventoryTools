using System.Collections.Generic;
using System.Numerics;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Lists;
using InventoryTools.Logic;
using InventoryTools.Mediator;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Ui;

public class TeamCraftImportWindow : GenericWindow
{
    private readonly ListImportExportService _importExportService;
    private string _importListItems = "";
    private bool _hasError;
    private List<(uint, uint)>? _parseResult;

    public TeamCraftImportWindow(ILogger<TeamCraftImportWindow> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, ListImportExportService importExportService, string name = "Teamcraft 导入") : base(logger, mediator, imGuiService, configuration, name)
    {
        _importExportService = importExportService;
        Flags = ImGuiWindowFlags.NoCollapse;
    }

    public List<(uint, uint)>? ParseResult => _parseResult;


    public override string GenericKey { get; } = "tcimport";
    public override string GenericName { get; } = "Teamcraft 导入";
    public override bool DestroyOnClose { get; }
    public override bool SaveState { get; } = false;
    public override Vector2? DefaultSize { get; } = new Vector2(300, 300);
    public override Vector2? MaxSize { get; }
    public override Vector2? MinSize { get; }
    public override void Initialize()
    {
    }

    public override void DrawWindow()
    {
        ImGui.Text("导入到制作清单: ");
        ImGui.SameLine();
        ImGuiService.HelpMarker("Teamcraft 清单导入指南。\r\n\r\n" +
                                "步骤 1. 在 Teamcraft 上打开一个包含你想要制作的物品的清单。\r\n\r\n" +
                                "步骤 2. 找到 Items 一栏的 \"Copy as Text\"（复制为文本）按钮，你只需要复制成品物品。\r\n\r\n" +
                                "步骤 3. 将其粘贴到本窗口下方的文本框中。\r\n\r\n" +
                                "步骤 4. 点击导入。");
        ImGui.Text("在此粘贴文本");
        ImGui.InputTextMultiline("###FinalItems", ref _importListItems, 10000000, new Vector2(ImGui.GetContentRegionAvail().X, 100));


        if (ImGui.Button("导入"))
        {
            var importedList = _importExportService.FromTCString(_importListItems ?? "");
            if (importedList is not null)
            {
                Close();
                MediatorService.Publish(new TeamCraftDataImported(importedList));
            }

        }
        ImGui.SameLine();
        if (ImGui.Button("取消"))
        {
            Close();
        }
    }

    public override void Invalidate()
    {
        
    }

    public override FilterConfiguration? SelectedConfiguration { get; }
}