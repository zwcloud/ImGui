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

            var result = DoHorizontalSlider(rect, label, value, minValue, maxValue);

            return result;
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

            var result = DoVerticalSlider(rect, label, value, minValue, maxValue);

            return result;
        }

        internal static double DoHorizontalSlider(Rect rect, string label, double value, double minValue, double maxValue)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            int id = window.GetID(label);

            var mousePos = Mouse.Instance.Position;
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
            var hovered = sliderRect.Contains(mousePos);

            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)//start track
                {
                    uiState.SetActiveID(id);
                }
            }
            if (uiState.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    var leftPoint = new Point(sliderRect.X + 10, sliderRect.Y + sliderRect.Height / 2);
                    var rightPoint = new Point(sliderRect.Right - 10, sliderRect.Y + sliderRect.Height / 2);
                    var minX = leftPoint.X;
                    var maxX = rightPoint.X;
                    var currentPointX = MathEx.Clamp(mousePos.X, minX, maxX);
                    value = minValue + (currentPointX - minX) / (maxX - minX) * (maxValue - minValue);
                }
                else//end track
                {
                    uiState.SetActiveID(GUIContext.None);
                }
            }

            // ui representation
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
            }
            if (uiState.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUI.Active;
            }

            // ui painting
            var s = g.StyleStack;
            var sliderModifiers = GUISkin.Instance[GUIControlName.Slider];
            s.PushRange(sliderModifiers);
            var style = s.Style;
            {
                DrawList d = window.DrawList;
                var colorForLineUsed = style.Get<Color>(GUIStyleName.Slider_LineUsed, state);
                var colorForLineUnused = style.Get<Color>(GUIStyleName.Slider_LineUnused, state);
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

                var buttonModifiers = GUISkin.Instance[GUIControlName.Button];
                s.PushRange(buttonModifiers);//TODO selectively push one style modifier of a control. For example, only the bgcolor modifier is needed here.

                var fillColor = style.Get<Color>(GUIStyleName.BackgroundColor, state);
                d.PathFill(fillColor);

                s.PopStyle(buttonModifiers.Length);
            }
            s.PopStyle(sliderModifiers.Length);

            return value;
        }

        internal static double DoVerticalSlider(Rect rect, string label, double value, double minValue, double maxValue)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            int id = window.GetID(label);

            var mousePos = Mouse.Instance.Position;
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
            var hovered = rect.Contains(mousePos);

            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)//start track
                {
                    uiState.SetActiveID(id);
                }
            }
            if (uiState.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    var upPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Y + 10);
                    var bottomPoint = new Point(sliderRect.X + sliderRect.Width / 2, sliderRect.Bottom - 10);
                    var minY = upPoint.Y;
                    var maxY = bottomPoint.Y;
                    var currentPointY = MathEx.Clamp(mousePos.Y, minY, maxY);
                    value = (float) (minValue + (currentPointY - minY) / (maxY - minY) * (maxValue - minValue));
                }
                else//end track
                {
                    uiState.SetActiveID(GUIContext.None);
                }
            }

            // ui representation
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
            }
            if (uiState.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                state = GUI.Active;
            }

            // ui painting
            var s = g.StyleStack;
            var sliderModifiers = GUISkin.Instance[GUIControlName.Slider];
            s.PushRange(sliderModifiers);
            var style = s.Style;
            {
                DrawList d = window.DrawList;
                var colorForLineUsed = style.Get<Color>(GUIStyleName.Slider_LineUsed, state);
                var colorForLineUnused = style.Get<Color>(GUIStyleName.Slider_LineUnused, state);

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

                var buttonModifiers = GUISkin.Instance[GUIControlName.Button];
                s.PushRange(buttonModifiers);

                var fillColor = style.Get<Color>(GUIStyleName.BackgroundColor, state);
                d.PathFill(fillColor);

                s.PopStyle(buttonModifiers.Length);
            }
            s.PopStyle(sliderModifiers.Length);
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

            var result = DoSlider(label, value, minValue, maxValue, true);

            return result;
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

            var result = DoSlider(label, value, minValue, maxValue, false);

            return result;
        }

        private static double DoSlider(string label, double value, double minValue, double maxValue, bool isHorizontal)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();

            var s = g.StyleStack;
            var sliderModifiers = GUISkin.Instance[GUIControlName.Slider];
            s.PushRange(sliderModifiers);
            var style = s.Style;

            var id = window.GetID(label);
            Size size = style.CalcSize(label, GUIState.Normal);//label size
            s.PushStretchFactor(!isHorizontal, 1);//+1
            if(isHorizontal)//full size
            {
                var minSilderWidth = 200;
                size.Width += minSilderWidth;
                size.Height = 20;
            }
            else
            {
                var minSilderHeight = 200;
                size.Width = 20;
                size.Height += minSilderHeight;
            }
            var rect = window.GetRect(id, size);
            //TODO reconsider reusing here: this is incorrect because sliderModifiers are pushed twice!
            var result = isHorizontal? GUI.DoHorizontalSlider(rect, label, value, minValue, maxValue):
                GUI.DoVerticalSlider(rect, label, value, minValue, maxValue);
            s.PopStyle();//-1

            s.PopStyle(sliderModifiers.Length);

            return result;
        }
    }

    partial class GUISkin
    {
        void InitSliderStyles()
        {
            var sliderStyles = new StyleModifier[]
            {
                new StyleModifier(GUIStyleName.Slider_LineUsed, StyleType.Color, Color.Rgb(0, 151, 167), GUIState.Normal),
                new StyleModifier(GUIStyleName.Slider_LineUsed, StyleType.Color, Color.Rgb(0, 151, 167), GUIState.Hover),
                new StyleModifier(GUIStyleName.Slider_LineUsed, StyleType.Color, Color.Rgb(0, 151, 167), GUIState.Active),
                new StyleModifier(GUIStyleName.Slider_LineUnused, StyleType.Color, Color.Rgb(117, 117, 117), GUIState.Normal),
                new StyleModifier(GUIStyleName.Slider_LineUnused, StyleType.Color, Color.Rgb(255, 128, 171), GUIState.Hover),
                new StyleModifier(GUIStyleName.Slider_LineUnused, StyleType.Color, Color.Rgb(255, 128, 171), GUIState.Active),
            };
            this.styles.Add(GUIControlName.Slider, sliderStyles);
        }
    }

}
