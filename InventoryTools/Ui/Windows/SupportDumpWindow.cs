using System.Linq;
using System.Numerics;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Ui
{
    public class SupportDumpWindow : GenericWindow
    {
        private readonly SupportDumpService _supportDumpService;
        private readonly IFileDialogManager _fileDialogManager;
        private readonly IChatUtilities _chatUtilities;

        public SupportDumpWindow(ILogger<SupportDumpWindow> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, SupportDumpService supportDumpService, IFileDialogManager fileDialogManager, IChatUtilities chatUtilities, string name = "生成支持转储") : base(logger, mediator, imGuiService, configuration, name)
        {
            _supportDumpService = supportDumpService;
            _fileDialogManager = fileDialogManager;
            _chatUtilities = chatUtilities;
        }

        public override void Initialize()
        {
            WindowName = "生成支持转储";
            Key = "supportdump";
        }

        public override bool SaveState => false;
        public override Vector2? DefaultSize { get; } = new Vector2(500, 200);
        public override Vector2? MaxSize { get; } = new Vector2(800, 400);
        public override Vector2? MinSize { get; } = new Vector2(300, 150);
        public override string GenericKey { get; } = "supportdump";
        public override string GenericName { get; } = "生成支持转储";
        public override bool DestroyOnClose => true;

        public override void DrawWindow()
        {
            ImGui.PushTextWrapPos();
            ImGui.TextWrapped(
                "只有在被明确指示时才点击此按钮，它会生成一个 zip 文件，其中包含你的库存、Allagan Tools 配置以及日志。如果你准备提供这些信息来帮助修复 bug，请先在 File 中开启详细日志（Verbose Logging），复现该 bug 后再生成此转储。是否继续？");
            ImGui.PopTextWrapPos();
            ImGui.NewLine();

            if (ImGui.Button("继续"))
            {
                _fileDialogManager.SaveFileDialog("保存支持转储", "*.zip", "support_dump.zip", ".zip",
                    (success, path) =>
                    {
                        if (success)
                        {
                            GenerateDump(path);
                        }
                    }, null, true);
            }

            ImGui.SameLine();

            if (ImGui.Button("取消"))
            {
                this.IsOpen = false;
            }
        }

        private void GenerateDump(string path)
        {
            var result = _supportDumpService.GenerateDump(path);
            if (result.Success)
            {
                var included = result.IncludedFiles.Any() ? string.Join(", ", result.IncludedFiles) : "无文件";
                _chatUtilities.Print($"支持转储已保存至 {result.ZipPath}（{included}）。");
                if (result.MissingFiles.Any())
                {
                    _chatUtilities.PrintError($"以下文件未能包含在支持转储中：{string.Join(", ", result.MissingFiles)}。");
                }
            }
            else
            {
                _chatUtilities.PrintError("生成支持转储失败。详情请查看 Dalamud 日志。");
            }

            this.IsOpen = false;
        }

        public override FilterConfiguration? SelectedConfiguration => null;

        public override void Invalidate()
        {

        }
    }
}
