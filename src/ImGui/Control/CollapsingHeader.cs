using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool CollapsingHeader(string text, ref bool open)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            var height = GUIStyle.Default.FontSize;
            var id = window.GetID(text);
            GUIStyle style = GUISkin.Instance[GUIControlName.Button];
            var rect = GUILayout.GetRect(new Size(0, height), text, style, GUILayout.ExpandWidth(true));

            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, ButtonFlags.PressedOnClick);
            if (pressed)
            {
                open = !open;
            }

            // Render
            DrawList d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            Color col = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.RenderFrame(rect.Min, rect.Max, col, false, 0);
            RenderCollapseTriangle(rect.Min, open, rect.Height, Color.White);
            //TODO test if following adjust is valid
            rect.X += rect.Height;
            rect.Width -= rect.Height;
            d.DrawText(rect, text, style, state);

            return open;
        }

        private static void RenderCollapseTriangle(Point pMin, bool isOpen, double height, Color color, double scale = 1)
        {
            Window window = GetCurrentWindow();

            double h = height;
            double r = h * 0.40f * scale;
            Point center = pMin + new Vector(h * 0.50f, h * 0.50f * scale);

            Point a, b, c;
            if (isOpen)
            {
                center.Y -= r * 0.25f;
                a = center + new Vector(0, 1) * r;
                b = center + new Vector(-0.866f, -0.5f) * r;
                c = center + new Vector(0.866f, -0.5f) * r;
            }
            else
            {
                a = center + new Vector(1, 0) * r;
                b = center + new Vector(-0.500f, 0.866f) * r;
                c = center + new Vector(-0.500f, -0.866f) * r;
            }

            window.DrawList.AddTriangleFilled(a, b, c, color);
        }
    }
}