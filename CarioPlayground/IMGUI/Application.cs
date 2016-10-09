//#define DrawDirtyRect
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ImGui
{
    /// <summary>
    /// Unique application class
    /// </summary>
    /// <remarks>
    /// Manage application-wide objects:
    /// * IME(Internal)
    /// * Input
    /// * Windows(internal)
    /// * Time(internal)
    /// </remarks>
    public static class Application
    {
        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> imeBuffer = new Queue<char>();

        internal static List<Form> Forms = new List<Form>();
        internal static List<Form> removeList = new List<Form>();
        internal static Map _map;
        private static readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        /// <summary>
        /// Time in ms since the application started.
        /// </summary>
        internal static long Time
        {
            get
            {
                if(!stopwatch.IsRunning)
                {
                    throw new InvalidOperationException(
                        "The application's time cannot be obtained because it isn't running. Call Application.Run to run it first.");
                }
                return stopwatch.ElapsedMilliseconds;
            }
        }

        internal static void InitSysDependencies()
        {
            if(Utility.CurrentOS.IsWindows)
            {
                _map = MapWindows.MapFactory();
            }
            else if(Utility.CurrentOS.IsLinux)
            {
                _map = MapLinux.MapFactory();
            }
        }

        private static bool RequestQuit;
#if DrawDirtyRect
        static Rect lastDirtyRect = Rect.Empty;
#endif

        public static void Run(Form mainForm)
        {
            #region Init
            //Check paramter
            if(mainForm == null)
            {
                throw new ArgumentNullException("mainForm");
            }

            //Time
            stopwatch.Start();
            var debugWatch = new Stopwatch();
            debugWatch.Start();

            //Initialize
            InitSysDependencies();
            mainForm.Name = "MainForm";
            InitEvents(mainForm.internalForm);

            Forms.Add(mainForm);

            //Show main form
            mainForm.Show();

            Debug.WriteLine("Init {0:F1}ms", debugWatch.ElapsedTicks * 1000d / Stopwatch.Frequency);
            debugWatch.Restart();
            #endregion

            #region Main loop
            //Process every form
            while (!mainForm.Closed)
            {
                var mousePos = Input.Mouse.GetMousePos(mainForm);
                mainForm.internalForm.SetTitle(
                    string.Format("{0:F1}, {1},{2}",
                    Stopwatch.Frequency*1.0/stopwatch.ElapsedTicks, mousePos.X, mousePos.Y));
                stopwatch.Restart();

                //Input
                Input.Mouse.Refresh(); //TODO remove this

                foreach (Form baseForm in Forms.ToList())
                {
                    var form = baseForm;
                    var window = (SFML.Window.Window) form.InternalForm;
                    var mouseSuspended = false;
                    if(!form.Focused)
                    {
                        Input.Mouse.Suspend();
                        mouseSuspended = true;
                    }

                    // Process events
                    window.DispatchEvents();

                    // Run GUI looper
                    var guiLoopResult = form.GUILoop();
                    if(guiLoopResult.needExit)
                    {
                        removeList.Add(form);
                        continue;
                    }
                    var dirtyRect = guiLoopResult.dirtyRect;
                    if (dirtyRect != Rect.Empty)
                    {
                        //update re-rendered section //TODO consider using cairo-gl?
                        // Make it the active window for OpenGL calls
                        window.SetActive();
#if DrawDirtyRect
                        //Clear last dirty rect
                        if(lastDirtyRect!=Rect.Empty)
                        {
                            var data = GetDirtyData(lastDirtyRect, form.DebugSurface);
                            form.guiRenderer.OnUpdateTexture(lastDirtyRect,
                                System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));
                        }

                        lastDirtyRect = dirtyRect;
#endif
                        //Get data of pixels in dirty rect
                        {
#if DrawDirtyRect
                            //Save orginal surface
                            form.DebugContext.SetSourceSurface(form.FrontSurface, 0, 0);
                            form.DebugContext.Paint();
#endif

                            //Update repainted section
#if DrawDirtyRect
                            var data = GetDirtyData(dirtyRect, form.FrontSurface, CairoEx.ColorArgb(100, 200, 240, 200));
#else
                            var data = GetDirtyData(dirtyRect, form.FrontSurface);
#endif
                            form.guiRenderer.OnUpdateTexture(dirtyRect,
                                System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));
                        }
                        form.guiRenderer.OnRenderFrame();
                        // Display the rendered frame on screen
                        window.Display();
                    }

                    if(mouseSuspended)
                    {
                        Input.Mouse.Resume();
                    }
                }

                if(removeList.Count != 0)
                {
                    foreach (var baseForm in removeList)
                    {
                        baseForm.Close();
                    }
                    removeList.Clear();
                }

                if (Input.Keyboard.KeyDown(SFML.Window.Keyboard.Key.Escape))
                {
                    Quit();
                }

                if(RequestQuit)
                {
                    break;
                }

                var msSleeping = 1000/60.0 - stopwatch.ElapsedMilliseconds;
                if (msSleeping > 0)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(msSleeping));//limit FPS to 60
                }
            }
            #endregion

            #region Clean up
            //Close unclosing forms
            foreach (var baseForm in Forms)
            {
                if(!baseForm.Closed)
                {
                    baseForm.Close();
                }
            }

            stopwatch.Stop();
            debugWatch.Stop();

            #endregion
        }

        public static void Quit()
        {
            RequestQuit = true;
        }

        private static byte[] GetDirtyData(Rect rect, Cairo.ImageSurface surface)
        {
            return GetDirtyData(rect, surface, CairoEx.ColorClear);
        }
        private static byte[] GetDirtyData(Rect rect, Cairo.ImageSurface surface, Cairo.Color blendColor)
        {
            var x = (int) rect.X;
            var w = (int) rect.Width;
            var h = (int) rect.Height;
            var data = new byte[4*w*h];
            var surfaceData = surface.Data;
            var surfaceWidth = surface.Width;
            var offset = 0;
            for (var y = (int)rect.Y; y < rect.Bottom; y++)
            {
                Array.Copy(surfaceData, 4 * (surfaceWidth * y + x), data, offset, 4 * w);
                offset += 4 * w;
            }
            if (blendColor != CairoEx.ColorClear)
            {
                for (int i = 0; i < data.Length; i+=4)
                {
                    Cairo.Color c = CairoEx.ColorArgb(
                        data[i + 3],
                        data[i + 2],
                        data[i + 1],
                        data[i]);
                    c = CairoEx.AlphaBlend(c, blendColor);
                    data[i+3] = (byte)(c.A * 255);
                    data[i+2] = (byte)(c.R * 255);
                    data[i+1] = (byte)(c.G * 255);
                    data[i] = (byte)(c.B * 255);
                }
            }
            return data;
        }

        #region Window events

        private static void InitEvents(SFML.Window.Window window)
        {
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;
            window.KeyReleased += OnKeyReleased;
            window.TextEntered += OnTextEntered;
        }

        private static void OnTextEntered(object sender, SFML.Window.TextEventArgs e)
        {
            var text = e.Unicode;
            for (var i = 0; i < text.Length; i++)
            {
                if(Char.IsControl(text[i]))
                    continue;
                ImeBuffer.Enqueue(text[i]);
            }
        }

        /// <summary>
        /// Function called when the window is closed
        /// </summary>
        private static void OnClosed(object sender, EventArgs e)
        {
            var window = (SFML.Window.Window) sender;
            window.Close();
        }

        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        private static void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if(e.Code == SFML.Window.Keyboard.Key.Unknown)
            {
                return;
            }

            Input.Keyboard.lastKeyStates[(int) e.Code] = Input.Keyboard.keyStates[(int) e.Code];
            Input.Keyboard.keyStates[(int) e.Code] = InputState.Down;
        }

        /// <summary>
        /// Function called when a key is released
        /// </summary>
        private static void OnKeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
            if(e.Code == SFML.Window.Keyboard.Key.Unknown)
            {
                return;
            }
            Input.Keyboard.lastKeyStates[(int) e.Code] = Input.Keyboard.keyStates[(int) e.Code];
            Input.Keyboard.keyStates[(int) e.Code] = InputState.Up;
        }

        #endregion
    }
}