using System;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace ImGui
{
    internal class Win32WindowContext : IWindowContext
    {
        #region Native
        [StructLayout(LayoutKind.Sequential)]
        struct WNDCLASS
        {
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszClassName;
        }

        [Flags]
        enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,
        }

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll")]
        static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("gdi32.dll")]
        static extern IntPtr GetStockObject(int fnObject);

        [DllImport("user32.dll", SetLastError = true)]
        static extern ushort RegisterClass([In] ref WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            IntPtr lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool UpdateWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(ref MSG message, IntPtr handle, uint filterMin, uint filterMax, uint flags);

        [DllImport("user32.dll")]
        static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", SetLastError = true)]
        static extern int _GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
        static extern int _GetWindowTextLength(IntPtr hWnd);
        public static string GetWindowText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = _GetWindowTextLength(hWnd);
            System.Text.StringBuilder sb = new System.Text.StringBuilder(length + 1);
            _GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        #endregion

        static SafeProcessHandle processHandle = Process.GetCurrentProcess().SafeHandle;

        public Win32WindowContext()
        {
        }

        public void MainLoop(Action guiMethod)
        {
            MSG msg = new MSG();
            if (PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0x0001/*PM_REMOVE*/))//handle windows messages
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            else//handle imgui logic
            {
                guiMethod();
            }
            //if(msg.message != 0x12/*WM_QUIT*/) //...
        }
        
        public void InputEventHandler(InputType type, float x, float y)
        {
            //dummy
        }

        private IntPtr WindowProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            #region Handle window moving and resizing
            const int wmNcHitTest = 0x84;
            const int HTCAPTION = 2;
            const int htLeft = 10;
            const int htRight = 11;
            const int htTop = 12;
            const int htTopLeft = 13;
            const int htTopRight = 14;
            const int htBottom = 15;
            const int htBottomLeft = 16;
            const int htBottomRight = 17;
            IntPtr Result;
            if (msg == wmNcHitTest)
            {
                int x = (int)(lParam.ToInt64() & 0xFFFF);
                int y = (int)((lParam.ToInt64() & 0xFFFF0000) >> 16);
                POINT pt = new POINT { X = x, Y = y };
                ScreenToClient(hWnd, ref pt);
                RECT rect;
                GetWindowRect(hWnd, out rect);
                Size clientSize = new Size(rect.right - rect.left, rect.bottom - rect.top);
                ///allow resize on the lower right corner
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htBottomRight);
                    return Result;
                }
                ///allow resize on the lower left corner
                if (pt.X <= 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htBottomLeft);
                    return Result;
                }
                ///allow resize on the upper right corner
                if (pt.X <= 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htTopLeft);
                    return Result;
                }
                ///allow resize on the upper left corner
                if (pt.X >= clientSize.Width - 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htTopRight);
                    return Result;
                }
                ///allow resize on the top border
                if (pt.Y <= 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htTop);
                    Input.Mouse.Cursor = Cursor.NsResize;
                    return Result;
                }
                ///allow resize on the bottom border
                if (pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htBottom);
                    Input.Mouse.Cursor = Cursor.NsResize;
                    return Result;
                }
                ///allow resize on the left border
                if (pt.X <= 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htLeft);
                    Input.Mouse.Cursor = Cursor.EwResize;
                    return Result;
                }
                ///allow resize on the right border
                if (pt.X >= clientSize.Width - 16 && clientSize.Height >= 16)
                {
                    Result = (IntPtr)(htRight);
                    Input.Mouse.Cursor = Cursor.EwResize;
                    return Result;
                }

                Result = (IntPtr)(HTCAPTION);
                Input.Mouse.Cursor = Cursor.NeswResize;
                return Result;
            }
            #endregion

            switch (msg)
            {
                #region keyboard
                case 0x100:/*WM_KEYDOWN*/
                    {
                        var keyCode = wParam.ToUInt32();
                        if (wParam.ToUInt64() < 256)
                        {
                            Input.Keyboard.lastKeyStates[keyCode] = Input.Keyboard.keyStates[keyCode];
                            Input.Keyboard.keyStates[keyCode] = InputState.Down;
                        }

                        //DEBUG only begin
                        if(keyCode == (int)Input.Keyboard.Key.Escape)
                        {
                            Application.Quit();
                        }
                        //DEBUG only end

                        return IntPtr.Zero;
                    }
                case 0x101:/*WM_KEYUP*/
                    {
                        var keyCode = wParam.ToUInt32();
                        if (wParam.ToUInt64() < 256)
                        {
                            Input.Keyboard.lastKeyStates[keyCode] = Input.Keyboard.keyStates[keyCode];
                            Input.Keyboard.keyStates[keyCode] = InputState.Up;
                        }
                        return IntPtr.Zero;
                    }
                #endregion
                #region mouse
                case 0x0201://WM_LBUTTONDOWN
                    Input.Mouse.LeftButtonState = InputState.Down;
                    return IntPtr.Zero;
                case 0x0202://WM_LBUTTONUP
                    Input.Mouse.LeftButtonState = InputState.Up;
                    return IntPtr.Zero;
                case 0x0203://WM_LBUTTONDBLCLK
                    return IntPtr.Zero;
                case 0x0206://WM_RBUTTONDBLCLK
                    return IntPtr.Zero;
                case 0x0204://WM_RBUTTONDOWN
                    Input.Mouse.RightButtonState = InputState.Down;
                    return IntPtr.Zero;
                case 0x0205://WM_RBUTTONUP
                    Input.Mouse.RightButtonState = InputState.Up;
                    return IntPtr.Zero;
                case 0x020A://WM_MOUSEWHEEL
                    Input.Mouse.MouseWheel += ((wParam.ToUInt32() >> 16) & 0xffff) > 0 ? +1.0f : -1.0f;
                    return IntPtr.Zero;
                case 0x0200://WM_MOUSEMOVE
                    var p = new POINT {
                        X = unchecked((short)lParam),
                        Y = unchecked((short)((uint)lParam >> 16))
                    };
                    ClientToScreen(hWnd, ref p);
                    Input.Mouse.MousePos = new Point(p.X, p.Y);
                    return IntPtr.Zero;
                case 0x0020:// WM_SETCURSOR
                    return IntPtr.Zero;//do nothing, we'll handle the cursor.
                #endregion
                #region ime
                case 0x0102:/*WM_CHAR*/
                    {
                        char c = (char)wParam;
                        if (c > 0 && !char.IsControl(c))
                            Application.ImeBuffer.Enqueue(c);
                        return IntPtr.Zero;
                    }
                #endregion
                case 0x2://WM_DESTROY
                    PostQuitMessage(0);
                    return IntPtr.Zero;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public IWindow CreateWindow(IntPtr nativeWindow)
        {
            throw new NotSupportedException();
        }

        WNDCLASS wndclass; //Forbid GC of the windowProc delegate instance
        public IWindow CreateWindow(Point position, Size size)
        {
            IntPtr hInstance = processHandle.DangerousGetHandle();
            string szAppName = "ImGuiApplication~";

            wndclass.style = 0x0002 /*CS_HREDRAW*/ | 0x0001/*CS_VREDRAW*/;
            wndclass.lpfnWndProc = WindowProc;

            wndclass.cbClsExtra = 0;
            wndclass.cbWndExtra = 0;
            wndclass.hInstance = hInstance;
            wndclass.hIcon = LoadIcon(IntPtr.Zero, new IntPtr(32512/*IDI_APPLICATION*/));
            wndclass.hCursor = LoadCursor(IntPtr.Zero, 32512/*IDC_ARROW*/);
            wndclass.hbrBackground = GetStockObject(0);
            wndclass.lpszMenuName = null;
            wndclass.lpszClassName = szAppName;

            ushort atom = RegisterClass(ref wndclass);

            if (atom == 0)
            {
                throw new Exception(string.Format("RegisterClass error: {0}", Marshal.GetLastWin32Error()));
            }

            IntPtr hwnd = CreateWindowEx(
                0,
                new IntPtr(atom), // window class name
                "ImGuiWindow~", // window caption
                (uint)(WindowStyles.WS_POPUP), // window style
                (int)position.X, // initial x position
                (int)position.Y, // initial y position
                (int)size.Width, // initial x size
                (int)size.Height, // initial y size
                IntPtr.Zero, // parent window handle
                IntPtr.Zero, // window menu handle
                hInstance, // program instance handle
                IntPtr.Zero); // creation parameters

            if (hwnd == IntPtr.Zero)
            {
                throw new Exception(string.Format("CreateWindowEx error: {0}", Marshal.GetLastWin32Error()));
            }

            ShowWindow(hwnd, 1/*SW_SHOWNORMAL*/);
            UpdateWindow(hwnd);

            Win32InputContext.LoadCursors();

            return new Win32Window(hwnd);
        }

        public Size GetWindowSize(IWindow window)
        {
            RECT rect;
            GetWindowRect(window.Pointer, out rect);
            return new Size(rect.right - rect.left, rect.bottom - rect.top);
        }

        public Point GetWindowPosition(IWindow window)
        {
            RECT rect;
            GetWindowRect(window.Pointer, out rect);
            return new Point(rect.left, rect.top);
        }

        public void SetWindowSize(IWindow window, Size size)
        {
            SetWindowPos(window.Pointer, IntPtr.Zero, 0, 0, (int)size.Width, (int)size.Height, 0x0002/*SWP_NOMOVE*/ | 0x0004/*SWP_NOZORDER*/);
        }

        public void SetWindowPosition(IWindow window, Point position)
        {
            SetWindowPos(window.Pointer, IntPtr.Zero, (int)position.X, (int)position.Y, 0, 0, 0x0001/*SWP_NOSIZE*/ | 0x0004/*SWP_NOZORDER*/);
        }

        public void SetWindowsRect(IWindow window, Rect rect)
        {
            SetWindowPos(window.Pointer, IntPtr.Zero, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, 0x0004/*SWP_NOZORDER*/);
        }

        public string GetWindowTitle(IWindow window)
        {
            return GetWindowText(window.Pointer);
        }

        public void SetWindowTitle(IWindow window, string title)
        {
            SetWindowText(window.Pointer, title);
        }

        public void ShowWindow(IWindow window)
        {
            ShowWindow(window.Pointer, 5/*SW_SHOW*/);
        }

        public void HideWindow(IWindow window)
        {
            ShowWindow(window.Pointer, 0/*SW_HIDE*/);
        }

        public void CloseWindow(IWindow window)
        {
            DestroyWindow(window.Pointer);
        }

        public void MinimizeWindow(IWindow window)
        {
            ShowWindow(window.Pointer, 2 /*ShowMinimized*/);
        }

        public void MaximizeWindow(IWindow window)
        {
            ShowWindow(window.Pointer, 3 /*ShowMaximized*/);
        }

        public void NormalizeWindow(IWindow window)
        {
            ShowWindow(window.Pointer, 1 /*Normal */);
        }

        private static Rect GetWorkarea()
        {
            RECT rectDesktop = new RECT();
            SystemParametersInfo(0x0030 /*SPI_GETWORKAREA*/, 0, ref rectDesktop, 0);
            return new Rect(new Point(rectDesktop.left, rectDesktop.top), new Point(rectDesktop.right, rectDesktop.bottom));
        }

        public Point ScreenToClient(IWindow window, Point point)
        {
            var posInScreen = point;
            POINT p = new POINT { X = (int)point.X, Y = (int)point.Y };
            ScreenToClient(window.Pointer, ref p);
            return new Point(p.X, p.Y);
        }

        public Point ClientToScreen(IWindow window, Point point)
        {
            var posInScreen = point;
            POINT p = new POINT { X = (int)point.X, Y = (int)point.Y };
            ClientToScreen(window.Pointer, ref p);
            return new Point(p.X, p.Y);
        }
    }
}
