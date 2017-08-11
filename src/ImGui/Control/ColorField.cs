using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static Color ColorField(string label, Color value)
        {
            return ColorField(label, value, GUISkin.Instance[GUIControlName.ColorField], null);
        }

        public static Color ColorField(string label, Color value, GUIStyle style, params LayoutOption[] options)
        {
            return DoColorField(label, value, style, options);
        }

        private static Color DoColorField(string label, Color value, GUIStyle style, params LayoutOption[] options)
        {
            var window = GetCurrentWindow();
            var id = window.GetID(label);

            var width = GUISkin.Instance.FieldControlWidth;
            var textSize = style.CalcSize(label, GUIState.Normal);
            var size = new Size(width + textSize.Width, textSize.Height);
            var rect = window.GetRect(id, size, style, options);
            var boxRect = new Rect(rect.X, rect.Y, width, textSize.Height);
            var labelRect = new Rect(boxRect.TopRight, rect.BottomRight);
            return GUI.ColorField(label, boxRect, labelRect, value);
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
            var window = GetCurrentWindow();

            // draw
            var d = window.DrawList;
            d.AddRectFilled(boxRect, value);
            d.DrawText(labelRect, label, GUIStyle.Default, GUIState.Normal);
            return value;
        }
    }
}
