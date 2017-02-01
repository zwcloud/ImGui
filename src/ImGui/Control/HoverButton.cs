using System.Diagnostics;

namespace ImGui
{
    class HoverButton
    {
        public static bool DoControl(Rect rect, Content content, string id)
        {
            var result = false;
            var hovered = rect.Contains(Form.current.GetMousePos());

            //control logic
            var uiState = Form.current.uiState;
            uiState.KeepAliveId(id);
            if (hovered)
            {
                uiState.HoverId = id;
                if (uiState.ActiveId == GUIState.None)
                {
                    uiState.SetActiveId(id);
                    result = true;
                }
            }
            else
            {
                if (uiState.ActiveId == id)
                {
                    uiState.SetActiveId(GUIState.None);
                }
                result = false;
            }

            // ui representation
            var state = GUI.Normal;
            if (uiState.ActiveId == id)
            {
                state = GUI.Active;
            }

            // ui painting
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Button[state]);
            }

            return result;
        }
    }
}