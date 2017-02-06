using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ImGui
{
    class AndroidWindowContext : IWindowContext
    {
        #region Native

        #region (not used)

        // Get native window from surface
        [DllImport("android")]
        private static extern IntPtr ANativeWindow_fromSurface(IntPtr jni, IntPtr surface);

        // Get native window from surface
        [DllImport("android")]
        private static extern void ANativeWindow_release(IntPtr surface);
        
        /// <summary>
        /// Get window height
        /// </summary>
        /// <param name="window">ANativeWindow*</param>
        [DllImport("android")]
        private static extern int ANativeWindow_getWidth(IntPtr window);

        /// <summary>
        /// Get window width
        /// </summary>
        /// <param name="window">ANativeWindow*</param>
        [DllImport("android")]
        private static extern int ANativeWindow_getHeight(IntPtr window);

        #endregion


        #endregion 

        #region Implementation of IWindowContext

        public void MainLoop(Action guiMethod)
        {
            guiMethod();
        }

        public IWindow CreateWindow(IntPtr nativeWindow)
        {
            // Only one initial window is available.
            return AndroidWindow.CreateAndroidWindow(nativeWindow);//this is dummy for now
        }

        public IWindow CreateWindow(Point position, Size size)
        {
            throw new InvalidOperationException();
        }

        public Size GetWindowSize(IWindow window)
        {
            //dummy
            return Size.Zero;
        }

        public Point GetWindowPosition(IWindow window)
        {
            //dummy
            return Point.Zero;
        }

        public void SetWindowSize(IWindow window, Size size)
        {
            //dummy
        }

        public void SetWindowPosition(IWindow window, Point position)
        {
            //dummy
        }

        public string GetWindowTitle(IWindow window)
        {
            //dummy
            return "dummy";
        }

        public void SetWindowTitle(IWindow window, string title)
        {
            //dummy
        }

        public void ShowWindow(IWindow window)
        {
            //dummy
        }

        public void HideWindow(IWindow window)
        {
            //dummy
        }

        public void CloseWindow(IWindow window)
        {
            //dummy
        }

        public void MinimizeWindow(IWindow window)
        {
            //dummy
        }

        public void MaximizeWindow(IWindow window)
        {
            //dummy
        }

        public void NormalizeWindow(IWindow window)
        {
            //dummy
        }

        public Point ScreenToClient(IWindow window, Point point)
        {
            return point;
        }

        public Point ClientToScreen(IWindow window, Point point)
        {
            return point;
        }

        #endregion
    }
}