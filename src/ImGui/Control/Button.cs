using System;
using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        internal static bool Button(string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();

            //apply skin and stack style modifiers
            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Button];
            s.Apply(modifiers);

            int id = window.GetID(text);
            var style = g.StyleStack.Style;
            Size size = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id, size);

            var result = GUI.DoButton(rect, text);

            s.Restore(modifiers);

            return result;
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
            GUIContext g = GetCurrentContext();
            //apply skin and stack style modifiers
            var s = g.StyleStack;
            s.Apply(GUISkin.Instance[GUIControlName.Button]);

            var result = DoButton(rect, text);

            s.Restore();

            return result;
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

            GUIStyle style = g.StyleStack.Style;

            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, 0);

            // Render
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            d.DrawBoxModel(rect, text, style, state);

            return pressed;
        }
    }

    partial class GUIBehavior
    {
        public static bool ButtonBehavior(Rect bb, int id, out bool out_hovered, out bool out_held, ButtonFlags flags = 0)
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
                if (!flags.HaveFlag(ButtonFlags.NoKeyModifiers) || (!Keyboard.Instance.KeyPressed(Key.LeftControl) && !Keyboard.Instance.KeyPressed(Key.LeftShift) && !Keyboard.Instance.KeyPressed(Key.LeftAlt)))
                {
                    //                        | CLICKING        | HOLDING with ButtonFlags.Repeat
                    // PressedOnClickRelease  |  <on release>*  |  <on repeat> <on repeat> .. (NOT on release)  <-- MOST COMMON! (*) only if both click/release were over bounds
                    // PressedOnClick         |  <on click>     |  <on click> <on repeat> <on repeat> ..
                    // PressedOnRelease       |  <on release>   |  <on repeat> <on repeat> .. (NOT on release)
                    // PressedOnDoubleClick   |  <on dclick>    |  <on dclick> <on repeat> <on repeat> ..
                    if (flags.HaveFlag(ButtonFlags.PressedOnClickRelease) && Mouse.Instance.LeftButtonPressed)
                    {
                        g.SetActiveID(id, window); // Hold on ID
                        w.FocusWindow(window);
                        g.ActiveIdClickOffset = Mouse.Instance.Position - bb.Min;
                    }
                    if (((flags.HaveFlag(ButtonFlags.PressedOnClick) && Mouse.Instance.LeftButtonPressed)
                        || (flags.HaveFlag(ButtonFlags.PressedOnDoubleClick) && Mouse.Instance.LeftButtonDoubleClicked)))
                    {
                        pressed = true;
                        g.SetActiveID(0);
                        w.FocusWindow(window);
                    }
                    if (flags.HaveFlag(ButtonFlags.PressedOnRelease) && Mouse.Instance.LeftButtonReleased)
                    {
                        if (!(flags.HaveFlag(ButtonFlags.Repeat) && Mouse.Instance.LeftButtonDownDurationPrev >= Keyboard.KeyRepeatDelay))  // Repeat mode trumps <on release>
                            pressed = true;
                        g.SetActiveID(0);
                    }

                    // 'Repeat' mode acts when held regardless of _PressedOn flags (see table above). 
                    // Relies on repeat logic of IsMouseClicked() but we may as well do it ourselves if we end up exposing finer RepeatDelay/RepeatRate settings.
                    if (flags.HaveFlag(ButtonFlags.Repeat) && g.ActiveId == id && Mouse.Instance.LeftButtonDownDuration > 0.0f && g.IsMouseLeftButtonClicked(true))
                        pressed = true;
                }
            }

            bool held = false;
            if (g.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    held = true;
                }
                else
                {
                    if (hovered && flags.HaveFlag(ButtonFlags.PressedOnClickRelease))
                        if (!(flags.HaveFlag(ButtonFlags.Repeat) && Mouse.Instance.LeftButtonDownDurationPrev >= Keyboard.KeyRepeatDelay))  // Repeat mode trumps <on release>
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
    }

    internal static class ButtonFlagsExtension
    {
        public static bool HaveFlag(this ButtonFlags value, ButtonFlags flag)
        {
            return (value & flag) != 0;
        }
    }
}