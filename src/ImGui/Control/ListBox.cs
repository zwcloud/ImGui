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

            var style = GUIStyle.Basic;
            style.Save();

            GUILayout.Label(label);
            BeginVertical("ListBox");
            {
                style.ApplySkin(GUIControlName.Button);
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
            }
            EndVertical();
            style.Restore();

            return selectedIndex;
        }
    }
}
