using System;
using System.Collections.Generic;
using ImGui.Layout;
using System.Diagnostics;
using System.Threading;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
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
        /// Position
        /// </summary>
        /// <remarks>Top-left point relative to the form.</remarks>
        public Point Position
        {
            get => this.RenderTree.Root.Rect.Location;
            set
            {
                var rect = this.RenderTree.Root.Rect;
                rect.Location = value;
                this.RenderTree.Root.Rect = rect;
            }
        }

        /// <summary>
        /// Size
        /// </summary>
        public Size Size => this.Collapsed ? this.TitleBarRect.Size : this.FullSize;

        /// <summary>
        /// Size when the window is not collapsed.
        /// </summary>
        public Size FullSize
        {
            get => this.RenderTree.Root.Rect.Size;
            set
            {
                var rect = this.RenderTree.Root.Rect;
                rect.Size = value;
                this.RenderTree.Root.Rect = rect;
            }
        }

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

        /// <summary>
        /// Render tree
        /// </summary>
        public RenderTree RenderTree;

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


        #region Window original sub nodes

        private Node titleBarNode;
        private Node frameNode;

        public Node FrameNode => this.frameNode;

        #endregion

        public Window(string name, Point position, Size size, WindowFlags Flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;

            this.ID = name.GetHashCode();
            this.Name = name;
            this.IDStack.Push(this.ID);
            this.Flags = Flags;

            this.RenderTree = new RenderTree(this.ID, position, size);

            this.DrawList = new DrawList();//DUMMY

            this.MoveID = this.GetID("#MOVE");
            this.Active = this.WasActive = false;

            #region Window styles
            // window title bar styles
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

            // window frame styles
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
                style.Set(GUIStyleName.WindowBorderColor, Color.Rgb(255, 0, 0), GUIState.Normal);
                style.Set(GUIStyleName.WindowBorderColor, Color.Rgb(0, 0, 255), GUIState.Active);
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
            #endregion

            #region Window nodes
            //title bar node
            {
                var id = this.GetID("TitleBar");
                var node = new Node(id, "TitleBar");
                this.titleBarNode = node;
            }
            this.IDStack.Push(this.titleBarNode.Id);
            {
                var id = this.GetID("TitleBar_Background");
                Node node = new Node(id, "TitleBar_Background");
                node.IsFill = true;
                var primitive = new PathPrimitive();
                primitive.PathRect(this.TitleBarRect);
                var brush = new Brush();
                brush.FillColor = this.TitleBarStyle.BackgroundColor;
                node.Primitive = primitive;
                node.Brush = brush;
                this.titleBarNode.Add(node);
            }
            {
                var id = this.GetID("TitleBar_Text");
                var node = new Node(id, "TitleBar_Text");
                node.Rect = this.TitleBarRect;
                var primitive = new TextPrimitive();
                primitive.Text = this.Name;
                node.Primitive = primitive;
                this.titleBarNode.Add(node);
            }
            this.IDStack.Pop();
            this.RenderTree.Root.Add(this.titleBarNode);

            //Window frame node
            {
                var id = this.GetID("Frame");
                var node = new Node(id, "Frame");
                this.frameNode = node;
            }
            this.IDStack.Push(this.frameNode.Id);
            //background
            {
                var id = this.GetID("Frame_Background");
                var node = new Node(id, "Frame_Background");
                node.IsFill = true;
                var primitive = new PathPrimitive();
                primitive.PathRect(this.Position + new Vector(0, this.TitleBarHeight),
                    this.Rect.BottomRight);
                var brush = new Brush();
                brush.FillColor = this.Style.BackgroundColor;
                node.Primitive = primitive;
                node.Brush = brush;
                this.frameNode.Add(node);
            }
            //border
            {
                var id = this.GetID("Frame_Border");
                var node = new Node(id, "Frame_Border");
                var primitive = new PathPrimitive();
                primitive.PathRect(this.Position + new Vector(0, this.TitleBarHeight), this.Rect.BottomRight);
                var strokeStyle = new StrokeStyle();
                node.IsFill = false;
                node.Primitive = primitive;
                node.StrokeStyle = strokeStyle;
                this.frameNode.Add(node);
            }
            this.IDStack.Pop();
            this.RenderTree.Root.Add(this.frameNode);

            this.titleBarNode.ActiveSelf = true;
            this.frameNode.ActiveSelf = !this.Collapsed;
            #endregion
        }

        public void FirstUpdate(string name, Point position, Size size, ref bool open, double backgroundAlpha,
            WindowFlags flags,
            long currentFrame, Window parentWindow)
        {
            //short names
            var form = Form.current;
            var g = form.uiContext;
            var w = g.WindowManager;

            this.Active = true;
            this.BeginCount = 0;
            this.ClipRect = Rect.Big;
            this.LastActiveFrame = currentFrame;

            var fullScreenRect = new Rect(0, 0, form.ClientSize);
            if (flags.HaveFlag(WindowFlags.ChildWindow) && !flags.HaveFlag(WindowFlags.ComboBox | WindowFlags.Popup))
            {
                //PushClipRect(parentWindow.ClipRect, true);
                //ClipRect = GetCurrentClipRect();
            }
            else
            {
                //PushClipRect(fullScreenRect, true);
                //ClipRect = GetCurrentClipRect();
            }

            // (draw outer clip rect for test only here)

            // determine if window is collapsed
            if (!flags.HaveFlag(WindowFlags.NoTitleBar) && !flags.HaveFlag(WindowFlags.NoCollapse))
            {
                // Collapse window by double-clicking on title bar
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

            // set window size and position
            #region size

            this.ApplySize(size);

            #endregion

            #region position

            if (flags.HaveFlag(WindowFlags.ChildWindow))
            {
                // 'size' provided by user passed via BeginChild()->Begin().
                this.RenderTree.Root.Rect = new Rect(position, size);
            }

            #endregion

            //update title bar
            var titleBarStyle = this.TitleBarStyle;
            var titleBarRect = this.TitleBarRect;
            var windowRounding = (float) this.Style.Get<double>(GUIStyleName.WindowRounding);
            if (!flags.HaveFlag(WindowFlags.NoTitleBar))
            {
                //background
                {
                    var node = this.titleBarNode.GetNodeByName("TitleBar_Background");
                    var brush = node.Brush;
                    brush.FillColor = w.FocusedWindow == this
                        ? titleBarStyle.Get<Color>(GUIStyleName.BackgroundColor, GUIState.Active)
                        : titleBarStyle.Get<Color>(GUIStyleName.BackgroundColor);

                    var primitive = (PathPrimitive)node.Primitive;
                    Debug.Assert(primitive != null);
                    primitive.PathClear();
                    primitive.PathRect(titleBarRect);
                }

                //text
                {
                    var node = this.titleBarNode.GetNodeByName("TitleBar_Text");
                    node.Rect = titleBarRect;
                    // title text
                    var state = w.FocusedWindow == this ? GUIState.Active : GUIState.Normal;
                    var textPrimitive = (TextPrimitive)node.Primitive;
                    if (textPrimitive.Text != this.Name)
                    {
                        textPrimitive.Text = this.Name;
                    }
                }

                //close button
                if (this.CloseButton(this.GetID("#CLOSE"),
                    new Rect(titleBarRect.TopRight + new Vector(-45, 0), titleBarRect.BottomRight)))
                {
                    open = false;
                }
            }

            this.frameNode.ActiveSelf = !this.Collapsed;
            if (this.Collapsed)
            {
                //TODO need to do something here?

            }
            else//show and update window frame
            {
                //resize grip
                var resizeGripColor = Color.Clear;
                var resizeGripSize = this.Style.Get<double>(GUIStyleName.ResizeGripSize);
                var resizeCornerSize = Math.Max(resizeGripSize * 1.35, windowRounding + 1.0 + resizeGripSize * 0.2);
                if (!flags.HaveFlag(WindowFlags.AlwaysAutoResize) && !flags.HaveFlag(WindowFlags.NoResize))
                {
                    // Manual resize
                    var br = this.Rect.BottomRight;
                    var resizeRect = new Rect(br - new Vector(resizeCornerSize * 0.75f, resizeCornerSize * 0.75f),
                        br);
                    var resizeId = this.GetID("#RESIZE");
                    GUIBehavior.ButtonBehavior(resizeRect, resizeId, out var hovered, out var held,
                        ButtonFlags.FlattenChilds);
                    resizeGripColor =
                        held
                            ? this.Style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Active)
                            : hovered
                                ? this.Style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Hover)
                                : this.Style.Get<Color>(GUIStyleName.ResizeGripColor);

                    if (hovered || held)
                    {
                        //Mouse.Instance.Cursor = Cursor.NeswResize;
                    }

                    if (held)
                    {
                        // We don't use an incremental MouseDelta but rather compute an absolute target size based on mouse position
                        var t = Mouse.Instance.Position - g.ActiveIdClickOffset - this.Position;
                        var newSizeWidth = t.X + resizeRect.Width;
                        var newSizeHeight = t.Y + resizeRect.Height;
                        newSizeWidth =
                            MathEx.Clamp(newSizeWidth, 330, fullScreenRect.Width); //min size of a window is 145x235
                        newSizeHeight = MathEx.Clamp(newSizeHeight, 150, fullScreenRect.Height);
                        var resizeSize = new Size(newSizeWidth, newSizeHeight);
                        this.ApplySize(resizeSize);

                        // adjust scroll parameters
                        var contentSize = this.ContentRect.Size;
                        if (contentSize != Size.Zero)
                        {
                            var vH = this.Rect.Height - this.TitleBarHeight - this.Style.BorderVertical - this.Style.PaddingVertical;
                            var cH = contentSize.Height;
                            if (cH > vH)
                            {
                                var oldScrollY = this.Scroll.Y;
                                oldScrollY = MathEx.Clamp(oldScrollY, 0, cH - vH);
                                this.Scroll.Y = oldScrollY;
                            }
                        }
                    }
                }

                //frame backgound
                var backgroundColor = this.Style.BackgroundColor;
                backgroundColor.A = backgroundAlpha;
                if (backgroundColor.A > 0.0f)
                {
                    var node = this.frameNode.GetNodeByName("Frame_Background");
                    node.Brush.FillColor = backgroundColor;
                    var primitive = (PathPrimitive)node.Primitive;
                    Debug.Assert(primitive != null);
                    primitive.PathClear();
                    primitive.PathRect(this.Position + new Vector(0, this.TitleBarHeight),
                        this.Rect.BottomRight);
                }
                //frame border
                {
                    var state = w.FocusedWindow == this ? GUIState.Active : GUIState.Normal;
                    var node = this.frameNode.GetNodeByName("Frame_Border");
                    node.Brush.LineColor = this.Style.Get<Color>(GUIStyleName.WindowBorderColor, state);
                    var pathPrimitive = (PathPrimitive)node.Primitive;
                    pathPrimitive.PathClear();
                    pathPrimitive.PathRect(this.Position, this.Position + (Vector)this.Size);
                }

                // Render resize grip
                // (after the input handling so we don't have a frame of latency)
                if (!flags.HaveFlag(WindowFlags.NoResize))
                {
                    var br = this.Rect.BottomRight;
                    var borderBottom = this.Style.BorderBottom;
                    var borderRight = this.Style.BorderRight;
                    //DrawList.PathLineTo(br + new Vector(-resizeCornerSize, -borderBottom));
                    //DrawList.PathLineTo(br + new Vector(-borderRight, -resizeCornerSize));
                    //DrawList.PathArcToFast(
                    //    new Point(br.X - windowRounding - borderRight, br.Y - windowRounding - borderBottom),
                    //    windowRounding,
                    //    0, 3);
                    //DrawList.PathFill(resizeGripColor);
                }

                // Scroll bar
                if (flags.HaveFlag(WindowFlags.VerticalScrollbar))
                {
                    //get content size without clip
                    var contentPosition = this.ContentRect.TopLeft;
                    var contentSize = this.ContentRect.Size;
                    if (contentSize != Size.Zero)
                    {
                        var id = this.GetID("#SCROLLY");

                        var scrollBarWidth = this.Style.Get<double>(GUIStyleName.ScrollBarWidth);
                        var scrollTopLeft = new Point(this.Rect.Right - scrollBarWidth - this.Style.BorderRight - this.Style.PaddingRight, this.Rect.Top + this.TitleBarHeight + this.Style.BorderTop + this.Style.PaddingTop);
                        var sH = this.Rect.Height - this.TitleBarHeight - this.Style.BorderVertical - this.Style.PaddingVertical
                                 + (flags.HaveFlag(WindowFlags.NoResize) ? 0 : -resizeCornerSize);
                        var vH = this.Rect.Height - this.TitleBarHeight - this.Style.BorderVertical - this.Style.PaddingVertical;
                        var scrollBottomRight = scrollTopLeft + new Vector(scrollBarWidth, sH);
                        var bgRect = new Rect(scrollTopLeft, scrollBottomRight);

                        var cH = contentSize.Height;
                        var top = this.Scroll.Y * sH / cH;
                        var height = sH * vH / cH;

                        if (height < sH)
                        {
                            // handle mouse click/drag
                            var held = false;
                            var hovered = false;
                            var previouslyHeld = g.ActiveId == id;
                            GUIBehavior.ButtonBehavior(bgRect, id, out hovered, out held);
                            if (held)
                            {
                                top = Mouse.Instance.Position.Y - bgRect.Y - 0.5 * height;
                                top = MathEx.Clamp(top, 0, sH - height);
                                var targetScrollY = top * cH / sH;
                                this.SetWindowScrollY(targetScrollY);
                            }

                            var scrollButtonTopLeft = scrollTopLeft + new Vector(0, top);
                            var scrllButtonBottomRight = scrollButtonTopLeft + new Vector(scrollBarWidth, height);
                            var buttonRect = new Rect(scrollButtonTopLeft, scrllButtonBottomRight);

                            //Draw vertical scroll bar and button
                            {
                                var bgColor = this.Style.Get<Color>(GUIStyleName.ScrollBarBackgroundColor);
                                var buttonColor = this.Style.Get<Color>(GUIStyleName.ScrollBarButtonColor,
                                    held ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal);
                                //DrawList.AddRectFilled(bgRect.TopLeft, buttonRect.TopRight, bgColor);
                                //DrawList.AddRectFilled(buttonRect.TopLeft, buttonRect.BottomRight, buttonColor);
                                //DrawList.AddRectFilled(buttonRect.BottomLeft, bgRect.BottomRight, bgColor);
                            }
                        }
                        else
                        {
                            var bgColor = this.Style.Get<Color>(GUIStyleName.ScrollBarBackgroundColor);
                            //DrawList.AddRectFilled(bgRect.TopLeft, bgRect.BottomRight, bgColor);
                        }
                    }
                }

                this.ContentRect = Rect.Zero;
            }

            // Save clipped aabb so we can access it in constant-time in FindHoveredWindow()
            this.WindowClippedRect = this.Rect;
            this.WindowClippedRect.Intersect(this.ClipRect);
        }

        /// <summary>
        /// Gets the rect of this window
        /// </summary>
        public Rect Rect => new Rect(this.Position, this.Size);

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

                return this.TitleBarStyle.PaddingVertical + 30;
            }
        }

        /// <summary>
        /// Gets the rect of the title bar
        /// </summary>
        public Rect TitleBarRect => new Rect(this.Position, this.FullSize.Width, this.TitleBarHeight);

        /// <summary>
        /// Gets or sets the rect of the client area
        /// </summary>
        /// //TODO consider scroll bar, which is not a part of the client area.
        public Rect ClientRect => new Rect(
            x: this.Position.X + this.Style.BorderLeft + this.Style.PaddingLeft,
            y: this.Position.Y + this.Style.BorderTop + this.Style.PaddingTop + this.TitleBarHeight +
               this.Style.PaddingTop,
            width: this.FullSize.Width - this.Style.BorderHorizontal - this.Style.PaddingHorizontal,
            height: this.FullSize.Height - this.Style.BorderVertical - this.Style.PaddingVertical -
                    this.TitleBarHeight - this.Style.PaddingTop);

        /// <summary>
        /// Gets or sets if the window is collapsed.
        /// </summary>
        public bool Collapsed { get; set; } = true;//FIXME TEMP collapsed

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
            int seed = this.IDStack.Peek();
            var id = this.Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        public int GetID(string str_id)
        {
            int seed = this.IDStack.Peek();
            int int_id = str_id.GetHashCode();
            var id = this.Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);

            return id;
        }

        public int GetID(ITexture texture)
        {
            int seed = this.IDStack.Peek();
            int int_id = texture.GetHashCode();
            var id = this.Hash(seed, int_id);

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
            if (this.FullSize == new_size)
            {
                return;
            }

            this.FullSize = new_size;
        }

        /// <summary>
        /// Get the rect for an automatic-layout control
        /// </summary>
        /// <param name="id">id of the control</param>
        /// <param name="style">style that will apply to requested rect</param>
        /// <returns></returns>
        public Rect GetRect(int id)
        {
            var node = this.RenderTree.GetNodeById(id);
            if(node == null)
            {
                throw new ArgumentException($"Cannot find node with id<{id}>", nameof(id));
            }

            var rect = node.Rect;

            //SIDE EFFECT TODO move this to other places
            // calculate the content rect fro this window
            Rect newContentRect = this.ContentRect;
            newContentRect.Union(rect);
            this.ContentRect = newContentRect;

            // apply window client area offset
            rect.Offset((Vector)this.ClientRect.TopLeft);
            // apply scroll offset
            rect.Offset(-this.Scroll);

            return rect;
        }

        /// <summary>
        /// Get the rect of a manual-positioned control
        /// </summary>
        public Rect GetRect(Rect rect)
        {
            Rect newContentRect = this.ContentRect;
            newContentRect.Union(rect);
            this.ContentRect = newContentRect;

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
            //d.AddRectFilled(rect, color);

            Point center = rect.Center;
            //float cross_extent = (15 * 0.7071f) - 1.0f;
            var fontColor = style.Get<Color>(GUIStyleName.FontColor, state);
            //d.AddLine(center + new Vector(+cross_extent, +cross_extent), center + new Vector(-cross_extent, -cross_extent), fontColor);
            //d.AddLine(center + new Vector(+cross_extent, -cross_extent), center + new Vector(-cross_extent, +cross_extent), fontColor);

            style.Restore();

            return pressed;
        }
    }
}
