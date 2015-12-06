using System;
using System.Collections.Generic;
using System.Diagnostics;
using TinyIoC;

namespace IMGUI
{
    /// <summary>
    /// Unique application class
    /// </summary>
    /// <remarks>
    /// Manage application-wide objects:
    /// 1. IME(Internal)
    /// 2. Input
    /// 3. Ioc container(Internal)
    /// 4. Windows(internal)
    /// 5. Time(not implemented, still in Utility.cs)
    /// </remarks>
    public static class Application
    {
        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> imeBuffer = new Queue<char>();

        internal static readonly TinyIoCContainer IocContainer = TinyIoCContainer.Current;

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        internal static List<BaseForm> Forms;

        private static void InitIocContainer()
        {
            if(Utility.CurrentOS.IsWindows)
            {
                IocContainer.Register<DWriteTextFormatProxy>();
                IocContainer.Register<ITextFormat, DWriteTextFormatProxy>().AsMultiInstance();

                IocContainer.Register<DWriteTextLayoutProxy>();
                IocContainer.Register<ITextLayout, DWriteTextLayoutProxy>().AsMultiInstance();
            }
            else if(Utility.CurrentOS.IsLinux)
            {
                throw new NotImplementedException();
                //IocContainer.Register<PangoTextFormatProxy>();
                //IocContainer.Register<ITextFormat, PangoTextFormatProxy>();
                //
                //IocContainer.Register<PangoTextLayoutProxy>();
                //IocContainer.Register<ITextLayout, PangoTextLayoutProxy>();
            }
        }

        private static readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Time in ms since the application started.
        /// </summary>
        public static long Time
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
            if(mainForm == null)
            {
                throw new ArgumentNullException("mainForm");
            }

            InitIocContainer();

            Forms = new List<BaseForm>();
            Forms.Add(mainForm);
            mainForm.Name = "MainForm";

            for (int i = 0; i < Forms.Count; i++)//Only one form now
            {
                var form = (SFMLForm) Forms[i];
                var window = (SFML.Window.Window) form.InternalForm;

                // Setup event handlers
                window.Closed += OnClosed;
                window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
                window.Resized += new EventHandler<SFML.Window.SizeEventArgs>(OnResized);
                window.TextEntered += new EventHandler<SFML.Window.TextEventArgs>(OnTextEntered);

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
                Input.Mouse.Refresh();
                Input.Keyboard.Refresh();

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
            SFML.Window.Window window = (SFML.Window.Window)sender;
            if(e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Function called when the window is resized
        /// </summary>
        static void OnResized(object sender, SFML.Window.SizeEventArgs e)
        {

        }

    }
}