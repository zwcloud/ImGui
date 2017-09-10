using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a button that acts like a toggle.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <returns>new value of the toggle-button</returns>
        public static bool ToggleButton(Rect rect, string text, bool value)
        {
            return DoToggleButton(rect, text, value);
        }

        public static bool DoToggleButton(Rect rect, string text, bool value)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            DrawList d = window.DrawList;
            int id = window.GetID(text);

            var mousePos = Mouse.Instance.Position;
            var hovered = rect.Contains(mousePos);

            var result = value;

            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)
                {
                    uiState.SetActiveID(id);
                }

                if (uiState.ActiveId == id && Mouse.Instance.LeftButtonReleased)
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
                if (hovered && Mouse.Instance.LeftButtonState == KeyState.Up)
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
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    state = GUI.Active;
                }
            }

            // ui painting
            {
                d.DrawBoxModel(rect, text, GUISkin.Instance[GUIControlName.Button], state);
            }
            
            return result;
        }
    }

    public partial class GUILayout
    { 
        /// <summary>
        /// Create an auto-layout button that acts like a toggle.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle-button</returns>
        public static bool ToggleButton(string text, bool value, params LayoutOption[] options)
        {
            return DoToggleButton(text, value, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool ToggleButton(string text, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoToggleButton(text, value, style, options);
        }

        private static bool DoToggleButton(string text, bool value, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            Size size = style.CalcSize(text, GUIState.Normal);
            var rect = window.GetRect(id, size, style, options);
            var result = GUI.ToggleButton(rect, text, value);
            return result;
        }
    }
}
