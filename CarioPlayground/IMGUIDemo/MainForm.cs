using System;
using System.Windows.Forms;
using IMGUI;
using Win32;
using Cairo;
using System.Diagnostics;

namespace WinFormCario
{
    public partial class MainForm : Form
    {
        Context g;
        ImageSurface BackSurface = BuildSurface(512, 512, new Color(1, 1, 1, 1));

        Context context;
        Surface FrontSurface;

        long lastFPSlog = 0;
        int frames = 0;
        int fps = 0;

        GUI gui;
        bool buttonClicked = false;

        public MainForm()
        {
            InitializeComponent();

            //Set up the game architecture
            Application.Idle += (sender, e) => 
            {
                if (gui == null)
                {
                    Init();
                }
                else
                {
                    while (Utility.IsApplicationIdle())
                    {
                        Update();
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
            if (gui.Button(Style.Default, 20, 20, 120, 40, "button!"))
            {
                Debug.WriteLine("button! clicked");
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
        static ImageSurface BuildSurface(int Width, int Height, Color Color)
        {
            Context _context = null;
            ImageSurface _surface = new ImageSurface(Format.Argb32, Width, Height);

            _context = new Context(_surface);

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
            g.SetSourceRGBA(1, 1, 1, 1);
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

        new void Update()
        {
            var clientRect = new RECT();
            clientRect.Left = this.ClientRectangle.Left;
            clientRect.Top = this.ClientRectangle.Top;
            clientRect.Right = this.ClientRectangle.Right;
            clientRect.Bottom = this.ClientRectangle.Bottom;
            var clientPos = this.PointToScreen(this.ClientRectangle.Location);

            Input.Refresh(clientPos.X, clientPos.Y, clientRect);

            //Debug.WriteLine("Mouse at {0},{1}", Input.MousePos.X, Input.MousePos.Y);

            if (Input.KeyPressed(Key.Escape))
                System.Diagnostics.Debug.WriteLine("ESC pressed");

            if (Input.LeftButtonClicked)
                System.Diagnostics.Debug.WriteLine("Mouse Left Button clicked");

            ++frames;
            long time = Utility.Millis;
            if (time > lastFPSlog + 1000)
            {
                fps = frames;
                frames = 0;
                lastFPSlog = time;
            }
        }

        void Render()
        {
            if (g == null || context == null) return;

            g.Antialias = Antialias.None;    //fastest method but low quality
            g.LineWidth = 7;
            g.MoveTo(10, 90);
            g.LineTo(40, 140);
            g.Stroke();

            OnGUI();
            PaintToWindow();
        }

        private void CleanUp()
        {
            if (g != null)
                g.Dispose();
        }

        #endregion

    }
}
