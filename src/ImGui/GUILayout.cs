using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with automatic layout.
    /// </summary>
    public partial class GUILayout
    {
        #region ID

        public static void PushID(int int_id)
        {
            Window window = GetCurrentWindow();
            window.IDStack.Push(window.GetID(int_id));
        }

        public static void PushID(string str_id)
        {
            Window window = GetCurrentWindow();
            window.IDStack.Push(window.GetID(str_id));
        }

        public static void PopID()
        {
            Window window = GetCurrentWindow();
            window.IDStack.Pop();
        }

        #endregion

        #region container

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
                        {
                            Input.Mouse.Cursor = Cursor.NeswResize;
                        }

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

                window.ClientRect = new Rect(point1: new Point(window.Position.X, window.Position.Y + window.TitleBarHeight),
                    point2: window.Rect.BottomRight);

                // Title bar
                if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                {
                    //const float pad = 2.0f;
                    //const float rad = (window.TitleBarHeight - pad * 2.0f) * 0.5f;
                    //if (CloseButton(window.GetID("#CLOSE"), window.Rect.TopRight + new Vector(-pad - rad, pad + rad), rad))
                    //    open = false;

                    Size text_size = headerStyle.CalcSize(name, GUIState.Normal, null);
                    //if (!flags.HaveFlag(WindowFlags.NoCollapse))
                    //    RenderCollapseTriangle(window->Pos + style.FramePadding, !window.Collapsed, 1.0f, true);

                    Point text_min = window.Position + new Vector(style.PaddingLeft, style.PaddingTop);
                    Point text_max = window.Position + new Vector(window.Size.Width - style.PaddingHorizontal, style.PaddingVertical * 2 + text_size.Height);
                    //ImVec2 clip_max = ImVec2(window->Pos.x + window->Size.x - (p_open ? title_bar_rect.GetHeight() - 3 : style.FramePadding.x), text_max.y); // Match the size of CloseWindowButton()
                    window.DrawList.DrawText(new Rect(text_min, text_max), name, headerStyle, GUIState.Normal);
                }
            }

            // Clear 'accessed' flag last thing
            if (first_begin_of_the_frame)
                window.Accessed = false;
            window.BeginCount++;

            window.StackLayout.Begin();

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

            window.ProcessLayout();

            // Pop
            g.CurrentWindowStack.RemoveAt(g.CurrentWindowStack.Count-1);
            g.CurrentWindow = ((g.CurrentWindowStack.Count==0) ? null : g.CurrentWindowStack[g.CurrentWindowStack.Count-1]);
        }

        public static void BeginHorizontal(string str_id, GUIStyle style = null, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            window.StackLayout.BeginLayoutGroup(id, false, style, options);
        }

        public static void EndHorizontal()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
        }

        public static void BeginVertical(string str_id, GUIStyle style = null, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            window.StackLayout.BeginLayoutGroup(id, true, style, null);
        }

        public static void EndVertical()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
        }

        public static bool CollapsingHeader(string text, ref bool open)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            var height = GUIStyle.Default.FontSize;
            var id = window.GetID(text);
            GUIStyle style = GUISkin.Instance[GUIControlName.Button];
            var rect = GetRect(new Size(0, height), text, style, GUILayout.ExpandWidth(true));

            bool hovered, held;
            bool pressed = ImGui.Button.ButtonBehavior(rect, id, out hovered, out held, ButtonFlags.PressedOnClick);
            if (pressed)
            {
                open = !open;
            }

            // Render
            DrawList d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            Color col = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.RenderFrame(rect.Min, rect.Max, col, true, 0);
            d.DrawText(rect, text, style, state);

            return open;
        }

        public static Rect GetWindowClientRect()
        {
            Window window = GetCurrentWindow();
            return window.ClientRect;
        }

        public static Rect GetRect(Size size, string str_id, GUIStyle style = null, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(str_id);
            var rect = window.GetRect(id, size, style, options);
            return rect;
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

        #region controls

        #region Space

        /// <summary>
        /// Put a fixed-size space inside a layout group.
        /// </summary>
        public static void Space(string str_id, double size)
        {
            Window window = GetCurrentWindow();
            var layout = window.StackLayout;

            int id = window.GetID(str_id);
            window.GetRect(id, Size.Zero, GUISkin.Instance[GUIControlName.Space],
                layout.InsideVerticalGroup ? new[] { GUILayout.Height(size) } : new[] { GUILayout.Width(size) });
        }

        private static Window GetCurrentWindow()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            return window;
        }

        /// <summary>
        /// Put a expanded space inside a layout group.
        /// </summary>
        public static void FlexibleSpace(string str_id)
        {
            Window window = GetCurrentWindow();
            var layout = window.StackLayout;

            int id = window.GetID(str_id);
            Rect rect = window.GetRect(id, Size.Zero, GUISkin.Instance[GUIControlName.Space],
                layout.InsideVerticalGroup ? new[] { GUILayout.StretchHeight(1) } : new[] { GUILayout.StretchWidth(1) });
        }

        #endregion

        #region Button

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

        #endregion

        #region Label

        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display on the label</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        public static void Label(string text, params LayoutOption[] options)
        {
            DoLabel(text, GUISkin.Instance[GUIControlName.Label], options);
        }

        private static void DoLabel(string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(text);
            Size contentSize = style.CalcSize(text, GUIState.Normal, options);
            Rect rect = window.GetRect(id, contentSize, style, options);
            GUI.Label(rect, text);
        }

        #endregion

        #region Toggle

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

        #endregion

        #region Radio

        public static bool Radio(string label, ref string active_id, string id)
        {
            return false;
            throw new NotImplementedException();//TODO implement this with separate logic from Toggle.
        }

        #endregion

        #region HoverButton

        /// <summary>
        /// Create an auto-layout button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(string text, params LayoutOption[] options)
        {
            return HoverButton(text, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool HoverButton(string text, GUIStyle style, params LayoutOption[] options)
        {
            return DoHoverButton(text, style, options);
        }

        private static bool DoHoverButton(string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            Size size = style.CalcSize(text, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style, options);
            return GUI.HoverButton(rect, text);
        }

        #endregion

        #region Slider

        /// <summary>
        /// Create an auto-layout horizontal slider that user can drag to select a value.
        /// </summary>
        /// <param name="label">label of the slider</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the left end of the slider.</param>
        /// <param name="maxValue">The value at the right end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double Slider(string label, double value, double minValue, double maxValue)
        {
            return DoSlider(label, value, minValue, maxValue, GUISkin.Instance[GUIControlName.Slider], true);
        }

        /// <summary>
        /// Create an auto-layout vertical slider that user can drag to select a value.
        /// </summary>
        /// <param name="label">label of the slider</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double VSlider(string label, double value, double minValue, double maxValue)
        {
            return DoSlider(label, value, minValue, maxValue, GUISkin.Instance[GUIControlName.Slider], false);
        }

        private static double DoSlider(string label, double value, double minValue, double maxValue, GUIStyle style, bool isHorizontal)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var id = window.GetID(label);
            var options = new LayoutOption[] { isHorizontal? GUILayout.ExpandWidth(true): GUILayout.ExpandHeight(true) };
            Size size = style.CalcSize(label, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style, options);
            //FIXME append slider rect
            return GUI.Slider(rect, label, value, minValue, maxValue, isHorizontal);
        }

        #endregion

        #region ToggleButton

        /// <summary>
        /// Create an auto-layout button that acts like a toggle.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle-button</returns>
        public static bool ToggleButton(string text, bool value, params LayoutOption[] options)
        {
            return DoToggleButton(text, value, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool ToggleButton(string text, bool value, GUIStyle style, string name, params LayoutOption[] options)
        {
            return DoToggleButton(text, value, style, options);
        }

        private static bool DoToggleButton(string text, bool value, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            Size size = style.CalcSize(text, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style, options);
            var result = GUI.ToggleButton(rect, text, value);
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
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, params LayoutOption[] options)
        {
            return PolygonButton(points, textRect, text, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, GUIStyle style, params LayoutOption[] options)
        {
            return DoPolygonButton(points, textRect, text, style, options);
        }

        private static bool DoPolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            var rect = new Rect();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                rect.Union(point);
            }
            rect = window.GetRect(id, rect.Size, style, options);
            return GUI.PolygonButton(rect, points, textRect, text);
        }


        #endregion

        #region Image

        /// <summary>
        /// Create an auto-layout image.
        /// </summary>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        public static void Image(string filePath, params LayoutOption[] options)
        {
            Image(filePath, GUISkin.Instance[GUIControlName.Image], options);
        }

        public static void Image(string filePath, GUIStyle style, params LayoutOption[] options)
        {
            DoImage(filePath, style, options);//var texture = TextureUtil.GetTexture(filePath);
        }

        private static void DoImage(string filePath, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(filePath);
            Size size = style.CalcSize(filePath, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style,
                new[] { GUILayout.Width(size.Width), GUILayout.Height(size.Height) });
            GUI.Image(rect, filePath, style);
        }

        internal static void Image(ITexture texture, params LayoutOption[] options)
        {
            Image(texture, GUISkin.Instance[GUIControlName.Image], options);
        }

        internal static void Image(ITexture texture, GUIStyle style, params LayoutOption[] options)
        {
            DoImage(texture, style, options);
        }

        private static void DoImage(ITexture texture, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(texture);
            Size size = style.CalcSize(texture, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style,
                new[] { GUILayout.Width(size.Width), GUILayout.Height(size.Height) });
            GUI.Image(rect, texture, style);
        }


        #endregion

        #endregion

    }
}