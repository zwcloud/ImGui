using System;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool Button(string text, params LayoutOption[] options)
        {
            return Button(text, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool Button(string text, GUIStyle style, params LayoutOption[] options)
        {
            return DoButton(text, style, options);
        }

        private static bool DoButton(string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(text);
            Size size = style.CalcSize(text, GUIState.Normal, options);
            Rect rect = window.GetRect(id, size, style, options);

            return GUI.Button(rect, text);
        }
    }

    public partial class GUI
    {
        /// <summary>
        /// Create a button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool Button(Rect rect, string text)
        {
            return DoButton(rect, text);
        }

        internal static bool DoButton(Rect rect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            if (window.SkipItems)
                return false;

            DrawList d = window.DrawList;
            int id = window.GetID(text);

            GUIStyle style = GUISkin.Instance[GUIControlName.Button];

            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, 0);

            // Render
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            Color col = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.RenderFrame(rect.Min, rect.Max, col, false, 0);
            d.DrawText(rect, text, style, state);

            return pressed;
        }
    }

    partial class GUIBehavior
    {
        public static bool ButtonBehavior(Rect bb, int id, out bool out_hovered, out bool out_held, ButtonFlags flags)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;


            if (flags.HaveFlag(ButtonFlags.Disabled))
            {
                out_hovered = false;
                out_held = false;
                if (g.ActiveId == id) g.SetActiveID(0);
                return false;
            }

            if (!flags.HaveFlag(ButtonFlags.PressedOnClickRelease | ButtonFlags.PressedOnClick | ButtonFlags.PressedOnRelease | ButtonFlags.PressedOnDoubleClick))
                flags |= ButtonFlags.PressedOnClickRelease;

            bool pressed = false;
            bool hovered = g.IsHovered(bb, id, flags.HaveFlag(ButtonFlags.FlattenChilds));
            if (hovered)
            {
                g.SetHoverID(id);
                if (!flags.HaveFlag(ButtonFlags.NoKeyModifiers) || (!Input.Keyboard.KeyPressed(Key.LeftControl) && !Input.Keyboard.KeyPressed(Key.LeftShift) && !Input.Keyboard.KeyPressed(Key.LeftAlt)))
                {
                    //                        | CLICKING        | HOLDING with ButtonFlags.Repeat
                    // PressedOnClickRelease  |  <on release>*  |  <on repeat> <on repeat> .. (NOT on release)  <-- MOST COMMON! (*) only if both click/release were over bounds
                    // PressedOnClick         |  <on click>     |  <on click> <on repeat> <on repeat> ..
                    // PressedOnRelease       |  <on release>   |  <on repeat> <on repeat> .. (NOT on release)
                    // PressedOnDoubleClick   |  <on dclick>    |  <on dclick> <on repeat> <on repeat> ..
                    if (flags.HaveFlag(ButtonFlags.PressedOnClickRelease) && Input.Mouse.LeftButtonPressed)
                    {
                        g.SetActiveID(id, window); // Hold on ID
                        w.FocusWindow(window);
                        g.ActiveIdClickOffset = Input.Mouse.MousePos - bb.Min;
                    }
                    if (((flags.HaveFlag(ButtonFlags.PressedOnClick) && Input.Mouse.LeftButtonPressed)
                        || (flags.HaveFlag(ButtonFlags.PressedOnDoubleClick) && Input.Mouse.LeftButtonDoubleClicked)))
                    {
                        pressed = true;
                        g.SetActiveID(0);
                        w.FocusWindow(window);
                    }
                    if (flags.HaveFlag(ButtonFlags.PressedOnRelease) && Input.Mouse.LeftButtonReleased)
                    {
                        if (!(flags.HaveFlag(ButtonFlags.Repeat) && Input.Mouse.LeftButtonDownDurationPrev >= Input.KeyRepeatDelay))  // Repeat mode trumps <on release>
                            pressed = true;
                        g.SetActiveID(0);
                    }

                    // 'Repeat' mode acts when held regardless of _PressedOn flags (see table above). 
                    // Relies on repeat logic of IsMouseClicked() but we may as well do it ourselves if we end up exposing finer RepeatDelay/RepeatRate settings.
                    if (flags.HaveFlag(ButtonFlags.Repeat) && g.ActiveId == id && Input.Mouse.LeftButtonDownDuration > 0.0f && g.IsMouseLeftButtonClicked(true))
                        pressed = true;
                }
            }

            bool held = false;
            if (g.ActiveId == id)
            {
                if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    held = true;
                }
                else
                {
                    if (hovered && flags.HaveFlag(ButtonFlags.PressedOnClickRelease))
                        if (!(flags.HaveFlag(ButtonFlags.Repeat) && Input.Mouse.LeftButtonDownDurationPrev >= Input.KeyRepeatDelay))  // Repeat mode trumps <on release>
                            pressed = true;
                    g.SetActiveID(0);
                }
            }

            // AllowOverlap mode (rarely used) requires previous frame HoveredId to be null or to match. This allows using patterns where a later submitted widget overlaps a previous one.
            if (hovered && flags.HaveFlag(ButtonFlags.AllowOverlapMode) && (g.HoveredIdPreviousFrame != id && g.HoveredIdPreviousFrame != 0))
                hovered = pressed = held = false;

            out_hovered = hovered;
            out_held = held;

            return pressed;
        }
    }

    [Flags]
    public enum ButtonFlags
    {
        Repeat = 1 << 0,   // hold to repeat
        PressedOnClickRelease = 1 << 1,   // (default) return pressed on click+release on same item (default if no PressedOn** flag is set)
        PressedOnClick = 1 << 2,   // return pressed on click (default requires click+release)
        PressedOnRelease = 1 << 3,   // return pressed on release (default requires click+release)
        PressedOnDoubleClick = 1 << 4,   // return pressed on double-click (default requires click+release)
        FlattenChilds = 1 << 5,   // allow interaction even if a child window is overlapping
        DontClosePopups = 1 << 6,   // disable automatically closing parent popup on press
        Disabled = 1 << 7,   // disable interaction
        AlignTextBaseLine = 1 << 8,   // vertically align button to match text baseline - ButtonEx() only
        NoKeyModifiers = 1 << 9,   // disable interaction if a key modifier is held
        AllowOverlapMode = 1 << 10   // require previous frame HoveredId to either match id or be null before being usable
    };
}