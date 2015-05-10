using System;
using Win32;

namespace WinFormCario
{
    static class Utility
    {
        public static bool IsApplicationIdle()
        {
            NativeMessage result;
            return Native.PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        //http://stackoverflow.com/a/15051945/3427520
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>Get extra long current timestamp</summary>
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }
    }
}
