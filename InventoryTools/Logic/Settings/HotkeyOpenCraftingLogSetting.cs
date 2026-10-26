using Dalamud.Game.ClientState.Keys;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Classes;

namespace InventoryTools.Logic.Settings
{
    public class HotkeyOpenCraftingLogSetting : HotKeySetting
    {
        public HotkeyOpenCraftingLogSetting(ILogger<HotkeyOpenCraftingLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ModifiableHotkey DefaultValue { get; set; } = new(VirtualKey.NO_KEY);

        public override ModifiableHotkey CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.OpenCraftingLogHotKey ?? new ModifiableHotkey();
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, ModifiableHotkey newValue)
        {
            configuration.OpenCraftingLogHotKey = newValue;
        }

        public override string Key { get; set; } = "OpenCraftingLogHotKey";
        public override string Name { get; set; } = "打开制作笔记快捷键";

        public override string HelpText { get; set; } =
            "悬停在物品上时，按此快捷键打开该物品的制作笔记。";
        
        public override string Version => "1.11.0.2";
    }
}