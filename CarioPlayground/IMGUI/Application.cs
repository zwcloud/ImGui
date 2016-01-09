using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        internal static List<BaseForm> Forms = new List<BaseForm>();
        internal static List<BaseForm> removeList = new List<BaseForm>();
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

        private static void InitSysDependencies()
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

        public static void Run(Form mainForm)
        {
            //Check paramter
            if(mainForm == null)
            {
                throw new ArgumentNullException("mainForm");
            }

            //Time
            stopwatch.Start();

            //Initialize
            InitSysDependencies();
            Forms.Add(mainForm);
            mainForm.Name = "MainForm";
            for (var i = 0; i < Forms.Count; i++) //Only one form now
            {
                var form = (SFMLForm) Forms[i];
                var window = (SFML.Window.Window) form.InternalForm;
                // Setup event handlers
                InitEvents(window);
            }

            //Show main form
            mainForm.Show();

            //Process every form
            while (!mainForm.Closed)
            {
                //Input
                Input.Mouse.Refresh(); //TODO remove this

                //TODO a better method for manage newly created windows
                foreach (BaseForm baseForm in Forms)
                {
                    var form = (SFMLForm) baseForm;
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
                    if(guiLoopResult.isRepainted)
                    {
                        //update re-rendered section //TODO consider using cairo-gl?
                        var dirtyRect = Rect.Empty;
                        foreach (var control in form.Controls.Values)
                        {
                            foreach (var rect in control.RenderRects)
                            {
                                dirtyRect.Union(rect);
                            }
                            control.RenderRects.Clear();
                        }
                        if(!dirtyRect.IsEmpty)
                        {
                            //Render dirty rect
                            //using (Cairo.Context c = new Cairo.Context(form.FrontSurface))
                            //{
                            //    c.FillRectangle(dirtyRect, CairoEx.ColorArgb(100, 200, 242, 200));
                            //}
                            //Get data of pixels in dirty rect
                            var x = (int) dirtyRect.X;
                            var w = (int) dirtyRect.Width;
                            var h = (int) dirtyRect.Height;
                            var data = new byte[4*w*h];
                            var surfaceData = form.FrontSurface.Data;
                            var surfaceWidth = form.FrontSurface.Width;
                            var offset = 0;
                            for (var y = (int) dirtyRect.Y; y < dirtyRect.Bottom; y++)
                            {
                                Array.Copy(surfaceData, 4*(surfaceWidth*y + x), data, offset, 4*w);
                                offset += 4*w;
                            }
                            // Make it the active window for OpenGL calls
                            window.SetActive();
                            form.guiRenderer.OnUpdateTexture(dirtyRect,
                                System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));
                            form.guiRenderer.OnRenderFrame();
                        }
                    }
                    // Display the rendered frame on screen
                    window.Display();

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
            }

            //Close unclosing forms
            foreach (var baseForm in Forms)
            {
                baseForm.Close();
            }

            stopwatch.Stop();
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