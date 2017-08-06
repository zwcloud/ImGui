using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        internal static List<Form> Forms = new List<Form>();
        internal static ImGui.OSAbstraction.PlatformContext platformContext;
        private static readonly Stopwatch stopwatch = new Stopwatch();

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
        private static long deltaTime = 0;
        /// <summary>
        /// The time in ms it took to complete the last frame
        /// </summary>
        internal static long DeltaTime => deltaTime;

        internal static void InitSysDependencies()
        {
            // create factory: service
            if (Application.IsRunningInUnitTest)
            {
                logger = new DebugLogger();
            }
            else
            {
                logger = Utility.Create<ILogger>(CurrentOS.Platform);
            }

            // load platform context:
            //     platform-dependent implementation
            if (CurrentOS.IsAndroid)
            {
                platformContext = OSImplentation.Android.AndroidContext.MapFactory();
            }
            else if(CurrentOS.IsWindows)
            {
                platformContext = OSImplentation.Windows.WindowsContext.MapFactory();
            }
            else if(CurrentOS.IsLinux)
            {
                platformContext = OSImplentation.Linux.LinuxContext.MapFactory();
            }
        }

        internal static bool RequestQuit;

        // HACK for Android
        public static Func<string, System.IO.Stream> FontFileRead;

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
                    childForm.MainLoop(childForm.GUILoop);
                }
                if (RequestQuit)
                {
                    break;
                }
                deltaTime = Time - frameStartTime;
            }
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

            Forms.Add(mainForm);

            //Show main form
            mainForm.Show();

            frameStartTime = Time;
        }

        public static void RunLoop(Form form)
        {
            frameStartTime = Time;
            form.MainLoop(form.GUILoop);
            deltaTime = Time - frameStartTime;
        }


        /// <summary>
        /// Closes all application windows and quit the application.
        /// </summary>
        public static void Quit()
        {
            RequestQuit = true;
        }

        public static bool IsRunningInUnitTest { get; set; } = false;

        internal static ILogger logger;
    }
}