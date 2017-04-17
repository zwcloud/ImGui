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
            var uiState = Form.current.uiContext;
            uiState.KeepAliveId(id);
            if (hovered)
            {
                uiState.HoverId = id;
                if (uiState.ActiveId == GUIContext.None)
                {
                    uiState.SetActiveId(id);
                    result = true;
                }
            }
            else
            {
                if (uiState.ActiveId == id)
                {
                    uiState.SetActiveId(GUIContext.None);
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
                GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
            }

            return result;
        }
    }
}