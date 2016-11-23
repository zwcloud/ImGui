using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class Button
    {
        public static bool DoControl(Rect rect, Content content, string id)
        {
            var uiState = Form.current.uiState;
            if (rect.Contains(Form.current.GetMousePos()))
            {
                uiState.hotitem = id;
                if (uiState.activeitem == GUIState.None && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    uiState.activeitem = id;
                }
            }

            var state = GUI.Normal;
            if (uiState.hotitem == id)
            {
                if (uiState.activeitem == id)
                {
                    state = GUI.Active;
                }
                else
                {
                    state = GUI.Hover;
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Button[state]);
            }

            if (Input.Mouse.LeftButtonState == InputState.Up &&
                uiState.hotitem == id && uiState.activeitem == id)
            {
                return true;
            }

            return false;
        }

    }
}