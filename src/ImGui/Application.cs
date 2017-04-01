using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace ImGui
{
    /// <summary>
    /// Encapsulates a ImGui application.
    /// </summary>
    /// <remarks>
    /// Application is a class that encapsulates application-specific functionality.
    /// An application can be started in two ways:
    /// 1. If you have access to the main entry point, use
    /// <code>
    /// Application.Init();
    /// Application.Run(mainForm);
    /// </code>
    /// 2. If you can only provide an callback in the main loop, use
    /// <code>
    /// //first init
    /// Application.Init();
    /// //then in the loop callback, call
    /// Application.RunLoop(mainForm);
    /// </code>
    /// </remarks>
    public static class Application
    {
        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> imeBuffer = new Queue<char>();

        internal static List<Form> Forms = new List<Form>();
        internal static PlatformContext platformContext;
        private static readonly Stopwatch stopwatch = new Stopwatch();

        internal static IWindowContext windowContext;
        internal static IInputContext inputContext;


        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        /// <summary>
        /// The time in ms since the application started.
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

        private static long frameStartTime = 0;
        private static long detlaTime = 0;
        /// <summary>
        /// The time in ms it took to complete the last frame
        /// </summary>
        internal static long DetlaTime
        {
            get { return detlaTime; }
        }
        
        internal static void InitSysDependencies()
        {
            // create factory: service
            logger = Utility.Create<ILogger>(Utility.CurrentOS.Platform);

            // load platform context:
            //     platform-dependent implementation
            if (Utility.CurrentOS.IsAndroid)
            {
                platformContext = AndroidContext.MapFactory();
            }
            else if(Utility.CurrentOS.IsWindows)
            {
                platformContext = WindowsContext.MapFactory();
            }
            else if(Utility.CurrentOS.IsLinux)
            {
                platformContext = LinuxContext.MapFactory();
            }

            // load the implementation into delegate instances
            windowContext = platformContext.CreateWindowContext();
            inputContext = platformContext.CreateInputContext();
        }

        private static bool RequestQuit;

        public static object AssetManager { get; set; }

        public static void Init()
        {
            InitSysDependencies();
        }

        /// <summary>
        /// Begins running a standard application on the current thread and makes the specified form visible.
        /// </summary>
        /// <param name="mainForm">A <see cref="Form"/> that represents the form to make visible.</param>
        public static void Run(Form mainForm)
        {
            Init(mainForm);

            while (!mainForm.Closed)
            {
                frameStartTime = Time;

                foreach (Form childForm in Forms)
                {
                    windowContext.MainLoop(childForm.GUILoop);
                }
                if (RequestQuit)
                {
                    break;
                }
                detlaTime = Time - frameStartTime;
            }

            //TODO clean up
        }

        public static void Init(Form mainForm)
        {
            //Check paramter
            if (mainForm == null)
            {
                throw new ArgumentNullException(nameof(mainForm));
            }

            //Time
            stopwatch.Start();
            var sw = new Stopwatch();
            sw.Start();

            Forms.Add(mainForm);

            //Show main form
            mainForm.Show();

            Debug.WriteLine("Init {0:F1}ms", sw.ElapsedTicks * 1000d / Stopwatch.Frequency);
            sw.Restart();

            frameStartTime = Time;
        }

        public static void RunLoop(Form form)
        {
            frameStartTime = Time;
            windowContext.MainLoop(form.GUILoop);
            detlaTime = Time - frameStartTime;
        }


        /// <summary>
        /// Closes all application windows and quit the application.
        /// </summary>
        public static void Quit()
        {
            RequestQuit = true;
        }

        internal static ILogger logger;
    }
}