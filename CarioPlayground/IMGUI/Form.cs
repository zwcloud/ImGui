using System.Diagnostics;
using System.Threading;
using Cairo;

namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class Form : System.Windows.Forms.Form
    {
        #region GUI compontents
        // ReSharper disable once InconsistentNaming
        // for short
        public GUI debugGui;
        // ReSharper disable once InconsistentNaming
        public GUI gui { get; set; }

        // ReSharper disable once InconsistentNaming
        public Context g;
        private Context _context;
        internal DualSurfaceLayer MainLayer { get; set; }
        private readonly Color _windowBackgroundColor = CairoEx.ColorRgb(0x6A, 0x6A, 0x6A);
        #endregion

        #region GUI paramters
        long _lastFpSlog;
        int _frames;
        int _fps; 
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

            MainLayer = new DualSurfaceLayer
            {
                BackSurface = BuildSurface(ClientSize.Width, ClientSize.Height, CairoEx.ColorWhite)
            };

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
            Context context = null;
            ImageSurface surface = new ImageSurface(Format.Argb32, Width, Height);

            context = new Context(surface);

            context.Rectangle(0, 0, Width, Height);
            context.SetSourceColor(Color);
            context.Fill();

            return surface;
        }

        /// <summary>
        /// Paint g(the image surface) to context(the win32 window surface)
        /// </summary>
        private void PaintToWindow()
        {
            _context.Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            _context.SetSource(MainLayer.BackSurface);
            _context.Fill();

            // Any additional rendering here
            g.Save();
            g.SetSourceColor(_windowBackgroundColor);
            g.Operator = Operator.Source;
            g.Paint();
            g.Restore();
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
                    Close();
                }
                if(debugGui == null)
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
            if(g == null)
            {
                g = new Context(MainLayer.BackSurface);
                var hdc = Native.GetDC(Handle);
                MainLayer.FrontSurface = new Win32Surface(hdc);
                _context = new Context(MainLayer.FrontSurface);
                debugGui = new GUI(g);
                gui = new GUI(g);
            }
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

            //Debug.WriteLine("Mouse at {0},{1}", Input.MousePos.X, Input.MousePos.Y);

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
            if(g == null || _context == null) return;

            debugGui.Label(
                new Rect(0, ClientRectangle.Bottom - g.FontExtents.Height, 200, g.FontExtents.Height),
                string.Format("FPS: {0} Mouse ({1},{2})", _fps, Input.MousePos.X, Input.MousePos.Y)
                );

            OnGUI(gui);

            PaintToWindow();
        }

        void DebugUpdate()
        {
            ++_frames;
            long time = Utility.Millis;
            if(time > _lastFpSlog + 1000)
            {
                _fps = _frames;
                _frames = 0;
                _lastFpSlog = time;
            }
        }

        private void CleanUp()
        {
            if(g != null)
                g.Dispose();
        }

        #endregion

        protected abstract void OnGUI(GUI gui);
    }
}