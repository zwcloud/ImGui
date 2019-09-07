using ImGui.OSAbstraction.Text;
using ImGui.Rendering;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout collapsing header.
        /// </summary>
        /// <param name="text">header text</param>
        /// <param name="open">opened</param>
        /// <param name="options">style options</param>
        /// <returns>true when opened</returns>
        /// <remarks> It is always horizontally stretched (factor 1).</remarks>
        public static bool CollapsingHeader(string text, ref bool open, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            int id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            Node node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            var displayText = (open ? "-" : "+") + text;
            if (node == null)
            {
                //create nodes
                node = new Node(id, $"CollapsingHeader<{text}>");
                node.AttachLayoutEntry();
                container.AppendChild(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.CollapsingHeader]);
            }
            node.RuleSet.ApplyOptions(options);
            node.RuleSet.ApplyOptions(Height(node.RuleSet.GetLineHeight()));
            node.ActiveSelf = true;

            // rect
            Rect rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(rect, id, out var hovered, out var held, ButtonFlags.PressedOnClick);
            if (pressed)
            {
                open = !open;
                node.State = open ? GUIState.Active : GUIState.Normal;
            }

            using (var dc = node.RenderOpen())
            {
                dc.DrawGlyphRun(new Brush(node.RuleSet.FontColor),
                    new GlyphRun(node.Rect.Location + new Vector(node.Rect.Height, 0), text, node.RuleSet.FontFamily,
                        node.RuleSet.FontSize));
                var color = GUISkin.Default[GUIControlName.Button].Get<Color>(GUIStyleName.BackgroundColor, node.State);
                dc.RenderCollapseTriangle(node.Rect.BottomLeft, open, node.Height, color);
            }

            return open;
        }

        public static bool CollapsingHeader(string text, ref bool open) => CollapsingHeader(text, ref open, null);

    }

    internal static partial class DrawingContextExtension
    {
        public static void RenderCollapseTriangle(this DrawingContext dc, Point pMin, bool isOpen, double height, Color color, double scale = 1)
        {
            double h = height;
            double r = h * 0.40f * scale;
            Point center = pMin + new Vector(h * 0.50f, h * 0.50f) * scale;

            Point a, b, c;
            if (isOpen)
            {
                center.Y -= r * 0.25f;
                a = center + new Vector(0, 1) * r;
                b = center + new Vector(0.866f, -0.5f) * r;
                c = center + new Vector(-0.866f, -0.5f) * r;
            }
            else
            {
                a = center + new Vector(1, 0) * r;
                b = center + new Vector(-0.500f, -0.866f) * r;
                c = center + new Vector(-0.500f, 0.866f) * r;
            }

            PathFigure figure = new PathFigure(a,
                new[] {new LineSegment(b, false), new LineSegment(c, false), new LineSegment(a, false)},
                true);
            PathGeometry path = new PathGeometry();
            path.Figures.Add(figure);
            dc.DrawGeometry(new Brush(color), new Pen(Color.Black, 1), path);
        }
    }

    internal partial class GUISkin
    {
        private void InitCollapsingHeaderStyles(StyleRuleSet button, out StyleRuleSet ruleSet)
        {
            ruleSet = new StyleRuleSet();
            ruleSet.Replace(button);
            ruleSet.Set(GUIStyleName.HorizontalStretchFactor, 1, GUIState.Normal);
            ruleSet.Set(GUIStyleName.HorizontalStretchFactor, 1, GUIState.Hover);
            ruleSet.Set(GUIStyleName.HorizontalStretchFactor, 1, GUIState.Active);
        }
    }
}
