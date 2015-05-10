using System;
using System.Windows.Forms;
using Cairo;
using IMGUI;

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
                    var hdc = Native.Win32.GetDC(this.Handle);
                    FrontSurface = new Win32Surface(hdc);
                    context = new Context(FrontSurface);
                }
            }

            Input.GetKeyStates();

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

            BeginRender();

            g.Antialias = Antialias.Subpixel;    // sets the anti-aliasing method
            g.LineWidth = 9;          // sets the line width
            g.SetSourceRGBA(0, 0, 0, 1);   // red, green, blue, alpha
            g.MoveTo(10, 10);          // sets the Context's start point.
            g.LineTo(40, 60);          // draws a "virtual" line from 5,5 to 20,30
            g.Stroke();          //stroke the line to the surface

            g.Antialias = Antialias.Gray;
            g.LineWidth = 8;
            g.SetSourceRGBA(0.7, 0.3, 0, 1);
            g.LineCap = LineCap.Round;
            g.MoveTo(10, 50);
            g.LineTo(40, 100);
            g.Stroke();

            g.Antialias = Antialias.None;    //fastest method but low quality
            g.LineWidth = 7;
            g.MoveTo(10, 90);
            g.LineTo(40, 140);
            g.Stroke();

            PointD p1, p2, p3, p4;
            p1 = new PointD(10, 10);
            p2 = new PointD(100, 10);
            p3 = new PointD(100, 100);
            p4 = new PointD(10, 100);

            g.MoveTo(p1);
            g.LineTo(p2);
            g.LineTo(p3);
            g.LineTo(p4);
            g.LineTo(p1);
            g.ClosePath();
            g.Stroke();

            g.MoveTo(new PointD(0, this.ClientSize.Height-30));
            g.SetFontSize(24.0);
            g.ShowText(string.Format("FPS: {0}", fps));
            g.Stroke();
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

        private void BeginRender()
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
