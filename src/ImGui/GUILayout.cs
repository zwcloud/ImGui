using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with automatic layout.
    /// </summary>
    public class GUILayout
    {
        #region container

        #region basic

        /// <summary>
        /// Begin a horizontal layout group.
        /// </summary>
        /// <param name="options"></param>
        public static void BeginHorizontal(params LayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Content.None, GUIStyle.Default, options);
        }

        internal static void BeginHorizontal(GUIStyle style, params LayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Content.None, style, options);
        }

        internal static void BeginHorizontal(Content content, GUIStyle style, params LayoutOption[] options)
        {
            LayoutGroup layoutGroup = LayoutUtility.BeginLayoutGroup(isVertical: false, style: style, options: options);
            if (style != GUIStyle.Default || content != Content.None)
            {
                GUI.Box(layoutGroup.rect, content, "box_" + layoutGroup.GetHashCode());
            }
        }

        /// <summary>
        /// End a horizontal layout group.
        /// </summary>
        public static void EndHorizontal()
        {
            LayoutUtility.EndLayoutGroup();
        }

        /// <summary>
        /// Begin a vertical layout group.
        /// </summary>
        /// <param name="options"></param>
        public static void BeginVertical(params LayoutOption[] options)
        {
            GUILayout.BeginVertical(Content.None, GUIStyle.Default, options);
        }

        public static void BeginVertical(GUIStyle style, params LayoutOption[] options)
        {
            GUILayout.BeginVertical(Content.None, style, options);
        }

        internal static void BeginVertical(Content content, GUIStyle style, params LayoutOption[] options)
        {
            LayoutGroup layoutGroup = LayoutUtility.BeginLayoutGroup(isVertical: true, style: style, options: options);
            if (style != GUIStyle.Default)
            {
                GUI.Box(layoutGroup.rect, content, "box_" + layoutGroup.GetHashCode());
            }
        }

        /// <summary>
        /// End a vertical layout group.
        /// </summary>
        public static void EndVertical()
        {
            LayoutUtility.EndLayoutGroup();
        }

        #endregion

        public static bool CollapsingHeader(string text, string id, bool open)
        {
            return ToggleButton((open ? "▼" : "▶") + text, open);
        }

        public static bool Begin(string name, ref bool open, Point position, Size size, double bg_alpha, WindowFlags flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Debug.Assert(name != null);                        // Window name required
            Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // Called ImGui::Render() or ImGui::EndFrame() and haven't called ImGui::NewFrame() again yet

            if (flags.HaveFlag(WindowFlags.NoInputs))
            {
                flags |= WindowFlags.NoMove | WindowFlags.NoResize;
            }

            // Find or create
            bool window_is_new = false;
            Window window = g.FindWindowByName(name);
            if (window == null)
            {
                window = new Window(name, position, size, flags);
                window_is_new = true;
            }

            long current_frame = g.FrameCount;
            bool first_begin_of_the_frame = (window.LastActiveFrame != current_frame);
            if (first_begin_of_the_frame)
            {
                window.Flags = flags;
            }
            else
            {
                flags = window.Flags;
            }

            // Add to stack
            Window parent_window = (!(g.CurrentWindowStack.Count==0)) ? g.CurrentWindowStack[g.CurrentWindowStack.Count-1] : null;
            g.CurrentWindowStack.Add(window);
            g.CurrentWindow = window;
            //CheckStacksSize(window, true);
            Debug.Assert(parent_window != null || !(flags.HaveFlag(WindowFlags.ChildWindow)));

            bool window_was_active = (window.LastActiveFrame == current_frame - 1);

            bool window_appearing_after_being_hidden = (window.HiddenFrames == 1);

            // Update known root window (if we are a child window, otherwise window == window->RootWindow)
            int root_idx, root_non_popup_idx;
            for (root_idx = g.CurrentWindowStack.Count - 1; root_idx > 0; root_idx--)
            {
                if (!(g.CurrentWindowStack[root_idx].Flags.HaveFlag(WindowFlags.ChildWindow)))
                    break;
            }
            for (root_non_popup_idx = root_idx; root_non_popup_idx > 0; root_non_popup_idx--)
            {
                if (!(g.CurrentWindowStack[root_non_popup_idx].Flags.HaveFlag(WindowFlags.ChildWindow | WindowFlags.Popup)))
                    break;
            }
            window.ParentWindow = parent_window;
            window.RootWindow = g.CurrentWindowStack[root_idx];

            // When reusing window again multiple times a frame, just append content (don't need to setup again)
            if (first_begin_of_the_frame)
            {
                window.Active = true;
                window.ClipRect = new Rect(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
                window.LastActiveFrame = current_frame;

                window.DrawList.Clear();
                Rect fullScreenRect = form.Rect;

                // clip
                window.ClipRect = fullScreenRect;

                // Collapse window by double-clicking on title bar
                if (g.HoveredWindow == window && g.IsMouseHoveringRect(window.TitleBarRect) && Input.Mouse.LeftButtonDoubleClicked)
                {
                    window.Collapsed = !window.Collapsed;
                }

                #region size

                // Apply minimum/maximum window size constraints and final size
                window.ApplySize(window.FullSize);
                window.Size = window.Collapsed ? window.TitleBarRect.Size : window.FullSize;

                #endregion

                #region position

                window.Position = new Point((int)window.PosFloat.X, (int)window.PosFloat.Y);

                #endregion

                // Draw window + handle manual resize
                GUIStyle style = window.Style;
                GUIStyle headerStyle = window.HeaderStyle;
                Rect title_bar_rect = window.TitleBarRect;
                float window_rounding = 3;
                if (window.Collapsed)
                {
                    // Draw title bar only
                    window.DrawList.RenderFrame(title_bar_rect.TopLeft, title_bar_rect.BottomRight, new Color(0.40f, 0.40f, 0.80f, 0.20f), true, window_rounding);
                }
                else
                {
                    Color resize_col = Color.Clear;
                    double resize_corner_size = Math.Max(window.Style.FontSize * 1.35, window_rounding + 1.0 + window.Style.FontSize * 0.2);
                    if (!flags.HaveFlag(WindowFlags.AlwaysAutoResize) && !flags.HaveFlag(WindowFlags.NoResize))
                    {
                        // Manual resize
                        var br = window.Rect.BottomRight;
                        Rect resize_rect = new Rect(br - new Vector(resize_corner_size * 0.75f, resize_corner_size * 0.75f), br);
                        int resize_id = window.GetID("#RESIZE");
                        bool hovered, held;
                        ImGui.Button.ButtonBehavior(resize_rect, resize_id, out hovered, out held, ButtonFlags.FlattenChilds);
                        resize_col =
                            held ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Active) :
                            hovered ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Hover) :
                            style.Get<Color>(GUIStyleName.ResizeGripColor);

                        if (hovered || held)
                            Input.Mouse.Cursor = Cursor.NeswResize;
                        
                        if (held)
                        {
                            // We don't use an incremental MouseDelta but rather compute an absolute target size based on mouse position
                            var t = Input.Mouse.MousePos - g.ActiveIdClickOffset - window.Position;
                            Size resize_size = new Size(t.X + resize_rect.Width, t.Y + resize_rect.Height);
                            window.ApplySize(resize_size);
                        }

                        window.Size = window.FullSize;
                        title_bar_rect = window.TitleBarRect;
                    }


                    // Window background
                    Color bg_color = style.BackgroundColor;
                    if (bg_alpha >= 0.0f)
                        bg_color.A = bg_alpha;
                    if (bg_color.A > 0.0f)
                        window.DrawList.AddRectFilled(window.Position + new Vector(0, window.TitleBarHeight), window.Rect.BottomRight, bg_color, window_rounding, flags.HaveFlag(WindowFlags.NoTitleBar) ? 15 : 4 | 8);

                    // Title bar
                    if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                    {
                        window.DrawList.AddRectFilled(title_bar_rect.TopLeft, title_bar_rect.BottomRight,
                            g.FocusedWindow == window ?
                            headerStyle.Get<Color>(GUIStyleName.BackgroundColor, GUIState.Active) :
                            headerStyle.Get<Color>(GUIStyleName.BackgroundColor), window_rounding, 1 | 2);
                    }

                    // Render resize grip
                    // (after the input handling so we don't have a frame of latency)
                    if (!flags.HaveFlag(WindowFlags.NoResize))
                    {
                        Point br = window.Rect.BottomRight;
                        var borderSize = 4;
                        window.DrawList.PathLineTo(br + new Vector(-resize_corner_size, -borderSize));
                        window.DrawList.PathLineTo(br + new Vector(-borderSize, -resize_corner_size));
                        window.DrawList.PathArcToFast(new Point(br.X - window_rounding - borderSize, br.Y - window_rounding - borderSize), window_rounding, 0, 3);
                        window.DrawList.PathFill(resize_col);
                    }

                    // Save clipped aabb so we can access it in constant-time in FindHoveredWindow()
                    window.WindowClippedRect = window.Rect;
                    window.WindowClippedRect.Intersect(window.ClipRect);
                }


                // Title bar
                if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                {
                    //const float pad = 2.0f;
                    //const float rad = (window.TitleBarHeight - pad * 2.0f) * 0.5f;
                    //if (CloseButton(window.GetID("#CLOSE"), window.Rect.TopRight + new Vector(-pad - rad, pad + rad), rad))
                    //    open = false;

                    Size text_size = headerStyle.CalcSize(Content.Cached(name, name), GUIState.Normal, null);
                    //if (!flags.HaveFlag(WindowFlags.NoCollapse))
                    //    RenderCollapseTriangle(window->Pos + style.FramePadding, !window.Collapsed, 1.0f, true);

                    Point text_min = window.Position + new Vector(style.PaddingLeft, style.PaddingTop);
                    Point text_max = window.Position + new Vector(window.Size.Width - style.PaddingHorizontal, style.PaddingVertical * 2 + text_size.Height);
                    //ImVec2 clip_max = ImVec2(window->Pos.x + window->Size.x - (p_open ? title_bar_rect.GetHeight() - 3 : style.FramePadding.x), text_max.y); // Match the size of CloseWindowButton()
                    window.DrawList.DrawText(new Rect(text_min, text_max), Content.Cached(name, name), headerStyle, GUIState.Normal);
                }

            }

            // Clear 'accessed' flag last thing
            if (first_begin_of_the_frame)
                window.Accessed = false;
            window.BeginCount++;

            // Return false if we don't intend to display anything to allow user to perform an early out optimization
            window.SkipItems = window.Collapsed || !window.Active;
            return !window.SkipItems;
        }

        public static void End()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            
            window.PopClipRect();   // inner window clip rectangle

            // Pop
            // NB: we don't clear 'window->RootWindow'. The pointer is allowed to live until the next call to Begin().
            g.CurrentWindowStack.RemoveAt(g.CurrentWindowStack.Count-1);
            g.CurrentWindow = ((g.CurrentWindowStack.Count!=0) ? null : g.CurrentWindowStack[g.CurrentWindowStack.Count-1]);
        }

        #endregion

        #region options

        /// <summary>
        /// Set the width of a control.
        /// </summary>
        /// <param name="width">width value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the width of a control/group.</returns>
        public static LayoutOption Width(double width)
        {
            return new LayoutOption(LayoutOption.Type.fixedWidth, width);
        }

        /// <summary>
        /// Set the height of a control.
        /// </summary>
        /// <param name="height">height value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the height of a control/group.</returns>
        public static LayoutOption Height(double height)
        {
            return new LayoutOption(LayoutOption.Type.fixedHeight, height);
        }

        /// <summary>
        /// Set whether the width of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the width of a control/group.</returns>
        public static LayoutOption ExpandWidth(bool expand)
        {
            return new LayoutOption(LayoutOption.Type.stretchWidth, (!expand) ? 0 : 1);
        }

        /// <summary>
        /// Set whether the height of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the height of a control/group.</returns>
        public static LayoutOption ExpandHeight(bool expand)
        {
            return new LayoutOption(LayoutOption.Type.stretchHeight, (!expand) ? 0 : 1);
        }

        /// <summary>
        /// Set the factor when expanding the width of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the width of a control/group.</returns>
        public static LayoutOption StretchWidth(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            return new LayoutOption(LayoutOption.Type.stretchWidth, factor);
        }

        /// <summary>
        /// Set the factor when expanding the height of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the height of a control/group.</returns>
        public static LayoutOption StretchHeight(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            return new LayoutOption(LayoutOption.Type.stretchHeight, factor);
        }

        #endregion

        #region Internal Styles
        internal static double fieldWidth = 50;
        #endregion

        #region controls

        #region basic controls

        #region Space

        /// <summary>
        /// Put a fixed-size space inside a layout group.
        /// </summary>
        /// <param name="pixels">size of the space</param>
        public static void Space(double pixels)
        {
            LayoutUtility.GetRect(Size.Zero, GUISkin.Instance[GUIControlName.Space],
                LayoutUtility.current.topGroup.isVertical
                    ? new[] { GUILayout.Height(pixels) }
                    : new[] { GUILayout.Width(pixels) });
        }

        /// <summary>
        /// Put a expanded space inside a layout group.
        /// </summary>
        public static void FlexibleSpace()
        {
            LayoutUtility.GetRect(Size.Zero, GUISkin.Instance[GUIControlName.Space],
                LayoutUtility.current.topGroup.isVertical
                    ? new[] { GUILayout.StretchHeight(1) }
                    : new[] { GUILayout.StretchWidth(1) });
        }

        #endregion

        #region Button

        /// <summary>
        /// Create an auto-layout button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool Button(string text, string id, params LayoutOption[] options)
        {
            return Button(text, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool Button(string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return Button(text, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool Button(Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return Button(content, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool Button(Content content, string id, params LayoutOption[] options)
        {
            return Button(content, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool Button(string text, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoButton(Content.Cached(text, name), style, name, options);
        }

        internal static bool Button(Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoButton(content, style, name);
        }

        private static bool DoButton(Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;

            Point pos = form.drawContext.CurrentPos;
            Size contentSize = style.CalcSize(content, GUIState.Normal, options);

            Rect rect = new Rect(pos, contentSize);

            return GUI.Button(rect, content, name);
        }

        #endregion

        #region Label

        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display on the label</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        public static void Label(string text, string id, params LayoutOption[] options)
        {
            Label(text, GUISkin.Instance[GUIControlName.Label], id, options);
        }

        internal static void Label(string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            Label(text, GUISkin.Instance[GUIControlName.Label], id, options);
        }

        internal static void Label(Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            Label(content, GUISkin.Instance[GUIControlName.Label], id, options);
        }

        internal static void Label(Content content, string id, params LayoutOption[] options)
        {
            Label(content, GUISkin.Instance[GUIControlName.Label], id, options);
        }

        internal static void Label(string text, GUIStyle style, string name, params LayoutOption[] options)
        {
            DoLabel(Content.Cached(text, name), style, name, options);
        }

        internal static void Label(Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            DoLabel(content, style, name, options);
        }

        private static void DoLabel(Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            Size contentSize = Size.Zero;
            if (Event.current.type == EventType.Layout)
            {
                contentSize = style.CalcSize(content, GUIState.Normal, options);
            }
            var rect = LayoutUtility.GetRect(contentSize, style, options);
            GUI.Label(rect, content, name);
        }

        #endregion

        #region Toggle

        #region Toggle with label

        /// <summary>
        /// Create an auto-layout toggle (check-box) with an label.
        /// </summary>
        /// <param name="text">text to display on the label</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(string text, bool value, string id, params LayoutOption[] options)
        {
            return DoToggle(Content.Cached(text, id), value, GUISkin.Instance[GUIControlName.Toggle], id, options);
        }

        internal static bool Toggle(string textWithPossibleId, bool value, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return DoToggle(Content.Cached(text, id), value, GUISkin.Instance[GUIControlName.Toggle], id, options);
        }

        internal static bool Toggle(string text, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoToggle(Content.Cached(text, name), value, style, name, options);
        }

        internal static bool Toggle(Content content, bool value, GUIStyle style, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return DoToggle(content, value, style, id, options);
        }

        internal static bool Toggle(Content content, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoToggle(content, value, style, name, options);
        }

        private static bool DoToggle(Content content, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            var result = GUI.Toggle(GUILayout.GetToggleRect(content, style, options), content, value, name);
            return result;
        }

        private static Rect GetToggleRect(Content content, GUIStyle style, params LayoutOption[] options)
        {
            if (content == null)
            {
                return LayoutUtility.GetRect(new Size(16, 16), style, options);
            }
            else
            {
                var textSize = style.CalcSize(content, GUIState.Normal, null);
                var size = new Size(16 + textSize.Width, 16 > textSize.Height ? 16 : textSize.Height);
                return LayoutUtility.GetRect(size, style, options);
            }
        }

        #endregion

        #region Toggle without label

        /// <summary>
        /// Create an auto-layout toggle (check-box).
        /// </summary>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle</returns>
        internal static bool Toggle(bool value, string id, params LayoutOption[] options)
        {
            return DoToggle(value, GUISkin.Instance[GUIControlName.Toggle], id, options);
        }

        internal static bool Toggle(bool value, GUIStyle style, string id, params LayoutOption[] options)
        {
            return DoToggle(value, style, id, options);
        }

        private static bool DoToggle(bool value, GUIStyle style, string id, params LayoutOption[] options)
        {
            return GUI.Toggle(LayoutUtility.GetRect(new Size(16, 16), style, options), value, id);
        }

        #endregion

        #endregion

        #region Radio

        public static bool Radio(string label, ref string active_id, string id)
        {
            bool pressed = DoRadio(Content.Cached(label, id), id == active_id, GUISkin.Instance[GUIControlName.Label], id);
            if (pressed)
            {
                active_id = id;
            }
            return pressed;
        }

        private static bool DoRadio(Content content, bool value, GUIStyle style, string id)
        {
            var result = GUILayout.Toggle(content, value, style, id);
            return result;
        }

        #endregion

        #region HoverButton

        /// <summary>
        /// Create an auto-layout button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(string text, string id, params LayoutOption[] options)
        {
            return HoverButton(text, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool HoverButton(string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return HoverButton(text, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool HoverButton(Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return HoverButton(content, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool HoverButton(Content content, string id, params LayoutOption[] options)
        {
            return HoverButton(content, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool HoverButton(string text, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoHoverButton(Content.Cached(text, name), style, name, options);
        }

        internal static bool HoverButton(Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoHoverButton(content, style, name);
        }

        private static bool DoHoverButton(Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            Size contentSize = Size.Zero;
            if (Event.current.type == EventType.Layout)
            {
                contentSize = style.CalcSize(content, GUIState.Normal, options);
            }
            var rect = LayoutUtility.GetRect(contentSize, style, options);
            return GUI.HoverButton(rect, content, name);
        }

        #endregion

        #region Slider

        /// <summary>
        /// Create an auto-layout horizontal slider that user can drag to select a value.
        /// </summary>
        /// <param name="size">size of the slider</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the left end of the slider.</param>
        /// <param name="maxValue">The value at the right end of the slider.</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double Slider(Size size, double value, double minValue, double maxValue, string id, params LayoutOption[] options)
        {
            return DoSlider(size, value, minValue, maxValue, GUISkin.Instance[GUIControlName.Slider], true, id, options);
        }

        internal static double Slider(Size size, double value, double minValue, double maxValue, GUIStyle style, string id, params LayoutOption[] options)
        {
            return DoSlider(size, value, minValue, maxValue, style, true, id, options);
        }

        /// <summary>
        /// Create an auto-layout vertical slider that user can drag to select a value.
        /// </summary>
        /// <param name="size">size of the slider</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double VSlider(Size size, double value, double minValue, double maxValue, string id, params LayoutOption[] options)
        {
            return DoSlider(size, value, minValue, maxValue, GUISkin.Instance[GUIControlName.Slider], false, id, options);
        }

        internal static double VSlider(Size size, double value, double minValue, double maxValue, GUIStyle style, string id, params LayoutOption[] options)
        {
            return DoSlider(size, value, minValue, maxValue, style, false, id, options);
        }

        private static double DoSlider(Size size, double value, double minValue, double maxValue, GUIStyle style, bool isHorizontal, string id, params LayoutOption[] options)
        {
            return GUI.Slider(LayoutUtility.GetRect(size, style, options), value, minValue, maxValue, isHorizontal, id);
        }


        #endregion

        #region ToggleButton

        /// <summary>
        /// Create an auto-layout button that acts like a toggle.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle-button</returns>
        public static bool ToggleButton(string text, bool value, string id, params LayoutOption[] options)
        {
            return DoToggleButton(Content.Cached(text, id), value, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool ToggleButton(string textWithPossibleId, bool value, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return DoToggleButton(Content.Cached(text, id), value, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool ToggleButton(string text, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoToggleButton(Content.Cached(text, name), value, style, name, options);
        }

        internal static bool ToggleButton(Content content, bool value, GUIStyle style, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return DoToggleButton(content, value, style, id, options);
        }

        internal static bool ToggleButton(Content content, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoToggleButton(content, value, style, name, options);
        }

        private static bool DoToggleButton(Content content, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            Size contentSize = Size.Zero;
            if (Event.current.type == EventType.Layout)
            {
                contentSize = style.CalcSize(content, GUIState.Normal, options);
            }
            var rect = LayoutUtility.GetRect(contentSize, style, options);
            var result = GUI.ToggleButton(rect, content, value, name);
            return result;
        }

        #endregion

        #region PolygonButton

        /// <summary>
        /// Create an auto-layout polyon-button.
        /// </summary>
        /// <param name="points"><see cref="ImGui.Point"/> list of the polygon.</param>
        /// <param name="textRect">the rect that occupied by the text</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, string id, params LayoutOption[] options)
        {
            return PolygonButton(points, textRect, text, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return PolygonButton(points, textRect, text, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return PolygonButton(points, textRect, content, GUISkin.Instance[GUIControlName.Button], id, options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, Content content, string name, params LayoutOption[] options)
        {
            return PolygonButton(points, textRect, content, GUISkin.Instance[GUIControlName.Button], name, options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoPolygonButton(points, textRect, Content.Cached(text, name), style, name, options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, Content content, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoPolygonButton(points, textRect, content, style, name);
        }

        private static bool DoPolygonButton(IReadOnlyList<Point> points, Rect textRect, Content content, GUIStyle style, string id, params LayoutOption[] options)
        {
            var rect = new Rect();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                rect.Union(point);
            }
            rect = LayoutUtility.GetRect(rect.Size, style, options);
            return GUI.PolygonButton(rect, points, textRect, content, id);
        }


        #endregion

        #region Image

        /// <summary>
        /// Create an auto-layout image.
        /// </summary>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        public static void Image(string filePath, string id, params LayoutOption[] options)
        {
            Image(Content.CachedTexture(filePath, id), GUISkin.Instance[GUIControlName.Image], id, options);
        }

        internal static void Image(string imageFilePathWithPossibleId, params LayoutOption[] options)
        {
            string imageFilePath, id;
            Utility.GetId(imageFilePathWithPossibleId, out imageFilePath, out id);
            Image(Content.CachedTexture(imageFilePath, id), GUISkin.Instance[GUIControlName.Image], id, options);
        }

        public static void Image(ITexture texture, string id, params LayoutOption[] options)
        {
            Image(Content.Cached(texture, id), GUISkin.Instance[GUIControlName.Image], id, options);
        }

        internal static void Image(ITexture texture, GUIStyle style, string id, params LayoutOption[] options)
        {
            Image(Content.Cached(texture, id), style, id, options);
        }

        internal static void Image(Content content, string id, params LayoutOption[] options)
        {
            Image(content, GUISkin.Instance[GUIControlName.Image], id, options);
        }

        internal static void Image(Content content, GUIStyle style, string id, params LayoutOption[] options)
        {
            DoImage(content, style, id, options);
        }

        private static void DoImage(Content content, GUIStyle style, string id, params LayoutOption[] options)
        {
            Size contentSize = style.CalcSize(content, GUIState.Normal, options);
            var rect = LayoutUtility.GetRect(contentSize, style,
                new[] { GUILayout.Width(contentSize.Width), GUILayout.Height(contentSize.Height) });
            GUI.Image(rect, content, style, id);
        }

        #endregion

        #endregion

        #region combined controls

        public static double Slider(string label, double value, double min, double max, string id)
        {
            return Slider(Content.Cached(label, id), value, min, max, GUISkin.Instance[GUIControlName.Slider], id);
        }

        internal static double Slider(Content content, double value, double min, double max, string id)
        {
            return Slider(content, value, min, max, GUISkin.Instance[GUIControlName.Slider], id);
        }

        internal static double Slider(Content content, double value, double min, double max, GUIStyle style, string id)
        {
            GUILayout.BeginHorizontal();
            {
                Size textSize = style.CalcSize(content, GUIState.Normal, null);
                GUILayout.Label(content, style, "SliderLabel" + id, GUILayout.Width(textSize.Width));
                var labelRect = GUILayout.GetLastRect();
                Rect rect;
                rect = LayoutUtility.GetRect(textSize, style, new[] { GUILayout.ExpandWidth(true)});
                value = GUI.Slider(rect, value, min, max, "Slider" + id);
            }
            GUILayout.EndHorizontal();
            return value;
        }

        public static double VSlider(string label, double value, double min, double max, string id, params LayoutOption[] options)
        {
            return VSlider(Content.Cached(label, id), value, min, max, GUISkin.Instance[GUIControlName.Slider], id, options);
        }

        internal static double VSlider(Content content, double value, double min, double max, string id, params LayoutOption[] options)
        {
            return VSlider(content, value, min, max, GUISkin.Instance[GUIControlName.Slider], id, options);
        }

        internal static double VSlider(Content content, double value, double min, double max, GUIStyle style, string id, params LayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            {
                Size textSize = style.CalcSize(content, GUIState.Normal, null);
                GUILayout.Label(content, style, "VSliderLabel" + id, GUILayout.Height(textSize.Height), GUILayout.Width(textSize.Width));
                value = GUILayout.VSlider(new Size(textSize.Width, textSize.Height), value, min, max, "VSlider" + id, GUILayout.Width(30), GUILayout.ExpandHeight(true));
            }
            GUILayout.EndVertical();
            return value;
        }
        #endregion

        #endregion

        public static Rect GetRect(Size size, GUIStyle style, params LayoutOption[] options)
        {
            return LayoutUtility.GetRect(size, style, options);
        }

        public static Rect GetLastRect()
        {
            return LayoutUtility.GetLastRect();
        }

    }
}