using DWriteSharp.Internal;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DWriteSharp
{
    [ComImport]
    [Guid("ef8a8135-5cc6-45fe-8825-c5a0724eb819")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDWriteTextRenderer { }

    internal class DirectWriteMeshTextRenderer : IDisposable
    {
        #region API

        public static DirectWriteMeshTextRenderer Create()
        {
            return new DirectWriteMeshTextRenderer(CreateTextRender());
        }

        public void ClearBuffer()
        {
            this.Clear();
        }

        #endregion

        #region COM internals

        public const string IID_IDirectWriteMeshTextRenderer = "1A87B373-3BD5-4E9C-BC2E-1811CE7248F2";
        [ComImport]
        [Guid(IID_IDirectWriteMeshTextRenderer)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDirectWriteMeshTextRenderer : IDWriteTextRenderer { }

        public static IDirectWriteMeshTextRenderer CreateTextRender()
        {
            object render = Internal.NativeMethods.DwriteCreateMeshTextRender(new Guid(IID_IDirectWriteMeshTextRenderer));
            Debug.Assert(render != null, "IDirectWriteMeshTextRenderer creating failed.");
            return (IDirectWriteMeshTextRenderer)render;
        }

        #region COM Method

        #region signature delegates of COM method

        [ComMethod(Index = 10)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ClearSignature(
            IDirectWriteMeshTextRenderer meshTextRenderer);
        private ClearSignature clear;

        #endregion
        public IDirectWriteMeshTextRenderer comObject;

        private DirectWriteMeshTextRenderer(IDirectWriteMeshTextRenderer obj)
        {
            this.comObject = obj;
            InitComMethods();
        }

        internal void InitComMethods()
        {
            bool result;

            result = ComHelper.GetMethod(comObject, 10, out clear);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 10);
        }

        ~DirectWriteMeshTextRenderer()
        {
            this.Release();
        }

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.clear = null;
            }
        }

        #region COM Method wrappers
        private void Clear()
        {
            //var hr = clear(comObject);
            //Marshal.ThrowExceptionForHR(hr);
        }

        #endregion

        #endregion

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}