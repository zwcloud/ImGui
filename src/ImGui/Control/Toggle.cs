using System;
using ImGui.Input;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a toggle (check-box) with a label.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="label">label</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(Rect rect, string label, bool value)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            var text = Utility.FindRenderedText(label);
            if (node == null)
            {
                node = new Node(id, $"Toggle<{label}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Toggle]);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            value = GUIBehavior.ToggleBehavior(node.Rect, id, value, out var hovered);

            // render
            var state = GUIState.Normal;
            if (hovered)
            {
                state = GUIState.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUIState.Active;
            }
            GUIAppearance.DrawToggle(node, text, value, state);

            return value;
        }

    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout toggle (check-box) with an label.
        /// </summary>
        /// <param name="label">text to display</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(string label, bool value)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            var text = Utility.FindRenderedText(label);
            if (node == null)
            {
                node = new Node(id, $"Toggle<{label}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Toggle]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                size.Width += size.Height + node.RuleSet.PaddingLeft;
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            value = GUIBehavior.ToggleBehavior(node.Rect, id, value, out var hovered);

            // render
            var state = GUIState.Normal;
            if (hovered)
            {
                state = GUIState.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUIState.Active;
            }
            GUIAppearance.DrawToggle(node, text, value, state);

            return value;
        }

        /// <summary>
        /// alias of Toggle
        /// </summary>
        public static bool CheckBox(string label, bool value) => Toggle(label, value);

    }

    internal partial class GUIBehavior
    {
        public static bool ToggleBehavior(Rect rect, int id, bool value, out bool hovered)
        {
            GUIContext g = Form.current.uiContext;

            hovered = g.IsHovered(rect, id);
            g.KeepAliveID(id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)
                {
                    g.SetActiveID(id);
                }

                if (g.ActiveId == id && Mouse.Instance.LeftButtonReleased)
                {
                    value = !value;
                    g.SetActiveID(0);
                }
            }
            return value;
        }
    }

    internal partial class GUIAppearance
    {
        /// <remarks>
        /// Note: Design of a toggle
        /// |←16→|
        /// |    |---------------+
        /// |    |               |
        /// +----+               |
        /// | √  | label         |
        /// +----+               |
        ///      |               |
        ///      +---------------+
        /// </remarks>
        public static void DrawToggle(Node node, string label, bool value, GUIState state)
        {
            var rect = node.Rect;
            var spacing = StyleRuleSet.Global.Get<double>("ControlLabelSpacing");
            var boxRect = new Rect(rect.X, rect.Y + MathEx.ClampTo0(rect.Height - 16) / 2, 16, 16);
            var labelRect = new Rect(rect.X + 16 + spacing, rect.Y, MathEx.ClampTo0(rect.Width - 16 - spacing),
                rect.Height);

            // box
            var filledBoxColor = Color.Rgb(0, 151, 167);
            var boxBorderColor = Color.White;
            var tickColor = Color.Rgb(48, 48, 48);

            using (var dc = node.RenderOpen())
            {
                dc.DrawRectangle(new Brush(filledBoxColor), new Pen(boxBorderColor, 1), boxRect) ; //□
                if (value) //√
                {
                    var h = boxRect.Height;
                    var tick = new PathGeometry();
                    var figure = new PathFigure();
                    figure.StartPoint = new Point(0.125f * h + boxRect.X, 0.50f * h + boxRect.Y);
                    figure.Segments.Add(new LineSegment(new Point(0.333f * h + boxRect.X, 0.75f * h + boxRect.Y), true));
                    figure.Segments.Add(new LineSegment(new Point(0.875f * h + boxRect.X, 0.25f * h + boxRect.Y), true));
                    figure.IsFilled = false;
                    tick.Figures.Add(figure);
                    dc.DrawGeometry(null, new Pen(tickColor, 2), tick);
                }
                // label
                dc.DrawBoxModel(label, node.RuleSet, labelRect);
            }
        }
    }

    internal partial class GUISkin
    {
        private void InitToggleStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Padding(1.0, GUIState.Normal)
                .Padding(1.0, GUIState.Hover)
                .Padding(1.0, GUIState.Active)
                .AlignmentVertical(Alignment.Center, GUIState.Normal)
                .AlignmentVertical(Alignment.Center, GUIState.Hover)
                .AlignmentVertical(Alignment.Center, GUIState.Active)
                .AlignmentHorizontal(Alignment.Start, GUIState.Normal)
                .AlignmentHorizontal(Alignment.Start, GUIState.Hover)
                .AlignmentHorizontal(Alignment.Start, GUIState.Active);
        }
    }
}

#region TODO
// toggle with label on the right (maybe this is the right choice)
// toggle without label
// tristate
#endregion
