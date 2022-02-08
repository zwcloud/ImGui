using System;
using System.Runtime.InteropServices;
using ImGui.OSAbstraction.Window;

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
        [DllImport("*")]
        private static extern void emscripten_run_script(string script);

        [DllImport("*")]
        private static extern int emscripten_run_script_int(string script);

        [DllImport("*")]
        private static extern string emscripten_run_script_string(string script);
        
        public void Init(string canvasId, Size size)
        {
            //TODO customize output html and canvas from emscripten
        }

        #region Implementation of IWindow

        public object Handle => IntPtr.Zero;

        public IntPtr Pointer => throw new NotSupportedException();

        public Point Position
        {
            get
            {
                var left = emscripten_run_script_int("window.screenX");
                var top = emscripten_run_script_int("window.screenY");
                return new Point(left, top);
            }
            set => throw new PlatformNotSupportedException();
        }

        public Size Size
        {
            get
            {
                var width = emscripten_run_script_int("window.outerWidth");
                var height = emscripten_run_script_int("window.outerHeight");
                return new Size(width, height);
            }
            set => throw new PlatformNotSupportedException("Cannot change the window size.");
        }

        public string Title
        {
            get => emscripten_run_script_string("window.title") as string;

            set => emscripten_run_script_string("window.title = {value}");
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
                var width = emscripten_run_script_int("document.getElementById('canvas').getBoundingClientRect().width");
                var height = emscripten_run_script_int("document.getElementById('canvas').getBoundingClientRect().height");
                return new Size(width, height);
            }
            set => throw new PlatformNotSupportedException("Cannot change the client area size.");
        }

        public void Close()
        {
            //TODO

            //won't work: Scripts may close only the windows that were opened by them.
            //emscripten_run_script("window.close");
        }

        public void Hide()
        {
            emscripten_run_script("document.getElementById('canvas').style.visibility = hidden");
        }

        public void Show()
        {
            emscripten_run_script("document.getElementById('canvas').style.visibility = visible");
        }

        public Point ScreenToClient(Point point)
        {
            return point - (Vector)this.ClientPosition;
        }

        public Point ClientToScreen(Point point)
        {
            return point + (Vector) this.ClientPosition;
        }

        public bool GetFocus()
        {
            return false;
        }

        public void SetFocus()
        {
        }

        public bool Minimized => false;

        public void MainLoop(Action guiMethod)
        {
            guiMethod();
        }

        #endregion
    }
}