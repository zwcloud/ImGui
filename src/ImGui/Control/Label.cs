using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="text">text to display</param>
        public static void Label(Rect rect, string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // style apply
            var s = g.StyleStack;
            GUIStyle style = g.StyleStack.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Label];
            s.PushRange(modifiers);

            // rect
            window.GetRect(rect);

            // render
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, text, style);

            // style restore
            s.PopStyle(modifiers.Length);
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display</param>
        public static void Label(string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            int id = window.GetID(text);

            // style apply
            var s = g.StyleStack;
            var style = g.StyleStack.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Label];
            s.PushRange(modifiers);

            // rect
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id, contentSize);

            // rendering
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, text, style);

            // style restore
            s.PopStyle(modifiers.Length);
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