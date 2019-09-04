using System;
using System.Collections.Generic;
using ImGui.Layout;
using System.Diagnostics;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Style;

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
        /// Render context
        /// </summary>
        public RenderContext RenderContext;

        /// <summary>
        /// Absolute placed visuals. (non-layout)
        /// </summary>
        public List<Visual> AbsoluteVisualList;

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
        /// ID stack
        /// </summary>
        public Stack<int> IDStack { get; set; } = new Stack<int>();

        public MeshList MeshList { get; set; } = new MeshList();

        public MeshBuffer MeshBuffer { get; set; } = new MeshBuffer();

        private readonly BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();

        #region Window original sub nodes
        private Node titleBar { get; }
        private Node titleIcon { get; }
        private Node titleText { get; }
        private Node clientArea { get; }
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

            this.AbsoluteVisualList = new List<Visual>();
            this.RenderTree = new RenderTree(this.ID, position, size);
            this.RenderContext = new RenderContext(this.geometryRenderer, this.MeshList);

            this.IDStack.Push(this.ID);
            this.MoveID = this.GetID("#MOVE");

            #region Window nodes

            {
                var windowContainer = new Node(this.GetID("window"),"window");
                this.WindowContainer = windowContainer;

                var style = windowContainer.RuleSet;
                style.BackgroundColor = Color.LightBlue;
                style.BorderRadius = (2, 2, 2, 2);
                style.BorderColor = (Color.Rgb(0x707070), Color.Rgb(0x707070), Color.Rgb(0x707070), Color.Rgb(0x707070));
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
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(0x77303030));
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(0xAA303030), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, Color.Argb(0xFF303030), GUIState.Active);
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
                this.titleBar = new Node(this.GetID("titleBar"),"title bar");
                titleBar.AttachLayoutGroup(false);
                titleBar.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(this.TitleBarHeight));
                titleBar.UseBoxModel = true;
                StyleRuleSetBuilder b = new StyleRuleSetBuilder(titleBar);
                b.Padding((top: 8, right: 8, bottom: 8, left: 8))
                    .FontColor(Color.Black)
                    .FontSize(12)
                    .BackgroundColor(Color.White)
                    .AlignmentVertical(Alignment.Center)
                    .AlignmentHorizontal(Alignment.Start);

                this.titleIcon = new Node(this.GetID("icon"),"icon");
                titleIcon.AttachLayoutEntry(new Size(20, 20));
                titleIcon.RuleSet.ApplyOptions(GUILayout.Width(20).Height(20));
                titleIcon.UseBoxModel = false;

                this.titleText = new Node(this.GetID("title"),"title");
                var contentSize = titleText.RuleSet.CalcSize(this.Name, GUIState.Normal);
                titleText.AttachLayoutEntry(contentSize);
                titleText.RuleSet.ApplyOptions(GUILayout.Height(20));
                titleText.UseBoxModel = false;

                var closeButton = new Node(this.GetID("close button"),"close button");
                closeButton.AttachLayoutEntry(new Size(20, 20));
                closeButton.RuleSet.ApplyOptions(GUILayout.Width(20).Height(20));
                closeButton.UseBoxModel = false;

                titleBar.AppendChild(titleIcon);
                titleBar.AppendChild(titleText);

                this.WindowContainer.AppendChild(titleBar);
            }

            //client area
            {
                this.clientArea = new Node(this.GetID("client area"),"client area");
                clientArea.AttachLayoutGroup(true);
                clientArea.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                clientArea.UseBoxModel = true;
                clientArea.RuleSet.OutlineWidth = 1;
                clientArea.RuleSet.OutlineColor = Color.Red;
                clientArea.RuleSet.refNode = clientArea;
                this.ClientAreaNode = clientArea;
                this.WindowContainer.AppendChild(clientArea);
            }

            //resize grip (lasy-initialized)

            this.ShowWindowTitleBar(true);
            this.ShowWindowClientArea(!this.Collapsed);
            #endregion
        }

        public void ShowWindowTitleBar(bool isShow)
        {
            this.titleBar.ActiveSelf = isShow;
            this.titleBar.ActiveSelf = isShow;
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
                    open = !this.Collapsed;//overwrite the open state
                }
            }

            this.Collapsed = !open;

            //window container
            using(var dc = WindowContainer.RenderOpen())
            {
                dc.DrawBoxModel(WindowContainer);
            }

            //update title bar
            var windowRounding = (float) this.WindowContainer.RuleSet.Get<double>(GUIStyleName.WindowRounding);
            if (!flags.HaveFlag(WindowFlags.NoTitleBar))
            {
                // background
                using (var dc = this.titleBar.RenderOpen())
                {
                    dc.DrawBoxModel(this.titleBar.RuleSet, this.titleBar.Rect);
                }
                //icon
                using (var dc = titleIcon.RenderOpen())
                {
                    dc.DrawImage(@"assets\images\logo.png");
                }
                //title
                using (var dc = titleText.RenderOpen())
                {
                    dc.DrawGlyphRun(new Brush(titleText.RuleSet.FontColor),
                        new GlyphRun(titleText.Rect.Location, this.Name, titleText.RuleSet.FontFamily,
                            titleText.RuleSet.FontSize));
                }
            }

            this.ShowWindowClientArea(!this.Collapsed);

            if (!this.Collapsed)
            {
                //show and update window client area
                clientArea.Rect = ClientRect;
                using (var dc = clientArea.RenderOpen())
                {
                    dc.DrawBoxModel(clientArea);
                }

                if (!flags.HaveFlag(WindowFlags.NoResize) && this.ResizeGripNode == null)
                {
                    var id = this.GetID("#RESIZE");
                    var node = new Node(id, "Window_ResizeGrip");
                    this.ResizeGripNode = node;
                    this.AbsoluteVisualList.Add(node);
                }
                //resize grip
                this.ResizeGripNode.ActiveSelf = true;
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
                    var paddingBottom = this.WindowContainer.RuleSet.PaddingBottom;
                    var borderRight = this.WindowContainer.RuleSet.BorderRight;
                    var paddingRight = this.WindowContainer.RuleSet.PaddingRight;
                    using (var dc = this.ResizeGripNode.RenderOpen())
                    {
                        var path = new PathGeometry();
                        var figure = new PathFigure();
                        var A = br + new Vector(-10 - borderRight - paddingRight, 0);
                        var B = br + new Vector(0, -10 - borderBottom - paddingBottom);
                        figure.StartPoint = A;
                        figure.IsFilled = true;
                        figure.Segments.Add(new LineSegment(B, false));
                        figure.Segments.Add(new LineSegment(br, false));
                        figure.Segments.Add(new LineSegment(A, false));
                        path.Figures.Add(figure);
                        dc.DrawGeometry(new Brush(resizeGripColor), null, path);
                    }

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
            var seed = this.IDStack.Peek();
            var hashCharIndex = str_id.IndexOf("##", StringComparison.Ordinal);
            string customId = null;
            if (hashCharIndex > 0)
            {
                customId = str_id.Substring(hashCharIndex + 1);
            }

            var subId = string.IsNullOrWhiteSpace(customId) ? str_id.GetHashCode() : customId.GetHashCode();
            var id = this.Hash(seed, subId);

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

            // TODO consider if we still need to apply window client area offset, since we have "client area" node
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

        public void Render(IRenderer renderer, Size clientSize)
        {
            this.RenderTree.Root.Render(this.RenderContext);

            foreach (var visual in this.AbsoluteVisualList)
            {
                visual.RenderContent(this.RenderContext);
            }

            //rebuild mesh buffer
            this.MeshBuffer.Clear();
            this.MeshBuffer.Init();
            this.MeshBuffer.Build(this.MeshList);

            this.MeshList.Clear();

            //draw meshes in MeshBuffer with underlying native renderer(OpenGL\Direct3D\Vulkan)
            renderer.DrawMeshes((int)clientSize.Width, (int)clientSize.Height,
                (shapeMesh: this.MeshBuffer.ShapeMesh, imageMesh: this.MeshBuffer.ImageMesh, this.MeshBuffer.TextMesh));
        }
    }
}
