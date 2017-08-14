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
            s.Apply(GUISkin.Instance[GUIControlName.Label]);

            int id = window.GetID(text);
            var style = g.StyleStack.Style;
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id, contentSize);
            GUI.DoLabel(rect, text);

            s.Restore();
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
            //apply skin and stack style modifiers
            var s = g.StyleStack;
            s.Apply(GUISkin.Instance[GUIControlName.Button]);

            DoLabel(rect, text);

            s.Restore();
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