using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.Input;

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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            int id = window.GetID(label);

            // rect
            rect = window.GetRect(rect);

            // interact
            var spacing = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            var labelWidth = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._LabelWidth);
            var sliderWidth = rect.Width - spacing - labelWidth;
            if(sliderWidth <= 0)
            {
                sliderWidth = 1;
            }
            var sliderRect = new Rect(rect.X, rect.Y,
                sliderWidth,
                rect.Height);
            bool hovered, held;
            value = GUIBehavior.SliderBehavior(sliderRect, id, true, value, minValue, maxValue, out hovered, out held);

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
            GUIAppearance.DrawSlider(rect, label, value, minValue, maxValue, state, sliderRect, labelWidth);

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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            int id = window.GetID(label);

            // rect
            rect = window.GetRect(rect);

            // interact
            var spacing = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            var labelHeight = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._LabelHeight);
            var sliderHeight = rect.Height - spacing - labelHeight;
            if (sliderHeight <= 0)
            {
                sliderHeight = 1;
            }
            var sliderRect = new Rect(rect.X, rect.Y,
                rect.Width, sliderHeight);
            bool hovered, held;
            value = GUIBehavior.SliderBehavior(sliderRect, id, false, value, minValue, maxValue, out hovered, out held);

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
            GUIAppearance.DrawVSlider(rect, label, value, minValue, maxValue, state, sliderRect, labelHeight);

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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var id = window.GetID(label);

            // style apply
            var s = g.StyleStack;
            var style = GUIStyle.Basic;

            // rect
            Size size = style.CalcSize(label, GUIState.Normal);
            s.PushStretchFactor(false, 1);//+1
            {
                var minSilderWidth = 200;
                size.Width += minSilderWidth;
                size.Height = 20;
            }
            var rect = window.GetRect(id, size);

            // interact
            var spacing = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            var labelWidth = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._LabelWidth);
            var sliderWidth = rect.Width - spacing - labelWidth;
            if(sliderWidth <= 0)
            {
                sliderWidth = 1;
            }
            var sliderRect = new Rect(rect.X, rect.Y,
                sliderWidth,
                rect.Height);
            bool hovered, held;
            value = GUIBehavior.SliderBehavior(sliderRect, id, true, value, minValue, maxValue, out hovered, out held);

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
            GUIAppearance.DrawSlider(rect, label, value, minValue, maxValue, state, sliderRect, labelWidth);

            // style restore
            s.PopStyle();//-1

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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var id = window.GetID(label);

            // style apply
            var s = g.StyleStack;
            var style = GUIStyle.Basic;

            // rect
            Size size = style.CalcSize(label, GUIState.Normal);
            s.PushStretchFactor(true, 1);//+1
            {
                var minSilderHeight = 200;
                size.Width = 20;
                size.Height += minSilderHeight;
            }
            var rect = window.GetRect(id, size);

            // interact
            var spacing = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            var labelHeight = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._LabelHeight);
            var sliderHeight = rect.Height - spacing - labelHeight;
            if (sliderHeight <= 0)
            {
                sliderHeight = 1;
            }
            var sliderRect = new Rect(rect.X, rect.Y,
                rect.Width,
                sliderHeight);

            bool hovered, held;
            value = GUIBehavior.SliderBehavior(sliderRect, id, false, value, minValue, maxValue, out hovered, out held);

            // render
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
            }
            if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUI.Active;
            }
            GUIAppearance.DrawVSlider(rect, label, value, minValue, maxValue, state, sliderRect, labelHeight);

            // style restore
            s.PopStyle();//-1

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
        public static void DrawSlider(Rect rect, string label, double value, double minValue, double maxValue, GUIState state,
            Rect sliderRect, double labelWidth)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;
            StyleStack s = g.StyleStack;
            GUIStyle style = GUIStyle.Basic;
            DrawList d = window.DrawList;

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
            var currentPoint = leftPoint
                               + new Vector((value - minValue) / (maxValue - minValue) * (maxX - minX), 0);

            var topArcCenter = currentPoint + new Vector(0, b);
            var bottomArcCenter = currentPoint + new Vector(0, -b);
            var bottomStartPoint = bottomArcCenter + new Vector(-a, 0);

            d.PathMoveTo(leftPoint);
            d.PathLineTo(currentPoint);
            d.PathStroke(colorForLineUsed, false, 2);

            d.PathMoveTo(currentPoint);
            d.PathLineTo(rightPoint);
            d.PathStroke(colorForLineUnused, false, 2);

            d.PathArcToFast(topArcCenter, a, 0, 6);
            d.PathLineTo(bottomStartPoint);
            d.PathArcToFast(bottomArcCenter, a, 6, 12);
            d.PathClose();

            //label
            var labelRect = new Rect(rect.Right - labelWidth, rect.Y,
                labelWidth, rect.Height);
            d.DrawText(labelRect, label, style, state);

            s.PushBgColor(new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal);//+1 TODO It's stupid to sprcifiy style like this. There should be a better way to do this.
            s.PushBgColor(new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover);//+1
            s.PushBgColor(new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active);//+1
            var fillColor = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.PathFill(fillColor);
            s.PopStyle(3);
        }

        public static void DrawVSlider(Rect rect, string label, double value, double minValue, double maxValue, GUIState state,
            Rect sliderRect, double labelHeight)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;
            StyleStack s = g.StyleStack;
            GUIStyle style = GUIStyle.Basic;
            DrawList d = window.DrawList;

            var colorForLineUsed = Color.Rgb(0, 151, 167);
            var colorForLineUnused = state == GUIState.Normal ? Color.Rgb(117, 117, 117) : Color.Rgb(255, 128, 171);

            //slider
            var h = sliderRect.Width;
            var a = 0.2 * h;
            var b = 0.3 * h;
            var upPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Y + 10);
            var bottomPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Bottom - 10);

            var minY = upPoint.Y;
            var maxY = bottomPoint.Y;
            var currentPoint = upPoint + new Vector(0, (value - minValue) / (maxValue - minValue) * (maxY - minY));

            var leftArcCenter = currentPoint + new Vector(-b, 0);
            var rightArcCenter = currentPoint + new Vector(b, 0);
            var rightStartPoint = rightArcCenter + new Vector(0, -a);

            d.PathMoveTo(upPoint);
            d.PathLineTo(currentPoint);
            d.PathStroke(colorForLineUsed, false, 2);

            d.PathMoveTo(currentPoint);
            d.PathLineTo(bottomPoint);
            d.PathStroke(colorForLineUnused, false, 2);

            d.PathArcToFast(leftArcCenter, a, 3, 9);
            d.PathLineTo(rightStartPoint);
            d.PathArcToFast(rightArcCenter, a, 9, 12);
            d.PathArcToFast(rightArcCenter, a, 0, 3);
            d.PathClose();

            //label
            var labelRect = new Rect(rect.X, rect.Bottom - labelHeight,
                rect.Width, labelHeight);
            d.DrawText(labelRect, label, style, state);

            s.PushBgColor(new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal);//+1 TODO It's stupid to sprcifiy style like this. There should be a better way to do this.
            s.PushBgColor(new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover);//+1
            s.PushBgColor(new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active);//+1
            var fillColor = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.PathFill(fillColor);
            s.PopStyle(3);
        }
    }

}
