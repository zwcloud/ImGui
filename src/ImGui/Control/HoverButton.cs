using System.Diagnostics;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the control</param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(Rect rect, string text)
        {
            return DoHoverButton(rect, text);
        }

        public static bool DoHoverButton(Rect rect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            DrawList d = window.DrawList;

            int id = window.GetID(text);
            var mousePos = Input.Mouse.Position;
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

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(string text, params LayoutOption[] options)
        {
            return HoverButton(text, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool HoverButton(string text, GUIStyle style, params LayoutOption[] options)
        {
            return DoHoverButton(text, style, options);
        }

        private static bool DoHoverButton(string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            Size size = style.CalcSize(text, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style, options);
            return GUI.HoverButton(rect, text);
        }
    }
}
