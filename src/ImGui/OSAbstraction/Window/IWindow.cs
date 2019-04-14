using System;

namespace ImGui.OSAbstraction.Window
{
    /// <summary>
    /// window-related functions
    /// </summary>
    internal interface IWindow
    {
        /// <summary>
        /// Platform specific handle related to the window.
        /// </summary>
        object Handle { get; }

        /// <summary>
        /// Platform specific pointer related to the window.
        /// </summary>
        IntPtr Pointer { get; }

        /// <summary>
        /// Position of the window
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Size of the window
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// position of the client area
        /// </summary>
        Point ClientPosition { get; set; }

        /// <summary>
        /// size of the client area
        /// </summary>
        Size ClientSize { get; set; }

        /// <summary>
        /// Title of the window
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Show the window
        /// </summary>
        void Show();

        /// <summary>
        /// main loop (only once)
        /// </summary>
        void MainLoop(Action guiMethod);

        /// <summary>
        /// Hide the window
        /// </summary>
        void Hide();

        /// <summary>
        /// Close the window
        /// </summary>
        void Close();

        /// <summary>
        /// Convert the screen coordinates of a specified point on the screen to client-area (window) coordinates
        /// </summary>
        Point ScreenToClient(Point point);

        /// <summary>
        /// Convert the client-area (window) coordinates of a specified point to screen coordinates.
        /// </summary>
        Point ClientToScreen(Point point);

    }
}
