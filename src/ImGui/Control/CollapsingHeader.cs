using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool CollapsingHeader(string text, ref bool open)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.CollapsingHeader];
            s.PushRange(modifiers);
            s.PushStretchFactor(false, 1);//+1, always expand width

            var height = GUIStyle.Default.FontSize;
            var id = window.GetID(text);
            var rect = window.GetRect(id, new Size(0, height));
            if(rect == Layout.StackLayout.DummyRect)//TODO how shold dummy rect be correctly handled in every control?
            {
                s.PopStyle();//-1
                s.PopStyle(modifiers.Length);
                return false;
            }

            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, ButtonFlags.PressedOnClick);
            if (pressed)
            {
                open = !open;
            }

            // Render
            DrawList d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            Color col = s.Style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.RenderFrame(rect.Min, rect.Max, col, false, 0);
            RenderCollapseTriangle(rect.Min, open, rect.Height, Color.White);
            //TODO test if following adjust is valid
            rect.X += rect.Height;
            var detla = rect.Width - rect.Height;
            if (detla > 0)
            {
                rect.Width = detla;
            }
            d.DrawText(rect, text, s.Style, state);

            s.PopStyle();//-1
            s.PopStyle(modifiers.Length);

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

    partial class GUISkin
    {
        void InitCollapsingHeaderStyles()
        {
            var borderColor = Color.Black;
            var collapsingHeaderStyles = new StyleModifier[]
            {
                //normal
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 2.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Normal, GUIState.Normal),
                //hover
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 2.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Normal, GUIState.Hover),
                //active
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Bold, GUIState.Active),
            };
            this.styles.Add(GUIControlName.CollapsingHeader, collapsingHeaderStyles);
        }
    }

}
