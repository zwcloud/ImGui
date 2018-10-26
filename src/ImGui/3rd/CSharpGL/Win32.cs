using System;
using System.Runtime.InteropServices;

namespace CSharpGL
{
    /// <summary>
    /// Windows platform-specific helper class
    /// </summary>
    public sealed class Win32
    {
        private Win32() { }

        /// <summary>
        /// glLibrary = Win32.LoadLibrary(OpenGL32);
        /// </summary>
        public static readonly IntPtr opengl32Library;
        /// <summary>
        /// Initializes the <see cref="Win32"/> class.
        /// </summary>
        static Win32()
        {
            //Load the openGL library - without this wgl calls will fail.
            opengl32Library = Win32.LoadLibrary(OpenGL32);
        }
        ~Win32()
        {
            FreeLibrary(opengl32Library);
        }

        public const string Kernel32 = "kernel32.dll";

        public const string OpenGL32 = "opengl32.dll";

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        internal static IntPtr GetProcAddress(string funcName)
        {
            IntPtr result = GetProcAddress(Win32.opengl32Library, funcName);

            return result;
        }

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr lib, String funcName);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr lib);
    }
}
