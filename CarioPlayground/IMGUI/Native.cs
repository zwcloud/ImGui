using System.Runtime.InteropServices;

namespace IMGUI
{
    public static class Native
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);
    }
}
