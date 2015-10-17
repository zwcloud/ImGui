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
    /// 5. Time
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

        internal static Tree<WinForm> Forms;

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

        private static Stopwatch stopwatch = new Stopwatch();

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

        public static void Run(WinForm form)
        {
            if(form == null)
            {
                throw new ArgumentNullException("form");
            }

            InitIocContainer();

            ToolTip.Init();

            stopwatch.Start();
            System.Windows.Forms.Application.Run(form.InternalForm);
            stopwatch.Stop();
        }

        public static void Run(BaseForm baseform)
        {
            if (baseform == null)
            {
                throw new ArgumentNullException("baseform");
            }

            InitIocContainer();

            ToolTip.Init();

            var form = (SFMLForm)baseform;

            stopwatch.Start();
            while (form.IsOpen)
            {
                form.DispatchEvents();

                

                form.Display();
            }
            stopwatch.Stop();
        }
    }
}