using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display on the label</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        public static void Label(string text, params LayoutOption[] options)
        {
            DoLabel(text, GUISkin.Instance[GUIControlName.Label], options);
        }

        private static void DoLabel(string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(text);
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id, contentSize, style, options);
            GUI.Label(rect, text);
        }
    }

    public partial class GUI
    {
        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the label</param>
        /// <param name="id">the unique id of this control</param>
        public static void Label(Rect rect, string text)
        {
            DoLabel(rect, text);
        }

        internal static void DoLabel(Rect rect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            DrawList d = window.DrawList;

            d.DrawBoxModel(rect, text, GUISkin.Instance[GUIControlName.Label]);
        }
    }
}