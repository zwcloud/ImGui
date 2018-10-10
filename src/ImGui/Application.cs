using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Input;

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

        internal static void InitSysDependencies()
        {
            Time.Init();

            // load logger
            if (IsRunningInUnitTest)
            {
                try
                {
                    Logger = new EchoLogger();
                    EchoLogger.Show();
                    Log.Enabled = true;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    Debug.WriteLine("Failed to connect to EchoLogger. The program will continue without logging.");
                    Log.Enabled = false;
                }
            }
            else
            {
                Logger = new ConsoleLogger();
                Log.Enabled = true;
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
                Time.OnFrameBegin();
                Keyboard.Instance.OnFrameBegin();

                foreach (Form childForm in Forms)
                {
                    childForm.MainLoop(childForm.GUILoop);
                }
                if (RequestQuit)
                {
                    break;
                }

                Keyboard.Instance.OnFrameEnd();
                Time.OnFrameEnd();
            }

        }

        public static void Init(Form mainForm)
        {
            //Check paramter
            if (mainForm == null)
            {
                throw new ArgumentNullException(nameof(mainForm));
            }

            Forms.Add(mainForm);

            //Show main form
            mainForm.Show();

        }

        public static void RunLoop(Form form)
        {
            Time.OnFrameBegin();
            Keyboard.Instance.OnFrameBegin();
            form.MainLoop(form.GUILoop);
            Keyboard.Instance.OnFrameEnd();
            Time.OnFrameEnd();
        }


        /// <summary>
        /// Closes all application windows and quit the application.
        /// </summary>
        public static void Quit()
        {
            if (IsRunningInUnitTest)
            {
                if (Log.Enabled)
                {
                    EchoLogger.Close();
                }
            }
            RequestQuit = true;
        }

        /// <summary>
        /// Helper property for the unit-test.
        /// </summary>
        internal static bool IsRunningInUnitTest { get; set; } = false;

        internal static ILogger Logger;
    }
}