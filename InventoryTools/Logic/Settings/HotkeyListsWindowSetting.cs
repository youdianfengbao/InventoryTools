using Dalamud.Game.ClientState.Keys;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Classes;

namespace InventoryTools.Logic.Settings
{
    public class HotKeyListsWindowSetting : HotKeySetting
    {
        public override ModifiableHotkey DefaultValue { get; set; } = new(VirtualKey.NO_KEY);

        public static string AsKey => "HotkeyListsWindow";
        public override string Key { get; set; } = AsKey;
        public override string Name { get; set; } = "切换清单窗口";

        public override string HelpText { get; set; } =
            "切换清单窗口的快捷键。";
        
        public override string Version => "1.7.0.12";

        public HotKeyListsWindowSetting(ILogger<HotKeyListsWindowSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}