using Cairo;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//TODO extract interface of window functions

namespace IMGUI
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
        public override bool Focused
        {
            get { return internalForm.HasFocus(); }
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
            if (!this.Visible)
            {
                foreach (var control in Controls.Values)
                {
                    control.NeedRepaint = true;
                }

                this.internalForm.SetVisible(true);
                this.Visible = true;
            }
        }

        /// <summary>
        /// Hide the form only if it is shown
        /// </summary>
        public override void Hide()
        {
            if (this.Visible)
            {
                this.internalForm.SetVisible(false);
                this.Visible = false;
            }
        }

        /// <summary>
        /// Is this form visible?
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// Custom GUI Logic. This should be overrided to create custom GUI elements
        /// </summary>
        /// <param name="gui">gui context</param>
        /// <remarks>This is the only method that can be specified by user.</remarks>
        protected abstract void OnGUI(GUI gui);

        #region Internal Implementation

        internal SFML.Window.Window internalForm;

        internal GUIRenderer guiRenderer;
        
        internal override object InternalForm
        {
            get { return internalForm; }
        }

        internal Surface FrontSurface { get { return renderContext.FrontSurface; } }

        #region the GUI Loop

        /// <summary>
        /// GUI Loop
        /// </summary>
        /// <returns>true:need repaint / false: not repaint</returns>
        internal bool GUILoop()
        {
            OnBasicGUI(GUI);
            var isRepaint = false;
            var exit = Update();
            if (exit)
            {
                CleanUp();
                Close();
                //TODO tell form to exit
            }

            if (this.Visible)
            {
                isRepaint = Render();
            }

            return isRepaint;
        }
        
        private GUI GUI { get; set; }
        private RenderContext renderContext;

        /// <summary>
        /// Call this to initialize GUI
        /// </summary>
        private void InitGUI()
        {
            var clientWidth = (int) internalForm.Size.X;
            var clientHeight = (int) internalForm.Size.Y;

            //build all surface
            renderContext = new RenderContext();
            renderContext.BackSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorWhite, Format.Argb32);
            renderContext.BackContext = new Context(renderContext.BackSurface);
            renderContext.FrontSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorWhite, Format.Argb32);
            renderContext.FrontContext = new Context(renderContext.FrontSurface);

            //create GUI
            GUI = new GUI(renderContext.BackContext, this);
        }

        protected void OnBasicGUI(GUI gui)
        {
            OnGUI(gui);
        }

        private readonly List<string> removeList = new List<string>();

        private bool Update()
        {
            foreach (var control in Controls.Values)
            {
                if(control.Active)
                {
                    control.OnUpdate();
                    control.Active = false;
                }
                else
                {
                    control.Dispose();
                    removeList.Add(control.Name);
                    control.OnClear(renderContext.BackContext);
                }
            }

            foreach (var controlName in removeList)
            {
                bool result = Controls.Remove(controlName);
                Debug.WriteLineIf(!result, "Remove failed");
            }
            removeList.Clear();

            return false;
        }

        private bool Render()
        {
            bool renderHappend = false;
            foreach (var control in Controls.Values)
            {
                if (control.NeedRepaint)
                {
                    renderHappend = true;
                    control.OnRender(renderContext.BackContext);
                    control.NeedRepaint = false;
                }
            }

            if(renderHappend)
            {
                renderContext.SwapSurface();
            }

            return renderHappend;
        }

        private void CleanUp() //Cleanup only happens when ESC is pressed
        {
            if (renderContext.BackContext != null)
                renderContext.BackContext.Dispose();
        }
#endregion
        
        protected SFMLForm(int width, int height)
        {
            internalForm = new SFML.Window.Window(
                new SFML.Window.VideoMode((uint)width, (uint)height),
                "DummyWindowTitle",
                SFML.Window.Styles.None, new SFML.Window.ContextSettings
                {
                    DepthBits = 24,
                    StencilBits = 0,
                    AntialiasingLevel = 0,
                    MajorVersion = 3,
                    MinorVersion = 3
                });
            //form is not show on creating
            internalForm.SetVisible(false);
            Visible = false;

            internalForm.SetVerticalSyncEnabled(true);
            controls = new Dictionary<string, Control>();
            InitGUI();
        }

        #endregion
        

    }
}