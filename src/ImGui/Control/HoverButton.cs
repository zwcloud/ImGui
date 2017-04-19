using System.Diagnostics;

namespace ImGui
{
    class HoverButton
    {
        public static bool DoControl(Rect rect, Content content, string str_id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            int id = window.GetID(str_id);
            var mousePos = Input.Mouse.MousePos
            var result = false;
            var hovered = rect.Contains(mousePos);

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
            {
                d.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
            }

            return result;
        }
    }
}