using System;
using System.Collections.Generic;
using System.Diagnostics;

//TODO extract interface of window functions

namespace ImGui
{
    /// <summary>
    /// The SFML Form
    /// </summary>
    public abstract class SFMLForm : BaseForm
    {
        public string Name { get; internal set; }

        /// <summary>
        /// Position of the form
        /// </summary>
        public Point Position
        {
            get
            {
                var l = internalForm.Position;
                return new Point(l.X, l.Y);
            }
            set { internalForm.Position = new SFML.System.Vector2i((int) value.X, (int) value.Y); }
        }

        /// <summary>
        /// Size of the form
        /// </summary>
        public Size Size
        {
            get
            {
                var s = internalForm.Size;
                return new Size(s.X, s.Y);
            }
            set { internalForm.Size = new SFML.System.Vector2u((uint) value.Width, (uint) value.Height); }
        }

        /// <summary>
        /// Cursor of the form (not implemented)
        /// </summary>
        public Cursor Cursor
        {
            set
            {
                //TODO implement SFMLForm::Cursor
                //Debug.WriteLine("SFMLForm::Cursor is mot implemented yet. Use default cursor instead.");
                switch (value)
                {
                    case Cursor.Default:
                        break;
                    case Cursor.Text:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Is the form focused? (readonly)
        /// </summary>
        public bool Focused
        {
            get { return internalForm.HasFocus(); }
        }

        /// <summary>
        /// Is this form visible?
        /// </summary>
        public bool Visible { get; private set; }

        public bool Closed { get { return closed; } }

        /// <summary>
        /// Close the form and distroy it.
        /// </summary>
        public virtual void Close()
        {
            this.closed = true;
            this.Visible = false;
            this.internalForm.Close();
            this.internalForm.Dispose();
        }

        /// <summary>
        /// Show the form only if it is hiden
        /// </summary>
        public virtual void Show()
        {
            if(!this.Visible)
            {
                this.internalForm.SetVisible(true);
                this.Visible = true;
            }
        }

        /// <summary>
        /// Hide the form only if it is shown
        /// </summary>
        public void Hide()
        {
            if(this.Visible)
            {
                this.internalForm.SetVisible(false);
                this.Visible = false;
            }
        }

        public abstract void Minimize();
        public abstract void Maximize();
        public abstract void Normalize();

        public object InternalForm
        {
            get { return internalForm; }
        }

        /// <summary>
        /// Custom GUI Logic. This should be overrided to create custom GUI elements
        /// </summary>
        /// <remarks>This is the only method exposed to user.</remarks>
        protected abstract void OnGUI();

        #region Internal Implementation

        internal SFML.Window.Window internalForm;

        private bool closed = false;

        protected SFMLForm(int width, int height)
        {
            internalForm = new SFML.Window.Window(
                new SFML.Window.VideoMode((uint) width, (uint) height),
                "DummyWindowTitle",
                SFML.Window.Styles.None, new SFML.Window.ContextSettings
                {
                    DepthBits = 0,
                    StencilBits = 0,
                    AntialiasingLevel = 0,
                    MajorVersion = 2,
                    MinorVersion = 1
                });
            internalForm.SetVisible(false);//not show form on creating
            internalForm.SetVerticalSyncEnabled(false);

            this.internalForm.SetActive(true);

            Visible = false;
        }

        #endregion
    }
}