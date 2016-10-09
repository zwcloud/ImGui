using System.Runtime.InteropServices;

namespace ImGui
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IntRect
    {
        public int Left, Top, Right, Bottom;
    }
}
