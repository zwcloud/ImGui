using System;
using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
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
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            int id = window.GetID(text);

            // style apply
            var s = g.StyleStack;
            var style = g.StyleStack.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Button];
            s.PushRange(modifiers);

            // rect
            rect = window.GetRect(rect);

            // interact
            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, 0);

            // render
            var d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            d.DrawBoxModel(rect, text, style, state);

            // style restore
            s.PopStyle(modifiers.Length);

            return pressed;
        }
    }

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
            if (window.SkipItems)
                return false;

            int id = window.GetID(text);

            // style apply
            var s = g.StyleStack;
            var style = g.StyleStack.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Button];
            s.PushRange(modifiers);

            // rect
            Rect rect;
            Size size = style.CalcSize(text, GUIState.Normal);
            rect = window.GetRect(id, size);

            // interact
            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, 0);

            // render
            var d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            d.DrawBoxModel(rect, text, style, state);

            // style restore
            s.PopStyle(modifiers.Length);

            return pressed;
        }

    }

    internal partial class GUIBehavior
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

    partial class GUISkin
    {
        void InitButtonStyles()
        {
            var borderColor = Color.Black;
            double border = 1.0;
            double padding = 5.0;
            var buttonStyles = new []
            {
                //normal
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, border),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, border),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, border),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, border),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, padding),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, padding),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, padding),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, padding),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.67f, 0.40f, 0.40f, 0.60f)),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Normal),

                //hover
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, border, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, border, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderBottom,StyleType.@double,  border, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, border, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, padding, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, padding, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, padding, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, padding, GUIState.Hover),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Normal, GUIState.Hover),

                //active
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, border, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, border, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, border, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, border, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, padding, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, padding, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, padding, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, padding, GUIState.Active),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Bold, GUIState.Active),
            };
            this.styles.Add(GUIControlName.Button, buttonStyles);
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