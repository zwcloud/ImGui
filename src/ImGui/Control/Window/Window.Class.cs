using System;
using System.Collections.Generic;
using ImGui.Layout;
using System.Diagnostics;
using ImGui.Development;
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
    internal partial class Window
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

        /// <summary>
        /// Last frame count when this window is active.
        /// </summary>
        public long LastActiveFrame;

        /// <summary>
        /// ID stack
        /// </summary>
        /// TODO move IDStack from window to a singleton class like IDManager or IDService
        public Stack<int> IDStack { get; set; } = new Stack<int>();

        public MeshList MeshList { get; set; } = new MeshList();

        public MeshBuffer MeshBuffer { get; set; } = new MeshBuffer();

        private readonly BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();

        public Storage StateStorage { get; } = new Storage();

        public WindowTempData TempData = new WindowTempData();

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

            this.MeshBuffer.OwnerName = this.Name;

            #region Window nodes

            {
                var windowContainer = new Node(this.GetID("window"),"window");
                this.WindowContainer = windowContainer;

                var style = windowContainer.RuleSet;
                style.BorderRadius = (2, 2, 2, 2);
                style.BorderColor = (Color.Rgb(0x707070), Color.Rgb(0x707070), Color.Rgb(0x707070), Color.Rgb(0x707070));
                style.Set(StylePropertyName.BorderTopColor, Color.Blue, GUIState.Active);
                style.Set(StylePropertyName.BorderRightColor, Color.Blue, GUIState.Active);
                style.Set(StylePropertyName.BorderBottomColor, Color.Blue, GUIState.Active);
                style.Set(StylePropertyName.BorderLeftColor, Color.Blue, GUIState.Active);
                style.Set(StylePropertyName.BorderTopColor, Color.Gray, GUIState.Hover);
                style.Set(StylePropertyName.BorderRightColor, Color.Gray, GUIState.Hover);
                style.Set(StylePropertyName.BorderBottomColor, Color.Gray, GUIState.Hover);
                style.Set(StylePropertyName.BorderLeftColor, Color.Gray, GUIState.Hover);
                style.Set(StylePropertyName.BorderTop, 1.0);
                style.Set(StylePropertyName.BorderRight, 1.0);
                style.Set(StylePropertyName.BorderBottom, 1.0);
                style.Set(StylePropertyName.BorderLeft, 1.0);
                style.Set(StylePropertyName.PaddingTop, 5.0);
                style.Set(StylePropertyName.PaddingRight, 10.0);
                style.Set(StylePropertyName.PaddingBottom, 5.0);
                style.Set(StylePropertyName.PaddingLeft, 10.0);
                style.Set(StylePropertyName.WindowBorderColor, Color.Rgb(255, 0, 0), GUIState.Normal);
                style.Set(StylePropertyName.WindowBorderColor, Color.Rgb(0, 0, 255), GUIState.Active);
                style.Set(StylePropertyName.WindowShadowColor, Color.Argb(100, 227, 227, 227));
                style.Set(StylePropertyName.WindowShadowWidth, 15.0);
                style.Set(StylePropertyName.BackgroundColor, Color.White);
                style.Set(StylePropertyName.ResizeGripColor, Color.Argb(0x77303030));
                style.Set(StylePropertyName.ResizeGripColor, Color.Argb(0xAA303030), GUIState.Hover);
                style.Set(StylePropertyName.ResizeGripColor, Color.Argb(0xFF303030), GUIState.Active);
                style.Set(StylePropertyName.WindowRounding, 20.0);

                windowContainer.AttachLayoutGroup(true);
                windowContainer.UseBoxModel = true;
                var windowStyleOptions = GUILayout.Width(this.FullSize.Width).Height(
                    this.Collapsed ? this.CollapsedHeight : this.FullSize.Height
                    );
                windowContainer.RuleSet.ApplyOptions(windowStyleOptions);

                this.RenderTree.Root.AppendChild(windowContainer);
            }

            //title bar
            if(!Flags.HaveFlag(WindowFlags.NoTitleBar))
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
                var contentSize = titleText.RuleSet.CalcContentBoxSize(this.Name, GUIState.Normal);
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
                clientArea.RuleSet.Set(StylePropertyName.OverflowY, (int)OverflowPolicy.Scroll);
                clientArea.RuleSet.Set(StylePropertyName.ScrollBarWidth, CurrentOS.IsDesktopPlatform ? 10.0 : 20.0);
                clientArea.RuleSet.Set(StylePropertyName.ScrollBarBackgroundColor, Color.Rgb(240));
                clientArea.RuleSet.Set(StylePropertyName.ScrollBarButtonColor, Color.Rgb(205), GUIState.Normal);
                clientArea.RuleSet.Set(StylePropertyName.ScrollBarButtonColor, Color.Rgb(166), GUIState.Hover);
                clientArea.RuleSet.Set(StylePropertyName.ScrollBarButtonColor, Color.Rgb(96), GUIState.Active);
                clientArea.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                clientArea.UseBoxModel = true;
                clientArea.RuleSet.refNode = clientArea;
#if ShowClientAreaOutline
                clientArea.RuleSet.OutlineWidth = 1;
                clientArea.RuleSet.OutlineColor = Color.DarkRed;
#endif
                this.ClientAreaNode = clientArea;
                this.WindowContainer.AppendChild(clientArea);
            }

            //resize grip (lasy-initialized)

            if (!Flags.HaveFlag(WindowFlags.NoTitleBar))
            {
                this.ShowWindowTitleBar(true);
            }
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
        public Rect ClientRect => ClientAreaNode.Rect;

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

        public int GetID<T>(T obj) where T : class
        {
            int seed = this.IDStack.Peek();
            var id = this.Hash(seed, obj.GetHashCode());

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        //TODO report text or object related to the control instead of raw ID:
        //an ID is implicit to user
        public void CheckStackSize(int id, bool write)
        {
            int current = IDStack.Count;
            if (write)//record id and stack size
            {
                TempData.StackSizeMap[id] = current;
                //Debug.WriteLine($"pushed id<{id}>, current stack size = {current}");
            }
            else//checking against recorded id and stack size
            {
                //Debug.WriteLine($"checking for id<{id}>, current stack size = {current}");
                if (!TempData.StackSizeMap.ContainsKey(id))
                {
                    throw new PushPopNotMatchException(
                        $"Id<{id}> is not found, extra PopID/TreePop happened.");
                }

                if(TempData.StackSizeMap[id] != current)
                {
                    throw new PushPopNotMatchException(
                        $"PushID/PopID or TreeNode/TreePop doesn't match at Id<{id}>.");
                }
            }
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
            return rect;
        }

        /// <summary>
        /// Sets scroll-y paramter
        /// </summary>
        /// <param name="newScrollY">new value</param>
        public void SetWindowScrollY(double newScrollY)
        {
            this.ClientAreaNode.ScrollOffset.Y = newScrollY;
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
