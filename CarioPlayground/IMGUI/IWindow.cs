namespace ImGui
{
    public interface IWindow
    {
        /// <summary>
        /// Position of the form
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Size of the form
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// Cursor of the form (not implemented)
        /// </summary>
        Cursor Cursor { set; }

        /// <summary>
        /// Is the form focused? (readonly)
        /// </summary>
        bool Focused { get; }

        /// <summary>
        /// Close the form and distroy it.
        /// </summary>
        void Close();

        /// <summary>
        /// Show the form only if it is hiden
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the form only if it is shown
        /// </summary>
        void Hide();
    }
}