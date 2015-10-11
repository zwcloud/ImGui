#define SHOW_FPS_Label

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Cairo;
using IMGUI.Input;
using Color = Cairo.Color;


//TODO make project independent of Winform(or only use Winform for creating window)
namespace IMGUI
{
    [DesignerCategory("")]
    public abstract class Form : System.Windows.Forms.Form
    {
        internal new Dictionary<string, Control> Controls { get; set; }

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
        private Cursor cursor = Cursor.Default;
        #endregion

        protected Form()
        {
            #region Form members
            AutoScaleDimensions = new SizeF(6F, 12F);
            AutoScaleMode = AutoScaleMode.Font;
            
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            this.Shown += new EventHandler(this.Form_Shown);
            ResumeLayout(false);
            #endregion

            Controls = new Dictionary<string, Control>(8);
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
                        Thread.Sleep(20);//Keep about 50fps
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
                    if(Char.IsControl(c))
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

        void Render()
        {
            if(Layer.BackContext == null || Layer.FrontContext == null || Layer.TopContext == null)
                return;
            OnGUI(GUI);

            foreach (var control in Controls.Values)
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

        private void CleanUp()//Cleanup only happened when ESC is pressed
        {
            foreach (var control in Controls.Values)
            {
                control.Dispose();
            }

            if(Layer.BackContext != null)
                Layer.BackContext.Dispose();
            if (Layer.TopContext != null)
                Layer.TopContext.Dispose();
        }

        #endregion

        protected abstract void OnGUI(GUI gui);
    }
}