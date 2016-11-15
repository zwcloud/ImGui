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

        public static bool DoControl(Rect rect, Content content, string name)
        {
            string lastState = GetState(name);

            string state;
            if (rect.Contains(Form.current.GetMousePos()))
            {
                if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = ButtonState.Active;
                }
                else
                {
                    state = ButtonState.Hover;
                }
            }
            else
            {
                state = ButtonState.Normal;
            }
            Debug.WriteLineIf(lastState != state, string.Format("Button##{0} {1}=>{2}", name, lastState, state));
            SetState(name, state);

            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Button[state]);
            }

            return lastState == ButtonState.Hover && state == ButtonState.Active;
        }

    }
}