namespace IMGUI
{
    /// <summary>
    /// Basic form without any gui logic
    /// </summary>
    public abstract class BasicForm : IWindow
    {
        private System.Windows.Forms.Form form;

        protected BasicForm()
        {
            form = new System.Windows.Forms.Form
            {
                BackColor = System.Drawing.Color.White,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable,
                StartPosition = System.Windows.Forms.FormStartPosition.Manual
            };
        }

        #region Implementation of IWindow

        public Point Position
        {
            get
            {
                var l = form.Location;
                return new Point(l.X, l.Y);
            }
            set { form.Location = new System.Drawing.Point((int) value.X, (int) value.Y); }
        }

        public Size Size
        {
            get
            {
                var s = form.Size;
                return new Size(s.Width, s.Height);
            }
            set { form.Size = new System.Drawing.Size((int) value.Width, (int) value.Height); }
        }

        public void WindowProc(object Context)
        {
            bool exit = false;
            while (Utility.IsApplicationIdle() && exit == false)
            {
                Utility.MillisFrameBegin = Utility.Millis;
                System.Threading.Thread.Sleep(20); //Keep about 50fps
                exit = Update();
                if (exit)
                {
                    CleanUp();
                    Close();
                }
                Render();
            }
        }

        #region Window loop internal

        private RenderContext renderContext = new RenderContext();

        private void SwapSurfaceBuffer()
        {
            renderContext.SwapSurface();
        }

        private bool Update()
        {
            var clientRect = new Rect
            {
                X = form.ClientRectangle.Left,
                Y = form.ClientRectangle.Top,
                Width = form.ClientRectangle.Width,
                Height = form.ClientRectangle.Height
            };
            var clientPos = form.PointToScreen(form.ClientRectangle.Location);

            Input.Mouse.Refresh(clientPos.X, clientPos.Y, clientRect);
            Input.Keyboard.Refresh();

            return false;
        }

        private void Render()
        {
            if (renderContext.BackContext == null || renderContext.FrontContext == null)
                return;
            SwapSurfaceBuffer();
        }
        
        private void CleanUp() //Cleanup only happened when ESC is pressed
        {
            if (renderContext.BackContext != null)
                renderContext.BackContext.Dispose();
        }

        private void Close()
        {
            this.form.Close();
        }

        #endregion

        #endregion
    }
}