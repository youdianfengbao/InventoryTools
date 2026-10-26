using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters.Abstract
{
    public abstract class BooleanFilter : Filter<bool?>
    {
        public override bool? DefaultValue { get; set; } = null;

        public override bool HasValueSet(FilterConfiguration configuration)
        {
            return CurrentValue(configuration) != DefaultValue;
        }

        public override bool? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.GetBooleanFilter(Key) ?? DefaultValue;
        }

        public string CurrentSelection(FilterConfiguration configuration)
        {
            var currentValue = CurrentValue(configuration);
            if (currentValue == null)
            {
                return "未设置";
            }

            if (currentValue == true)
            {
                return "是";
            }

            return "否";
        }

        public bool? ConvertSelection(string selection)
        {
            if (selection == "未设置")
            {
                return null;
            }

            if (selection == "是")
            {
                return true;
            }

            return false;
        }

        private readonly string[] Choices = new []{"未设置", "是", "否"};

        public virtual string[] GetChoices()
        {
            return Choices;
        }

        public override void Draw(FilterConfiguration configuration)
        {
            var currentValue = CurrentSelection(configuration);

            if (HasValueSet(configuration))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText("##" + Key + "Label", GetName(configuration) + ":");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText("##" + Key + "Label", GetName(configuration) + ":");
            }
            ImGui.Indent();
            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
            {
                    ImGui.PushTextWrapPos();
                    ImGui.TextUnformatted(GetHelpText(configuration));
                    ImGui.PopTextWrapPos();
            }
            ImGui.SetNextItemWidth(InputSize);
            using (var combo = ImRaii.Combo("##"+Key+"Combo", currentValue))
            {
                if (combo.Success)
                {
                    foreach (var item in GetChoices())
                    {
                        if (ImGui.Selectable(item, currentValue == item))
                        {
                            UpdateFilterConfiguration(configuration, ConvertSelection(item));
                        }
                    }
                }
            }
            if (HasValueSet(configuration) && ShowReset)
            {
                ImGui.SameLine();
                if (ImGui.Button("重置##" + Key + "Reset"))
                {
                    ResetFilter(configuration);
                }
            }
            ImGui.Unindent();
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
        {
            if (newValue.HasValue)
            {
                configuration.UpdateBooleanFilter(Key, newValue.Value);
            }
            else
            {
                configuration.RemoveBooleanFilter(Key);
            }
        }

        public override void ResetFilter(FilterConfiguration configuration)
        {
            UpdateFilterConfiguration(configuration, DefaultValue);
        }

        protected BooleanFilter(ILogger logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}