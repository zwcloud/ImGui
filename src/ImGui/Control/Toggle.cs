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
            return DoToggle(rect, label, value);
        }

        internal static bool DoToggle(Rect rect, string label, bool value)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            DrawList d = window.DrawList;
            int id = window.GetID(label);

            var mousePos = Input.Mouse.MousePos;
            var hovered = rect.Contains(mousePos);
            var result = value;
            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Input.Mouse.LeftButtonPressed)
                {
                    uiState.SetActiveID(id);
                }

                if (uiState.ActiveId == id && Input.Mouse.LeftButtonReleased)
                {
                    result = !value;
                    uiState.SetActiveID(GUIContext.None);
                }
            }

            // ui representation
            // nothing to do

            // ui painting
            // |←16→|
            // |    |---------------+
            // |    |               |
            // +----+               |
            // | √  | label         |
            // +----+               |
            //      |               |
            //      +---------------+
            {
                var spacing = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
                var boxRect = new Rect(rect.X, rect.Y + MathEx.ClampTo0(rect.Height - 16) / 2, 16, 16);
                var textRect = new Rect(rect.X + 16 + spacing, rect.Y, MathEx.ClampTo0(rect.Width - 16 - spacing), rect.Height);

                // box
                var filledBoxColor = Color.Rgb(0, 151, 167);
                var boxBorderColor = Color.White;
                var tickColor = Color.Rgb(48, 48, 48);
                d.AddRectFilled(boxRect.TopLeft, boxRect.BottomRight, filledBoxColor);//□
                d.AddRect(boxRect.TopLeft, boxRect.BottomRight, boxBorderColor, 0, 0, 2);//□
                if (result)//√
                {
                    var h = boxRect.Height;
                    d.PathMoveTo(new Point(0.125f * h + boxRect.X, 0.50f * h + boxRect.Y));
                    d.PathLineTo(new Point(0.333f * h + boxRect.X, 0.75f * h + boxRect.Y));
                    d.PathLineTo(new Point(0.875f * h + boxRect.X, 0.25f * h + boxRect.Y));
                    d.PathStroke(tickColor, false, 2);
                }
                // label
                d.DrawBoxModel(textRect, label, GUISkin.Instance[GUIControlName.Label]);
            }

            return result;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout toggle (check-box) with an label.
        /// </summary>
        /// <param name="text">text to display on the label</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(string text, bool value, params LayoutOption[] options)
        {
            return DoToggle(text, value, GUISkin.Instance[GUIControlName.Toggle], options);
        }

        private static bool Toggle(string text, bool value, GUIStyle style, params LayoutOption[] options)
        {
            return DoToggle(text, value, style, options);
        }

        private static bool DoToggle(string text, bool value, GUIStyle style, params LayoutOption[] options)
        {
            var result = GUI.Toggle(GUILayout.GetToggleRect(text, style, options), text, value);
            return result;
        }

        private static Rect GetToggleRect(string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            var textSize = style.CalcSize(text, GUIState.Normal, null);
            var size = new Size(16 + textSize.Width, 16 > textSize.Height ? 16 : textSize.Height);
            return window.GetRect(id, size, style, options);
        }
    }

}
