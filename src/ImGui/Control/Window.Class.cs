using System;
using System.Collections.Generic;
using ImGui.Layout;
using System.Diagnostics;
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
        public Point Position { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public Size Size => this.Collapsed ? this.TitleBarRect.Size : this.FullSize;

        /// <summary>
        /// Size when the window is not collapsed.
        /// </summary>
        public Size FullSize { get; set; }

        /// <summary>
        /// Window flags. See <see cref="WindowFlags"/>.
        /// </summary>
        public WindowFlags Flags;

        /// <summary>
        /// Draw list
        /// </summary>
        public DrawList DrawList;

        /// <summary>
        /// Root of node tree of plain nodes
        /// </summary>
        public Node NodeTreeRoot;

        /// <summary>
        /// Render tree of layout-ed nodes
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

        public MeshList MeshList { get; set; } = new MeshList();

        public MeshBuffer MeshBuffer { get; set; } = new MeshBuffer();

        #region Window original sub nodes
        private Node titleBarNode { get; }
        private Node titleBarTitleNode { get; }
        public Node ClientAreaNode { get; }
        internal Node WindowContainer { get; }
        private Node ResizeGripNode { get; set; }

        #endregion

        public Window(string name, Point position, Size size, WindowFlags Flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;

            this.ID = name.GetHashCode();
            this.Name = name;
            this.Active = this.WasActive = false;
            this.Position = position;
            this.FullSize = size;

            this.Flags = Flags;

            this.NodeTreeRoot = new Node(this.ID, "root");
            this.NodeTreeRoot.Children = new List<Node>();
            this.RenderTree = new RenderTree(this.ID, position, size);

            this.DrawList = new DrawList();//DUMMY

            this.IDStack.Push(this.ID);
            this.MoveID = this.GetID("#MOVE");

            #region Window nodes

            {
                var windowContainer = new Node(this.GetID("window"),"window");
                this.WindowContainer = windowContainer;

                var style = windowContainer.RuleSet;
                style.BackgroundColor = Color.White;
                style.Border = (1, 1, 1, 1);
                style.BorderColor = (Color.Black, Color.Black, Color.Black, Color.Black);
                style.Set(GUIStyleName.BorderTopColor, Color.Blue, GUIState.Active);
                style.Set(GUIStyleName.BorderRightColor, Color.Blue, GUIState.Active);
                style.Set(GUIStyleName.BorderBottomColor, Color.Blue, GUIState.Active);
                style.Set(GUIStyleName.BorderLeftColor, Color.Blue, GUIState.Active);
                style.Set(GUIStyleName.BorderTopColor, Color.Gray, GUIState.Hover);
                style.Set(GUIStyleName.BorderRightColor, Color.Gray, GUIState.Hover);
                style.Set(GUIStyleName.BorderBottomColor, Color.Gray, GUIState.Hover);
                style.Set(GUIStyleName.BorderLeftColor, Color.Gray, GUIState.Hover);
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
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(75, 102, 102, 102));
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(150, 102, 102, 102), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(225, 102, 102, 102), GUIState.Active);
                style.Set(GUIStyleName.WindowRounding, 20.0);
                style.Set(GUIStyleName.ScrollBarWidth, CurrentOS.IsDesktopPlatform ? 10.0 : 20.0);
                style.Set(GUIStyleName.ScrollBarBackgroundColor, Color.Rgb(240));
                style.Set(GUIStyleName.ScrollBarButtonColor, Color.Rgb(205), GUIState.Normal);
                style.Set(GUIStyleName.ScrollBarButtonColor, Color.Rgb(166), GUIState.Hover);
                style.Set(GUIStyleName.ScrollBarButtonColor, Color.Rgb(96), GUIState.Active);

                windowContainer.AttachLayoutGroup(true);
                windowContainer.UseBoxModel = true;
                var windowStyleOptions = GUILayout.Width(this.FullSize.Width).Height(
                    this.Collapsed ? this.CollapsedHeight : this.FullSize.Height
                    );
                windowContainer.RuleSet.ApplyOptions(windowStyleOptions);

                this.RenderTree.Root.AppendChild(windowContainer);
            }

            //title bar
            {
                var titleBarContainer = new Node(this.GetID("titleBar"),"title bar");
                this.titleBarNode = titleBarContainer;
                titleBarContainer.AttachLayoutGroup(false);
                titleBarContainer.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(this.TitleBarHeight));
                titleBarContainer.UseBoxModel = true;
                StyleRuleSetBuilder b = new StyleRuleSetBuilder(titleBarContainer);
                b.Padding((top: 8, right: 8, bottom: 8, left: 8))
                    .FontColor(Color.Black)
                    .FontSize(12)
                    .BackgroundColor(Color.White)
                    .AlignmentVertical(Alignment.Center)
                    .AlignmentHorizontal(Alignment.Start);

                var icon = new Node(this.GetID("icon"),"icon");
                icon.AttachLayoutEntry(new Size(20, 20));
                icon.RuleSet.ApplyOptions(GUILayout.Width(20).Height(20));
                icon.UseBoxModel = false;
                icon.Primitive = new ImagePrimitive(@"assets\images\logo.png");

                var title = new Node(this.GetID("title"),"title");
                title.AttachLayoutEntry(Size.Zero);
                title.RuleSet.ApplyOptions(GUILayout.Height(20));
                title.UseBoxModel = false;
                title.Primitive = new TextPrimitive(this.Name);
                this.titleBarTitleNode = title;

                var closeButton = new Node(this.GetID("close button"),"close button");
                closeButton.AttachLayoutEntry(new Size(20, 20));
                closeButton.RuleSet.ApplyOptions(GUILayout.Width(20).Height(20));
                closeButton.UseBoxModel = false;
                PathPrimitive path = new PathPrimitive();
                path.PathRect(new Rect(0, 0, 20, 20));
                closeButton.Primitive = path;

                titleBarContainer.AppendChild(icon);
                titleBarContainer.AppendChild(title);
                //titleBarContainer.AppendChild(closeButton);
                this.WindowContainer.AppendChild(titleBarContainer);
            }

            //client area
            {
                var node = new Node(this.GetID("client area"),"client area");
                node.AttachLayoutGroup(true);
                node.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                node.UseBoxModel = true;
                this.ClientAreaNode = node;
                this.WindowContainer.AppendChild(node);
            }

            //resize grip (lasy-initialized)

            this.ShowWindowTitleBar(true);
            this.ShowWindowClientArea(!this.Collapsed);
            #endregion
        }

        public void ShowWindowTitleBar(bool isShow)
        {
            this.titleBarNode.ActiveSelf = isShow;
            this.titleBarNode.ActiveSelf = isShow;
        }

        public void ShowWindowClientArea(bool isShow)
        {
            this.ClientAreaNode.ActiveSelf = isShow;
            if (!isShow)
            {
                this.WindowContainer.RuleSet.ApplyOptions(GUILayout.Height(this.CollapsedHeight));
            }
            else
            {
                this.WindowContainer.RuleSet.ApplyOptions(GUILayout.Height(this.FullSize.Height));
            }
        }

        public void FirstUpdate(string name, Size size, ref bool open, double backgroundAlpha,
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

            //update title bar
            var titleBarRect = this.TitleBarRect;
            var windowRounding = (float) this.WindowContainer.RuleSet.Get<double>(GUIStyleName.WindowRounding);
            if (!flags.HaveFlag(WindowFlags.NoTitleBar))
            {
                //text
                {
                    // title text
                    var textPrimitive = (TextPrimitive)this.titleBarTitleNode.Primitive;
                    if (textPrimitive.Text != this.Name)
                    {
                        textPrimitive.Text = this.Name;
                    }
                }
            }

            this.ShowWindowClientArea(!this.Collapsed);

            if (this.Collapsed)
            {
                //TODO need to do something here?
            }
            else//show and update window client area
            {
                if (!flags.HaveFlag(WindowFlags.NoResize) && this.ResizeGripNode == null)
                {
                    var id = this.GetID("#RESIZE");
                    var node = new Node(id, "Window_ResizeGrip");
                    node.Primitive = new PathPrimitive();
                    this.ResizeGripNode = node;
                    this.NodeTreeRoot.AppendChild(node);
                }
                //resize grip
                var resizeGripColor = Color.Clear;
                if (!flags.HaveFlag(WindowFlags.AlwaysAutoResize) && !flags.HaveFlag(WindowFlags.NoResize))
                {
                    // Manual resize
                    var br = this.Rect.BottomRight;
                    var resizeRect = new Rect(br - new Vector(windowRounding, windowRounding), br);
                    var resizeId = this.GetID("#RESIZE");
                    GUIBehavior.ButtonBehavior(resizeRect, resizeId, out var hovered, out var held,
                        ButtonFlags.FlattenChilds);
                    resizeGripColor =
                        held
                            ? this.WindowContainer.RuleSet.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Active)
                            : hovered
                                ? this.WindowContainer.RuleSet.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Hover)
                                : this.WindowContainer.RuleSet.Get<Color>(GUIStyleName.ResizeGripColor);

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
                        var resizeSize = new Size(newSizeWidth, newSizeHeight);
                        this.ApplySize(resizeSize);

                        // adjust scroll parameters
                        var contentSize = this.ContentRect.Size;
                        if (contentSize != Size.Zero)
                        {
                            var vH = this.Rect.Height - this.TitleBarHeight - this.WindowContainer.RuleSet.BorderVertical - this.WindowContainer.RuleSet.PaddingVertical;
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

                // Render resize grip
                // (after the input handling so we don't have a frame of latency)
                if (!flags.HaveFlag(WindowFlags.NoResize))
                {
                    var br = this.Rect.BottomRight;
                    var borderBottom = this.WindowContainer.RuleSet.BorderBottom;
                    var borderRight = this.WindowContainer.RuleSet.BorderRight;
                    var primitive = (PathPrimitive)this.ResizeGripNode.Primitive;
                    primitive.PathClear();
                    primitive.PathLineTo(br + new Vector(-borderRight, -borderBottom));
                    primitive.PathLineTo(br + new Vector(-borderRight, -windowRounding));
                    primitive.PathArcFast(br + new Vector(-windowRounding - borderRight, -windowRounding - borderBottom), windowRounding, 0, 3);
                    primitive.PathFill(resizeGripColor);
                }

                this.ContentRect = Rect.Zero;
            }

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

                return 40;
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
            x: this.Position.X + this.WindowContainer.RuleSet.BorderLeft + this.WindowContainer.RuleSet.PaddingLeft,
            y: this.Position.Y + this.WindowContainer.RuleSet.BorderTop + this.WindowContainer.RuleSet.PaddingTop + this.TitleBarHeight +
               this.WindowContainer.RuleSet.PaddingTop,
            width: this.FullSize.Width - this.WindowContainer.RuleSet.BorderHorizontal - this.WindowContainer.RuleSet.PaddingHorizontal,
            height: this.FullSize.Height - this.WindowContainer.RuleSet.BorderVertical - this.WindowContainer.RuleSet.PaddingVertical -
                    this.TitleBarHeight - this.WindowContainer.RuleSet.PaddingTop);

        /// <summary>
        /// Gets or sets if the window is collapsed.
        /// </summary>
        public bool Collapsed { get; set; } = true;

        public double CollapsedHeight => this.TitleBarHeight
            + this.WindowContainer.RuleSet.BorderVertical
            + this.WindowContainer.RuleSet.PaddingVertical;

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
            this.WindowContainer.RuleSet.ApplyOptions(GUILayout.Width(new_size.Width));
            this.WindowContainer.RuleSet.ApplyOptions(GUILayout.Height(new_size.Height));
        }

        private Point RenderTreeNodesPivotPoint => this.Position;
        private Point NodeTreeNodesPivotPoint => this.ClientRect.Location;

        internal void Layout()
        {
            this.RenderTree.Root.Layout(this.RenderTreeNodesPivotPoint);
        }

        /// <summary>
        /// Get the rect for an automatic-layout control
        /// </summary>
        /// <param name="id">id of the control</param>
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
            rect.Offset(this.RenderTreeNodesPivotPoint.X, this.RenderTreeNodesPivotPoint.Y);
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

            rect.Offset(this.NodeTreeNodesPivotPoint.X, this.NodeTreeNodesPivotPoint.Y);
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
