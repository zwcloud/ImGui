using System;
using System.Runtime.InteropServices;

namespace DWriteSharp.Internal
{
    internal static class NativeMethods
    {
        [DllImport("d2d1.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object D2D1CreateFactory(D2D1Factory.FactoryType factoryType,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            D2D1Factory.FactoryOptions factoryOptions);

        [DllImport("dwrite.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object DWriteCreateFactory(DWriteFactory.FactoryType factoryType,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        [DllImport("DWriteSharpNativeHelper.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object DwriteCreateMeshTextRender(
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
    }
}