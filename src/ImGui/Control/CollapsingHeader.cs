using System;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Internal;

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
            if (node == null)
            {
                //create nodes
                node = new Node(id, $"CollapsingHeader<{text}>");
                node.AttachLayoutEntry();
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.CollapsingHeader]);
            }
            node.RuleSet.ApplyOptions(options);
            node.RuleSet.ApplyOptions(Height(node.RuleSet.GetLineHeight()));
            node.ActiveSelf = true;

            container.AppendChild(node);

            // rect
            Rect rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(rect, id, out var hovered, out var held, ButtonFlags.PressedOnClick);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            if (pressed)
            {
                open = !open;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(node);
                dc.DrawGlyphRun(node.RuleSet, text, node.ContentRect.TopLeft + new Vector(node.Rect.Height + node.PaddingLeft, 0));
                dc.RenderArrow(node.Rect.Min + new Vector(node.PaddingLeft, 0),
                    node.Height, node.RuleSet.FontColor, open ? Direcion.Down : Direcion.Right, 1.0);
            }

            return open;
        }

        public static bool CollapsingHeader(string text, ref bool open) => CollapsingHeader(text, ref open, null);

    }

    internal static partial class DrawingContextExtension
    {
        public static void RenderArrow(this DrawingContext dc, Point pos, double height, Color color, Direcion dir, double scale)
        {
            var h = height;
            var r = h * 0.40f * scale;
            Point center = pos + new Vector(h * 0.50f, h * 0.50f * scale);

            Vector a, b, c;
            switch (dir)
            {
                case Direcion.Up:
                case Direcion.Down:
                    if (dir == Direcion.Up) r = -r;
                    a = new Vector(+0.000f,+0.750f) * r;
                    b = new Vector(-0.866f,-0.750f) * r;
                    c = new Vector(+0.866f,-0.750f) * r;
                    break;
                case Direcion.Left:
                case Direcion.Right:
                    if (dir == Direcion.Left) r = -r;
                    a = new Vector(+0.750f,+0.000f) * r;
                    b = new Vector(-0.750f,+0.866f) * r;
                    c = new Vector(-0.750f,-0.866f) * r;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir));
            }

            var A = center + a;
            var B = center + b;
            var C = center + c;
            PathFigure figure = new PathFigure(A,
                new[] {new LineSegment(B, false), new LineSegment(C, false), new LineSegment(A, false)},
                true);
            figure.IsFilled = true;
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
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 0.31f), GUIState.Normal);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 0.80f), GUIState.Hover);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 1.00f), GUIState.Active);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Normal);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Hover);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Active);
        }
    }
}
