using System;
using CriticalCommonLib.MarketBoard;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class MarketBoardSaleCountLimitSetting : IntegerSetting
    {
        public MarketBoardSaleCountLimitSetting(ILogger<MarketBoardSaleCountLimitSetting> logger, ImGuiService imGuiService, IHostedUniversalisConfiguration universalisConfiguration) : base(logger, imGuiService)
        {
            _universalisConfiguration = universalisConfiguration;
        }
        
        private readonly IHostedUniversalisConfiguration _universalisConfiguration;
        public override int DefaultValue { get; set; } = 7;
        public override int CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.MarketSaleHistoryLimit;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, int newValue)
        {
            newValue = Math.Min(30, newValue);
            newValue = Math.Max(1, newValue);
            _universalisConfiguration.SaleHistoryLimit = newValue;
            configuration.MarketSaleHistoryLimit = newValue;
        }

        public override string Key { get; set; } = "MBSaleCountLimit";
        public override string Name { get; set; } = "市场板出售历史天数";

        public override string WizardName { get; } = "出售历史上限";

        public override string HelpText { get; set; } =
            "计算物品出售总数时，往前追溯多少天的出售数据。修改此项不会清空已有数据，你需要手动请求刷新市场板价格，或等待自动刷新。";
        
        public override string Version => "1.7.0.0";
    }
}