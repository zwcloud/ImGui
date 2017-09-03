using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout collapsing header.
        /// </summary>
        /// <param name="text">header text</param>
        /// <param name="open">opened</param>
        /// <returns>true when opened</returns>
        /// <remarks> It is horizontally stretched (factor 1).</remarks>
        public static bool CollapsingHeader(string text, ref bool open, float scale = 1)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            var id = window.GetID(text);

            // style apply
            var style = GUIStyle.Basic;
            style.PushStretchFactor(false, 1);//+1, always expand width
            style.PushPadding(2);//4

            // rect
            var height = style.GetLineHeight();
            Rect rect = window.GetRect(id, new Size(0, height));
            if (rect == Layout.StackLayout.DummyRect)//TODO how shold dummy rect be correctly handled in every control?
            {
                style.PopStyle();//-1
                style.PopStyle(4);//-4
                return false;
            }

            // interact
            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, ButtonFlags.PressedOnClick);
            if (pressed)
            {
                open = !open;
            }

            // render
            {
                DrawList d = window.DrawList;
                style.PushBgColor(new Color(0.40f, 0.40f, 0.90f, 0.45f), GUIState.Normal);//+1 TODO It's stupid to sprcifiy style like this. There should be a better way to do this.
                style.PushBgColor(new Color(0.45f, 0.45f, 0.90f, 0.80f), GUIState.Hover);//+1
                style.PushBgColor(new Color(0.53f, 0.53f, 0.87f, 0.80f), GUIState.Active);//+1
                var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
                Color col = style.Get<Color>(GUIStyleName.BackgroundColor, state);
                d.RenderFrame(rect.Min, rect.Max, col, false, 0);
                style.PopStyle(3);
                d.RenderCollapseTriangle(rect.Min, open, rect.Height, Color.White, scale);
                rect.X += rect.Height;
                var delta = rect.Width - rect.Height;
                if (delta > 0)
                {
                    rect.Width = delta;
                }
                d.DrawText(rect, text, style, state);
            }

            // style restore
            style.PopStyle();//-1
            style.PopStyle(4);//-4

            return open;
        }

    }

    internal static partial class DrawListExtension
    {
        public static void RenderCollapseTriangle(this DrawList drawList, Point pMin, bool isOpen, double height, Color color, double scale = 1)
        {
            double h = height;
            double r = h * 0.40f * scale;
            Point center = pMin + new Vector(h * 0.50f, h * 0.50f) * scale;

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

            drawList.AddTriangleFilled(a, b, c, color);
        }
    }
}
