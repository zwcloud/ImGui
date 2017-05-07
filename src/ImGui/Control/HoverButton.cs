using System.Diagnostics;

namespace ImGui
{
    class HoverButton
    {
        public static bool DoControl(Rect rect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            int id = window.GetID(text);
            var mousePos = Input.Mouse.MousePos;
            var result = false;
            var hovered = rect.Contains(mousePos);

            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);
                if (uiState.ActiveId == GUIContext.None)
                {
                    uiState.SetActiveID(id);
                    result = true;
                }
            }
            else
            {
                if (uiState.ActiveId == id)
                {
                    uiState.SetActiveID(GUIContext.None);
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
                d.DrawBoxModel(rect, text, GUISkin.Instance[GUIControlName.Button], state);
            }

            return result;
        }
    }
}