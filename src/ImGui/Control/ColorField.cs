using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUI
    {
        public static Color ColorField(string label, Rect boxRect, Rect labelRect, Color value)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            // style apply
            var s = g.StyleStack;
            var colorFieldModifiers = GUISkin.Instance[GUIControlName.ColorField];
            s.PushRange(colorFieldModifiers);

            // interact


            // render
            var d = window.DrawList;
            var style = s.Style;
            d.AddRectFilled(boxRect, value);
            d.DrawText(labelRect, label, style, GUIState.Normal);

            // style restore
            s.PopStyle(colorFieldModifiers.Length);

            return value;
        }
    }

    public partial class GUILayout
    {
        public static Color ColorField(string label, Color value)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var id = window.GetID(label);

            // style apply
            var s = g.StyleStack;
            var style = s.Style;
            var colorFieldModifiers = GUISkin.Instance[GUIControlName.ColorField];
            s.PushRange(colorFieldModifiers);

            // rect
            var width = GUISkin.Instance.FieldControlWidth;
            var textSize = style.CalcSize(label, GUIState.Normal);
            var size = new Size(width + textSize.Width, textSize.Height);
            var rect = window.GetRect(id, size);
            var boxRect = new Rect(rect.X, rect.Y, width, textSize.Height);
            var labelRect = new Rect(boxRect.TopRight, rect.BottomRight);

            // interact


            // render
            var d = window.DrawList;
            d.AddRectFilled(boxRect, value);
            d.DrawText(labelRect, label, style, GUIState.Normal);

            // style restore
            s.PopStyle(colorFieldModifiers.Length);

            return value;
        }
    }

    internal partial class GUISkin
    {
        private void InitColorFieldStyles()
        {
            var colorFieldStyles = new StyleModifier[] { };
            this.styles.Add(GUIControlName.ColorField, colorFieldStyles);
        }
    }

}
