using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using InventoryTools.Ui.Widgets;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace InventoryTools.Logic.Editors;

public class InventoryScopePicker
{
    private readonly ICharacterMonitor _characterMonitor;
    private readonly ImGuiService _imGuiService;
    private readonly ExcelSheet<World> _worldSheet;
    private readonly HoverImageButton _editButton;

    public InventoryScopePicker(ICharacterMonitor characterMonitor, ImGuiService imGuiService, ExcelSheet<World> worldSheet)
    {
        _characterMonitor = characterMonitor;
        _imGuiService = imGuiService;
        _worldSheet = worldSheet;
        _editButton = new("editButton");
    }

    public string GetScopeName(InventorySearchScope scope)
    {
        var scopeName = "";
        if (scope.Mode == InventorySearchScopeMode.Invert)
        {
            scopeName += "排除 ";
        }
        if (scope.CharacterId != null && scope.CharacterId != 0)
        {
            var character = _characterMonitor.GetCharacterById(scope.CharacterId.Value);
            scopeName += character?.FormattedName ?? "未知角色";
        }
        if (scope.ActiveCharacter != null)
        {
            scopeName += "当前角色";
        }

        if (scope.WorldId != null && scope.WorldId != 0)
        {
            var world = _worldSheet.GetRowOrDefault(scope.WorldId.Value);
            scopeName += world?.Name.ExtractText() ?? "未知世界";
        }

        if (scopeName == "")
        {
            scopeName = "全部";
        }

        if (scope.Invert)
        {
            scopeName += " (反转)";
        }

        return scopeName;
    }

    private InventorySearchScope? _selectedScope;

    public bool ValidateSearchScopes(List<InventorySearchScope> searchScopes)
    {
        var wasChanged = false;
        foreach (var scope in searchScopes)
        {
            if (scope.CharacterId != null && scope.CharacterId != 0)
            {
                var character = _characterMonitor.GetCharacterById(scope.CharacterId.Value);
                if (character != null && scope.Categories != null)
                {
                    foreach (var category in scope.Categories)
                    {
                        if (!category.IsApplicable(character.CharacterType))
                        {
                            scope.Categories.Remove(category);
                            wasChanged = true;
                            break;
                        }
                    }
                }
            }
        }

        return wasChanged;
    }

    public bool Draw(string label, List<InventorySearchScope> searchScopes)
    {

        var changed = false;
        var fakeRef = searchScopes.Count + " 个已定义的范围";
        using (ImRaii.Disabled())
        {
            ImGui.InputText(label, ref fakeRef, 200);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            using var tt = ImRaii.Tooltip();
            foreach (var searchScope in searchScopes)
            {
                ImGui.TextUnformatted(GetScopeName(searchScope));
                if (searchScope.Categories != null)
                {
                    using (var indent = ImRaii.PushIndent())
                    {
                        foreach (var category in searchScope.Categories)
                        {
                            ImGui.Text(category.FormattedDetailedName());
                        }
                    }
                }

                if (searchScope.CharacterTypes != null)
                {
                    using (var indent = ImRaii.PushIndent())
                    {
                        foreach (var characterType in searchScope.CharacterTypes)
                        {
                                                ImGui.Text(characterType.FormattedName() + " (所有库存)");
                        }
                    }
                }
            }
        }

        ImGui.SameLine();
        var cursorScreenPos = ImGui.GetCursorScreenPos();
        if (_editButton.Draw(_imGuiService.LoadImage("edit").GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale))
        {
            ImGui.OpenPopup("scopePopup");
            ImGui.SetNextWindowPos(cursorScreenPos);
        }

        ImGui.SetNextWindowSize(new Vector2(600, 600));
        using(var combo = ImRaii.Popup("scopePopup", ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoSavedSettings))
        {
            if (combo)
            {
                ImGui.Text("库存范围编辑器");
                using(var child = ImRaii.Child("selected", new Vector2(200, 0) * ImGui.GetIO().FontGlobalScale , true, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        using (var main = ImRaii.Child("main", new Vector2(0, -24) * ImGui.GetIO().FontGlobalScale))
                        {
                            if (main)
                            {
                                if (searchScopes.Count == 0)
                                {
                                    ImGui.TextWrapped("尚未定义任何范围。点击添加即可开始。");
                                }

                                for (var index = 0; index < searchScopes.Count; index++)
                                {
                                    var searchScope = searchScopes[index];
                                    if (ImGui.Selectable(GetScopeName(searchScope) + "##" + index, _selectedScope == searchScope))
                                    {
                                        _selectedScope = searchScope;
                                    }

                                    if (searchScope.Categories != null)
                                    {
                                        using (var indent = ImRaii.PushIndent())
                                        {
                                            foreach (var category in searchScope.Categories)
                                            {
                                                ImGui.Text(category.FormattedDetailedName());
                                            }
                                        }
                                    }

                                    if (searchScope.CharacterTypes != null)
                                    {
                                        using (var indent = ImRaii.PushIndent())
                                        {
                                            foreach (var characterType in searchScope.CharacterTypes)
                                            {
                                                ImGui.Text(characterType.FormattedName());
                                            }
                                        }
                                    }

                                    ImGui.Separator();
                                }
                            }
                        }

                        using (var commandBar = ImRaii.Child("commandBar", new Vector2(0, 24) * ImGui.GetIO().FontGlobalScale))
                        {
                            if (commandBar)
                            {
                                if (ImGui.Button("添加"))
                                {
                                    _selectedScope = new InventorySearchScope();
                                    searchScopes.Add(_selectedScope);
                                    changed = true;
                                }
                                ImGui.SameLine();
                                if (ImGui.Button("保存"))
                                {
                                    ImGui.CloseCurrentPopup();
                                }
                            }
                        }
                    }
                }

                ImGui.SameLine();
                using (var child = ImRaii.Child("editor", new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar))
                {
                    if (_selectedScope == null)
                    {
                        ImGui.TextWrapped("库存范围编辑器用于定义要跨哪些库存进行搜索。");
                        ImGui.TextWrapped("默认情况下，Allagan Tools 已知的所有库存都会被搜索。");
                        ImGui.TextWrapped("提供一组范围后，显示的库存范围将被缩小。");
                    }
                    else
                    {
                        if (child)
                        {
                            using (var main = ImRaii.Child("main", new Vector2(0, -24) * ImGui.GetIO().FontGlobalScale))
                            {
                                if (main)
                                {
                                    var isCharacter = _selectedScope.CharacterId != null;
                                    var isWorld = _selectedScope.WorldId != null;
                                    var isActiveCharacter = _selectedScope.ActiveCharacter != null;
                                    ImGui.Text("搜索范围：");
                                    ImGui.Separator();
                                    if (ImGui.RadioButton("全部",!isCharacter && !isWorld && !isActiveCharacter))
                                    {
                                        _selectedScope.Reset();
                                    }
                                    ImGui.SameLine();
                                    _imGuiService.HelpMarker("匹配所有库存");
                                    ImGui.NewLine();

                                    if (ImGui.RadioButton("角色",isCharacter))
                                    {
                                        _selectedScope.Reset();
                                        _selectedScope.CharacterId = 0;
                                    }
                                    ImGui.SameLine();
                                    _imGuiService.HelpMarker("匹配特定角色（玩家角色、雇员、部队等）");

                                    if (_selectedScope.CharacterId != null)
                                    {
                                        var selectedCharacter = _characterMonitor.GetCharacterById(_selectedScope.CharacterId.Value);
                                        using (var characterSelector = ImRaii.Combo("##character",
                                                   selectedCharacter?.FormattedName ?? "选择角色"))
                                        {
                                            if (characterSelector)
                                            {
                                                var allCharacters = _characterMonitor.AllCharacters();
                                                var byType = allCharacters.GroupBy(c => c.Value.CharacterType);
                                                foreach (var type in byType)
                                                {
                                                    ImGui.Text(type.Key.FormattedName());
                                                    ImGui.Separator();
                                                    foreach (var character in type)
                                                    {
                                                        if (ImGui.Selectable(character.Value.FormattedName))
                                                        {
                                                            _selectedScope.CharacterId = character.Key;
                                                        }
                                                    }
                                                    ImGui.NewLine();
                                                }
                                            }
                                        }
                                    }
                                    ImGui.NewLine();
                                    if (ImGui.RadioButton("当前角色",isActiveCharacter))
                                    {
                                        _selectedScope.Reset();
                                        _selectedScope.ActiveCharacter = true;
                                    }
                                    ImGui.SameLine();
                                    _imGuiService.HelpMarker("匹配当前登录的角色，包括该角色拥有的所有雇员/部队等。可使用分类或角色类型进一步筛选。");
                                    ImGui.NewLine();

                                    if (ImGui.RadioButton("世界",isWorld))
                                    {
                                        _selectedScope.Reset();
                                        _selectedScope.WorldId = 0;
                                    }
                                    ImGui.SameLine();
                                    _imGuiService.HelpMarker("匹配特定世界");
                                    if (_selectedScope.WorldId != null)
                                    {
                                        var selectedWorld = _selectedScope.WorldId == 0 ? null : _worldSheet.GetRowOrDefault(_selectedScope.WorldId.Value);
                                        using (var worldSelector = ImRaii.Combo("##world",
                                                   selectedWorld?.Name.ExtractText() ?? "选择世界"))
                                        {
                                            if (worldSelector)
                                            {
                                                foreach (var world in _worldSheet.Where(c => c.IsPublic))
                                                {
                                                    if (ImGui.Selectable(world.Name.ExtractText()))
                                                    {
                                                        _selectedScope.WorldId = world.RowId;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    ImGui.NewLine();
                                    ImGui.Separator();

                                    string categoryPreview;
                                    if (_selectedScope.Categories != null)
                                    {
                                        categoryPreview = String.Join(", ", _selectedScope.Categories.Select(c => c.FormattedDetailedName()));
                                    }
                                    else
                                    {
                                        categoryPreview = "选择分类";
                                    }

                                    ImGui.LabelText("##categories", "库存分类：");
                                    using (var categorySelector = ImRaii.Combo("##categories",categoryPreview))
                                    {
                                        if (categorySelector)
                                        {
                                            var inventoryCategories = Enum.GetValues<InventoryCategory>();
                                            if (_selectedScope.CharacterId != null)
                                            {
                                                var character = _characterMonitor.GetCharacterById(_selectedScope.CharacterId.Value);
                                                if (character != null)
                                                {
                                                    inventoryCategories = inventoryCategories.Where(c =>
                                                        c.IsApplicable(character.CharacterType)).ToArray();
                                                }
                                            }
                                            foreach (var category in inventoryCategories)
                                            {
                                                if (ImGui.Selectable(category.FormattedDetailedName(), _selectedScope.Categories?.Contains(category) ?? false))
                                                {
                                                    if (_selectedScope.Categories == null)
                                                    {
                                                        _selectedScope.Categories = new();
                                                    }

                                                    if (_selectedScope.Categories.Contains(category))
                                                    {
                                                        _selectedScope.Categories.Remove(category);
                                                        if (_selectedScope.Categories.Count == 0)
                                                        {
                                                            _selectedScope.Categories = null;
                                                        }
                                                        changed = true;
                                                    }
                                                    else
                                                    {
                                                        _selectedScope.Categories.Add(category);
                                                        changed = true;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    ImGui.SameLine();
                                    _imGuiService.HelpMarker("选择分类后，仅显示该分类下的物品。再次点击已选项可取消选择。");

                                    if (_selectedScope.CharacterId == null)
                                    {
                                        string characterTypesPreview;
                                        if (_selectedScope.CharacterTypes != null)
                                        {
                                            characterTypesPreview = String.Join(", ",
                                                _selectedScope.CharacterTypes.Select(c => c.FormattedName()));
                                        }
                                        else
                                        {
                                            characterTypesPreview = "选择角色类型";
                                        }

                                        ImGui.LabelText("##characterTypesLabel", "角色类型：");
                                        using (var characterTypeSelector =
                                               ImRaii.Combo("##characterTypes", characterTypesPreview))
                                        {
                                            if (characterTypeSelector)
                                            {
                                                foreach (var category in Enum.GetValues<CharacterType>().Where(c =>
                                                             c != CharacterType.Unknown && c != CharacterType.Orphaned))
                                                {
                                                    if (ImGui.Selectable(category.FormattedName(),
                                                            _selectedScope.CharacterTypes?.Contains(category) ?? false))
                                                    {
                                                        if (_selectedScope.CharacterTypes == null)
                                                        {
                                                            _selectedScope.CharacterTypes = new();
                                                        }

                                                        if (_selectedScope.CharacterTypes.Contains(category))
                                                        {
                                                            _selectedScope.CharacterTypes.Remove(category);
                                                            if (_selectedScope.CharacterTypes.Count == 0)
                                                            {
                                                                _selectedScope.CharacterTypes = null;
                                                            }

                                                            changed = true;
                                                        }
                                                        else
                                                        {
                                                            _selectedScope.CharacterTypes.Add(category);
                                                            changed = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        ImGui.SameLine();
                                        _imGuiService.HelpMarker("选择“全部”或“世界”时，选择要筛选的角色类型。再次点击已选项可取消选择。");
                                    }

                                    ImGui.Separator();
                                    ImGui.NewLine();
                                    var invert = _selectedScope.Invert;
                                    if (ImGui.Checkbox("反转", ref invert))
                                    {
                                        _selectedScope.Invert = invert;
                                        changed = true;
                                    }

                                    ImGui.SameLine();
                                    _imGuiService.HelpMarker("勾选后，匹配与所选内容相反的对象。");
                                }
                            }

                            using (var commandBar = ImRaii.Child("commandBar", new Vector2(0, 24) * ImGui.GetIO().FontGlobalScale))
                            {
                                if (commandBar)
                                {
                                    if (ImGui.Button("保存"))
                                    {
                                        _selectedScope = null;
                                        changed = true;
                                    }
                                    ImGui.SameLine();
                                    if (ImGui.Button("删除"))
                                    {
                                        searchScopes.Remove(_selectedScope);
                                        _selectedScope = null;
                                        changed = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (ValidateSearchScopes(searchScopes))
        {
            changed = true;
        }
        return changed;
    }
}