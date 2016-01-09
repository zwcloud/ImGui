namespace ImGui
{
    public interface IWindow
    {
        /// <summary>
        /// Position of the window
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Size of the window
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// Cursor of the window (not implemented)
        /// </summary>
        Cursor Cursor { set; }

        /// <summary>
        /// Is the window focused? (readonly)
        /// </summary>
        bool Focused { get; }

        /// <summary>
        /// Is the window closed? (readonly)
        /// </summary>
        bool Closed { get; }

        /// <summary>
        /// Close the window and distroy it.
        /// </summary>
        void Close();

        /// <summary>
        /// Show the window only if it is hiden
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the window only if it is shown
        /// </summary>
        void Hide();
    }
}