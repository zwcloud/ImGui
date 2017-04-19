using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class Button
    {
        public static bool DoControl(Rect rect, Content content, string str_id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;
            int id = window.GetID(str_id);

            var mousePos = Input.Mouse.MousePos;

            if (Utility.CurrentOS.IsAndroid)
            {
                var clicked = false;
                var inside = rect.Contains(mousePos);

                //control logic
                g.KeepAliveId(id);
                if (inside && Input.Mouse.LeftButtonPressed)//start track
                {
                    g.SetActiveId(id);
                }

                if (g.ActiveId == id && Input.Mouse.LeftButtonReleased)//end track
                {
                    clicked = true;
                    g.SetActiveId(0);
                }

                // ui representation
                var state = GUI.Normal;
                if (g.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUI.Active;
                }

                // ui painting
                d.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
                return clicked;
            }
            else
            {
                var clicked = false;
                var hovered = rect.Contains(mousePos);

                //control logic
                g.KeepAliveId(id);
                if (hovered)
                {
                    g.SetHoverId(id);

                    if (Input.Mouse.LeftButtonPressed)//start track
                    {
                        g.SetActiveId(id);
                    }

                    if (Input.Mouse.LeftButtonReleased)//end track
                    {
                        clicked = true;
                        g.SetActiveId(0);
                    }
                }

                // ui representation
                var state = GUI.Normal;
                if (hovered)
                {
                    state = GUI.Hover;
                    if (g.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                    {
                        state = GUI.Active;
                    }
                }

                d.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
                return clicked;
            }
        }

        public static bool ButtonBehavior(Rect bb, int id, out bool out_hovered, out bool out_held, ButtonFlags flags)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.CurrentWindow;

            if (flags.HasFlag(ButtonFlags.Disabled))
            {
                out_hovered = false;
                out_held = false;
                if (g.ActiveId == id) g.SetActiveID(0);
                return false;
            }

            if (!flags.HasFlag(ButtonFlags.PressedOnClickRelease | ButtonFlags.PressedOnClick | ButtonFlags.PressedOnRelease | ButtonFlags.PressedOnDoubleClick))
                flags |= ButtonFlags.PressedOnClickRelease;

            bool pressed = false;
            bool hovered = g.IsHovered(bb, id, flags.HasFlag(ButtonFlags.FlattenChilds));
            if (hovered)
            {
                g.SetHoverId(id);
                if (!flags.HasFlag(ButtonFlags.NoKeyModifiers) || (!Input.Keyboard.KeyPressed(Key.LeftControl) && !Input.Keyboard.KeyPressed(Key.LeftShift) && !Input.Keyboard.KeyPressed(Key.LeftAlt)))
                {
                    //                        | CLICKING        | HOLDING with ButtonFlags.Repeat
                    // PressedOnClickRelease  |  <on release>*  |  <on repeat> <on repeat> .. (NOT on release)  <-- MOST COMMON! (*) only if both click/release were over bounds
                    // PressedOnClick         |  <on click>     |  <on click> <on repeat> <on repeat> ..
                    // PressedOnRelease       |  <on release>   |  <on repeat> <on repeat> .. (NOT on release)
                    // PressedOnDoubleClick   |  <on dclick>    |  <on dclick> <on repeat> <on repeat> ..
                    if (flags.HasFlag(ButtonFlags.PressedOnClickRelease) && Input.Mouse.LeftButtonPressed)
                    {
                        g.SetActiveID(id, window); // Hold on ID
                        g.FocusWindow(window);
                        g.ActiveIdClickOffset = Input.Mouse.MousePos - bb.Min;
                    }
                    if (((flags.HasFlag(ButtonFlags.PressedOnClick) && Input.Mouse.LeftButtonPressed)
                        || (flags.HasFlag(ButtonFlags.PressedOnDoubleClick) && Input.Mouse.LeftButtonDoubleClicked)))
                    {
                        pressed = true;
                        g.SetActiveID(0);
                        g.FocusWindow(window);
                    }
                    if (flags.HasFlag(ButtonFlags.PressedOnRelease) && Input.Mouse.LeftButtonReleased)
                    {
                        if (!(flags.HasFlag(ButtonFlags.Repeat) && Input.Mouse.LeftButtonDownDurationPrev >= Input.KeyRepeatDelay))  // Repeat mode trumps <on release>
                            pressed = true;
                        g.SetActiveID(0);
                    }

                    // 'Repeat' mode acts when held regardless of _PressedOn flags (see table above). 
                    // Relies on repeat logic of IsMouseClicked() but we may as well do it ourselves if we end up exposing finer RepeatDelay/RepeatRate settings.
                    if (flags.HasFlag(ButtonFlags.Repeat) && g.ActiveId == id && Input.Mouse.LeftButtonDownDuration > 0.0f && g.IsMouseLeftButtonClicked(true))
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
                    if (hovered && flags.HasFlag(ButtonFlags.PressedOnClickRelease))
                        if (!(flags.HasFlag(ButtonFlags.Repeat) && Input.Mouse.LeftButtonDownDurationPrev >= Input.KeyRepeatDelay))  // Repeat mode trumps <on release>
                            pressed = true;
                    g.SetActiveID(0);
                }
            }

            // AllowOverlap mode (rarely used) requires previous frame HoveredId to be null or to match. This allows using patterns where a later submitted widget overlaps a previous one.
            if (hovered && flags.HasFlag(ButtonFlags.AllowOverlapMode) && (g.HoveredIdPreviousFrame != id && g.HoveredIdPreviousFrame != 0))
                hovered = pressed = held = false;

            out_hovered = hovered;
            out_held = held;

            return pressed;
        }
    }

    [Flags]
    enum ButtonFlags
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