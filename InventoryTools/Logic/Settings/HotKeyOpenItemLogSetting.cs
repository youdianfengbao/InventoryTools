using Dalamud.Game.ClientState.Keys;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Classes;

namespace InventoryTools.Logic.Settings
{
    public class HotkeyOpenItemLogSetting : HotKeySetting
    {
        public HotkeyOpenItemLogSetting(ILogger<HotkeyOpenItemLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ModifiableHotkey DefaultValue { get; set; } = new(VirtualKey.NO_KEY);

        public override ModifiableHotkey CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.OpenItemLogHotKey ?? new ModifiableHotkey();
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, ModifiableHotkey newValue)
        {
            configuration.OpenItemLogHotKey = newValue;
        }

        public override string Key { get; set; } = "OpenItemLogHotKey";
        public override string Name { get; set; } = "打开物品日志快捷键";

        public override string HelpText { get; set; } =
            "悬停在物品上时，按此快捷键打开对应的日志条目（采集、制作、钓鱼）。";
        
        public override string Version => "1.11.0.2";
    }
}