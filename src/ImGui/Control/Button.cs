using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class Button
    {
        public static bool DoControl(Rect rect, Content content, string id)
        {
            var clicked = false;
            var hovered = rect.Contains(Form.current.GetMousePos());

            //control logic
            var uiState = Form.current.uiState;
            uiState.KeepAliveId(id);
            if (hovered)
            {
                uiState.SetHoverId(id);

                if (Input.Mouse.LeftButtonPressed)//start track
                {
                    uiState.SetActiveId(id);
                }

                if (Input.Mouse.LeftButtonReleased)//end track
                {
                    clicked = true;
                    uiState.SetActiveId(GUIState.None);
                }
            }

            // ui representation
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
                if (uiState.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUI.Active;
                }
            }

            // ui painting
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Button[state]);
            }

            return clicked;
        }

    }
}