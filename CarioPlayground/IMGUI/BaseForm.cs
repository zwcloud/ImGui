namespace IMGUI
{
    public abstract class BaseForm
    {
        internal System.Collections.Generic.Dictionary<string, Control> controls;

        /// <summary>
        /// Position of the form
        /// </summary>
        public abstract Point Position { get; set; }

        /// <summary>
        /// Size of the form
        /// </summary>
        public abstract Size Size { get; set; }

        /// <summary>
        /// Cursor of the form (not implemented)
        /// </summary>
        public abstract Cursor Cursor { set; }
        
        /// <summary>
        /// Is the form focused? (readonly)
        /// </summary>
        public abstract bool Focused { get; }

        /// <summary>
        /// Close the form and distroy it.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Show the form only if it is hiden
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Hide the form only if it is shown
        /// </summary>
        public abstract void Hide();

        internal abstract object InternalForm { get; }

        internal System.Collections.Generic.Dictionary<string, Control> Controls
        {
            get { return controls; }
        }
    }
}