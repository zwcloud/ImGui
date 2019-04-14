using ImGui.Input;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a button that acts like a toggle.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="selected">whether this selectable is selected</param>
        /// <returns>new value of the toggle-button</returns>
        public static bool Selectable(Rect rect, string text, bool selected)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            int id = window.GetID(text);

            // style
            var style = GUIStyle.Basic;
            style.ApplySkin(GUIControlName.Selectable);

            // rect
            Size size = style.CalcSize(text, GUIState.Normal);
            rect = window.GetRect(rect);

            // interact
            selected = GUIBehavior.SelectableBehavior(rect, id, selected, out bool hovered, out bool held);

            // render
            DrawList d = window.DrawList;
            var state = (selected || (hovered && held)) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            d.DrawBoxModel(rect, text, style, state);

            style.Restore();

            return selected;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button that acts like a toggle.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="selected">whether this selectable is selected</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle-button</returns>
        public static bool Selectable(string text, bool selected, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            int id = window.GetID(text);

            // style
            var style = GUIStyle.Basic;
            style.ApplySkin(GUIControlName.Selectable);
            style.ApplyOption(options);

            // rect
            Size size = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id);

            // interact
            selected = GUIBehavior.SelectableBehavior(rect, id, selected, out bool hovered, out bool held);

            // render
            DrawList d = window.DrawList;
            var state = (selected || (hovered && held)) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            d.DrawBoxModel(rect, text, style, state);

            style.Restore();

            return selected;
        }

        public static bool Selectable(string text, bool selected)
        {
            return Selectable(text, selected, null);
        }
    }

    internal partial class GUIBehavior
    {
        public static bool SelectableBehavior(Rect rect, int id, bool selected, out bool out_hovered, out bool out_held)
        {
            GUIContext g = Form.current.uiContext;

            bool hovered = g.IsHovered(rect, id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)
                {
                    g.SetActiveID(id);
                }

                if (g.ActiveId == id && Mouse.Instance.LeftButtonReleased)
                {
                    selected = !selected;
                    g.SetActiveID(0);
                }
            }
            
            bool held = false;
            if (g.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    held = true;
                }
            }

            out_hovered = hovered;
            out_held = held;

            return selected;
        }
    }

    internal partial class GUISkin
    {
        private void InitSelectableStyles()
        {
            StyleModifierBuilder builder = new StyleModifierBuilder();
            builder.PushPadding(2.0);
            builder.PushBgColor(Color.Clear, GUIState.Normal);
            builder.PushBgColor(Color.Rgb(206, 220, 236), GUIState.Hover);
            builder.PushBgColor(Color.Rgb(30, 144, 255), GUIState.Active);

            this.styles.Add(GUIControlName.Selectable, builder.ToArray());
        }
    }
}
