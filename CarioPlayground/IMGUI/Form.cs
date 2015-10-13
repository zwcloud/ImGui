#define SHOW_FPS_Label

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Cairo;
using IMGUI.Input;
using Color = Cairo.Color;


//TODO make project independent of Winform(or only use Winform for creating window)

namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class Form : System.Windows.Forms.Form
    {
        private readonly Color windowBackgroundColor = CairoEx.ColorWhite;
        private Cursor cursor = Cursor.Default;
        private bool exit;
        internal Layer Layer;

        protected Form()
        {
            Layer = new Layer();
            Controls = new Dictionary<string, Control>(8);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            this.Shown += this.Form_Shown;
        }

        internal new Dictionary<string, Control> Controls { get; private set; }
        public GUI GUI { get; set; }

        protected override bool CanEnableIme
        {
            get { return true; }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public new Cursor Cursor
        {
            get { return cursor; }
            set
            {
                CursorChanged = true;
                cursor = value;
            }
        }

        public new bool CursorChanged { get; set; }

        private void Form_Shown(object ob, EventArgs ea)
        {
            //build all surface
            Layer.BackSurface = BuildSurface(ClientSize.Width, ClientSize.Height, CairoEx.ColorWhite, Format.Rgb24);
            Layer.BackContext = new Context(Layer.BackSurface);

            //Now, the hdc of form window can be acquired
            var hdc = Native.GetDC(Handle);
            Layer.FrontSurface = new Win32Surface(hdc);
            Layer.FrontContext = new Context(Layer.FrontSurface);

            //create GUI
            GUI = new GUI(Layer.BackContext);

            //Set up the game loop
            System.Windows.Forms.Application.Idle += (sender, e) =>
            {
                if(exit)
                {
                    CleanUp();
                    Close();
                }
                else
                {
                    while (Utility.IsApplicationIdle() && exit == false)
                    {
                        Utility.MillisFrameBegin = Utility.Millis;
                        Thread.Sleep(20); //Keep about 50fps
                        exit = Update();
                        Render();
#if DEBUG
                        //Show FPS and mouse position on the title
                        Text = String.Format("FPS: {0} Mouse ({1},{2})", fps, Mouse.MousePos.X,
                            Mouse.MousePos.Y);
#endif
                    }
                }
            };
        }

        protected abstract void OnGUI(GUI gui);
#if DEBUG
        private long lastFpSlog;
        private int frames;
        private int fps;
#endif

        #region helper for Cario

        /// <summary>
        /// Build a ImageSurface
        /// </summary>
        /// <param name="Width">width</param>
        /// <param name="Height">height</param>
        /// <param name="Color">color</param>
        /// <param name="format">surface format</param>
        /// <returns>the created ImageSurface</returns>
        private static ImageSurface BuildSurface(int Width, int Height, Color Color, Format format)
        {
            var surface = new ImageSurface(format, Width, Height);
            var c = new Context(surface);
            c.Rectangle(0, 0, Width, Height);
            c.SetSourceColor(Color);
            c.Fill();
            c.Dispose();
            return surface;
        }

        #endregion

        /// <summary>
        /// Paint all backend surfaces to the win32 window surface
        /// </summary>
        private void SwapSurfaceBuffer()
        {
            //Draw back surface to front surface
            Layer.BackSurface.Flush();
            Layer.FrontContext.SetSourceSurface(Layer.BackSurface, 0, 0);
            Layer.FrontContext.Paint();
        }

        #region Window loop

        protected override void WndProc(ref Message m)
        {
            var msg = (WM) m.Msg;
            switch (msg)
            {
                case WM.CHAR:
                    var c = (char) m.WParam;
                    if(Char.IsControl(c))
                        break;
                    Application.ImeBuffer.Enqueue(c);
                    break;
            }
            base.WndProc(ref m);
        }

        private new bool Update()
        {
            if(!Focused)
            {
                return false;
            }

            if(CursorChanged)
            {
                base.Cursor = Utility.GetFormCursor(Cursor);
                CursorChanged = false;
            }

            var clientRect = new Rect
            {
                X = ClientRectangle.Left,
                Y = ClientRectangle.Top,
                Width = ClientRectangle.Width,
                Height = ClientRectangle.Height
            };
            var clientPos = PointToScreen(ClientRectangle.Location);

            Mouse.Refresh(clientPos.X, clientPos.Y, clientRect);
            Keyboard.Refresh();

#if DEBUG
            //Quit when ESC is pressed
            if(Keyboard.KeyPressed(Key.Escape))
            {
                Debug.WriteLine("ESC pressed");
                return true;
            }
            DebugUpdate();
#endif
            foreach (var control in Controls.Values)
            {
                control.OnUpdate();
            }

            return false;
        }

        private void Render()
        {
            if(Layer.BackContext == null || Layer.FrontContext == null)
                return;
            OnGUI(GUI);

            foreach (var control in Controls.Values)
            {
                if(control.NeedRepaint)
                {
                    control.OnRender(Layer.BackContext);
                    control.NeedRepaint = false;
                }
            }

            SwapSurfaceBuffer();
        }

        private void DebugUpdate()
        {
            ++frames;
            var time = Utility.Millis;
            if(time > lastFpSlog + 1000)
            {
                fps = frames;
                frames = 0;
                lastFpSlog = time;
            }
        }

        private void CleanUp() //Cleanup only happened when ESC is pressed
        {
            foreach (var control in Controls.Values)
            {
                control.Dispose();
            }

            if(Layer.BackContext != null)
                Layer.BackContext.Dispose();
        }

        #endregion
    }
}