using System;
using System.Runtime.InteropServices;

namespace DWriteSharp.Internal
{
    /// <summary>
    /// Helper class for COM interop
    /// </summary>
    internal static class ComHelper
    {
        /// <summary>
        /// Get com method
        /// </summary>
        /// <typeparam name="TObject">interface type contains the method</typeparam>
        /// <typeparam name="TMethod">Method to get</typeparam>
        /// <param name="comObj">COM interface instance</param>
        /// <param name="slot">Method index in the VTable</param>
        /// <param name="method">Method name</param>
        /// <returns>true: succeed; false: failed</returns>
        internal static bool GetMethod<TObject, TMethod>(TObject comObj, int slot, out TMethod method) where TMethod : class
        {
            IntPtr objectAddress = Marshal.GetComInterfaceForObject(comObj, typeof(TObject));
            if (objectAddress == IntPtr.Zero)
            {
                method = null;
                return false;
            }

            try
            {
                IntPtr vTable = Marshal.ReadIntPtr(objectAddress, 0);
                IntPtr methodAddress = Marshal.ReadIntPtr(vTable, slot * IntPtr.Size);

                // We can't have a Delegate constraint, so we have to cast to
                // object then to our desired delegate
                method = (TMethod)((object)Marshal.GetDelegateForFunctionPointer(methodAddress, typeof(TMethod)));
                return true;
            }
            finally
            {
                Marshal.Release(objectAddress); // Prevent memory leak
            }
        }

    }

    /// <summary>
    /// Specifies info 
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate,
        AllowMultiple = false,
        Inherited = false)]
    internal class ComMethodAttribute : Attribute
    {
        /// <summary>
        /// Index of this method in the VTable of a COM interface
        /// NOTE Make sure you count in the memebers form base COM interface(including IUnknown)!
        /// NOTE Index start form 0
        /// Hint: reference the IDL file of the COM interface
        /// </summary>
        public uint Index { get; set; }
    }
}