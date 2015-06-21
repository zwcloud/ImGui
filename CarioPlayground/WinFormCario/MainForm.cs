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

#if false
            g.SetSourceColor(new Color(0,1,0));
            g.MoveTo(p1);
            g.LineTo(p2);
            g.LineTo(p3);
            g.LineTo(p4);
            g.LineTo(p1);
            g.ClosePath();
            var extentsRectangle = g.StrokeExtents();
            g.Fill();

            g.SetSourceColor(new Color(0,0,0));
            g.LineWidth = 1;
            //Top extents
            g.MoveTo(extentsRectangle.X, extentsRectangle.Y);
            g.LineTo(extentsRectangle.X + extentsRectangle.Width, extentsRectangle.Y);
            //Right extents
            g.LineTo(extentsRectangle.X + extentsRectangle.Width, extentsRectangle.Y + extentsRectangle.Height);
            //Bottom extents
            g.LineTo(extentsRectangle.X, extentsRectangle.Y + extentsRectangle.Height);
            //Left extents
            g.LineTo(extentsRectangle.X, extentsRectangle.Y);
            g.ClosePath();
            g.Stroke();
            
            g.SetSourceColor(new Color(1,0,0));
            p1.X += 100;
            p2.X += 100;
            p3.X += 100;
            p4.X += 100;
            g.MoveTo(p1);
            g.LineTo(p2);
            g.LineTo(p3);
            g.LineTo(p4);
            g.LineTo(p1);
            //g.ClosePath();
            //g.Fill();

            
            var topColor = new Color(1,0,0);
            var rightColor = new Color(0,1,0);
            var bottomColor = new Color(0,0,1);
            var leftColor = new Color(1,0,1);
            p1.X += 10;
            p1.Y += 10;
            p2.X -= 10;
            p2.Y += 10;
            p3.X -= 10;
            p3.Y -= 10;
            p4.X += 10;
            p4.Y -= 10;
            g.MoveTo(p1);
            g.LineTo(p2);
            g.LineTo(p3);
            g.LineTo(p4);
            g.LineTo(p1);
            g.ClosePath();
            g.FillRule =  FillRule.EvenOdd;
            g.Fill();
#endif

            {
                g.SetSourceSurface(imageSurface, 0, 0);
                g.Paint();
            }


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
            imageSurface = new ImageSurface("W:/VS2013/CarioPlayground/Resources/Toggle.Off.png");
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
