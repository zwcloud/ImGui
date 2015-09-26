using System;
using System.Collections.Generic;
using TinyIoC;

namespace IMGUI
{
    public delegate void OnGUIDelegate(GUI gui);

    /// <summary>
    /// A single window IMGUI application
    /// </summary>
    public sealed class Application
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

        public static void Run(Form form)
        {
            InitIocContainer();

            System.Windows.Forms.Application.Run(form);
        }

        internal static TinyIoCContainer IocContainer = TinyIoCContainer.Current;

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
    }
}