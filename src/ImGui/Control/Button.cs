using System;
using ImGui.Common.Primitive;
using ImGui.Input;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Rendering;

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
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            int id = window.GetID(text);
            var t = window.RenderTree.CurrentContainer;
            Node node = t.GetNodeById(id);
            Node backgroundNode = null, textNode = null;
            if (node == null)
            {
                //create button node
                node = new Node(id, $"Button<{text}>");
                t.AppendChild(node);
                window.IDStack.Push(id);

                //background
                var backgroundNodeId = window.GetID("Background");
                backgroundNode = new Node(backgroundNodeId, "Background");
                backgroundNode.Primitive = new PathPrimitive();
                //TODO implement button rendering with box-model

                //text
                var textNodeId = window.GetID("Text");
                textNode = new Node(textNodeId, "Text");
                var textPrimitive = new TextPrimitive();
                textPrimitive.Text = text;
                textNode.Primitive = textPrimitive;

                node.AppendChild(backgroundNode);
                node.AppendChild(textNode);
                window.IDStack.Pop();
            }
            else
            {
                backgroundNode = node.GetNodeByName("Background");
                textNode = node.GetNodeByName("Text");
            }

            Debug.Assert(backgroundNode != null);
            Debug.Assert(textNode != null);

            // rect
            backgroundNode.Rect = window.GetRect(rect);
            textNode.Rect = backgroundNode.Rect;

            var primitive = backgroundNode.Primitive as PathPrimitive;
            Debug.Assert(primitive != null, nameof(primitive) + " != null");
            primitive.PathClear();
            primitive.PathRect(backgroundNode.Rect);

            // style apply
            var style = GUIStyle.Basic;
            style.Save();
            style.ApplySkin(GUIControlName.Button);

            // interact
            bool pressed = GUIBehavior.ButtonBehavior(backgroundNode.Rect, node.Id, out var hovered, out var held, 0);
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;

            var fillColor = style.Get<Color>(GUIStyleName.BackgroundColor, state);

            var lineColor = style.GetBorderColor(state);

            style.Restore();

            return pressed;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="options"></param>
        public static bool Button(string text, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            // style apply
            var style = GUIStyle.Basic;
            style.Save();
            style.ApplySkin(GUIControlName.Button);
            style.ApplyOption(options);

            //get or create the root node
            int id = window.GetID(text);
            var t = window.RenderTree.CurrentContainer;
            Node node = t.GetNodeById(id);
            Node backgroundNode = null, textNode = null;
            if (node == null)
            {
                //create button node
                node = new Node(id, $"Button<{text}>");
                var contentSize = style.CalcSize(text, GUIState.Normal);//TEMP: GUI state should be de-coupled from GUI style
                node.AttachLayoutEntry(contentSize, options);//TODO *************** properly assign layout entry and group to every node that needs layout!!!
                t.AppendChild(node);
                window.IDStack.Push(id);

                //background
                var backgroundNodeId = window.GetID("Background");
                backgroundNode = new Node(backgroundNodeId, "Background");
                backgroundNode.Primitive = new PathPrimitive();
                //TODO implement button rendering with box-model

                //text
                var textNodeId = window.GetID("Text");
                textNode = new Node(textNodeId, "Text");
                var textPrimitive = new TextPrimitive();
                textPrimitive.Text = text;
                textNode.Primitive = textPrimitive;

                node.AppendChild(backgroundNode);
                node.AppendChild(textNode);
                window.IDStack.Pop();
            }
            else
            {
                backgroundNode = node.GetNodeByName("Background");
                textNode = node.GetNodeByName("Text");
            }

            Debug.Assert(backgroundNode != null);
            Debug.Assert(textNode != null);


            // rect
            Rect rect = window.GetRect(id);//************* rect got is incorrect
            backgroundNode.Rect = rect;
            textNode.Rect = backgroundNode.Rect;

            var primitive = backgroundNode.Primitive as PathPrimitive;
            Debug.Assert(primitive != null, nameof(primitive) + " != null");
            primitive.PathClear();
            primitive.PathRect(backgroundNode.Rect);


            // interact
            bool pressed = GUIBehavior.ButtonBehavior(backgroundNode.Rect, node.Id, out var hovered, out var held, 0);
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;

            var fillColor = style.Get<Color>(GUIStyleName.BackgroundColor, state);

            var lineColor = style.GetBorderColor(state);

            //style restore
            style.Restore();

            return pressed;
        }

        public static bool Button(string text)
        {
            return Button(text, null);
        }

        public static bool ImageButton(string filePath, Size size, Point uv0, Point uv1)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            var id = window.GetID(filePath);

            // style
            var style = GUIStyle.Basic;
            style.Save();
            style.ApplySkin(GUIControlName.Button);

            // rect
            var texture = TextureUtil.GetTexture(filePath);
            if(size == Size.Empty)
            {
                size = style.CalcSize(texture, GUIState.Normal);
            }
            var rect = window.GetRect(id);
            if(rect == Layout.StackLayout.DummyRect)
            {
                style.Restore();
                return false;
            }

            // interact
            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, 0);

            // render
            style.PushUV(uv0, uv1);
            var d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            d.DrawBoxModel(rect, texture, style, state);

            style.Restore();

            return pressed;
        }

        public static bool ImageButton(string filePath)
        {
            return ImageButton(filePath, Size.Empty, Point.Zero, Point.One);
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

    internal partial class GUISkin
    {
        private void InitButtonStyles()
        {
            StyleModifierBuilder builder = new StyleModifierBuilder();
            builder.PushBorder(1.0);
            builder.PushPadding(5.0);
            builder.PushBorderColor(Color.Rgb(166, 166, 166), GUIState.Normal);
            builder.PushBorderColor(Color.Rgb(123, 123, 123), GUIState.Hover);
            builder.PushBorderColor(Color.Rgb(148, 148, 148), GUIState.Active);

            builder.PushBgColor(Color.Rgb(0x65a9d7), GUIState.Normal);
            builder.PushBgColor(Color.Rgb(0x28597a), GUIState.Hover);
            builder.PushBgColor(Color.Rgb(0x1b435e), GUIState.Active);

            //TODO use gradient color for background
            /*
            builder.PushBgGradient(Gradient.TopBottom);
            builder.PushGradientColor(Color.Rgb(247, 247, 247), Color.Rgb(221, 221, 221), GUIState.Normal);
            builder.PushGradientColor(Color.Rgb(247, 247, 247), Color.Rgb(221, 221, 221), GUIState.Hover);
            builder.PushGradientColor(Color.Rgb(222, 222, 222), Color.Rgb(248, 248, 248), GUIState.Active);
            */
            this.styles.Add(GUIControlName.Button, builder.ToArray());
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