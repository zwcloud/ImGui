using System;
using System.Runtime.InteropServices;

namespace ImGui
{
    /// <summary>
    /// Current OS information
    /// </summary>
    internal static class CurrentOS
    {
        public static Platform Platform { get; }

        static CurrentOS()
        {
            var envars = Environment.GetEnvironmentVariables();
            IsAndroid = envars.Contains("ANDROID_PROPERTY_WORKSPACE");
            if (IsAndroid)
            {
                Platform = Platform.Android;
                IsDesktopPlatform = false;
                return;
            }
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (IsWindows)
            {
                Platform = Platform.Windows;
                IsDesktopPlatform = true;
                return;
            }
            IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            if (IsMac)
            {
                Platform = Platform.Mac;
                IsDesktopPlatform = true;
                return;
            }
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if (IsLinux)
            {
                Platform = Platform.Linux;
                IsDesktopPlatform = true;
                return;
            }
            IsUnknown = true;
        }

        public static bool IsWindows { get; }
        public static bool IsMac { get; }
        public static bool IsLinux { get; }
        public static bool IsAndroid { get; }
        public static bool IsUnknown { get; }

        public static bool IsDesktopPlatform { get; }

        public static bool Is64BitProcess => IntPtr.Size == 8;
        public static bool Is32BitProcess => IntPtr.Size == 4;
    }
}
