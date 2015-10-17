using Cairo;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IMGUI
{
    /// <summary>
    /// The SFML Form (not used)
    /// </summary>
    public abstract class SFMLForm : BaseForm
    {
        /// <summary>
        /// Position of the form
        /// </summary>
        public override Point Position
        {
            get
            {
                var l = internalForm.Position;
                return new Point(l.X, l.Y);
            }
            set { internalForm.Position = new SFML.System.Vector2i((int)value.X, (int)value.Y); }
        }

        /// <summary>
        /// Size of the form
        /// </summary>
        public override Size Size
        {
            get
            {
                var s = internalForm.Size;
                return new Size(s.X, s.Y);
            }
            set { internalForm.Size = new SFML.System.Vector2u((uint)value.Width, (uint)value.Height); }
        }

        /// <summary>
        /// Cursor of the form (not implemented)
        /// </summary>
        public override Cursor Cursor
        {
            set
            {
                Debug.WriteLine(/*TODO*/"SFMLForm::Cursor is mot implemented yet. Use default cursor instead.");
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
        /// Open the form.
        /// </summary>
        public override void Open()
        {
            if(this.internalForm == null)
            {
                throw new InvalidOperationException("This form hasn't been created yet.");
            }

            this.internalForm = new SFML.Window.Window(this.internalForm.SystemHandle, this.internalForm.Settings);
            this.internalForm.SetVisible(true);
        }

        /// <summary>
        /// Close the form and distroy it.
        /// </summary>
        public override void Close()
        {
            this.internalForm.Close();
            this.internalForm.Dispose();
        }

        /// <summary>
        /// Show the form only if it is hiden
        /// </summary>
        public override void Show()
        {
            foreach (var control in Controls.Values)
            {
                control.NeedRepaint = true;
            }

            this.internalForm.SetVisible(true);
        }

        /// <summary>
        /// Hide the form only if it is shown
        /// </summary>
        public override void Hide()
        {
            this.internalForm.SetVisible(false);
        }


        /// <summary>
        /// Custom GUI Logic. This should be overrided to create custom GUI elements
        /// </summary>
        /// <param name="gui">gui context</param>
        /// <remarks>This is the only method that can be specified by user.</remarks>
        protected abstract void OnGUI(GUI gui);

        #region Internal Implementation

        internal SFML.Window.Window internalForm;

        #region the GUI Loop

        /// <summary>
        /// Form processor
        /// </summary>
        void FormProc()
        {
            var exit = false;
            while (exit == false)
            {
                Utility.MillisFrameBegin = Utility.Millis;
                OnBasicGUI(GUI);
                exit = Update();
                if (exit)
                {
                    CleanUp();
                    Close();
                }
                Render();
            }
        }

        private GUI GUI { get; set; }
        private RenderContext renderContext;

        /// <summary>
        /// When render context is ready (OnRenderContextReady), call this to initialize GUI
        /// </summary>
        /// <param name="context">custom context</param>
        private void InitGUI(object context)
        {
            var hdc = (IntPtr)context;

            //build all surface
            renderContext = new RenderContext();
            renderContext.BackSurface = CairoEx.BuildSurface((int)internalForm.Size.X, (int)internalForm.Size.Y,
                CairoEx.ColorWhite, Format.Rgb24);
            renderContext.BackContext = new Context(renderContext.BackSurface);
            renderContext.FrontSurface = new Win32Surface(hdc);
            renderContext.FrontContext = new Context(renderContext.FrontSurface);

            //create GUI
            GUI = new GUI(renderContext.BackContext, this);
        }

        protected void OnBasicGUI(GUI gui)
        {
            #region essential GUI logic

            //Draw essential form controls: title(maybe), menu, close button and so on.
            //TODO

            #endregion

            OnGUI(gui);
        }

        private bool Update()
        {
            //Input
            var clientRect = new Rect
            {
                X = this.Position.X,
                Y = this.Position.Y,
                Width = this.Size.Width,
                Height = this.Size.Height
            };
            var clientPos = this.PointToScreen(this.Position);
            Input.Mouse.Refresh((int)clientPos.X, (int)clientPos.Y, clientRect);
            Input.Keyboard.Refresh();

            //Control
            foreach (var control in Controls.Values)
            {
                control.OnUpdate();
            }

            return false;
        }

        private void Render()
        {
            foreach (var control in Controls.Values)
            {
                if (control.NeedRepaint)
                {
                    control.OnRender(renderContext.BackContext);
                    control.NeedRepaint = false;
                }
            }

            renderContext.SwapSurface();
        }

        private void CleanUp() //Cleanup only happened when ESC is pressed
        {
            if (renderContext.BackContext != null)
                renderContext.BackContext.Dispose();
        }
#endregion

#region Wrapper for hiding the SFML.Window.Window
        internal bool IsOpen { get { return internalForm.IsOpen; } }

        internal void DispatchEvents()
        {
            internalForm.DispatchEvents();
        }

        internal void Display()
        {
            internalForm.Display();
        }
#endregion

        /// <summary>
        /// Not used
        /// </summary>
        protected SFMLForm()
        {
            internalForm = new SFML.Window.Window(
                SFML.Window.VideoMode.DesktopMode,
                "DummyWindowTitle",
                SFML.Window.Styles.None);
        }

        #endregion
        

    }
}