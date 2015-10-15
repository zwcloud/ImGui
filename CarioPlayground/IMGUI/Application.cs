using System;
using System.Collections.Generic;
using TinyIoC;

namespace IMGUI
{
    /// <summary>
    /// Unique appliction class
    /// </summary>
    /// <remarks>
    /// Manage application-wide objects:
    /// 1. IME(Internal)
    /// 2. Input
    /// 3. Ioc container(Internal)
    /// 4. Windows(internal)
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

        internal static Tree<BasicForm> Forms;

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
        
        public static void Run(BasicForm form)
        {
            if(form == null)
            {
                throw new ArgumentNullException("form");
            }

            InitIocContainer();
            System.Windows.Forms.Application.Run(form.InternalForm);
        }
    }
}