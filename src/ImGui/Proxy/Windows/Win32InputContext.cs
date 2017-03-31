using System;
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

        [DllImport("user32.dll")]
        static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        static extern IntPtr LoadCursor(IntPtr hInstance, uint lpCursorName);

        [DllImport("user32.dll")]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction,
            UInt32 uiParam, String pvParam, UInt32 fWinIni);

        enum IDC_STANDARD_CURSORS
        {
            IDC_ARROW = 32512,
            IDC_IBEAM = 32513,
            IDC_WAIT = 32514,
            IDC_CROSS = 32515,
            IDC_UPARROW = 32516,
            IDC_SIZE = 32640,
            IDC_ICON = 32641,
            IDC_SIZENWSE = 32642,
            IDC_SIZENESW = 32643,
            IDC_SIZEWE = 32644,
            IDC_SIZENS = 32645,
            IDC_SIZEALL = 32646,
            IDC_NO = 32648,
            IDC_HAND = 32649,
            IDC_APPSTARTING = 32650,
            IDC_HELP = 32651
        }

        private static IntPtr NormalCursurHandle;
        private static IntPtr IBeamCursurHandle;
        private static IntPtr HorizontalResizeCursurHandle;
        private static IntPtr VerticalResizeCursurHandle;
        private static IntPtr MoveCursurHandle;

        internal static void LoadCursors()//called by windowcontext
        {
            NormalCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_ARROW);
            IBeamCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_IBEAM);
            HorizontalResizeCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZEWE);
            VerticalResizeCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZENS);
            MoveCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZEALL);
        }

        static public void RevertCursors()
        {
            SystemParametersInfo(0x0057, 0, null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        #endregion

        Cursor mouseCursor = Cursor.Default;

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

        public Cursor MouseCursor
        {
            get
            {
                return mouseCursor;
            }
            set
            {
                if (value != mouseCursor)
                {
                    mouseCursor = value;
                    switch (mouseCursor)
                    {
                        case Cursor.Default:
                            SetCursor(NormalCursurHandle);
                            break;
                        case Cursor.Text:
                            SetCursor(IBeamCursurHandle);
                            break;
                        case Cursor.EwResize:
                            SetCursor(HorizontalResizeCursurHandle);
                            break;
                        case Cursor.NsResize:
                            SetCursor(VerticalResizeCursurHandle);
                            break;
                        case Cursor.NeswResize:
                            SetCursor(MoveCursurHandle);
                            break;
                        default:
                            RevertCursors();
                            break;
                    }
                }
            }
        }
    }
}
