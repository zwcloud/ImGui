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

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        internal static List<BaseForm> Forms;

        internal static Map _map;

        private static void InitSysDependencies()
        {
            if (Utility.CurrentOS.IsWindows)
            {
                _map = MapWindows.MapFactory();
            }
            else if(Utility.CurrentOS.IsLinux)
            {
                _map = MapLinux.MapFactory();
            }
        }

        private static readonly Stopwatch stopwatch = new Stopwatch();

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

        public static void Run(Form mainForm)
        {
            stopwatch.Start();
            if(mainForm == null)
            {
                throw new ArgumentNullException("mainForm");
            }

            InitSysDependencies();

            //Initialize forms
            Forms = new List<BaseForm>();
            Forms.Add(mainForm);
            mainForm.Name = "MainForm";
            for (int i = 0; i < Forms.Count; i++)//Only one form now
            {
                var form = (SFMLForm) Forms[i];
                var window = (SFML.Window.Window) form.InternalForm;

                // Setup event handlers
                window.Closed += OnClosed;
                window.KeyPressed += OnKeyPressed;
                window.KeyReleased += OnKeyReleased;
                window.Resized += OnResized;
                window.TextEntered += OnTextEntered;

                // Make it the active window for OpenGL calls
                window.SetActive();

                // Create OpenGL GUI renderer
                form.guiRenderer = new GUIRenderer(form.Size);
                form.guiRenderer.OnLoad();
            }

            mainForm.Show();

            while (mainForm.internalForm.IsOpen)
            {
                //Input
                Input.Mouse.Refresh();//TODO remove this

                //TODO a better method for manage newly created windows
                for (int i = 0; i < Forms.Count; i++)
                {
                    var form = (SFMLForm)Forms[i];
                    var window = (SFML.Window.Window)form.InternalForm;

                    bool mouseSuspended = false;
                    if(!form.Focused)
                    {
                        Input.Mouse.Suspend();
                        mouseSuspended = true;
                    }

                    // Start window loop
                    if(window.IsOpen)
                    {
                        // Process events
                        window.DispatchEvents();
                        // Run GUI looper
                        var isRepainted = form.GUILoop();
                        if(isRepainted)
                        {
                            var surface = (form.FrontSurface as Cairo.ImageSurface);
                            if(surface != null)
                            {
                                // Make it the active window for OpenGL calls
                                window.SetActive();
                                if (form.guiRenderer == null)
                                {
                                    form.guiRenderer = new GUIRenderer(form.Size);
                                    form.guiRenderer.OnLoad();
                                }
                                //TODO update entire surface is too slow(When it happens, a CPU peak occurs.), this can be improved by only updating re-rendered section (or using cairo-gl?)
                                form.guiRenderer.OnUpdateTexture(surface.Width, surface.Height, surface.DataPtr);
                                form.guiRenderer.OnRenderFrame();
                            }
                        }
                        // Display the rendered frame on screen
                        window.Display();
                    }

                    if (mouseSuspended)
                    {
                        Input.Mouse.Resume();
                    }

                }
                System.Threading.Thread.Sleep(10);//TODO Without this, High CPU rate occurs, why?
            }
            stopwatch.Stop();
        }

        private static void OnTextEntered(object sender, SFML.Window.TextEventArgs e)
        {
            var text = e.Unicode;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsControl(text[i]))
                    continue;
                ImeBuffer.Enqueue(text[i]);
            }
        }

        /// <summary>
        /// Function called when the window is closed
        /// </summary>
        static void OnClosed(object sender, EventArgs e)
        {
            SFML.Window.Window window = (SFML.Window.Window)sender;
            window.Close();
        }

        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        static void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
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
        static void OnKeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == SFML.Window.Keyboard.Key.Unknown)
            {
                return;
            }
            Input.Keyboard.lastKeyStates[(int)e.Code] = Input.Keyboard.keyStates[(int)e.Code];
            Input.Keyboard.keyStates[(int)e.Code] = InputState.Up;
        }

        /// <summary>
        /// Function called when the window is resized
        /// </summary>
        static void OnResized(object sender, SFML.Window.SizeEventArgs e)
        {

        }

    }
}