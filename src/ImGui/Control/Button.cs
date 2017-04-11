using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class Button
    {
        public static bool DoControl(Rect rect, Content content, string id)
        {
            var mousePos = Form.current.GetMousePos();

            if (Utility.CurrentOS.IsAndroid)
            {
                var clicked = false;
                var inside = rect.Contains(Input.Mouse.MousePos);

                //control logic
                var uiState = Form.current.uiState;
                uiState.KeepAliveId(id);
                if (inside && Input.Mouse.LeftButtonPressed)//start track
                {
                    uiState.SetActiveId(id);
                }

                if (uiState.ActiveId == id && Input.Mouse.LeftButtonReleased)//end track
                {
                    clicked = true;
                    uiState.SetActiveId(UIState.None);
                }

                // ui representation
                var state = GUIState.Normal;
                if (uiState.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUIState.Active;
                }

                // ui painting
                if (Event.current.type == EventType.Repaint)
                {
                    GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
                }
                return clicked;
            }
            else
            {
                var clicked = false;
                var hovered = rect.Contains(mousePos);

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
                        uiState.SetActiveId(UIState.None);
                    }
                }

                // ui representation
                var state = GUIState.Normal;
                if (hovered)
                {
                    state = GUIState.Hover;
                    if (uiState.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                    {
                        state = GUIState.Active;
                    }
                }

                // ui painting
                if (Event.current.type == EventType.Repaint)
                {
                    GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
                }
                return clicked;
            }
        }
    }
}