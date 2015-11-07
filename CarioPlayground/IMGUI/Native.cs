using System;
using System.Runtime.InteropServices;

namespace IMGUI
{
    public static class Native
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref SFML.System.Vector2i lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref SFML.System.Vector2i lpPoint);
    }
}
