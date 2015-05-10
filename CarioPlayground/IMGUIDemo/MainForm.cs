using System;
using System.Windows.Forms;
using IMGUI;
using Win32;
using Cairo;

namespace WinFormCario
{
    public partial class MainForm : Form
    {
        Context g;
        ImageSurface BackSurface = build_Surface(512, 512, new Color(1, 1, 1, 1));

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
            Application.Idle += HandleApplicationIdle;
        }

        void HandleApplicationIdle (object sender, EventArgs e)
        {
            while (Utility.IsApplicationIdle())
            {
                Update();
                Render();
            }
        }

        new void Update()
        {
            if (g == null)
            {
                g = new Context(BackSurface);
                if (this.IsHandleCreated)
                {
                    var hdc = Native.GetDC(this.Handle);
                    FrontSurface = new Win32Surface(hdc);
                    context = new Context(FrontSurface);
                }
                gui = new GUI(g);
            }

            if (Input.KeyDown(Key.Escape))
                System.Diagnostics.Debug.WriteLine("ESC pressed");

            var clientRect = new RECT();
            clientRect.Left = this.ClientRectangle.Left;
            clientRect.Top = this.ClientRectangle.Top;
            clientRect.Right = this.ClientRectangle.Right;
            clientRect.Bottom = this.ClientRectangle.Bottom;
            Input.Refresh(clientRect);

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
            SwapSurface();
        }

        void OnGUI()
        {
            buttonClicked = gui.Button(Style.Default, 20, 20, 120, 40, "button!");
        }

        static ImageSurface build_Surface(int Width, int Height, Color Color)
        {
            Context _context = null;
            ImageSurface _surface = new ImageSurface(Format.Argb32, Width, Height);

            _context = new Context(_surface);

            _context.Rectangle(0, 0, Width, Height);
            _context.SetSourceColor(Color);
            _context.Fill();

            return _surface;
        }

        private void SwapSurface()
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

        protected override void OnLoad(EventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (g != null)
                g.Dispose();
        }


    }
}
