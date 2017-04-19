using System.Diagnostics;

namespace ImGui
{
    class HoverButton
    {
        public static bool DoControl(Rect rect, Content content, string str_id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            DrawList d = form.DrawList;
            Window window = g.CurrentWindow;

            int id = window.GetID(str_id);
            var mousePos = form.GetMousePos();
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
                GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
            }

            return result;
        }
    }
}