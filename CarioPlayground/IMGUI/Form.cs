namespace ImGui
{
    public abstract class Form : SFMLForm, IDragableWindow
    {
        protected Form(int width, int height)
            : base(width, height)
        {
            //this.internalForm.MouseButtonPressed += OnMouseButtonPressed;
            //this.internalForm.MouseMoved += OnMouseMoved;
            //this.internalForm.MouseButtonReleased += OnMouseButtonReleased;
            //IsWinddowDragable = true;
        }

        #region Implementation of IDragableWindow

        private SFML.System.Vector2i grabbedOffset;
        private bool isWindowGrabbed;

        internal bool IsWinddowDragable { private get; set; }

        public void OnMouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if(!IsWinddowDragable)
            {
                return;
            }

            var window = (SFML.Window.Window)sender;
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                grabbedOffset = window.Position - SFML.Window.Mouse.GetPosition();
                isWindowGrabbed = true;
            }
        }

        public void OnMouseMoved(object sender, SFML.Window.MouseMoveEventArgs e)
        {
            if (!IsWinddowDragable)
            {
                return;
            }

            var window = (SFML.Window.Window)sender;
            if (isWindowGrabbed)
            {
                var position = SFML.Window.Mouse.GetPosition();
                var newPosition = position + grabbedOffset;
                if (window.Position != newPosition)
                {
                    window.Position = newPosition;
                }
            }
        }

        public void OnMouseButtonReleased(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (!IsWinddowDragable)
            {
                return;
            }

            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                isWindowGrabbed = false;
            }
        }

        #endregion
    }
}