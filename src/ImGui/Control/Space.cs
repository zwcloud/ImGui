using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Put a fixed-size space inside a layout group.
        /// </summary>
        public static void Space(string str_id, double size)
        {
            Window window = GetCurrentWindow();
            var layout = window.StackLayout;

            int id = window.GetID(str_id);
            window.GetRect(id, Size.Zero, GUISkin.Instance[GUIControlName.Space],
                layout.InsideVerticalGroup ? new[] { GUILayout.Height(size) } : new[] { GUILayout.Width(size) });
        }

        private static Window GetCurrentWindow()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            return window;
        }

        /// <summary>
        /// Put a expanded space inside a layout group.
        /// </summary>
        public static void FlexibleSpace(string str_id)
        {
            Window window = GetCurrentWindow();
            var layout = window.StackLayout;

            int id = window.GetID(str_id);
            Rect rect = window.GetRect(id, Size.Zero, GUISkin.Instance[GUIControlName.Space],
                layout.InsideVerticalGroup ? new[] { GUILayout.StretchHeight(1) } : new[] { GUILayout.StretchWidth(1) });
        }
    }
}
