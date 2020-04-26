using System;
using System.Diagnostics;
using ImGui.OSAbstraction.Window;
using WebAssembly;

namespace ImGui.OSImplementation.Web
{
    /// <summary>
    /// Web window based on a canvas element.
    /// </summary>
    /// <remarks>
    /// Window title: the title of the brower window or tab.
    /// Window area: the area of the browser window.
    /// Client area: the area inside the canvas's content-box.
    /// </remarks>
    internal class WebWindow : IWindow
    {
        /// <summary>
        /// The global window object
        /// </summary>
        private JSObject window;

        /// <summary>
        /// The global document object
        /// </summary>
        private JSObject document;

        /// <summary>
        /// The canvas DOM object
        /// </summary>
        private JSObject canvas;

        /// <summary>
        /// The canvas style object
        /// </summary>
        private JSObject canvasStyle;

        public void Init(string canvasId)
        {
            if (string.IsNullOrEmpty(canvasId))
            {
                throw new ArgumentNullException(nameof(canvasId));
            }

            this.window = Runtime.GetGlobalObject("window") as JSObject;
            Debug.Assert(this.window != null);
            this.document = Runtime.GetGlobalObject("document") as JSObject;
            Debug.Assert(this.document != null);
            this.canvas = this.document.Invoke("getElementById", canvasId) as JSObject;
            if (this.canvas == null)
            {
                throw new WindowCreateException($"Cannot find the canvas with id<{canvasId}>");
            }
            this.canvasStyle = this.canvas.GetObjectProperty("style") as JSObject;
        }

        #region Implementation of IWindow

        public object Handle => this.canvas;

        public IntPtr Pointer => throw new NotSupportedException();

        public Point Position
        {
            get
            {
                var left = (int)this.window.GetObjectProperty("screenX");
                var top = (int)this.window.GetObjectProperty("screenY");
                return new Point(left, top);
            }
            set => throw new PlatformNotSupportedException();
        }

        public Size Size
        {
            get
            {
                var width = (int)this.window.GetObjectProperty("outerWidth");
                var height = (int)this.window.GetObjectProperty("outerheight");
                return new Size(width, height);
            }
            set => throw new PlatformNotSupportedException("Cannot change the window size.");
        }

        public string Title
        {
            get => this.window.GetObjectProperty("title") as string;

            set => this.window.SetObjectProperty("title", value);
        }

        public Point ClientPosition
        {
            get => throw new NotImplementedException();
            set => throw new PlatformNotSupportedException("Cannot change the client area position.");
        }

        public Size ClientSize
        {
            get
            {
                var width = (int)this.canvas.GetObjectProperty("width");
                var height = (int)this.canvas.GetObjectProperty("height");
                return new Size(width, height);
            }
            set => throw new PlatformNotSupportedException("Cannot change the client area size.");
        }

        public void Close()
        {
            this.window.Invoke("close");
        }

        public void Hide()
        {
            this.canvasStyle.SetObjectProperty("visibility", "hidden");
        }

        public void Show()
        {
            this.canvasStyle.SetObjectProperty("visibility", "visible");
        }

        public Point ScreenToClient(Point point)
        {
            return point - (Vector)this.ClientPosition;
        }

        public Point ClientToScreen(Point point)
        {
            return point + (Vector) this.ClientPosition;
        }

        public void MainLoop(Action guiMethod)
        {
            guiMethod();
        }

        #endregion
    }
}