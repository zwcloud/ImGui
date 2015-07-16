using System.Diagnostics;
using System.Threading;
using Cairo;

namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class Form : System.Windows.Forms.Form
    {
        #region GUI compontents
        public GUI debugGui;
        // ReSharper disable once InconsistentNaming
        public GUI gui { get; set; }
        
        private readonly Color windowBackgroundColor = CairoEx.ColorRgb(0x6A, 0x6A, 0x6A);
        #endregion

        #region GUI paramters
#if DEBUG
        long lastFpSlog;
        int frames;
        int fps;
#endif
        #endregion

        protected Form()
        {
            #region Form members
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(284, 262);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = new System.Drawing.Icon(@"W:\VS2013\CarioPlayground\IMGUIDemo\calc32x32.ico");
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Calculator";
            ResumeLayout(false);
            #endregion

            Layer.BackSurface = BuildSurface(ClientSize.Width, ClientSize.Height, CairoEx.ColorWhite);
            Layer.TopSurface = BuildSurface(ClientSize.Width, ClientSize.Height, new Color(0,0,0,0));

            InitIMGUI();
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #region helper for Cario
        /// <summary>
        /// Build a ImageSurface
        /// </summary>
        /// <param name="Width">width</param>
        /// <param name="Height">height</param>
        /// <param name="Color">color</param>
        /// <returns>the created ImageSurface</returns>
        static ImageSurface BuildSurface(int Width, int Height, Color Color)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, Width, Height);
            var c = new Context(surface);
            c.Rectangle(0, 0, Width, Height);
            c.SetSourceColor(Color);
            c.Fill();
            c.Dispose();
            return surface;
        }

        /// <summary>
        /// Paint g(the image surface) to context(the win32 window surface)
        /// </summary>
        private void SwapSurfaceBuffer()
        {
            //Draw top surface to back surface
            Layer.BackContext.Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            Layer.BackContext.SetSource(Layer.TopSurface);
            Layer.BackContext.Operator = Operator.Atop;
            Layer.BackContext.Fill();

            //Draw back surface to front surface (from bottom surface to top surface)
            Layer.FrontContext.Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            Layer.FrontContext.SetSource(Layer.BackSurface);
            Layer.FrontContext.Fill();
        }
        #endregion

        #region application architecture

        internal void InitIMGUI()
        {
            bool exit = false;
            //Set up the game architecture
            System.Windows.Forms.Application.Idle += (sender, e) =>
            {
                if(exit)
                {
                    CleanUp();
                    Close();
                }
                if(gui == null)
                {
                    Init();
                }
                else
                {
                    while(Utility.IsApplicationIdle() && exit == false)
                    {
                        Thread.Sleep(30);//Keep about 30fps
                        exit = Update();
                        Render();
                    }
                }
            };
        }

        private void Init()
        {
            Layer.BackContext = new Context(Layer.BackSurface);
            var hdc = Native.GetDC(Handle);
            Layer.FrontSurface = new Win32Surface(hdc);
            Layer.FrontContext = new Context(Layer.FrontSurface);
            Layer.TopContext  = new Context(Layer.TopSurface);
            debugGui = new GUI(Layer.BackContext, Layer.TopContext);
            gui = new GUI(Layer.BackContext, Layer.TopContext);
        }

        new bool Update()
        {
            var clientRect = new Rect
            {
                X = ClientRectangle.Left,
                Y = ClientRectangle.Top,
                Width = ClientRectangle.Width,
                Height = ClientRectangle.Height
            };
            var clientPos = PointToScreen(ClientRectangle.Location);

            Input.Refresh(clientPos.X, clientPos.Y, clientRect);

            //Quit when ESC is pressed
            if(Input.KeyPressed(Key.Escape))
            {
                Debug.WriteLine("ESC pressed");
                return true;
            }

#if DEBUG
            DebugUpdate();
#endif

            return false;
        }

        void Render()
        {
            if(Layer.BackContext == null || Layer.FrontContext == null || Layer.TopContext == null)
                return;

            //Clear back surface
            Layer.BackContext.SetSourceColor(windowBackgroundColor);
            Layer.BackContext.Paint();

            //Clear Top surface
            Layer.TopContext.Save();
            Layer.TopContext.SetSourceColor(new Color(0,0,0,0));
            Layer.TopContext.Operator = Operator.Source;
            Layer.TopContext.Paint();
            Layer.TopContext.Restore();

#if DEBUG
            debugGui.Label(
                new Rect(0, ClientRectangle.Bottom - Layer.BackContext.FontExtents.Height, 200, Layer.BackContext.FontExtents.Height),
                string.Format("FPS: {0} Mouse ({1},{2})", fps, Input.MousePos.X, Input.MousePos.Y),
                "DebugInfoLabel"
                );
#endif

            OnGUI(gui);

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
        }

        #endregion

        protected abstract void OnGUI(GUI gui);
    }
}