using System.Runtime.InteropServices;

namespace ImGui
{
    class Win32InputContext : IInputContext
    {
        #region Native
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        #endregion

        static bool WinIsKeyPressing(int code)
        {
            return (GetKeyState(code) & 0xF0) != 0;
        }

        static bool WinIsKeyToggled(int code)
        {
            return (GetKeyState(code) & 0x0F) != 0;
        }
        
        public bool IsMouseLeftButtonDown
        {
            get
            {
                return WinIsKeyPressing(0x01/*VK_LBUTTON*/);
            }
        }

        public bool IsMouseMiddleButtonDown
        {
            get
            {
                return WinIsKeyPressing(0x04/*VK_MBUTTON*/);
            }
        }

        public bool IsMouseRightButtonDown
        {
            get
            {
                return WinIsKeyPressing(0x02/*VK_RBUTTON*/);
            }
        }

        public Point MousePosition
        {
            get
            {
                POINT p;
                GetCursorPos(out p);
                return new Point(p.X, p.Y);
            }
        }

    }
}
