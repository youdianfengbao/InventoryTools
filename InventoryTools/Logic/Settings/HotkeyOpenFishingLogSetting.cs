using Dalamud.Game.ClientState.Keys;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Classes;

namespace InventoryTools.Logic.Settings
{
    public class HotkeyOpenFishingLogSetting : HotKeySetting
    {
        public HotkeyOpenFishingLogSetting(ILogger<HotkeyOpenFishingLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ModifiableHotkey DefaultValue { get; set; } = new(VirtualKey.NO_KEY);

        public override ModifiableHotkey CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.OpenFishingLogHotKey ?? new ModifiableHotkey();
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, ModifiableHotkey newValue)
        {
            configuration.OpenFishingLogHotKey = newValue;
        }

        public override string Key { get; set; } = "OpenFishingLogHotKey";
        public override string Name { get; set; } = "打开钓鱼笔记快捷键";

        public override string HelpText { get; set; } =
            "悬停在物品上时，按此快捷键打开该物品的钓鱼笔记。";
        
        public override string Version => "1.11.0.2";
    }
}