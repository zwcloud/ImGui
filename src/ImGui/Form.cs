using System;
using System.IO;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using SixLabors.ImageSharp.Processing;

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
        private string debugName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class at specific rectangle.
        /// </summary>
        /// <param name="rect">initial rectangle of the form</param>
        protected Form(Rect rect):this(rect.TopLeft, rect.Size)
        {
        }

        internal Form(Rect rect, string title, WindowTypes type)
            : this(rect.TopLeft, rect.Size, title, type)
        {
        }

        internal Form(Point position, Size size, string title = "ImGui Form",
            WindowTypes type = WindowTypes.Regular)
        {
            this.debugName = title;

            Profile.Start("Create Window");
            this.nativeWindow = Application.PlatformContext.CreateWindow(position, size, type);
            if (type == WindowTypes.Regular)
            {
                this.nativeWindow.Title = title;
            }
            Profile.End();

            Profile.Start("Create Renderer");
            this.renderer = Application.PlatformContext.CreateRenderer();
            this.renderer.Init(this.Pointer, this.nativeWindow.ClientSize);
            Profile.End();

            uiContext.InitializeBackForegroundRenderContext();
        }

        internal void MainLoop(Action guiMethod)
        {
            this.nativeWindow.MainLoop(guiMethod);
        }

        #region window management

        internal bool Closed { get; private set; }

        internal IntPtr Pointer => this.nativeWindow.Pointer;

        internal Size Size
        {
            get => this.nativeWindow.Size;
            set => this.nativeWindow.Size = value;
        }

        internal Point ClientPosition
        {
            get => this.nativeWindow.ClientPosition;
            set => this.nativeWindow.ClientPosition = value;
        }

        internal Size ClientSize
        {
            get => this.nativeWindow.ClientSize;
            set => this.nativeWindow.ClientSize = value;
        }

        internal Point Position
        {
            get => this.nativeWindow.Position;
            set => this.nativeWindow.Position = value;
        }

        internal Rect Rect => new Rect(this.Position, this.Size);

        internal bool Focused => throw new NotImplementedException();

        internal void Show()
        {
            this.nativeWindow.Show();
        }

        internal void Hide()
        {
            this.nativeWindow.Hide();
        }

        internal void Close()
        {
            this.renderer.ShutDown();
            this.nativeWindow.Close();
            this.Closed = true;
        }

        #endregion

        internal Point ScreenToClient(Point point)
        {
            return this.nativeWindow.ScreenToClient(point);
        }

        internal Point ClientToScreen(Point point)
        {
            return this.nativeWindow.ClientToScreen(point);
        }

        internal void SaveClientAreaToPng(string filePath)
        {
            byte[] data = this.renderer.GetRawBackBuffer(out var width, out var height);
            var image = SixLabors.ImageSharp.Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(SixLabors.ImageSharp.Configuration.Default, data, width, height);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            using (var stream = File.OpenWrite(filePath))
            {
                SixLabors.ImageSharp.ImageExtensions.SaveAsPng(image, stream);
            }
        }
    }
}