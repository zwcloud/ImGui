using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.Input;

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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            int id = window.GetID(label);

            // rect
            rect = window.GetRect(rect);

            // interact
            bool hovered;
            value = GUIBehavior.ToggleBehavior(rect, id, value, out hovered);

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
            GUIAppearance.DrawToggle(rect, label, value, state);

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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            int id = window.GetID(label);

            // rect
            var style = GUIStyle.Basic;
            var textSize = style.CalcSize(label, GUIState.Normal);
            var size = new Size(16 + textSize.Width, 16 > textSize.Height ? 16 : textSize.Height);
            var rect = window.GetRect(id);

            // interact
            bool hovered;
            value = GUIBehavior.ToggleBehavior(rect, id, value, out hovered);

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
            GUIAppearance.DrawToggle(rect, label, value, state);

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
        public static void DrawToggle(Rect rect, string label, bool value, GUIState state)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;
            GUIStyle style = GUIStyle.Basic;
            DrawList d = window.DrawList;

            var spacing = GUISkin.Instance.InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            var boxRect = new Rect(rect.X, rect.Y + MathEx.ClampTo0(rect.Height - 16) / 2, 16, 16);
            var textRect = new Rect(rect.X + 16 + spacing, rect.Y, MathEx.ClampTo0(rect.Width - 16 - spacing),
                rect.Height);

            // box
            var filledBoxColor = Color.Rgb(0, 151, 167);
            var boxBorderColor = Color.White;
            var tickColor = Color.Rgb(48, 48, 48);
            d.AddRectFilled(boxRect.TopLeft, boxRect.BottomRight, filledBoxColor); //□
            d.AddRect(boxRect.TopLeft, boxRect.BottomRight, boxBorderColor, 0, 0, 2); //□
            if (value) //√
            {
                var h = boxRect.Height;
                d.PathMoveTo(new Point(0.125f * h + boxRect.X, 0.50f * h + boxRect.Y));
                d.PathLineTo(new Point(0.333f * h + boxRect.X, 0.75f * h + boxRect.Y));
                d.PathLineTo(new Point(0.875f * h + boxRect.X, 0.25f * h + boxRect.Y));
                d.PathStroke(tickColor, false, 2);
            }
            // label
            d.DrawBoxModel(textRect, label, style, state);
        }
    }
}

#region TODO
// toggle with label on the right (maybe this is the right choice)
// toggle without label
// tristate
#endregion
