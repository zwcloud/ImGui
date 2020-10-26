using System.Collections.Generic;
using ImGui.Input;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a polyon-button.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="points"><see cref="Point"/> list of the polygon.</param>
        /// <param name="textRect">the rect that occupied by the text</param>
        /// <param name="text">text to display on the button</param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, string text)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create button node
                node = new Node(id, $"PolygonButton<{text}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);
            textRect = window.GetRect(textRect);

            // interact
            var mousePos = Mouse.Instance.Position;
            var g = GetCurrentContext();
            var clicked = false;
            var hovered = MathEx.IsPointInPolygon(mousePos, points, (Vector)node.Rect.Location);
            g.KeepAliveID(id);
            var state = GUIState.Normal;
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)//start track
                {
                    g.SetActiveID(id);
                }

                if (Mouse.Instance.LeftButtonReleased)//end track
                {
                    clicked = true;
                    g.SetActiveID(GUIContext.None);
                }
            }
            if (hovered)
            {
                state = GUIState.Hover;
                if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    state = GUIState.Active;
                }
            }
            node.State = state;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            GUIAppearance.DrawPolygonButton(points, textRect, text, node);

            return clicked;
        }

    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout polyon-button.
        /// </summary>
        /// <param name="points"><see cref="Point"/> list of the polygon.</param>
        /// <param name="textRect">the rect that occupied by the text</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>true when the users clicks the button.</returns>
        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, LayoutOptions? options)
        {
            if (points.Count < 3)
            {
                return false;
            }

            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create node
                node = new Node(id, $"PolygonButton<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
                var boundingRect = new Rect(points[0], points[1]);
                foreach (var point in points)
                {
                    boundingRect = Rect.Union(boundingRect, point);
                }
                node.AttachLayoutEntry(boundingRect.Size);
            }
            container.AppendChild(node);
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);
            textRect.Offset((Vector) node.Rect.Location);

            // interact
            var mousePos = Mouse.Instance.Position;
            var g = GetCurrentContext();
            var clicked = false;
            var hovered = MathEx.IsPointInPolygon(mousePos, points, (Vector)node.Rect.Location);
            g.KeepAliveID(id);
            var state = GUIState.Normal;
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)//start track
                {
                    g.SetActiveID(id);
                }

                if (Mouse.Instance.LeftButtonReleased)//end track
                {
                    clicked = true;
                    g.SetActiveID(GUIContext.None);
                }
            }
            if (hovered)
            {
                state = GUIState.Hover;
                if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    state = GUIState.Active;
                }
            }
            node.State = state;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            GUIAppearance.DrawPolygonButton(points, textRect, text, node);

            return clicked;
        }

        public static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text) => PolygonButton(points, textRect, text, null);
    }

    internal partial class GUIAppearance
    {
        public static void DrawPolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, Node node)
        {
            using (var d = node.RenderOpen())
            {
                var style = node.RuleSet;
                var offset = (Vector)node.Rect.Location;
                PathGeometry g = new PathGeometry();
                PathFigure f = new PathFigure();
                f.StartPoint = points[0] + offset;
                for (var i = 1; i < points.Count; i++)
                {
                    f.Segments.Add(new LineSegment(points[i] + offset, false));
                }
                f.Segments.Add(new LineSegment(f.StartPoint, false));
                f.IsClosed = false;
                f.IsFilled = true;
                g.Figures.Add(f);
                d.DrawGeometry(new Brush(style.BackgroundColor), new Pen(style.BorderLeftColor, style.BorderLeft), g);

                if (!string.IsNullOrWhiteSpace(text))
                {
                    d.DrawGlyphRun(node.RuleSet, text, textRect.Location);
                }
            }
        }
    }

}