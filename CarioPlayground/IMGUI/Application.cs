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

        private static long frameStartTime = 0;
        private static long detlaTime = 0;
        /// <summary>
        /// The time in ms it took to complete the last frame
        /// </summary>
        public static long DetlaTime
        {
            get { return detlaTime; }
        }

        static Application()
        {
            InitSysDependencies();
        }

        internal static void InitSysDependencies()
        {
            // create factory:
            //     platform-dependent object implementation
            if(Utility.CurrentOS.IsWindows)
            {
                _map = MapWindows.MapFactory();
            }
            else if(Utility.CurrentOS.IsLinux)
            {
                _map = MapLinux.MapFactory();
            }

            // load the implementation into delegate instances
            windowContext = _map.CreateWindowContext();
            inputContext = _map.CreateInputContext();
        }

        private static bool RequestQuit;
#if DrawDirtyRect
        static Rect lastDirtyRect = Rect.Empty;
#endif

        public static void Run(Form mainForm)
        {
            #region Init
            //Check paramter
            if (mainForm == null)
            {
                throw new ArgumentNullException("mainForm");
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
            #endregion

            while (!mainForm.Closed)
            {
                frameStartTime = Time;
                Input.Mouse.Refresh();
                foreach (Form form in Forms)
                {
                    windowContext.MainLoop(form.GUILoop);
                }
                if (RequestQuit)
                {
                    break;
                }
                detlaTime = Time - frameStartTime;
            }
            
            //TODO clean up
        }

        public static void Quit()
        {
            RequestQuit = true;
        }

    }
}