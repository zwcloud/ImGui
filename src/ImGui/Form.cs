using System;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;

namespace ImGui
{
    /// <summary>
    /// Represents a window that makes up an application's user interface.
    /// </summary>
    public abstract partial class Form
    {
        public static Form current;

        private readonly IWindow nativeWindow;

        internal IRenderer renderer;
        internal GUIContext uiContext = new GUIContext();

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class at specific rectangle.
        /// </summary>
        /// <param name="rect">initial rectangle of the form</param>
        protected Form(Rect rect):this(rect.TopLeft, rect.Size)
        {
        }

        internal Form(Point position, Size size, string Title = "ImGui Form")
        {
            this.nativeWindow = Application.platformContext.CreateWindow(position, size, WindowTypes.Regular);
            this.nativeWindow.Title = Title;

            Profile.Start("CreateRenderer");
            renderer = Application.platformContext.CreateRenderer();
            renderer.Init(this.Pointer, nativeWindow.ClientSize);
            Profile.End();
        }

        internal void MainLoop(Action guiMethod)
        {
            nativeWindow.MainLoop(guiMethod);
        }

        #region window management

        internal bool Closed { get; private set; }

        internal IntPtr Pointer { get { return nativeWindow.Pointer; } }

        internal Size Size
        {
            get { return nativeWindow.Size; }
            set { nativeWindow.Size = value; }
        }

        internal Point ClientPosition
        {
            get => nativeWindow.ClientPosition;
            set => nativeWindow.ClientPosition = value;
        }

        internal Size ClientSize
        {
            get => nativeWindow.ClientSize;
            set => nativeWindow.ClientSize = value;
        }

        internal Point Position
        {
            get { return nativeWindow.Position; }
            set { nativeWindow.Position = value; }
        }

        internal Rect Rect
        {
            get { return new Rect(Position, Size); }
        }

        internal bool Focused { get { throw new NotImplementedException(); } }

        internal void Show()
        {
            nativeWindow.Show();
        }

        internal void Hide()
        {
            nativeWindow.Hide();
        }

        internal void Close()
        {
            this.renderer.ShutDown();
            nativeWindow.Close();
            this.Closed = true;
        }

        #endregion

        internal Point ScreenToClient(Point point)
        {
            return nativeWindow.ScreenToClient(point);
        }

        internal Point ClientToScreen(Point point)
        {
            return nativeWindow.ClientToScreen(point);
        }
    }
}