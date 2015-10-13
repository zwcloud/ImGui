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

        }

        #endregion
    }
}