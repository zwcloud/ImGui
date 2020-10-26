using System;
using ImGui.Input;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button, optionally incuding the id: "#MyButton"</param>
        /// <param name="options">style options</param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool Button(Rect rect, string text, LayoutOptions? options)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create button node
                node = new Node(id, $"Button<{text}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
            }
            node.RuleSet.ApplyStack();
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(text, node.RuleSet, node.Rect);
            }

            return pressed;
        }

        public static bool Button(Rect rect, string text) => Button(rect, text, null);
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">style options</param>
        public static bool Button(string text, LayoutOptions? options)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create node
                node = new Node(id, $"Button<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(text, node.RuleSet, node.Rect);
            }

            return pressed;
        }

        public static bool Button(string text) => Button(text, null);
        public static bool ImageButton(string filePath, Size size, Vector offset)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(filePath);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"Button<{filePath}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
                node.RuleSet.ObjectPosition = (offset.X, offset.Y);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                var texture = TextureUtil.GetTexture(filePath);
                dc.DrawBoxModel(texture, node.RuleSet, node.Rect);
            }

            return pressed;
        }

        public static bool ImageButton(string filePath)
        {
            var texture = TextureUtil.GetTexture(filePath);
            return ImageButton(filePath, texture.Size, Vector.Zero);
        }

        public static bool ImageButton(string filePath, Size size, (double top, double left, double right, double bottom) borderSlice)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(filePath);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"Button<{filePath}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.RuleSet.BorderImageSource = filePath;
            node.RuleSet.BorderImageSlice = borderSlice;
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(node.RuleSet, node.Rect);
            }

            return pressed;
        }

    }

    internal partial class GUIBehavior
    {
        public static bool ButtonBehavior(Rect bb, int id, out bool outHovered, out bool outHeld, ButtonFlags flags = 0)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            if (flags.HaveFlag(ButtonFlags.Disabled))
            {
                outHovered = false;
                outHeld = false;
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

            outHovered = hovered;
            outHeld = held;

            return pressed;
        }
    }

    internal partial class GUISkin
    {
        private void InitButtonStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Border(1.0, GUIState.Normal)
                .Border(1.0, GUIState.Hover)
                .Border(1.0, GUIState.Active)
                .Padding(5.0, GUIState.Normal)
                .Padding(5.0, GUIState.Hover)
                .Padding(5.0, GUIState.Active)
                .BorderColor(new Color(0.26f, 0.59f, 0.98f, 0.40f), GUIState.Normal)
                .BorderColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), GUIState.Hover)
                .BorderColor(new Color(0.06f, 0.53f, 0.98f, 1.00f), GUIState.Active)
                .BackgroundColor(Color.Rgb(0x65a9d7), GUIState.Normal)
                .BackgroundColor(Color.Rgb(0x28597a), GUIState.Hover)
                .BackgroundColor(Color.Rgb(0x1b435e), GUIState.Active)
                .BackgroundGradient(Gradient.TopBottom)
                .FontColor(Color.Black)
                .AlignmentVertical(Alignment.Center, GUIState.Normal)
                .AlignmentVertical(Alignment.Center, GUIState.Hover)
                .AlignmentVertical(Alignment.Center, GUIState.Active)
                .AlignmentHorizontal(Alignment.Center, GUIState.Normal)
                .AlignmentHorizontal(Alignment.Center, GUIState.Hover)
                .AlignmentHorizontal(Alignment.Center, GUIState.Active)
                .GradientTopDownColor(Color.Rgb(247, 247, 247), Color.Rgb(221, 221, 221), GUIState.Normal)
                .GradientTopDownColor(Color.Rgb(247, 247, 247), Color.Rgb(221, 221, 221), GUIState.Hover)
                .GradientTopDownColor(Color.Rgb(222, 222, 222), Color.Rgb(248, 248, 248), GUIState.Active);
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