using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool Begin(string name, ref bool open)
        {
            return Begin(name, ref open, Point.Zero, new Size(400, 300), 1, WindowFlags.VerticalScrollbar);
        }

        public static bool Begin(string name, ref bool open, Point position, Size size, double bg_alpha, WindowFlags flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;
            Debug.Assert(name != null);                        // Window name required
            Debug.Assert(g.Initialized);                       // Forgot to call NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // Called Render() or EndFrame() and haven't called NewFrame() again yet

            if (flags.HaveFlag(WindowFlags.NoInputs))
            {
                flags |= WindowFlags.NoMove | WindowFlags.NoResize;
            }

            // Find or create
            Window window = w.FindWindowByName(name) ?? w.CreateWindow(name, position, size, flags);

            // Check if this is the first call of Begin
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
            Window parent_window = w.WindowStack.Count != 0 ? w.WindowStack[w.WindowStack.Count - 1] : null;
            w.WindowStack.Add(window);
            w.CurrentWindow = window;
            Debug.Assert(parent_window != null || !flags.HaveFlag(WindowFlags.ChildWindow));

            // Update known root window (if we are a child window, otherwise window == window->RootWindow)
            int root_idx, root_non_popup_idx;
            for (root_idx = w.WindowStack.Count - 1; root_idx > 0; root_idx--)
            {
                if (!(w.WindowStack[root_idx].Flags.HaveFlag(WindowFlags.ChildWindow)))
                    break;
            }
            for (root_non_popup_idx = root_idx; root_non_popup_idx > 0; root_non_popup_idx--)
            {
                if (!(w.WindowStack[root_non_popup_idx].Flags.HaveFlag(WindowFlags.ChildWindow | WindowFlags.Popup)))
                    break;
            }
            window.RootWindow = w.WindowStack[root_idx];

            // When reusing window again multiple times a frame, just append content (don't need to setup again)
            if (first_begin_of_the_frame)
            {
                window.Active = true;
                window.ClipRect = Rect.Big;
                window.LastActiveFrame = current_frame;

                window.DrawList.Clear();
                window.DrawList.Init();
                Rect fullScreenRect = new Rect(0, 0, form.Rect.Size);
                if ((flags & WindowFlags.ChildWindow)!=0 && !((flags & (WindowFlags.ComboBox | WindowFlags.Popup))!=0))
                    window.DrawList.PushClipRect(parent_window.ClipRect);
                else
                    window.DrawList.PushClipRect(fullScreenRect);

                // clip
                window.ClipRect = fullScreenRect;

                // Collapse window by double-clicking on title bar
                if (w.HoveredWindow == window && g.IsMouseHoveringRect(window.TitleBarRect) && Input.Mouse.LeftButtonDoubleClicked)
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
                float window_rounding = (float)style.Get<double>(GUIStyleName.WindowRounding);
                if (window.Collapsed)
                {
                    // Draw title bar only
                    window.DrawList.AddRectFilled(title_bar_rect.Min, title_bar_rect.Max, new Color(0.40f, 0.40f, 0.80f, 0.20f));
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
                        GUIBehavior.ButtonBehavior(resize_rect, resize_id, out hovered, out held, ButtonFlags.FlattenChilds);
                        resize_col =
                            held ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Active) :
                            hovered ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Hover) :
                            style.Get<Color>(GUIStyleName.ResizeGripColor);

                        if (hovered || held)
                        {
                            //Input.Mouse.Cursor = Cursor.NeswResize;
                        }

                        if (held)
                        {
                            // We don't use an incremental MouseDelta but rather compute an absolute target size based on mouse position
                            var t = Input.Mouse.MousePos - g.ActiveIdClickOffset - window.Position;
                            Size resize_size = new Size(t.X + resize_rect.Width, t.Y + resize_rect.Height);
                            window.ApplySize(resize_size);

                            // adjust scroll parameters
                            var contentSize = window.ContentRect.Size;
                            if (contentSize != Size.Zero)
                            {
                                var vH = window.Rect.Height - window.TitleBarHeight - window.Style.BorderVertical - window.Style.PaddingVertical;
                                var cH = contentSize.Height;
                                if(cH > vH)
                                {
                                    var oldScrollY = window.Scroll.Y;
                                    oldScrollY = MathEx.Clamp(oldScrollY, 0, cH - vH);
                                    window.Scroll.Y = oldScrollY;
                                }
                            }
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
                            w.FocusedWindow == window ?
                            headerStyle.Get<Color>(GUIStyleName.BackgroundColor, GUIState.Active) :
                            headerStyle.Get<Color>(GUIStyleName.BackgroundColor), window_rounding, 1 | 2);
                    }

                    // Render resize grip
                    // (after the input handling so we don't have a frame of latency)
                    if (!flags.HaveFlag(WindowFlags.NoResize))
                    {
                        Point br = window.Rect.BottomRight;
                        var borderBottom = window.Style.BorderBottom;
                        var borderRight = window.Style.BorderRight;
                        window.DrawList.PathLineTo(br + new Vector(-resize_corner_size, -borderBottom));
                        window.DrawList.PathLineTo(br + new Vector(-borderRight, -resize_corner_size));
                        window.DrawList.PathArcToFast(new Point(br.X - window_rounding - borderRight, br.Y - window_rounding - borderBottom), window_rounding, 0, 3);
                        window.DrawList.PathFill(resize_col);
                    }

                    // Save clipped aabb so we can access it in constant-time in FindHoveredWindow()
                    window.WindowClippedRect = window.Rect;
                    window.WindowClippedRect.Intersect(window.ClipRect);

                    // Scroll bar
                    if (flags.HaveFlag(WindowFlags.VerticalScrollbar))
                    {
                        //get content size without clip
                        var contentPosition = window.ContentRect.TopLeft;
                        var contentSize = window.ContentRect.Size;
                        if (contentSize != Size.Zero)
                        {
                            int id = window.GetID("#SCROLLY");

                            double scrollBarWidth = window.Style.Get<double>(GUIStyleName.ScrollBarWidth);
                            Point scroll_TopLeft = new Point(
                                window.Rect.Right - scrollBarWidth - window.Style.BorderRight - window.Style.PaddingRight,
                                window.Rect.Top + window.TitleBarHeight + window.Style.BorderTop + window.Style.PaddingTop);
                            var sH = window.Rect.Height - window.TitleBarHeight - window.Style.BorderVertical - window.Style.PaddingVertical- resize_corner_size;
                            var vH = window.Rect.Height - window.TitleBarHeight - window.Style.BorderVertical - window.Style.PaddingVertical;
                            Point scroll_BottomRight = scroll_TopLeft + new Vector(scrollBarWidth, sH);
                            Rect bgRect = new Rect(scroll_TopLeft, scroll_BottomRight);

                            var cH = contentSize.Height;
                            var top = window.Scroll.Y * sH / cH;
                            var height = sH * vH / cH;

                            if(height < sH)
                            {
                                // handle mouse click/drag
                                bool held = false;
                                bool hovered = false;
                                bool previously_held = (g.ActiveId == id);
                                GUIBehavior.ButtonBehavior(bgRect, id, out hovered, out held);
                                if (held)
                                {
                                    top = Input.Mouse.MousePos.Y - bgRect.Y - 0.5 * height;
                                    top = MathEx.Clamp(top, 0, sH - height);
                                    var targetScrollY = top * cH / sH;
                                    window.SetWindowScrollY(targetScrollY);
                                }

                                Point scrollButton_TopLeft = scroll_TopLeft + new Vector(0, top);
                                Point scrllButton_BottomRight = scrollButton_TopLeft + new Vector(scrollBarWidth, height);
                                Rect buttonRect = new Rect(scrollButton_TopLeft, scrllButton_BottomRight);

                                //Draw vertical scroll bar and button
                                {
                                    var bgColor = window.Style.Get<Color>(GUIStyleName.ScrollBarBackgroundColor);
                                    var buttonColor = window.Style.Get<Color>(GUIStyleName.ScrollBarButtonColor, held ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal);
                                    window.DrawList.AddRectFilled(bgRect.TopLeft, buttonRect.TopRight, bgColor);
                                    window.DrawList.AddRectFilled(buttonRect.TopLeft, buttonRect.BottomRight, buttonColor);
                                    window.DrawList.AddRectFilled(buttonRect.BottomLeft, bgRect.BottomRight, bgColor);
                                }
                            }
                            else
                            {
                                var bgColor = window.Style.Get<Color>(GUIStyleName.ScrollBarBackgroundColor);
                                window.DrawList.AddRectFilled(bgRect.TopLeft, bgRect.BottomRight, bgColor);
                            }

                        }
                    }
                    window.ContentRect = Rect.Zero;
                }

                window.ClientRect = new Rect(
                    point1: new Point(window.Position.X, window.Position.Y + window.TitleBarHeight),
                    point2: window.Rect.BottomRight);

                // Title bar
                if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                {
                    Size text_size = headerStyle.CalcSize(name, GUIState.Normal, null);
                    Point text_min = window.Position + new Vector(style.PaddingLeft, style.PaddingTop);
                    Point text_max = window.Position + new Vector(window.Size.Width - style.PaddingHorizontal, style.PaddingVertical * 2 + text_size.Height);
                    window.DrawList.DrawText(new Rect(text_min, text_max), name, headerStyle, GUIState.Normal);
                }

                // Borders
                if (flags.HaveFlag(WindowFlags.ShowBorders))
                {
                    var borderColor = window.Style.Get<Color>(GUIStyleName.WindowBorderColor);
                    var borderShadowColor = window.Style.Get<Color>(GUIStyleName.WindowBorderShadowColor);
                    window.DrawList.AddRect(window.Position + new Vector(1, 1),
                        window.Position + new Vector(window.Size.Width, window.Size.Height) + new Vector(1, 1),
                        borderShadowColor, window_rounding);
                    window.DrawList.AddRect(window.Position, window.Position + new Vector(window.Size.Width, window.Size.Height),
                        borderColor, window_rounding);
                    if (!(flags.HaveFlag(WindowFlags.NoTitleBar)))
                    {
                        window.DrawList.AddLine(title_bar_rect.BottomLeft + new Vector(1, 0), title_bar_rect.BottomRight - new Vector(1, 0), borderColor);
                    }
                }
            }

            // Inner clipping rectangle
            {
                // We set this up after processing the resize grip so that our clip rectangle doesn't lag by a frame
                // Note that if our window is collapsed we will end up with a null clipping rectangle which is the correct behavior.
                Rect title_bar_rect = window.TitleBarRect;
                const float border_size = 0;
                var paddingHorizontal = window.Style.PaddingHorizontal;
                // Force round to ensure that e.g. (int)(max.x-min.x) in user's render code produce correct result.
                Rect clip_rect = new Rect(
                    new Point(Math.Floor(0.5f + title_bar_rect.Min.X + Math.Max(border_size, Math.Floor(paddingHorizontal * 0.5f))),
                              Math.Floor(0.5f + title_bar_rect.Max.Y + border_size)),
                    new Point(Math.Floor(0.5f + window.Position.X + window.Size.Width - Math.Max(border_size, Math.Floor(paddingHorizontal * 0.5f))),
                              Math.Floor(0.5f + window.Position.Y + window.Size.Height - border_size)));
                window.DrawList.PushClipRect(clip_rect);
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
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            window.DrawList.PopClipRect();

            window.StackLayout.Layout(window.ClientRect.Size);

            // Pop
            w.WindowStack.RemoveAt(w.WindowStack.Count - 1);
            w.CurrentWindow = (w.WindowStack.Count == 0) ? null : w.WindowStack[w.WindowStack.Count - 1];
        }
    }

    [Flags]
    public enum WindowFlags
    {
        Default = 0,
        NoTitleBar = 1 << 0,   // Disable title-bar
        NoResize = 1 << 1,   // Disable user resizing with the lower-right grip
        NoMove = 1 << 2,   // Disable user moving the window
        NoScrollbar = 1 << 3,   // Disable scrollbars (window can still scroll with mouse or programatically)
        NoScrollWithMouse = 1 << 4,   // Disable user vertically scrolling with mouse wheel
        NoCollapse = 1 << 5,   // Disable user collapsing window by double-clicking on it
        AlwaysAutoResize = 1 << 6,   // Resize every window to its content every frame
        ShowBorders = 1 << 7,   // Show borders around windows and items
        NoSavedSettings = 1 << 8,   // Never load/save settings in .ini file
        NoInputs = 1 << 9,   // Disable catching mouse or keyboard inputs
        MenuBar = 1 << 10,  // Has a menu-bar
        HorizontalScrollbar = 1 << 11,  // Allow horizontal scrollbar to appear (off by default). You may use SetNextWindowContentSize(ImVec2(width,0.0f)); prior to calling Begin() to specify width. Read code in imgui_demo in the "Horizontal Scrolling" section.
        NoFocusOnAppearing = 1 << 12,  // Disable taking focus when transitioning from hidden to visible state
        NoBringToFrontOnFocus = 1 << 13,  // Disable bringing window to front when taking focus (e.g. clicking on it or programatically giving it focus)
        AlwaysVerticalScrollbar = 1 << 14,  // Always show vertical scrollbar (even if ContentSize.y < Size.y)
        AlwaysHorizontalScrollbar = 1 << 15,  // Always show horizontal scrollbar (even if ContentSize.x < Size.x)
        AlwaysUseWindowPadding = 1 << 16,  // Ensure child windows without border uses style.WindowPadding (ignored by default for non-bordered child windows, because more convenient)

        VerticalScrollbar = 1 << 17,

        // [Internal]
        ChildWindow = 1 << 20,  // Don't use! For internal use by BeginChild()
        ChildWindowAutoFitX = 1 << 21,  // Don't use! For internal use by BeginChild()
        ChildWindowAutoFitY = 1 << 22,  // Don't use! For internal use by BeginChild()
        ComboBox = 1 << 23,  // Don't use! For internal use by ComboBox()
        Tooltip = 1 << 24,  // Don't use! For internal use by BeginTooltip()
        Popup = 1 << 25,  // Don't use! For internal use by BeginPopup()
        Modal = 1 << 26,  // Don't use! For internal use by BeginPopupModal()
        ChildMenu = 1 << 27   // Don't use! For internal use by BeginMenu()
    };
}
