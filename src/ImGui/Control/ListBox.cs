using ImGui.Common.Primitive;
using System.Collections.Generic;

namespace ImGui
{
    public partial class GUILayout
    {
        public static int ListBox<T>(string label, IReadOnlyList<T> items, int selectedIndex)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return selectedIndex;

            int id = window.GetID(label);
            PushID("ListboxField" + id);

            var style = GUIStyle.Basic;

            BeginHorizontal("Field");
            BeginVertical("Items", GUILayout.ExpandWidth(true));
            {
                style.Save();
                style.ApplySkin(GUIControlName.ListBox);
                style.PushStretchFactor(false, 1);
                style.PushCellSpacing(true, 0);
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    string itemText = item.ToString();
                    var itemId = window.GetID(string.Format("Item_{0}_{1}", i, itemText));
                    var textSize = style.CalcSize(itemText, GUIState.Normal);
                    var itemRect = window.GetRect(itemId, textSize);

                    bool hovered;
                    bool on = GUIBehavior.ToggleBehavior(itemRect, id, selectedIndex == i, out hovered);
                    if (on)
                    {
                        selectedIndex = i;
                    }

                    var d = window.DrawList;
                    var state = on ? GUIState.Active : GUIState.Normal;
                    d.DrawBoxModel(itemRect, itemText, style, state);
                }
                style.Restore();
            }
            EndVertical();
            GUILayout.Space("FieldSpacing", GUISkin.Current.FieldSpacing);
            GUILayout.Label(label, GUILayout.Width((int)GUISkin.Current.LabelWidth));
            EndHorizontal();

            PopID();

            return selectedIndex;
        }
    }

    internal partial class GUISkin
    {
        private void InitListBoxStyles()
        {
            StyleModifierBuilder builder = new StyleModifierBuilder();
            builder.PushPadding(2.0);
            builder.PushBgColor(Color.Clear, GUIState.Normal);
            builder.PushBgColor(Color.Rgb(206, 220, 236), GUIState.Hover);
            builder.PushBgColor(Color.Rgb(30, 144, 255), GUIState.Active);

            this.styles.Add(GUIControlName.ListBox, builder.ToArray());
        }
    }
}
