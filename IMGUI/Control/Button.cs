using System;
using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// Button
    /// </summary>
    /// <remarks>
    /// The button is a simple control, which only contains a text as its content.
    /// It handles the click event to respond when the user clicks a Button.
    /// </remarks>
    internal class Button
    {
        static class ButtonState
        {
            public const string Normal = "Normal";
            public const string Hover = "Hover";
            public const string Active = "Active";
        }

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

            var state = ButtonState.Normal;
            if (uiState.hotitem == id)
            {
                if (uiState.activeitem == id)
                {
                    state = ButtonState.Active;
                }
                else
                {
                    state = ButtonState.Hover;
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