using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CSharpGL
{
    /// <summary>
    /// The OpenGL class wraps Sun's OpenGL 3D library.
    /// </summary>
    public static partial class GL
    {
        /// <summary>
        /// Returns a delegate for an extension function. This delegate can be called to execute the extension function.
        /// </summary>
        /// <typeparam name="T">The extension delegate type.</typeparam>
        /// <returns>The delegate that points to the extension function.</returns>
        private static T GetDelegateFor<T>() where T : class
        {
            //  Get the type of the extension function.
            Type delegateType = typeof(T);//This is slow!!! FIXME right now

            //  Get the name of the extension function.
            string name = delegateType.Name;

            // ftlPhysicsGuy - Better way
            Delegate del = null;
            if (!extensionFunctions.TryGetValue(name, out del))
            {
                IntPtr proc = IntPtr.Zero;
                if(ImGui.Utility.CurrentOS.IsWindows)
                {
                    // check https://www.opengl.org/wiki/Load_OpenGL_Functions
                    proc = Win32.wglGetProcAddress(name);
                    var pointer = proc.ToInt64();
                    if (-1 <= pointer && pointer <= 3)
                    {
                        proc = Win32.GetProcAddress(name);
                        pointer = proc.ToInt64();
                        if(-1 <= pointer && pointer <= 3)
                        {
                            throw new NotSupportedException("Extension function " + name + " not supported.");
                        }
                    }
                }
                else if(ImGui.Utility.CurrentOS.IsLinux)
                {
                    proc = glxGetProcAddress(name);
                    if(proc == IntPtr.Zero)
                    {
                        throw new NotSupportedException("Extension function " + name + " not supported.");
                    }
                }
                else if(ImGui.Utility.CurrentOS.IsMac)
                {
                    throw new NotImplementedException("Binding for macOS hasn't not implemented yet.");
                }
                else
                {
                    throw new NotImplementedException("Unsupported OS.");
                }

                //  Get the delegate for the function pointer.
                del = Marshal.GetDelegateForFunctionPointer(proc, delegateType);

                //  Add to the dictionary.
                extensionFunctions.Add(name, del);
            }

            return del as T;
        }

        /// <summary>
        /// The set of extension functions.
        /// </summary>
        private static readonly Dictionary<string, Delegate> extensionFunctions = new Dictionary<string, Delegate>();

        [DllImport("libGL.so", EntryPoint = "glXGetProcAddress")]
        internal static extern IntPtr glxGetProcAddress(string s);
    }
}
