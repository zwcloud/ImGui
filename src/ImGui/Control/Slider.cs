using System;
using System.Net.Http.Headers;
using ImGui.Input;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a horizontal slider that user can drag to select a value.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="label">label</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double Slider(Rect rect, string label, double value, double minValue, double maxValue)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return 0;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            var text = Utility.FindRenderedText(label);
            if (node == null)
            {
                node = new Node(id, $"Slider<{label}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Slider]);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            var spacing = node.RuleSet.Get<double>("ControlLabelSpacing");
            var labelWidth = node.RuleSet.Get<double>("LabelWidth");
            var sliderWidth = rect.Width - spacing - labelWidth;
            if(sliderWidth <= 0)
            {
                sliderWidth = 1;
            }
            var sliderRect = new Rect(node.Rect.X, node.Rect.Y,
                sliderWidth,
                node.Rect.Height);
            bool hovered, held;
            value = GUIBehavior.SliderBehavior(sliderRect, id, true, value, minValue, maxValue, out hovered, out held);

            var state = GUIState.Normal;
            if (hovered)
            {
                state = GUIState.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUIState.Active;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;
            
            // render
            GUIAppearance.DrawSlider(node, text, value, minValue, maxValue, state, sliderRect, labelWidth);

            return value;
        }

        /// <summary>
        /// Create a vertical slider that user can drag to select a value.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="label">label</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double VSlider(Rect rect, string label, double value, double minValue, double maxValue)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return 0;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            var text = Utility.FindRenderedText(label);
            if (node == null)
            {
                node = new Node(id, $"VSlider<{label}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Slider]);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            var spacing = node.RuleSet.Get<double>("ControlLabelSpacing");
            var labelHeight = node.RuleSet.Get<double>("LabelHeight");
            var sliderHeight = rect.Height - spacing - labelHeight;
            if (sliderHeight <= 0)
            {
                sliderHeight = 1;
            }
            var sliderRect = new Rect(node.Rect.X, node.Rect.Y,
                node.Rect.Width, sliderHeight);
            value = GUIBehavior.SliderBehavior(sliderRect, id, false, value, minValue, maxValue, out var hovered, out _);

            var state = GUIState.Normal;
            if (hovered)
            {
                state = GUIState.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUIState.Active;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;
            
            // render
            GUIAppearance.DrawVSlider(node, label, value, minValue, maxValue, state, sliderRect, labelHeight);

            return value;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout horizontal slider that user can drag to select a value.
        /// </summary>
        /// <param name="label">label of the slider</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the left end of the slider.</param>
        /// <param name="maxValue">The value at the right end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double Slider(string label, double value, double minValue, double maxValue)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return 0;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            var text = Utility.FindRenderedText(label);
            if (node == null)
            {
                node = new Node(id, $"Slider<{label}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Slider]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                size.Width += size.Height + node.RuleSet.PaddingLeft;
                var minSilderWidth = 200;
                size.Width += minSilderWidth;
                size.Height = 20;
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            var spacing = node.RuleSet.Get<double>("ControlLabelSpacing");
            var labelWidth = node.RuleSet.Get<double>("LabelWidth");
            var sliderWidth = node.Rect.Width - spacing - labelWidth;
            if(sliderWidth <= 0)
            {
                sliderWidth = 1;
            }
            var sliderRect = new Rect(node.Rect.Location, sliderWidth, node.Rect.Height);
            value = GUIBehavior.SliderBehavior(sliderRect, id, true, value, minValue, maxValue, out var hovered, out var held);

            var state = GUIState.Normal;
            if (hovered)
            {
                state = GUIState.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUIState.Active;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;
            
            // render
            GUIAppearance.DrawSlider(node, label, value, minValue, maxValue, state, sliderRect, labelWidth);

            return value;
        }

        /// <summary>
        /// Create an auto-layout vertical slider that user can drag to select a value.
        /// </summary>
        /// <param name="label">label of the slider</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double VSlider(string label, double value, double minValue, double maxValue)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return 0;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            var text = Utility.FindRenderedText(label);
            if (node == null)
            {
                node = new Node(id, $"Slider<{label}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Slider]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                var minSilderHeight = 200;
                size.Width = 20;
                size.Height += minSilderHeight;
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            var spacing = node.RuleSet.Get<double>("ControlLabelSpacing");
            var labelHeight = node.RuleSet.Get<double>("LabelHeight");
            var sliderHeight = node.Rect.Height - spacing - labelHeight;
            if (sliderHeight <= 0)
            {
                sliderHeight = 1;
            }
            var sliderRect = new Rect(node.Rect.Location, node.Rect.Width, sliderHeight);
            value = GUIBehavior.SliderBehavior(sliderRect, id, false, value, minValue, maxValue, out var hovered, out var held);

            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUI.Active;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;
            
            // render
            GUIAppearance.DrawVSlider(node, label, value, minValue, maxValue, state, sliderRect, labelHeight);

            return value;
        }
    }

    internal partial class GUIBehavior
    {
        public static double SliderBehavior(Rect sliderRect, int id, bool horizontal, double value, double minValue, double maxValue, out bool hovered, out bool held)
        {
            GUIContext g = Form.current.uiContext;

            hovered = false;
            held = false;

            hovered = g.IsHovered(sliderRect, id);
            g.KeepAliveID(id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed) //start track
                {
                    g.SetActiveID(id);
                }
            }
            if (g.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    var mousePos = Mouse.Instance.Position;
                    if (horizontal)
                    {
                        var leftPoint = new Point(sliderRect.X + 10, sliderRect.Y + sliderRect.Height / 2);
                        var rightPoint = new Point(sliderRect.Right - 10, sliderRect.Y + sliderRect.Height / 2);
                        var minX = leftPoint.X;
                        var maxX = rightPoint.X;
                        var currentPointX = MathEx.Clamp(mousePos.X, minX, maxX);
                        value = minValue + (currentPointX - minX) / (maxX - minX) * (maxValue - minValue);
                    }
                    else
                    {
                        var upPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Y + 10);
                        var bottomPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Bottom - 10);
                        var minY = upPoint.Y;
                        var maxY = bottomPoint.Y;
                        var currentPointY = MathEx.Clamp(mousePos.Y, minY, maxY);
                        value = (float)(minValue + (currentPointY - minY) / (maxY - minY) * (maxValue - minValue));
                    }
                }
                else //end track
                {
                    g.SetActiveID(0);
                }
            }

            if(g.ActiveId == id)
            {
                held = true;
            }

            return value;
        }
    }

    internal partial class GUIAppearance
    {
        public static void DrawSlider(Node node, string label, double value, double minValue, double maxValue, GUIState state,
            Rect sliderRect, double labelWidth)
        {
            var rect = node.Rect;

            var colorForLineUsed = Color.Rgb(0, 151, 167);
            var colorForLineUnused = state == GUIState.Normal ? Color.Rgb(117, 117, 117) : Color.Rgb(255, 128, 171);

            //slider
            var h = sliderRect.Height;
            var a = 0.2f * h;
            var b = 0.3f * h;
            var leftPoint = new Point(sliderRect.X + 10, sliderRect.Y + sliderRect.Height / 2);
            var rightPoint = new Point(sliderRect.Right - 10, sliderRect.Y + sliderRect.Height / 2);

            var minX = leftPoint.X;
            var maxX = rightPoint.X;
            var currentPoint = leftPoint + new Vector((value - minValue) / (maxValue - minValue) * (maxX - minX), 0);

            var dc = node.RenderOpen();
            //slider
            dc.DrawLine(new Pen(colorForLineUsed, 2), leftPoint, currentPoint);
            dc.DrawLine(new Pen(colorForLineUnused, 2), currentPoint, rightPoint);
            var A = currentPoint + new Vector(-a,  b);
            var B = currentPoint + new Vector(-a, -b);
            var C = currentPoint + new Vector( a, -b);
            var D = currentPoint + new Vector( a,  b);

            PathGeometry g = new PathGeometry();
            PathFigure f = new PathFigure();
            f.StartPoint = A;
            f.Segments.Add(new LineSegment(B, false));
            f.Segments.Add(new ArcSegment(C, new Size(a,a), 0, true, SweepDirection.Clockwise, false));
            f.Segments.Add(new LineSegment(D, false));
            f.Segments.Add(new ArcSegment(A, new Size(a,a), 0, false, SweepDirection.Clockwise, false));
            f.IsClosed = true;
            f.IsFilled = true;
            g.Figures.Add(f);
            var fillColor = node.RuleSet.Get<Color>(StylePropertyName.BackgroundColor, state);
            dc.DrawGeometry(new Brush(fillColor), null, g);

            //label
            var labelRect = new Rect(rect.Right - labelWidth, rect.Y,
                labelWidth, rect.Height);
            dc.DrawGlyphRun(node.RuleSet, label, labelRect.Location);
            dc.Close();
        }

        public static void DrawVSlider(Node node, string label, double value, double minValue, double maxValue, GUIState state,
            Rect sliderRect, double labelHeight)
        {
            var rect = node.Rect;

            var colorForLineUsed = Color.Rgb(0, 151, 167);
            var colorForLineUnused = state == GUIState.Normal ? Color.Rgb(117, 117, 117) : Color.Rgb(255, 128, 171);

            var h = sliderRect.Width;
            var a = 0.2 * h;
            var b = 0.3 * h;
            var upPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Y + 10);
            var bottomPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Bottom - 10);

            var minY = upPoint.Y;
            var maxY = bottomPoint.Y;
            var currentPoint = upPoint + new Vector(0, (value - minValue) / (maxValue - minValue) * (maxY - minY));

            var dc = node.RenderOpen();
            //slider
            dc.DrawLine(new Pen(colorForLineUsed, 2), upPoint, currentPoint);
            dc.DrawLine(new Pen(colorForLineUnused, 2), currentPoint, bottomPoint);
            var A = currentPoint + new Vector(-b,  a);
            var B = currentPoint + new Vector(-b, -a);
            var C = currentPoint + new Vector( b, -a);
            var D = currentPoint + new Vector( b,  a);
            PathGeometry g = new PathGeometry();
            PathFigure f = new PathFigure();
            f.StartPoint = A;
            f.Segments.Add(new ArcSegment(B, new Size(a,a), 0, false, SweepDirection.Clockwise, false));
            f.Segments.Add(new LineSegment(C, false));
            f.Segments.Add(new ArcSegment(D, new Size(a,a), 0, true, SweepDirection.Clockwise, false));
            f.Segments.Add(new LineSegment(A, false));
            f.IsClosed = true;
            f.IsFilled = true;
            g.Figures.Add(f);
            var fillColor = node.RuleSet.Get<Color>(StylePropertyName.BackgroundColor, state);
            dc.DrawGeometry(new Brush(fillColor), null, g);

            //label
            var labelRect = new Rect(rect.X, rect.Bottom - labelHeight, rect.Width, labelHeight);
            dc.DrawGlyphRun(node.RuleSet, label, labelRect.TopLeft);
            dc.Close();
        }
    }

    internal partial class GUISkin
    {
        private void InitSliderStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder.BackgroundColor(new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal)
                .BackgroundColor(new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover)
                .BackgroundColor(new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active);
        }
    }
}
