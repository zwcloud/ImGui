using System.Collections.Generic;
using ImGui.Layout;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

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
                style.Set(GUIStyleName.WindowBorderColor, new Color(0.70f, 0.70f, 0.70f, 0.65f));
                style.Set(GUIStyleName.WindowBorderShadowColor, new Color(0.00f, 0.00f, 0.00f, 0.00f));
                style.Set(GUIStyleName.BackgroundColor, new Color(0.30f, 0.30f, 0.30f, 0.70f));
                style.Set(GUIStyleName.ResizeGripSize, 20.0);
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.30f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.60f), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.90f), GUIState.Active);
                style.Set(GUIStyleName.WindowRounding, 3.0);
                style.Set(GUIStyleName.ScrollBarWidth, 10.0);
                style.Set(GUIStyleName.ScrollBarBackgroundColor, Color.Metal);
                style.Set(GUIStyleName.ScrollBarButtonColor, new Color(0.40f, 0.40f, 0.80f, 0.30f), GUIState.Normal);
                style.Set(GUIStyleName.ScrollBarButtonColor, new Color(0.40f, 0.40f, 0.80f, 0.40f), GUIState.Hover);
                style.Set(GUIStyleName.ScrollBarButtonColor, new Color(0.80f, 0.50f, 0.50f, 0.40f), GUIState.Active);
                this.Style = style;
            }

            // window header styles
            {
                var style = new GUIStyle();
                style.Set(GUIStyleName.BackgroundColor, new Color(0.27f, 0.27f, 0.54f, 0.83f));
                style.Set(GUIStyleName.BackgroundColor, new Color(0.32f, 0.32f, 0.63f, 0.87f), GUIState.Active);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.40f, 0.40f, 0.80f, 0.20f), GUIState.Disabled);
                style.Set(GUIStyleName.PaddingTop, 8.0);
                style.Set(GUIStyleName.PaddingRight, 8.0);
                style.Set(GUIStyleName.PaddingBottom, 8.0);
                style.Set(GUIStyleName.PaddingLeft, 8.0);
                style.Set(GUIStyleName.FontColor, Color.White);
                style.FontFamily = GUIStyle.Default.FontFamily;
                style.FontSize = 20.0;
                this.TitleBarStyle = style;
            }

            this.TitleBarHeight = TitleBarStyle.PaddingVertical + 30;

            var scrollBarWidth = this.Style.Get<double>(GUIStyleName.ScrollBarWidth);
            var clientSize = new Size(
                this.Size.Width - scrollBarWidth - this.Style.PaddingHorizontal - this.Style.BorderHorizontal,
                this.Size.Height - this.Style.PaddingVertical - this.Style.BorderVertical);
            this.StackLayout = new StackLayout(this.ID, clientSize);
        }

        /// <summary>
        /// Gets the rect of this window
        /// </summary>
        public Rect Rect => new Rect(Position, Size);

        /// <summary>
        /// Gets the height of the title bar
        /// </summary>
        public double TitleBarHeight { get; }

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
        public Rect GetRect(int id, Size size)
        {
            var rect = StackLayout.GetRect(id, size);

            if(rect != StackLayout.DummyRect)
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

    }
}
