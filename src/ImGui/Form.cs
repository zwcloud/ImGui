#define UseLog
using System;
using System.Collections.Generic;
using Ivony.Logs;

namespace ImGui
{
    /// <summary>
    /// Represents a window that makes up an application's user interface.
    /// </summary>
    public abstract partial class Form
    {
        public static Form current;

        private readonly IWindow window;
        private bool needRender = false;

        internal DrawList DrawList = new DrawList();
        internal IRenderer renderer;
        internal LayoutCache layoutCache = new LayoutCache();
        internal GUIContext uiContext = new GUIContext();
        internal GUIDrawContext drawContext = new GUIDrawContext();

        /// <summary>
        /// Create form for android.
        /// </summary>
        /// <param name="nativeWindow"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        protected Form(IntPtr nativeWindow, Point position, Size size)
        {
            this.window = Application.windowContext.CreateWindow(position, size, WindowTypes.Regular);
            renderer = Application.platformContext.CreateRenderer();
            renderer.Init(IntPtr.Zero);//dummy paramters

            this.DrawList.DrawBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    ClipRect = new Rect(this.ClientSize)
                });
            this.DrawList.BezierBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    ClipRect = new Rect(this.ClientSize)
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class at specific rectangle.
        /// </summary>
        /// <param name="rect">initial rectangle of the form</param>
        protected Form(Rect rect):this(rect.TopLeft, rect.Size)
        {
        }

        internal Form(Point position, Size size, string Title = "<unnamed>")
        {
            this.window = Application.windowContext.CreateWindow(position, size, WindowTypes.Regular);
            this.window.Title = Title;

            renderer = Application.platformContext.CreateRenderer();
            renderer.Init(this.Pointer);
            
            this.DrawList.DrawBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    //ClipRect = new Rect(this.ClientSize)
                });
            this.DrawList.BezierBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    //ClipRect = new Rect(this.ClientSize)
                });

            Input.Mouse.Cursor = Cursor.Default;
        }

        internal LayoutCache LayoutCache
        {
            get { return layoutCache; }
        }

        #region window management

        internal bool Closed { get; private set; }

        internal IntPtr Pointer { get { return window.Pointer; } }

        internal Size Size
        {
            get { return window.Size; }
            set { window.Size = value; }
        }

        internal Point ClientPosition
        {
            get => window.ClientPosition;
            set => window.ClientPosition = value;
        }

        internal Size ClientSize
        {
            get => window.ClientSize;
            set => window.ClientSize = value;
        }

        internal Point Position
        {
            get { return window.Position; }
            set { window.Position = value; }
        }

        internal Rect Rect
        {
            get { return new Rect(Position, Size); }
        }

        internal bool Focused { get { throw new NotImplementedException(); } }

        public bool NeedRender
        {
            get { return needRender; }
            set { needRender = value; }
        }

        internal void Show()
        {
            window.Show();
        }

        internal void Hide()
        {
            window.Hide();
        }

        internal void Close()
        {
            this.renderer.ShutDown();
            window.Close();
            this.Closed = true;
        }

        #endregion
        
        /// <summary>
        /// Get the mouse position relative to this form
        /// </summary>
        internal Point GetMousePos()
        {
            return ScreenToClient(Input.Mouse.MousePos);
        }

        internal Point ScreenToClient(Point point)
        {
            return window.ScreenToClient(point);
        }

        internal Point ClientToScreen(Point point)
        {
            return window.ClientToScreen(point);
        }
    }
}