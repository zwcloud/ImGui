using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static Color ColorField(string label, Color value)
        {
            return DoColorField(label, value);
        }

        private static Color DoColorField(string label, Color value)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var s = g.StyleStack;
            var colorFieldModifiers = GUISkin.Instance[GUIControlName.ColorField];
            s.PushRange(colorFieldModifiers);

            var id = window.GetID(label);
            var style = s.Style;
            var width = GUISkin.Instance.FieldControlWidth;
            var textSize = style.CalcSize(label, GUIState.Normal);
            var size = new Size(width + textSize.Width, textSize.Height);
            var rect = window.GetRect(id, size);
            var boxRect = new Rect(rect.X, rect.Y, width, textSize.Height);
            var labelRect = new Rect(boxRect.TopRight, rect.BottomRight);
            var result = GUI.ColorField(label, boxRect, labelRect, value);

            s.PopStyle(colorFieldModifiers.Length);

            return result;
        }
    }

    public partial class GUI
    {
        public static Color ColorField(string label, Rect boxRect, Rect labelRect, Color value)
        {
            return DoColorField(label, boxRect, labelRect, value);
        }

        private static Color DoColorField(string label, Rect boxRect, Rect labelRect, Color value)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var s = g.StyleStack;
            var colorFieldModifiers = GUISkin.Instance[GUIControlName.ColorField];
            s.PushRange(colorFieldModifiers);

            // draw
            var d = window.DrawList;
            var style = s.Style;
            d.AddRectFilled(boxRect, value);
            d.DrawText(labelRect, label, style, GUIState.Normal);

            s.PopStyle(colorFieldModifiers.Length);

            return value;
        }
    }

    partial class GUISkin
    {
        void InitColorFieldStyles()
        {
            var colorFieldStyles = new StyleModifier[] { };
            this.styles.Add(GUIControlName.ColorField, colorFieldStyles);
        }
    }

}
