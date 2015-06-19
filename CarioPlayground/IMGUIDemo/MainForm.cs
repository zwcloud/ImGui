using System;
using System.Windows.Forms;
using IMGUI;
using Win32;
using System.Diagnostics;
using Cairo;

using Point = IMGUI.Point;

namespace WinFormCario
{
    public partial class MainForm : Form
    {
        Cairo.Context g;
        Cairo.ImageSurface BackSurface = BuildSurface(512, 512, new Color(1, 1, 1, 1));

        Cairo.Context context;
        Cairo.Surface FrontSurface;

        long lastFPSlog = 0;
        int frames = 0;
        int fps = 0;

        GUI gui;
        private Color WindowBackgroundColor = new Color(0x6A/255.0, 0x6A/255.0, 0x6A/255.0);

        public MainForm()
        {
            InitializeComponent();

            bool exit = false;
            //Set up the game architecture
            Application.Idle += (sender, e) =>
            {
                if (exit)
                {
                    this.Close();
                }
                if (gui == null)
                {
                    Init();
                }
                else
                {
                    while (Utility.IsApplicationIdle() && exit == false)
                    {
                        System.Threading.Thread.Sleep(30);
                        exit = Update();
                        Render();
                    }
                }
            };
        }

        /// <summary>
        /// GUI logic here
        /// </summary>
        void OnGUI()
        {
            gui.Label(
                new Rect(0, ClientRectangle.Bottom - g.FontExtents.Height, 200, g.FontExtents.Height),
                string.Format("FPS: {0}", fps)
                );

            if (gui.Button(new Rect(new Point(20, 20), new Point(120, 40)), "button 0!"))
            {
                Debug.WriteLine("button 0 clicked!");
            }
            
            if (gui.Button(new Rect(new Point(20, 42), new Point(120, 62)), "button 1!"))
            {
                Debug.WriteLine("button 1 clicked!");
            }

            if (gui.Button(new Rect(new Point(20, 64), new Point(120, 84)), "button 2!"))
            {
                Debug.WriteLine("button 2 clicked!");
            }
        }

        /// <summary>
        /// Called when the form is closed
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>Do clean up here</remarks>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CleanUp();
        }

        #region helper for Cario
        /// <summary>
        /// Build a ImageSurface
        /// </summary>
        /// <param name="Width">width</param>
        /// <param name="Height">height</param>
        /// <param name="Color">color</param>
        /// <returns>the created ImageSurface</returns>
        static Cairo.ImageSurface BuildSurface(int Width, int Height, Color Color)
        {
            Cairo.Context _context = null;
            Cairo.ImageSurface _surface = new Cairo.ImageSurface(Cairo.Format.Argb32, Width, Height);

            _context = new Cairo.Context(_surface);

            _context.Rectangle(0, 0, Width, Height);
            _context.SetSourceColor(Color);
            _context.Fill();

            return _surface;
        }

        /// <summary>
        /// Paint g(the image surface) to context(the win32 window surface)
        /// </summary>
        private void PaintToWindow()
        {
            context.Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            context.SetSource(BackSurface);
            context.Fill();

            // Any additional rendering here
            g.Save();
            g.SetSourceColor(WindowBackgroundColor);
            g.Operator = Operator.Source;
            g.Paint();
            g.Restore();
        }
        #endregion

        #region game architecture
        private void Init()
        {
            if (g == null)
            {
                g = new Context(BackSurface);
                var hdc = Native.GetDC(this.Handle);
                FrontSurface = new Win32Surface(hdc);
                context = new Context(FrontSurface);
                gui = new GUI(g);
            }
        }

        new bool Update()
        {
            var clientRect = new RECT();
            clientRect.Left = this.ClientRectangle.Left;
            clientRect.Top = this.ClientRectangle.Top;
            clientRect.Right = this.ClientRectangle.Right;
            clientRect.Bottom = this.ClientRectangle.Bottom;
            var clientPos = this.PointToScreen(this.ClientRectangle.Location);

            Input.Refresh(clientPos.X, clientPos.Y, clientRect);

            //Debug.WriteLine("Mouse at {0},{1}", Input.MousePos.X, Input.MousePos.Y);

            //Quit when ESC is pressed
            if (Input.KeyPressed(Key.Escape))
            {
                System.Diagnostics.Debug.WriteLine("ESC pressed");
                return true;
            }

#if DEBUG
            DebugUpdate();
#endif

            return false;
        }

        void Render()
        {
            if (g == null || context == null) return;

            OnGUI();

            PaintToWindow();
        }

        void DebugUpdate()
        {
            ++frames;
            long time = Utility.Millis;
            if (time > lastFPSlog + 1000)
            {
                fps = frames;
                frames = 0;
                lastFPSlog = time;
            }
        }

        private void CleanUp()
        {
            if (g != null)
                g.Dispose();
        }

        #endregion

    }
}
