#define SHOW_FPS_Label

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Cairo;


//TODO make project independent of Winform(or only use Winform for creating window)
namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class Form : System.Windows.Forms.Form
    {
        #region GUI compontents
        public GUI GUI { get; set; }

        private readonly Color windowBackgroundColor = CairoEx.ColorWhite;
        #endregion

        #region GUI paramters
#if DEBUG
        long lastFpSlog;
        int frames;
        int fps;
#endif
        private Cursor cursor = IMGUI.Cursor.Default;
        #endregion

        protected Form()
        {
            #region Form members
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = new System.Drawing.Icon(@"W:\VS2013\CarioPlayground\IMGUIDemo\calc32x32.ico");
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Calculator";
            this.Shown += new System.EventHandler(this.Form_Shown);
            ResumeLayout(false);
            #endregion

        }

        bool exit = false;

        private void Form_Shown(object ob, EventArgs ea)
        {
            //build all surface
            Layer.BackSurface = BuildSurface(ClientSize.Width, ClientSize.Height, CairoEx.ColorWhite, Format.Rgb24);
            Layer.BackContext = new Context(Layer.BackSurface);
            Layer.TopSurface = BuildSurface(ClientSize.Width, ClientSize.Height, CairoEx.ColorWhite, Format.Argb32);
            Layer.TopContext = new Context(Layer.TopSurface);

            //Now, the hdc of form window can be acquired
            var hdc = Native.GetDC(Handle);
            Layer.FrontSurface = new Win32Surface(hdc);
            Layer.FrontContext = new Context(Layer.FrontSurface);

            //create GUI
            GUI = new GUI(Layer.BackContext, Layer.TopContext);

            //Set up the game loop
            System.Windows.Forms.Application.Idle += (sender, e) =>
            {
                if (exit)
                {
                    CleanUp();
                    Close();
                }
                else
                {
                    while (Utility.IsApplicationIdle() && exit == false)
                    {
                        Utility.MillisFrameBegin = Utility.Millis;
                        Thread.Sleep(20);//Keep about 30fps
                        exit = Update();
                        Render();
#if DEBUG
                        //Show FPS and mouse position on the title
                        Text = string.Format("FPS: {0} Mouse ({1},{2})", fps, Input.Mouse.MousePos.X,
                            Input.Mouse.MousePos.Y);
#endif
                    }
                }
            };
        }

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
            get
            {
                return cursor;
            }
            set
            {
                CursorChanged = true;
                cursor = value;
            }
        }

        public new bool CursorChanged { get; set; }

        #region helper for Cario

        /// <summary>
        /// Build a ImageSurface
        /// </summary>
        /// <param name="Width">width</param>
        /// <param name="Height">height</param>
        /// <param name="Color">color</param>
        /// <param name="format">surface format</param>
        /// <returns>the created ImageSurface</returns>
        static ImageSurface BuildSurface(int Width, int Height, Color Color, Format format)
        {
            ImageSurface surface = new ImageSurface(format, Width, Height);
            var c = new Context(surface);
            c.Rectangle(0, 0, Width, Height);
            c.SetSourceColor(Color);
            c.Fill();
            c.Dispose();
            return surface;
        }

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
        #endregion

        #region application architecture

        protected override void WndProc(ref Message m)
        {
            var msg = (WM)m.Msg;
            switch (msg)
            {
                case WM.CHAR:
                    char c = (char)m.WParam;
                    if(char.IsControl(c))
                        break;
                    Application.ImeBuffer.Enqueue(c);
                    break;
            }
            base.WndProc(ref m);
        }

        new bool Update()
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

            Input.Mouse.Refresh(clientPos.X, clientPos.Y, clientRect);
            Input.Keyboard.Refresh();

            //Quit when ESC is pressed
            if(Input.Keyboard.KeyPressed(Key.Escape))
            {
                Debug.WriteLine("ESC pressed");
                return true;
            }
#if DEBUG
            DebugUpdate();
#endif
            foreach (var control in Control.Controls.Values)
            {
                control.OnUpdate();
            }

            return false;
        }

        void Render()
        {
            if(Layer.BackContext == null || Layer.FrontContext == null || Layer.TopContext == null)
                return;
            OnGUI(GUI);

            foreach (var control in Control.Controls.Values)
            {
                if (control.NeedRepaint)
                {
                    control.OnRender(Layer.BackContext);
                    control.NeedRepaint = false;
                }
            }

            SwapSurfaceBuffer();
        }

        void DebugUpdate()
        {
            ++frames;
            long time = Utility.Millis;
            if(time > lastFpSlog + 1000)
            {
                fps = frames;
                frames = 0;
                lastFpSlog = time;
            }
        }

        private void CleanUp()
        {
            if(Layer.BackContext != null)
                Layer.BackContext.Dispose();
            if (Layer.TopContext != null)
                Layer.TopContext.Dispose();
        }

        #endregion

        protected abstract void OnGUI(GUI gui);
    }
}