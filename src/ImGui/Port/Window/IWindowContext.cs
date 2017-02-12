using System;

namespace ImGui
{
    interface IWindowContext
    {
        void MainLoop(Action guiMethod);

        void InputEventHandler(float x, float y, bool isDown);

        IWindow CreateWindow(IntPtr nativeWindow);

        IWindow CreateWindow(Point position, Size size);

        Size GetWindowSize(IWindow window);

        Point GetWindowPosition(IWindow window);

        void SetWindowSize(IWindow window, Size size);

        void SetWindowPosition(IWindow window, Point position);
        
        string GetWindowTitle(IWindow window);

        void SetWindowTitle(IWindow window, string title);

        void ShowWindow(IWindow window);

        void HideWindow(IWindow window);

        void CloseWindow(IWindow window);

        void MinimizeWindow(IWindow window);

        void MaximizeWindow(IWindow window);

        void NormalizeWindow(IWindow window);

        Point ScreenToClient(IWindow window, Point point);

        Point ClientToScreen(IWindow window, Point point);
    }
}
