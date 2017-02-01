using System;

namespace ImGui
{
    internal class Win32Window : IWindow
    {
        readonly IntPtr hwnd;
        
        public Win32Window(IntPtr hwnd)
        {
            this.hwnd = hwnd;
        }

        #region implementation of IWindow

        public object Handle
        {
            get
            {
                return hwnd;
            }
        }

        public IntPtr Pointer
        {
            get
            {
                return hwnd;
            }
        }

        public Point Position
        {
            get
            {
                return Application.windowContext.GetWindowPosition(this);
            }

            set
            {
                Application.windowContext.SetWindowPosition(this, value);
            }
        }

        public Size Size
        {
            get
            {
                return Application.windowContext.GetWindowSize(this);
            }

            set
            {
                Application.windowContext.SetWindowSize(this, value);
            }
        }

        public string Title
        {
            get
            {
                return Application.windowContext.GetWindowTitle(this);
            }

            set
            {
                Application.windowContext.SetWindowTitle(this, value);
            }
        }

        public void Close()
        {
            Application.windowContext.CloseWindow(this);
        }

        public void Hide()
        {
            Application.windowContext.HideWindow(this);
        }

        public void Maximize()
        {
            Application.windowContext.MaximizeWindow(this);
        }

        public void Minimize()
        {
            Application.windowContext.MinimizeWindow(this);
        }

        public void Normalize()
        {
            Application.windowContext.NormalizeWindow(this);
        }

        public void Show()
        {
            Application.windowContext.ShowWindow(this);
        }

        public Point ScreenToClient(Point point)
        {
            return Application.windowContext.ScreenToClient(this, point);
        }

        public Point ClientToScreen(Point point)
        {
            return Application.windowContext.ClientToScreen(this, point);
        }

        #endregion
    }
}
