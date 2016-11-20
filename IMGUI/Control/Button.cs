using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        static Dictionary<string, string> stateMap = new Dictionary<string, string>();
        static string GetState(string name)
        {
            string state;
            if(stateMap.TryGetValue(name, out state))
            {
                return state;
            }
            else
            {
                return ButtonState.Normal;
            }
        }

        static void SetState(string id, string state)
        {
            stateMap[id] = state;
        }

        public static bool DoControl(Rect rect, Content content, string id)
        {
            var uiState = Form.current.uiState;
            if (rect.Contains(Form.current.GetMousePos()))
            {
                uiState.hotitem = id;
                if (uiState.activeitem == null && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    uiState.activeitem = id;
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
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