using Dalamud.Game.ClientState.Keys;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Classes;

namespace InventoryTools.Logic.Settings
{
    public class HotkeyAirshipWindowSetting : HotKeySetting
    {
        public override ModifiableHotkey DefaultValue { get; set; } = new(VirtualKey.NO_KEY);
        public static string AsKey => "HotkeyAirshipsWindow";
        public override string Key { get; set; } = AsKey;
        public override string Name { get; set; } = "切换飞艇窗口";

        public override string HelpText { get; set; } =
            "切换飞艇窗口的快捷键。";
        
        public override string Version => "1.7.0.0";

        public HotkeyAirshipWindowSetting(ILogger<HotkeyAirshipWindowSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}