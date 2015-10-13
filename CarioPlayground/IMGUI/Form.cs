#define SHOW_FPS_Label

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cairo;
using IMGUI.Input;


//TODO make project independent of Winform(or only use Winform for creating window)

namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class Form : System.Windows.Forms.Form
    {
        private Cursor cursor = Cursor.Default;
        private bool exit;
        private RenderContext renderContext;

        protected Form()
        {
            renderContext = new RenderContext();
            Controls = new Dictionary<string, Control>(8);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
            //Now, the hdc of form window can be acquired
            var hdc = Native.GetDC(Handle);

            InitGUI(hdc);

            //Set up the IMGUI loop
            System.Windows.Forms.Application.Idle += OnIdle;
        }

        private void InitGUI(object context)
        {
            var hdc = (IntPtr)context;

            //build all surface
            renderContext.BackSurface = CairoEx.BuildSurface(ClientSize.Width, ClientSize.Height, CairoEx.ColorWhite, Format.Rgb24);
            renderContext.BackContext = new Context(renderContext.BackSurface);
            renderContext.FrontSurface = new Win32Surface(hdc);
            renderContext.FrontContext = new Context(renderContext.FrontSurface);

            //create GUI
            GUI = new GUI(renderContext.BackContext);
        }

        private void OnIdle(object sender, EventArgs e)
        {
            WindowLoop();
        }

        private void WindowLoop()
        {
            while (Utility.IsApplicationIdle() && exit == false)
            {
                Utility.MillisFrameBegin = Utility.Millis;
                Thread.Sleep(20); //Keep about 50fps
                exit = Update();
                if(exit)
                {
                    CleanUp();
                    Close();
                }
                Render();
#if DEBUG
                //Show FPS and mouse position on the title
                Text = string.Format("FPS: {0} Mouse ({1},{2})", fps, Mouse.MousePos.X, Mouse.MousePos.Y);
#endif
            }
        }


        protected abstract void OnGUI(GUI gui);
#if DEBUG
        private long lastFpSlog;
        private int frames;
        private int fps;
#endif


        #region Window loop internal

        private void SwapSurfaceBuffer()
        {
            renderContext.SwapSurface();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
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
            if(!renderContext.IsReady)
                return;
            OnGUI(GUI);

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

            if(renderContext.BackContext != null)
                renderContext.BackContext.Dispose();
        }

        #endregion
    }
}