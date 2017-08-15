using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display on the label</param>
        public static void Label(string text)
        {
            var g = GetCurrentContext();
            Window window = GetCurrentWindow();

            //apply skin and stack style modifiers
            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Label];
            s.PushRange(modifiers);

            int id = window.GetID(text);
            var style = g.StyleStack.Style;
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id, contentSize);
            GUI.DoLabel(rect, text);

            s.PopStyle(modifiers.Length);
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
            var g = GetCurrentContext();

            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Label];
            s.PushRange(modifiers);

            DoLabel(rect, text);

            s.PopStyle(modifiers.Length);
        }

        internal static void DoLabel(Rect rect, string text)
        {
            var g = GetCurrentContext();
            Window window = GetCurrentWindow();

            GUIStyle style = g.StyleStack.Style;
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, text, style);
        }
    }

    partial class GUISkin
    {
        void InitLabelStyles()
        {
            var labelStyles = new StyleModifier[] { };
            this.styles.Add(GUIControlName.Label, labelStyles);
        }
    }
}