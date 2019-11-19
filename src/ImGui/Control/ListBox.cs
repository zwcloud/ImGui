using ImGui.Rendering;
using System;
using System.Collections.Generic;

namespace ImGui
{
    public partial class GUILayout
    {
        public static int ListBox<T>(string label, IReadOnlyList<T> items, int selectedIndex)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return selectedIndex;

            var id = window.GetID(label);
            PushID(id);
            //TODO create child window to support clip and render background box here
            BeginHorizontal("Field");
            {
                BeginVertical("Items", GUILayout.ExpandWidth(true));
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        var item_selected = (i == selectedIndex);
                        var item = items[i];
                        string itemText = item.ToString();

                        PushID(i);
                        if (GUILayout.Selectable(itemText, item_selected))
                        {
                            selectedIndex = i;
                        }
                        PopID();
                    }
                }
                EndVertical();
                GUILayout.Space("FieldSpacing", StyleRuleSet.Global.Get<double>("ControlLabelSpacing"));
                GUILayout.Label(label, GUILayout.Width(StyleRuleSet.Global.Get<double>("LabelWidth"))); 
            }
            EndHorizontal();
            PopID();

            return selectedIndex;
        }
    }

    internal partial class GUISkin
    {
        private void InitListBoxStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
            .Padding(2.0)
            .BackgroundColor(Color.Clear, GUIState.Normal)
            .BackgroundColor(Color.Rgb(206, 220, 236), GUIState.Hover)
            .BackgroundColor(Color.Rgb(30, 144, 255), GUIState.Active);
        }
    }
}
