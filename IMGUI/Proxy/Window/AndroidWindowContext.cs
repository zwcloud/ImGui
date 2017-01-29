using System;
using System.Runtime.InteropServices;

namespace ImGui
{
    class AndroidWindowContext : IWindowContext
    {
        #region Native

        // Get native window from surface
        [DllImport("android")]
        private static extern IntPtr ANativeWindow_fromSurface(IntPtr jni, IntPtr surface);

        // Get native window from surface
        [DllImport("android")]
        private static extern void ANativeWindow_release(IntPtr surface);

        #endregion

        #region Implementation of IWindowContext

        public void MainLoop(Action<InputInfo> guiMethod, InputInfo inputInfo)
        {
            throw new NotImplementedException();
        }

        public IWindow CreateWindow(Point position, Size size)
        {
            throw new NotImplementedException();
        }

        public Size GetWindowSize(IWindow window)
        {
            throw new NotImplementedException();
        }

        public Point GetWindowPosition(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void SetWindowSize(IWindow window, Size size)
        {
            throw new NotImplementedException();
        }

        public void SetWindowPosition(IWindow window, Point position)
        {
            throw new NotImplementedException();
        }

        public string GetWindowTitle(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void SetWindowTitle(IWindow window, string title)
        {
            throw new NotImplementedException();
        }

        public void ShowWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void HideWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void MinimizeWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void MaximizeWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void NormalizeWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        public Point ScreenToClient(IWindow window, Point point)
        {
            throw new NotImplementedException();
        }

        public Point ClientToScreen(IWindow window, Point point)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}