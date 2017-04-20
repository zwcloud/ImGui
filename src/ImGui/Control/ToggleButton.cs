namespace ImGui
{
    internal class ToggleButton
    {
        public static bool DoControl(Rect rect, Content content, bool value, string str_id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;
            int id = window.GetID(str_id);

            var mousePos = Input.Mouse.MousePos;
            var hovered = rect.Contains(mousePos);

            var result = value;

            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Input.Mouse.LeftButtonPressed)
                {
                    uiState.SetActiveID(id);
                }

                if (uiState.ActiveId == id && Input.Mouse.LeftButtonReleased)
                {
                    result = !value;
                    uiState.SetActiveID(GUIContext.None);
                }
            }

            // ui representation
            GUIState state = GUI.Normal;
            if(result)
            {
                state = GUI.Active;
                if (hovered && Input.Mouse.LeftButtonState == InputState.Up)
                {
                    state = GUI.Hover;
                }
            }
            else
            {
                if (hovered)
                {
                    state = GUI.Hover;
                }
                if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUI.Active;
                }
            }

            // ui painting
            {
                d.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
            }
            
            return result;
        }
    }
}