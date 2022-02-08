using System;
using System.Runtime.InteropServices;

namespace CSharpGL
{
    /// <summary>
    /// OpenGL API
    /// </summary>
    public static partial class GL
    {
        #region Native

        private const string OpenGL32 = "opengl32.dll";

        /// <summary>
        /// Gets a proc address.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>The address of the function.</returns>
        [DllImport(OpenGL32)]
        public static extern IntPtr wglGetProcAddress(string name);
        
        [DllImport("EGL")]
        public static extern IntPtr eglGetProcAddress(string name);
        
        //from emscripten's libGL,
        //see https://github.com/emscripten-core/emscripten/blob/ff141e4fc78c9ae5f082f5dcb31cf0951558214b/system/lib/gl/gl.c#L1801
        [DllImport("*")]
        public static extern IntPtr emscripten_GetProcAddress(string name);

        #endregion

        /// <summary>
        /// Returns a delegate for an extension function. This delegate can be called to execute the extension function.
        /// </summary>
        /// <typeparam name="T">The extension delegate type.</typeparam>
        /// <returns>The delegate that points to the extension function.</returns>
        private static T GetDelegateFor<T>() where T : class
        {
            if(GL.allFunctionsLoaded)
            {
                throw new InvalidOperationException("GetDelegateFor cannot be used at rendering time");
            }

            //  Get the type of the extension function.
            Type delegateType = typeof(T);

            //  Get the name of the extension function.
            string name = delegateType.Name;

            IntPtr proc = IntPtr.Zero;
            if(OperatingSystem.IsWindows())
            {
                // check https://www.opengl.org/wiki/Load_OpenGL_Functions
                proc = wglGetProcAddress(name);
                var pointer = proc.ToInt64();
                if (-1 <= pointer && pointer <= 3)
                {
                    proc = Win32.GetProcAddress(name);
                    pointer = proc.ToInt64();
                    if(-1 <= pointer && pointer <= 3)
                    {
                        proc = IntPtr.Zero;//not supported
                    }
                }
            }
            else if (OperatingSystem.IsAndroid())
            {
                proc = eglGetProcAddress(name);
            }
            else if(OperatingSystem.IsLinux())
            {
                proc = eglGetProcAddress(name);
            }
            else if (OperatingSystem.IsBrowser())
            {
                proc = emscripten_GetProcAddress(name);
            }
            else if(OperatingSystem.IsMacOS())
            {
                throw new NotImplementedException("Binding for macOS hasn't been implemented.");
            }
            else
            {
                throw new NotImplementedException("Unsupported OS.");
            }

            if(proc == IntPtr.Zero)
            {
                throw new NotSupportedException("Extension function " + name + " not supported.");
            }

            // Get the delegate for the function pointer.
            T del = Marshal.GetDelegateForFunctionPointer<T>(proc);
            return del;
        }
    }
}
