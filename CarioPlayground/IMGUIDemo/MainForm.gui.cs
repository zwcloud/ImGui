using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Cairo;
using IMGUI;
using Win32;
using WinFormCario;
using Point = IMGUI.Point;

namespace IMGUIDemo
{
    public partial class MainForm : Form
    {
        #region GUI compontents

        // ReSharper disable once InconsistentNaming
        // for short
        Context g;
        readonly ImageSurface _backSurface = BuildSurface(512, 512, new Color(1, 1, 1, 1));

        Context _context;
        Surface _frontSurface;

        // ReSharper disable once InconsistentNaming
        // for short
        GUI gui;
        private readonly Color _windowBackgroundColor = new Color(0x6A/255.0, 0x6A/255.0, 0x6A/255.0);

        #endregion

        #region GUI paramters
        
        long _lastFpSlog;
        int _frames;
        int _fps;

        private bool _opened;

        private double _result;

        #endregion

        /// <summary>
        /// GUI logic here
        /// </summary>
        void OnGUI()
        {
            gui.Label(
                new Rect(0, ClientRectangle.Bottom - g.FontExtents.Height, 200, g.FontExtents.Height),
                string.Format("FPS: {0} Mouse ({1},{2})", _fps, Input.MousePos.X, Input.MousePos.Y)
                );

#if Test
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

            var oldValueOpened = _opened;
            _opened = gui.Toggle(new Rect(new Point(20, 86), new Point(120, 108)), "Toggle 0", _opened);
            if(_opened ^ oldValueOpened)
                Debug.WriteLine("Toggle 0 {0}", new object[]{_opened?"on!":"off"});
#else
            gui.Label(new Rect(new Point(14, 14), new Size(190, 50)), Math.Abs(_result) < double.Epsilon ?"0":_result.ToString("F", CultureInfo.InvariantCulture));
            
            gui.Button(new Rect(new Point(14, 68), new Size(34, 27)), "←—");
            gui.Button(new Rect(new Point(53, 68), new Size(34, 27)), "CE");
            gui.Button(new Rect(new Point(92, 68), new Size(34, 27)), "C");
            gui.Button(new Rect(new Point(131, 68), new Size(34, 27)), "±");
            gui.Button(new Rect(new Point(170, 68), new Size(34, 27)), "√");
            
            gui.Button(new Rect(new Point(14, 100), new Size(34, 27)), "7");
            gui.Button(new Rect(new Point(53, 100), new Size(34, 27)), "8");
            gui.Button(new Rect(new Point(92, 100), new Size(34, 27)), "9");
            gui.Button(new Rect(new Point(131,100), new Size(34, 27)), "/");
            gui.Button(new Rect(new Point(170,100), new Size(34, 27)), "%");
            
            gui.Button(new Rect(new Point(14, 132), new Size(34, 27)), "4");
            gui.Button(new Rect(new Point(53, 132), new Size(34, 27)), "5");
            gui.Button(new Rect(new Point(92, 132), new Size(34, 27)), "6");
            gui.Button(new Rect(new Point(131,132), new Size(34, 27)), "*");
            gui.Button(new Rect(new Point(170,132), new Size(34, 27)), "1/x");
            
            gui.Button(new Rect(new Point(14, 132), new Size(34, 27)), "4");
            gui.Button(new Rect(new Point(53, 132), new Size(34, 27)), "5");
            gui.Button(new Rect(new Point(92, 132), new Size(34, 27)), "6");
            gui.Button(new Rect(new Point(131,132), new Size(34, 27)), "*");
            gui.Button(new Rect(new Point(170,132), new Size(34, 27)), "1/x");
            
            gui.Button(new Rect(new Point(14, 164), new Size(34, 27)), "1");
            gui.Button(new Rect(new Point(53, 164), new Size(34, 27)), "2");
            gui.Button(new Rect(new Point(92, 164), new Size(34, 27)), "3");
            gui.Button(new Rect(new Point(131,164), new Size(34, 27)), "-");

            gui.Button(new Rect(new Point(14,196), new Size(73, 27)), "0");
            gui.Button(new Rect(new Point(92,196), new Size(34, 27)), ".");
            gui.Button(new Rect(new Point(131,196), new Size(34, 27)), "+");
            
            gui.Button(new Rect(new Point(170,164), new Size(34, 59)), "=");
#endif
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
            _context.Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            _context.SetSource(_backSurface);
            _context.Fill();

            // Any additional rendering here
            g.Save();
            g.SetSourceColor(_windowBackgroundColor);
            g.Operator = Operator.Source;
            g.Paint();
            g.Restore();
        }
        #endregion

        #region game architecture

        private void InitIMGUI()
        {
            bool exit = false;
            //Set up the game architecture
            Application.Idle += (sender, e) =>
            {
                if (exit)
                {
                    Close();
                }
                if (gui == null)
                {
                    Init();
                }
                else
                {
                    while (Utility.IsApplicationIdle() && exit == false)
                    {
                        Thread.Sleep(30);
                        exit = Update();
                        Render();
                    }
                }
            };
        }

        private void Init()
        {
            if (g == null)
            {
                g = new Context(_backSurface);
                var hdc = Native.GetDC(Handle);
                _frontSurface = new Win32Surface(hdc);
                _context = new Context(_frontSurface);
                gui = new GUI(g);
            }
        }

        new bool Update()
        {
            var clientRect = new RECT();
            clientRect.Left = ClientRectangle.Left;
            clientRect.Top = ClientRectangle.Top;
            clientRect.Right = ClientRectangle.Right;
            clientRect.Bottom = ClientRectangle.Bottom;
            var clientPos = PointToScreen(ClientRectangle.Location);

            Input.Refresh(clientPos.X, clientPos.Y, clientRect);

            //Debug.WriteLine("Mouse at {0},{1}", Input.MousePos.X, Input.MousePos.Y);

            //Quit when ESC is pressed
            if (Input.KeyPressed(Key.Escape))
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
            if (g == null || _context == null) return;

            OnGUI();

            PaintToWindow();
        }

        void DebugUpdate()
        {
            ++_frames;
            long time = Utility.Millis;
            if (time > _lastFpSlog + 1000)
            {
                _fps = _frames;
                _frames = 0;
                _lastFpSlog = time;
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
