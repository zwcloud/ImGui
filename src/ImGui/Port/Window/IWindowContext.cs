using System;

namespace ImGui
{
    interface IWindowContext
    {
        void MainLoop(Action guiMethod);

        IWindow CreateWindow(Point position, Size size, WindowTypes windowType);

        Size GetWindowSize(IWindow window);

        Point GetWindowPosition(IWindow window);

        void SetWindowSize(IWindow window, Size size);

        Point GetClientPosition(IWindow window);

        void SetClientPosition(IWindow window, Point position);

        Size GetClientSize(IWindow window);

        void SetClientSize(IWindow window, Size size);

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
