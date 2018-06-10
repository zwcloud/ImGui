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
        internal static OSAbstraction.PlatformContext PlatformContext;
        private static readonly Stopwatch _applicationWatch = new Stopwatch();

        /// <summary>
        /// The time in ms since the application started.
        /// </summary>
        internal static long Time
        {
            get
            {
                if(!_applicationWatch.IsRunning)
                {
                    throw new InvalidOperationException(
                        "The application's time cannot be obtained because it isn't running. Call Application.Run to run it first.");
                }
                return _applicationWatch.ElapsedMilliseconds;
            }
        }

        private static long _frameStartTime;
        private static long _deltaTime;

        /// <summary>
        /// The time in ms it took to complete the last frame
        /// </summary>
        public static long DeltaTime => _deltaTime;

        internal static void InitSysDependencies()
        {
            // load logger
            if (IsRunningInUnitTest)
            {
                Logger = new EchoLogger();
                EchoLogger.Show();
            }
            else
            {
                Logger = new DebugLogger();
            }
            Log.Init(Logger);

            // load platform context:
            //     platform-dependent implementation
            if (CurrentOS.IsAndroid)
            {
                PlatformContext = OSImplentation.Android.AndroidContext.MapFactory();
            }
            else if(CurrentOS.IsWindows)
            {
                PlatformContext = OSImplentation.Windows.WindowsContext.MapFactory();
            }
            else if(CurrentOS.IsLinux)
            {
                PlatformContext = OSImplentation.Linux.LinuxContext.MapFactory();
            }
        }

        internal static bool RequestQuit;

        // HACK for Android
        public static Func<string, System.IO.Stream> OpenAndroidAssets;

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
                _frameStartTime = Time;

                foreach (Form childForm in Forms)
                {
                    childForm.MainLoop(childForm.GUILoop);
                }
                if (RequestQuit)
                {
                    break;
                }
                _deltaTime = Time - _frameStartTime;
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
            _applicationWatch.Start();

            Forms.Add(mainForm);

            //Show main form
            mainForm.Show();

            _frameStartTime = Time;
        }

        public static void RunLoop(Form form)
        {
            _frameStartTime = Time;
            form.MainLoop(form.GUILoop);
            _deltaTime = Time - _frameStartTime;
        }


        /// <summary>
        /// Closes all application windows and quit the application.
        /// </summary>
        public static void Quit()
        {
            if (IsRunningInUnitTest)
            {
                EchoLogger.Close();
            }
            RequestQuit = true;
        }

        public static bool IsRunningInUnitTest { get; set; } = false;

        internal static ILogger Logger;
    }
}