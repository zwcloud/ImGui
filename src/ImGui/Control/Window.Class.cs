using System;
using System.Collections.Generic;
using ImGui.Layout;
using System.Diagnostics;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.Input;
using ImGui.OSAbstraction.Graphics;
using ImGui.Rendering;

namespace ImGui
{
    [DebuggerDisplay("{Name}:[{ID}]")]
    internal class Window
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// Name/Title
        /// </summary>
        public string Name;

        /// <summary>
        /// Position (rounded-up to nearest pixel)
        /// </summary>
        /// <remarks>Top-left point relative to the form.</remarks>
        public Point Position;

        /// <summary>
        /// Position
        /// </summary>
        public Point PosFloat;

        /// <summary>
        /// Size
        /// </summary>
        public Size Size;

        /// <summary>
        /// Size when the window is not collapsed.
        /// </summary>
        public Size FullSize { get; set; }

        /// <summary>
        /// Window flags. See <see cref="WindowFlags"/>.
        /// </summary>
        public WindowFlags Flags;

        /// <summary>
        /// Style
        /// </summary>
        public GUIStyle Style;

        /// <summary>
        /// Style of the title bar
        /// </summary>
        public GUIStyle TitleBarStyle;

        /// <summary>
        /// Draw list
        /// </summary>
        public DrawList DrawList;

        private RenderTree renderTree;

        public Rect ClipRect;

        /// <summary>
        /// Scroll values: (horizontal, vertical)
        /// </summary>
        public Vector Scroll;

        /// <summary>
        /// Last frame count when this window is active.
        /// </summary>
        public long LastActiveFrame;

        /// <summary>
        /// stack layout manager
        /// </summary>
        public StackLayout StackLayout { get; set; }

        /// <summary>
        /// ID stack
        /// </summary>
        public Stack<int> IDStack { get; set; } = new Stack<int>();

        public Window(string name, Point position, Size size, WindowFlags Flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;

            this.ID = name.GetHashCode();
            this.Name = name;
            this.IDStack.Push(this.ID);
            this.Flags = Flags;
            this.PosFloat = position;
            this.Position = new Point((int)PosFloat.X, (int)PosFloat.Y);
            this.Size = this.FullSize = size;
            this.DrawList = new DrawList();
            this.renderTree = new RenderTree(this.ID, this.Size);
            this.MoveID = GetID("#MOVE");
            this.Active = WasActive = false;

            // window styles
            {
                var style = new GUIStyle();
                style.Set(GUIStyleName.BorderTop, 1.0);
                style.Set(GUIStyleName.BorderRight, 1.0);
                style.Set(GUIStyleName.BorderBottom, 1.0);
                style.Set(GUIStyleName.BorderLeft, 1.0);
                style.Set(GUIStyleName.PaddingTop, 5.0);
                style.Set(GUIStyleName.PaddingRight, 10.0);
                style.Set(GUIStyleName.PaddingBottom, 5.0);
                style.Set(GUIStyleName.PaddingLeft, 10.0);
                style.Set(GUIStyleName.WindowBorderColor, Color.Rgb(170, 170, 170), GUIState.Normal);
                style.Set(GUIStyleName.WindowBorderColor, Color.Rgb(24, 131, 215), GUIState.Active);
                style.Set(GUIStyleName.WindowShadowColor, Color.Argb(100, 227, 227, 227));
                style.Set(GUIStyleName.WindowShadowWidth, 15.0);
                style.Set(GUIStyleName.BackgroundColor, Color.White);
                style.Set(GUIStyleName.ResizeGripSize, 20.0);
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(75, 102, 102, 102));
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(150, 102, 102, 102), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(225, 102, 102, 102), GUIState.Active);
                style.Set(GUIStyleName.WindowRounding, 3.0);
                style.Set(GUIStyleName.ScrollBarWidth, CurrentOS.IsDesktopPlatform ? 10.0 : 20.0);
                style.Set(GUIStyleName.ScrollBarBackgroundColor, Color.Rgb(240));
                style.Set(GUIStyleName.ScrollBarButtonColor, Color.Rgb(205), GUIState.Normal);
                style.Set(GUIStyleName.ScrollBarButtonColor, Color.Rgb(166), GUIState.Hover);
                style.Set(GUIStyleName.ScrollBarButtonColor, Color.Rgb(96), GUIState.Active);
                this.Style = style;
            }

            // window header styles
            {
                var style = new GUIStyle();
                style.Set(GUIStyleName.BackgroundColor, Color.White);
                style.Set(GUIStyleName.BackgroundColor, Color.White, GUIState.Active);
                style.Set(GUIStyleName.BackgroundColor, Color.White, GUIState.Disabled);
                style.Set<double>(GUIStyleName.BorderTopLeftRadius, 3.0);
                style.Set<double>(GUIStyleName.BorderTopRightRadius, 3.0);
                style.Set(GUIStyleName.PaddingTop, 8.0);
                style.Set(GUIStyleName.PaddingRight, 8.0);
                style.Set(GUIStyleName.PaddingBottom, 8.0);
                style.Set(GUIStyleName.PaddingLeft, 8.0);
                style.Set(GUIStyleName.FontColor, Color.Black, GUIState.Normal);
                style.Set(GUIStyleName.FontColor, Color.Rgb(153, 153, 153), GUIState.Active);
                style.FontFamily = GUIStyle.Default.FontFamily;
                style.FontSize = 12.0;
                this.TitleBarStyle = style;
            }

            var scrollBarWidth = this.Style.Get<double>(GUIStyleName.ScrollBarWidth);
            var clientSize = new Size(
                this.Size.Width - scrollBarWidth - this.Style.PaddingHorizontal - this.Style.BorderHorizontal,
                this.Size.Height - this.Style.PaddingVertical - this.Style.BorderVertical - this.TitleBarHeight);
            this.StackLayout = new StackLayout(this.ID, clientSize);
        }

        /// <summary>
        /// Gets the rect of this window
        /// </summary>
        public Rect Rect => new Rect(Position, Size);

        /// <summary>
        /// Gets the height of the title bar
        /// </summary>
        public double TitleBarHeight
        {
            get
            {
                if(this.Flags.HaveFlag(WindowFlags.NoTitleBar))
                {
                    return 0;
                }

                return TitleBarStyle.PaddingVertical + 30;
            }
        }

        /// <summary>
        /// Gets the rect of the title bar
        /// </summary>
        public Rect TitleBarRect => new Rect(Position, Size.Width, TitleBarHeight);

        /// <summary>
        /// Gets or sets the rect of the client area
        /// </summary>
        public Rect ClientRect { get; set; }

        /// <summary>
        /// Gets or sets if the window is collapsed.
        /// </summary>
        public bool Collapsed { get; set; } = false;

        /// <summary>
        /// Gets or sets if the window is active
        /// </summary>
        public bool Active
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the content rect
        /// </summary>
        public Rect ContentRect
        {
            get;
            set;
        } = Rect.Zero;

        /// <summary>
        /// Gets or sets the root window
        /// </summary>
        public Window RootWindow { get; set; }

        /// <summary>
        /// Gets or sets move ID, equals to <code>window.GetID("#MOVE")</code>.
        /// </summary>
        public int MoveID { get; internal set; }

        public Rect WindowClippedRect { get; internal set; }

        /// <summary>
        /// Gets or sets whether the window was active in last frame.
        /// </summary>
        public bool WasActive { get; internal set; }

        /// <summary>
        /// Gets or sets whether the window does nothing.
        /// </summary>
        public bool SkipItems { get; internal set; } = false;

        /// <summary>
        /// Gets or sets how many times <code>Begin()</code> was called in this frame.
        /// </summary>
        public int BeginCount { get; internal set; }

        /// <summary>
        /// Gets or sets whether the window is used in this frame
        /// </summary>
        public bool Accessed { get; internal set; }

        #region ID
        private int Hash(int seed, int int_id)
        {
            int hash = seed + 17;
            hash = hash * 23 + this.ID.GetHashCode();
            var result = hash * 23 + int_id;
            return result;
        }

        public int GetID(int int_id)
        {
            int seed = IDStack.Peek();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        public int GetID(string str_id)
        {
            int seed = IDStack.Peek();
            int int_id = str_id.GetHashCode();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);

            return id;
        }

        public int GetID(ITexture texture)
        {
            int seed = IDStack.Peek();
            int int_id = texture.GetHashCode();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }
        #endregion

        /// <summary>
        /// Apply new size to window
        /// </summary>
        /// <param name="new_size"></param>
        public void ApplySize(Size new_size)
        {
            if (this.FullSize != new_size)
            {
                {
                    var topLeft = this.Position + new Vector(this.Style.PaddingLeft + this.Style.BorderLeft, this.Style.PaddingTop + this.Style.BorderTop);
                    var bottomRight = this.Rect.BottomRight
                        - new Vector(this.Style.PaddingRight + this.Style.BorderRight, this.Style.PaddingBottom + this.Style.BorderBottom)
                        - new Vector(this.Style.Get<double>(GUIStyleName.ScrollBarWidth), 0);
                    this.ClientRect = new Rect(topLeft, bottomRight);
                }
                this.StackLayout.SetRootSize(this.ClientRect.Size);
            }
            this.FullSize = new_size;
        }

        /// <summary>
        /// Get the rect for an automatic-layout control
        /// </summary>
        /// <param name="id">id of the control</param>
        /// <param name="size">size of content, border and padding NOT included</param>
        /// <param name="style">style that will apply to requested rect</param>
        /// <returns></returns>
        public Rect GetRect(int id, Size size, LayoutOptions? options = null, string str_id = null, bool isGroup = false)
        {
            //var rect = StackLayout.GetRect(id, size, options, str_id);

            var node = this.renderTree.GetNode(id);
            if(node == null)
            {
                node = new Node();
                node.Id = id;
                node.StrId = str_id;
                if (isGroup)
                {
                    node.AttachLayoutGroup(true, options);
                }
                else
                {
                    node.AttachLayoutEntry(size, options);
                }
                this.renderTree.CurrentContainer.Add(node);
            }

            var rect = node.Rect;

            if(rect == StackLayout.DummyRect)
            {
                Rect newContentRect = ContentRect;
                newContentRect.Union(rect);
                ContentRect = newContentRect;

                // Apply window position, style(border and padding) and titlebar
                rect.Offset(this.Position.X + this.Style.BorderLeft + this.Style.PaddingLeft, this.Position.Y + this.TitleBarHeight + this.Style.BorderTop + this.Style.PaddingTop);
                rect.Offset(-this.Scroll);
            }

            return rect;
        }

        /// <summary>
        /// Get the rect of a manual-positioned control
        /// </summary>
        public Rect GetRect(Rect rect)
        {
            Rect newContentRect = ContentRect;
            newContentRect.Union(rect);
            ContentRect = newContentRect;

            rect.Offset(this.Position.X, this.Position.Y + this.TitleBarHeight);
            rect.Offset(-this.Scroll);
            return rect;
        }

        /// <summary>
        /// Sets scroll-y paramter
        /// </summary>
        /// <param name="newScrollY">new value</param>
        public void SetWindowScrollY(double newScrollY)
        {
            this.Scroll.Y = newScrollY;
        }

        public void Setup(string name, Point position, Size size, ref bool open, double bg_alpha, WindowFlags flags,
            long current_frame, Window parent_window)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;

            this.Active = true;
            this.BeginCount = 0;
            this.ClipRect = Rect.Big;
            this.LastActiveFrame = current_frame;

            // clear draw list, setup outer clip rect
            this.DrawList.Clear();
            this.DrawList.Init();
            Rect fullScreenRect = new Rect(0, 0, form.ClientSize);
            if (flags.HaveFlag(WindowFlags.ChildWindow) && !flags.HaveFlag(WindowFlags.ComboBox | WindowFlags.Popup))
            {
                this.DrawList.PushClipRect(parent_window.ClipRect, true);
                this.ClipRect = this.DrawList.GetCurrentClipRect();
            }
            else
            {
                this.DrawList.PushClipRect(fullScreenRect, true);
                this.ClipRect = this.DrawList.GetCurrentClipRect();
            }

            // draw outer clip rect
            //this.DrawList.AddRect(this.ClipRect.TopLeft, this.ClipRect.BottomRight, Color.Blue);//test only

            // Collapse window by double-clicking on title bar
            if (!(flags.HaveFlag(WindowFlags.NoTitleBar)) && !(flags.HaveFlag(WindowFlags.NoCollapse)))
            {
                if (w.HoveredWindow == this && g.IsMouseHoveringRect(this.TitleBarRect) &&
                    Mouse.Instance.LeftButtonDoubleClicked)
                {
                    this.Collapsed = !this.Collapsed;
                    w.FocusWindow(this);
                }
            }
            else
            {
                this.Collapsed = false;
            }

            #region size

            this.ApplySize(this.FullSize);
            this.Size = this.Collapsed ? this.TitleBarRect.Size : this.FullSize;

            #endregion

            #region position

            this.Position = new Point((int)this.PosFloat.X, (int)this.PosFloat.Y);
            if (flags.HaveFlag(WindowFlags.ChildWindow))
            {
                this.Position = this.PosFloat = position;
                this.Size = this.FullSize = size; // 'size' provided by user passed via BeginChild()->Begin().
            }

            #endregion

            // Draw window + handle manual resize
            GUIStyle style = this.Style;
            GUIStyle titleBarStyle = this.TitleBarStyle;
            Rect title_bar_rect = this.TitleBarRect;
            float window_rounding = (float)style.Get<double>(GUIStyleName.WindowRounding);
            if (this.Collapsed)
            {
                // Draw title bar only
                this.DrawList.AddRectFilled(title_bar_rect.Min, title_bar_rect.Max, new Color(0.40f, 0.40f, 0.80f, 0.50f));
            }
            else
            {
                Color resize_col = Color.Clear;
                double rezie_size = this.Style.Get<double>(GUIStyleName.ResizeGripSize);
                double resize_corner_size = Math.Max(rezie_size * 1.35, window_rounding + 1.0 + rezie_size * 0.2);
                if (!flags.HaveFlag(WindowFlags.AlwaysAutoResize) && !flags.HaveFlag(WindowFlags.NoResize))
                {
                    // Manual resize
                    var br = this.Rect.BottomRight;
                    Rect resize_rect = new Rect(br - new Vector(resize_corner_size * 0.75f, resize_corner_size * 0.75f), br);
                    int resize_id = this.GetID("#RESIZE");
                    bool hovered, held;
                    GUIBehavior.ButtonBehavior(resize_rect, resize_id, out hovered, out held, ButtonFlags.FlattenChilds);
                    resize_col =
                        held
                            ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Active)
                            : hovered
                                ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Hover)
                                : style.Get<Color>(GUIStyleName.ResizeGripColor);

                    if (hovered || held)
                    {
                        //Mouse.Instance.Cursor = Cursor.NeswResize;
                    }

                    if (held)
                    {
                        // We don't use an incremental MouseDelta but rather compute an absolute target size based on mouse position
                        var t = Mouse.Instance.Position - g.ActiveIdClickOffset - this.Position;
                        var new_size_width = t.X + resize_rect.Width;
                        var new_size_height = t.Y + resize_rect.Height;
                        new_size_width =
                            MathEx.Clamp(new_size_width, 330, fullScreenRect.Width); //min size of a window is 145x235
                        new_size_height = MathEx.Clamp(new_size_height, 150, fullScreenRect.Height);
                        Size resize_size = new Size(new_size_width, new_size_height);
                        this.ApplySize(resize_size);

                        // adjust scroll parameters
                        var contentSize = this.ContentRect.Size;
                        if (contentSize != Size.Zero)
                        {
                            var vH = this.Rect.Height - this.TitleBarHeight - this.Style.BorderVertical -
                                     this.Style.PaddingVertical;
                            var cH = contentSize.Height;
                            if (cH > vH)
                            {
                                var oldScrollY = this.Scroll.Y;
                                oldScrollY = MathEx.Clamp(oldScrollY, 0, cH - vH);
                                this.Scroll.Y = oldScrollY;
                            }
                        }
                    }

                    this.Size = this.FullSize;
                    title_bar_rect = this.TitleBarRect;
                }


                // Window background
                Color bg_color = style.BackgroundColor;
                if (bg_alpha >= 0.0f)
                    bg_color.A = bg_alpha;
                if (bg_color.A > 0.0f)
                    this.DrawList.AddRectFilled(this.Position + new Vector(0, this.TitleBarHeight),
                        this.Rect.BottomRight, bg_color, window_rounding,
                        flags.HaveFlag(WindowFlags.NoTitleBar) ? 15 : 4 | 8);

                // Title bar
                if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                {
                    this.DrawList.AddRectFilled(title_bar_rect.TopLeft, title_bar_rect.BottomRight,
                        w.FocusedWindow == this
                            ? titleBarStyle.Get<Color>(GUIStyleName.BackgroundColor, GUIState.Active)
                            : titleBarStyle.Get<Color>(GUIStyleName.BackgroundColor), window_rounding, 1 | 2);
                }

                // Render resize grip
                // (after the input handling so we don't have a frame of latency)
                if (!flags.HaveFlag(WindowFlags.NoResize))
                {
                    Point br = this.Rect.BottomRight;
                    var borderBottom = this.Style.BorderBottom;
                    var borderRight = this.Style.BorderRight;
                    this.DrawList.PathLineTo(br + new Vector(-resize_corner_size, -borderBottom));
                    this.DrawList.PathLineTo(br + new Vector(-borderRight, -resize_corner_size));
                    this.DrawList.PathArcToFast(
                        new Point(br.X - window_rounding - borderRight, br.Y - window_rounding - borderBottom), window_rounding,
                        0, 3);
                    this.DrawList.PathFill(resize_col);
                }

                // Scroll bar
                if (flags.HaveFlag(WindowFlags.VerticalScrollbar))
                {
                    //get content size without clip
                    var contentPosition = this.ContentRect.TopLeft;
                    var contentSize = this.ContentRect.Size;
                    if (contentSize != Size.Zero)
                    {
                        int id = this.GetID("#SCROLLY");

                        double scrollBarWidth = this.Style.Get<double>(GUIStyleName.ScrollBarWidth);
                        Point scroll_TopLeft = new Point(
                            this.Rect.Right - scrollBarWidth - this.Style.BorderRight - this.Style.PaddingRight,
                            this.Rect.Top + this.TitleBarHeight + this.Style.BorderTop + this.Style.PaddingTop);
                        var sH = this.Rect.Height - this.TitleBarHeight - this.Style.BorderVertical -
                                 this.Style.PaddingVertical
                                 + (flags.HaveFlag(WindowFlags.NoResize) ? 0 : -resize_corner_size);
                        var vH = this.Rect.Height - this.TitleBarHeight - this.Style.BorderVertical -
                                 this.Style.PaddingVertical;
                        Point scroll_BottomRight = scroll_TopLeft + new Vector(scrollBarWidth, sH);
                        Rect bgRect = new Rect(scroll_TopLeft, scroll_BottomRight);

                        var cH = contentSize.Height;
                        var top = this.Scroll.Y * sH / cH;
                        var height = sH * vH / cH;

                        if (height < sH)
                        {
                            // handle mouse click/drag
                            bool held = false;
                            bool hovered = false;
                            bool previously_held = (g.ActiveId == id);
                            GUIBehavior.ButtonBehavior(bgRect, id, out hovered, out held);
                            if (held)
                            {
                                top = Mouse.Instance.Position.Y - bgRect.Y - 0.5 * height;
                                top = MathEx.Clamp(top, 0, sH - height);
                                var targetScrollY = top * cH / sH;
                                this.SetWindowScrollY(targetScrollY);
                            }

                            Point scrollButton_TopLeft = scroll_TopLeft + new Vector(0, top);
                            Point scrllButton_BottomRight = scrollButton_TopLeft + new Vector(scrollBarWidth, height);
                            Rect buttonRect = new Rect(scrollButton_TopLeft, scrllButton_BottomRight);

                            //Draw vertical scroll bar and button
                            {
                                var bgColor = this.Style.Get<Color>(GUIStyleName.ScrollBarBackgroundColor);
                                var buttonColor = this.Style.Get<Color>(GUIStyleName.ScrollBarButtonColor,
                                    held ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal);
                                this.DrawList.AddRectFilled(bgRect.TopLeft, buttonRect.TopRight, bgColor);
                                this.DrawList.AddRectFilled(buttonRect.TopLeft, buttonRect.BottomRight, buttonColor);
                                this.DrawList.AddRectFilled(buttonRect.BottomLeft, bgRect.BottomRight, bgColor);
                            }
                        }
                        else
                        {
                            var bgColor = this.Style.Get<Color>(GUIStyleName.ScrollBarBackgroundColor);
                            this.DrawList.AddRectFilled(bgRect.TopLeft, bgRect.BottomRight, bgColor);
                        }
                    }
                }
                this.ContentRect = Rect.Zero;
            }

            // draw title bar text
            if (!flags.HaveFlag(WindowFlags.NoTitleBar))
            {
                // title text
                var state = w.FocusedWindow == this ? GUIState.Active : GUIState.Normal;
                this.DrawList.DrawBoxModel(title_bar_rect, name, titleBarStyle, state);

                // close button
                if (CloseButton(this.GetID("#CLOSE"), new Rect(title_bar_rect.TopRight + new Vector(-45, 0), title_bar_rect.BottomRight)))
                {
                    open = false;
                }
            }

            // Borders
            if (flags.HaveFlag(WindowFlags.ShowBorders))
            {
                var state = w.FocusedWindow == this ? GUIState.Active : GUIState.Normal;
                // window border
                var borderColor = this.Style.Get<Color>(GUIStyleName.WindowBorderColor, state);
                this.DrawList.AddRect(this.Position, this.Position + new Vector(this.Size.Width, this.Size.Height),
                    borderColor, window_rounding);
                // window shadow
#if false
                    {
                        var state = w.FocusedWindow == this ? GUIState.Active : GUIState.Normal;
                        var shadowColor = this.Style.Get<Color>(GUIStyleName.WindowShadowColor, state);
                        var shadowWidth = this.Style.Get<double>(GUIStyleName.WindowShadowWidth, state);
                        var d = this.DrawList;

                        //top-left corner

                        d.AddRectFilledGradientTopLeftToBottomRight(this.Rect.TopLeft + new Vector(-shadowWidth, -shadowWidth), this.Rect.TopLeft, Color.Clear, shadowColor);
                        //top
                        d.AddRectFilledGradient(this.Rect.TopLeft + new Vector(0, -shadowWidth), this.Rect.TopRight, Color.Clear, shadowColor);
                        d.AddRectFilledGradient(this.Rect.BottomLeft, this.Rect.BottomRight + new Vector(0, shadowWidth), shadowColor, Color.Clear);
                    }
#endif
            }

            // Save clipped aabb so we can access it in constant-time in FindHoveredWindow()
            this.WindowClippedRect = this.Rect;
            this.WindowClippedRect.Intersect(this.ClipRect);
        }

        bool CloseButton(int id, Rect rect)
        {
            Window window = GUI.GetCurrentWindow();

            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out bool hovered, out bool held);

            GUIStyle style = GUIStyle.Basic;
            style.Save();
            style.ApplySkin(GUIControlName.Button);
            style.PushBgColor(Color.White, GUIState.Normal);
            style.PushBgColor(Color.Rgb(232, 17, 35), GUIState.Hover);
            style.PushBgColor(Color.Rgb(241, 112, 122), GUIState.Active);

            // Render
            var d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            var color = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.AddRectFilled(rect, color);

            Point center = rect.Center;
            float cross_extent = (15 * 0.7071f) - 1.0f;
            var fontColor = style.Get<Color>(GUIStyleName.FontColor, state);
            d.AddLine(center + new Vector(+cross_extent, +cross_extent), center + new Vector(-cross_extent, -cross_extent), fontColor);
            d.AddLine(center + new Vector(+cross_extent, -cross_extent), center + new Vector(-cross_extent, +cross_extent), fontColor);

            style.Restore();

            return pressed;
        }
    }
}
