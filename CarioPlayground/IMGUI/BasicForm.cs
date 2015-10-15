using System;
using System.Collections.Generic;
using System.Threading;
using Cairo;
using IMGUI.Input;

namespace IMGUI
{
    /// <summary>
    /// Basic form without any gui logic
    /// </summary>
    public abstract class BasicForm
    {
        public Point Position
        {
            get
            {
                var l = InternalForm.Location;
                return new Point(l.X, l.Y);
            }
            set { InternalForm.Location = new System.Drawing.Point((int) value.X, (int) value.Y); }
        }

        public Size Size
        {
            get
            {
                var s = InternalForm.Size;
                return new Size(s.Width, s.Height);
            }
            set { InternalForm.Size = new System.Drawing.Size((int) value.Width, (int) value.Height); }
        }

        public Cursor Cursor
        {
            set
            {
                switch (value)
                {
                    case Cursor.Default:
                        InternalForm.Cursor = System.Windows.Forms.Cursors.Default;
                        break;
                    case Cursor.Text:
                        InternalForm.Cursor = System.Windows.Forms.Cursors.IBeam;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool Visible { get { throw new NotImplementedException(); } }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        private void Close()
        {
            this.InternalForm.Close();
        }

        /// <summary>
        /// Custom GUI Logic. This should be overrided to create custom GUI elements
        /// </summary>
        /// <param name="gui">gui context</param>
        /// <remarks>This is the only method that can be specified by user.</remarks>
        protected abstract void OnGUI(GUI gui);

        #region Internal Implementation

        private class ImeEnabledForm : System.Windows.Forms.Form
        {
            protected override bool CanEnableIme
            {
                get { return true; }
            }

            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                var msg = (WM)m.Msg;
                switch (msg)
                {
                    case WM.CHAR:
                        var c = (char)m.WParam;
                        if (Char.IsControl(c))
                            break;
                        Application.ImeBuffer.Enqueue(c);//Should IME buffer be shared among forms
                        break;
                }
                base.WndProc(ref m);
            }
        }

        private readonly ImeEnabledForm form;

        private readonly Dictionary<string, Control> controls;

        internal Dictionary<string, Control> Controls
        {
            get { return controls; }
        }

        protected BasicForm()
        {
            form = new ImeEnabledForm
            {
                BackColor = System.Drawing.Color.White,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable,
                StartPosition = System.Windows.Forms.FormStartPosition.Manual
            };
            
            form.Shown += (sender, args) =>
            {
                //Now, the hdc of form window can be acquired
                var hdc = Native.GetDC(form.Handle);

                InitGUI(hdc);

                //Set up the IMGUI loop
                System.Windows.Forms.Application.Idle += (o, eventArgs) =>
                {
                    var exit = false;
                    while (Utility.IsApplicationIdle() && exit == false)
                    {
                        Utility.MillisFrameBegin = Utility.Millis;
                        Thread.Sleep(20); //Keep about 50fps
                        OnBasicGUI(GUI);
                        exit = Update();
                        if (exit)
                        {
                            CleanUp();
                            Close();
                        }
                        Render();
                    }
                };
            };

            controls = new Dictionary<string, Control>();
        }

        private GUI GUI { get; set; }

        internal System.Windows.Forms.Form InternalForm
        {
            get { return form; }
        }

        private RenderContext renderContext;

        /// <summary>
        /// When render context is ready (OnRenderContextReady), call this to initialize GUI
        /// </summary>
        /// <param name="context">custom context</param>
        private void InitGUI(object context)
        {
            var hdc = (IntPtr) context;

            //build all surface
            renderContext = new RenderContext();
            renderContext.BackSurface = CairoEx.BuildSurface(InternalForm.ClientSize.Width, InternalForm.ClientSize.Height,
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

            //Draw essential form controls: title, menu, close button and so on.
            //TODO

            #endregion

            OnGUI(gui);
        }

        private bool Update()
        {
            //Input
            var clientRect = new Rect
            {
                X = InternalForm.ClientRectangle.Left,
                Y = InternalForm.ClientRectangle.Top,
                Width = InternalForm.ClientRectangle.Width,
                Height = InternalForm.ClientRectangle.Height
            };
            var clientPos = InternalForm.PointToScreen(InternalForm.ClientRectangle.Location);
            Mouse.Refresh(clientPos.X, clientPos.Y, clientRect);
            Keyboard.Refresh();

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
                if(control.NeedRepaint)
                {
                    control.OnRender(renderContext.BackContext);
                    control.NeedRepaint = false;
                }
            }

            renderContext.SwapSurface();
        }

        private void CleanUp() //Cleanup only happened when ESC is pressed
        {
            if(renderContext.BackContext != null)
                renderContext.BackContext.Dispose();
        }

        #endregion
    }
}