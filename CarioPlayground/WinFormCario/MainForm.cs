using System;
using System.Windows.Forms;
using Cairo;
using Win32;

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

        private ImageSurface imageSurface;

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
            }

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

            g.Antialias = Antialias.None;    //fastest method but low quality

            PointD p1, p2, p3, p4;
            p1 = new PointD(10, 10);
            p2 = new PointD(100, 10);
            p3 = new PointD(100, 100);
            p4 = new PointD(10, 100);


            {
                g.SetSourceSurface(imageSurface, 0, 0);
                g.Paint();
            }

            {
                LinearGradient gradient = new LinearGradient(0,10, 0, 100);
                gradient.AddColorStop(0, new Color(0.87, 0.93, 0.96));
                gradient.AddColorStop(1, new Color(0.65, 0.85, 0.96));
                g.SetSource(gradient);
                g.MoveTo(p1);
                g.LineTo(p2);
                g.LineTo(p3);
                g.LineTo(p4);
                g.LineTo(p1);
                g.ClosePath();
                g.Fill();
                gradient.Dispose();
            }

            g.MoveTo(new PointD(0, this.ClientSize.Height-30));
            g.SetFontSize(30.0f);
            g.SetSourceColor(new Color(0,0,0));
            g.ShowText(string.Format("FPS: {0}", fps));
        }

        static ImageSurface build_Surface(int Width, int Height, Color Color)
        {
            Context _context = null;
            ImageSurface _surface = new ImageSurface(Format.Rgb24, Width, Height);

            _context = new Context(_surface);

            _context.Rectangle(0, 0, Width, Height);
            _context.SetSourceRGBA(Color.R, Color.G, Color.B, Color.A);
            _context.Fill();

            _context.Dispose();

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
            imageSurface = new ImageSurface("W:/VS2013/CarioPlayground/Resources/Toggle.Off.png");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (g != null)
                g.Dispose();
            if( context!=null )
                context.Dispose();
        }


    }
}
