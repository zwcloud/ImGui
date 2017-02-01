using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DWriteSharp.Internal;

namespace DWriteSharp
{
    internal sealed class D2D1Factory : IDisposable
    {
        public static D2D1Factory Create()
        {
            return new D2D1Factory(CreateFactory());
        }

    #region COM internals

    #region COM interface creation

        public const string IID_ID2D1Factory = "06152247-6f50-465a-9245-118bfd3b6007";

        public enum FactoryType
        {
            SingleThreaded,
            MultiThreaded,
        }

        //Dummy
        public struct FactoryOptions
        {
            D2D1DebugLevel debugLevel;
        }

        public enum D2D1DebugLevel
        {
            D2D1_DEBUG_LEVEL_NONE = 0,
            D2D1_DEBUG_LEVEL_ERROR = 1,
            D2D1_DEBUG_LEVEL_WARNING = 2,
            D2D1_DEBUG_LEVEL_INFORMATION = 3,
        }

        [ComImport]
        [Guid(IID_ID2D1Factory)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ID2D1Factory { }

        private ID2D1Factory comObject;

        private D2D1Factory(ID2D1Factory obj)
        {
            this.comObject = obj;
        }

        public static ID2D1Factory CreateFactory()
        {
            var factoryOptions = new FactoryOptions();
            object factory = NativeMethods.D2D1CreateFactory(
                FactoryType.SingleThreaded,
                new Guid(IID_ID2D1Factory), factoryOptions);
            if (factory != null)
            {
                Debug.WriteLine("ID2D1Factory created.");
            }
            return (ID2D1Factory) factory;
        }

    #endregion

    #region COM Methods
        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
            }
        }

        ~D2D1Factory()
        {
            this.Release();
        }

    #region COM Method wrappers
    //None
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