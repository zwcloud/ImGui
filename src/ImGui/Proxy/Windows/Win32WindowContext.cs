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
        static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

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

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustWindowRect(ref RECT lpRect, uint dwStyle, bool bMenu);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        const int GWL_STYLE = -16;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
        }
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
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
        static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

        #endregion

        static SafeProcessHandle processHandle = Process.GetCurrentProcess().SafeHandle;

        public Win32WindowContext()
        {
        }
        public void MainLoop(Action guiMethod)
        {
            // process windows message
            MSG msg = new MSG();
            if (PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0x0001/*PM_REMOVE*/))//handle windows messages
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }

            //run gui
            guiMethod();
        }

        private IntPtr WindowProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                #region keyboard
                case 0x100:/*WM_KEYDOWN*/
                    {
                        var keyCode = wParam.ToUInt32();
                        if (wParam.ToUInt64() < 256)
                        {
                            Input.Keyboard.lastKeyStates[keyCode] = Input.Keyboard.keyStates[keyCode];
                            Input.Keyboard.keyStates[keyCode] = KeyState.Down;
                        }

                        if (keyCode == (int)Key.Enter)
                        {
                            Input.ImeBuffer.Enqueue('\n');
                            return new IntPtr(1);
                        }

                        //DEBUG only begin
                        if(keyCode == (int)Key.Escape)
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
                            Input.Keyboard.keyStates[keyCode] = KeyState.Up;
                        }
                        return IntPtr.Zero;
                    }
                #endregion
                #region mouse
                case 0x0201://WM_LBUTTONDOWN
                    Input.Mouse.LeftButtonState = KeyState.Down;
                    return IntPtr.Zero;
                case 0x0202://WM_LBUTTONUP
                    Input.Mouse.LeftButtonState = KeyState.Up;
                    return IntPtr.Zero;
                case 0x0203://WM_LBUTTONDBLCLK
                    return IntPtr.Zero;
                case 0x0206://WM_RBUTTONDBLCLK
                    return IntPtr.Zero;
                case 0x0204://WM_RBUTTONDOWN
                    Input.Mouse.RightButtonState = KeyState.Down;
                    return IntPtr.Zero;
                case 0x0205://WM_RBUTTONUP
                    Input.Mouse.RightButtonState = KeyState.Up;
                    return IntPtr.Zero;
                case 0x020A://WM_MOUSEWHEEL
                    Input.Mouse.MouseWheel = ((short)(wParam.ToUInt64() >> 16));
                    return IntPtr.Zero;
                case 0x0200://WM_MOUSEMOVE
                    var p = new POINT {
                        X = unchecked((short)lParam),
                        Y = unchecked((short)((uint)lParam >> 16))
                    };
                    Input.Mouse.Position = new Point(p.X, p.Y);
                    return IntPtr.Zero;
                #endregion
                #region ime
                //http://blog.csdn.net/shuilan0066/article/details/7679825
                case 0x0286:/*WM_IME_CHAR*/
                    {
                        char c = (char)wParam;
                        if (c > 0 && !char.IsControl(c))
                            Input.ImeBuffer.Enqueue(c);
                        return IntPtr.Zero;
                    }
                case 0x0102:/*WM_CHAR*/
                    {
                        char c = (char)wParam;
                        if (c > 0 && !char.IsControl(c))
                            Input.ImeBuffer.Enqueue(c);
                        return IntPtr.Zero;
                    }
                #endregion
                case 0x2://WM_DESTROY
                    PostQuitMessage(0);
                    //DEBUG only begin
                    Application.Quit();
                    //DEBUG only end
                    return IntPtr.Zero;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        WNDCLASS wndclass; //Forbid GC of the windowProc delegate instance
        public IWindow CreateWindow(Point position, Size size, WindowTypes windowType)
        {
            IntPtr hInstance = processHandle.DangerousGetHandle();
            string szAppName = "ImGuiApplication~";

            wndclass.style = 0x0002 /*CS_HREDRAW*/ | 0x0001/*CS_VREDRAW*/ | 0x0020/*CS_OWNDC*/;
            wndclass.lpfnWndProc = WindowProc;
            wndclass.cbClsExtra = 0;
            wndclass.cbWndExtra = 0;
            wndclass.hInstance = hInstance;
            wndclass.hIcon = LoadIcon(IntPtr.Zero, new IntPtr(32512/*IDI_APPLICATION*/));
            wndclass.hCursor = LoadCursor(IntPtr.Zero, 32512/*IDC_ARROW*/);
            wndclass.hbrBackground = IntPtr.Zero;// GetStockObject(0);
            wndclass.lpszMenuName = null;
            wndclass.lpszClassName = szAppName;

            ushort atom = RegisterClassW(ref wndclass);

            if (atom == 0)
            {
                throw new WindowCreateException(string.Format("RegisterClass error: {0}", Marshal.GetLastWin32Error()));
            }

            WindowStyles windowStyle;
            switch (windowType)
            {
                case WindowTypes.Regular:
                    windowStyle = WindowStyles.WS_OVERLAPPEDWINDOW;
                    break;
                case WindowTypes.ToolBox:
                    windowStyle = WindowStyles.WS_DLGFRAME;
                    break;
                case WindowTypes.ToolTip:
                    windowStyle = WindowStyles.WS_POPUP;
                    break;
                default:
                    windowStyle = WindowStyles.WS_OVERLAPPEDWINDOW;
                    break;
            }

            // rect of the desired client area
            RECT rc = new RECT
            {
                left = (int)position.X,
                top = (int)position.Y,
                right = (int)(position.X + size.Width),
                bottom = (int)(position.Y + size.Height)
            };

            if(!AdjustWindowRect(ref rc, (uint)windowStyle, false))
            {
                throw new WindowCreateException(string.Format("AdjustWindowRectEx fails, win32 error: {0}", Marshal.GetLastWin32Error()));
            }

            //now rc is the rect of the window

            IntPtr hwnd = CreateWindowEx(
                0/*0x00000008*//*WS_EX_TOPMOST*/,// window style ex //HACK always on top
                new IntPtr(atom), // window class name
                "ImGuiWindow~", // window caption
                (uint)windowStyle, // window style
                rc.left, // initial x position
                rc.top, // initial y position
                rc.right - rc.left, // initial x size
                rc.bottom - rc.top, // initial y size
                IntPtr.Zero, // parent window handle
                IntPtr.Zero, // window menu handle
                hInstance, // program instance handle
                IntPtr.Zero); // creation parameters

            if (hwnd == IntPtr.Zero)
            {
                throw new WindowCreateException(string.Format("CreateWindowEx error: {0}", Marshal.GetLastWin32Error()));
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

        public Point GetClientPosition(IWindow window)
        {
            RECT rect;
            GetClientRect(window.Pointer, out rect);
            return new Point(rect.left, rect.top);
        }

        public void SetClientPosition(IWindow window, Point position)
        {
            RECT windowRect;
            GetWindowRect(window.Pointer, out windowRect);

            RECT clientRect;
            GetClientRect(window.Pointer, out clientRect);
            var clientWidth = clientRect.right - clientRect.left;
            var clientHeight = clientRect.bottom - clientRect.top;

            var windowStyle = GetWindowLong(window.Pointer, GWL_STYLE);

            //rect of the desired client area: size adjusted relative to the top-left corner
            RECT rc = new RECT
            {
                left = (int)position.X,
                top = (int)position.Y,
                right = (int)position.X + clientWidth,
                bottom = (int)position.Y + clientHeight
            };

            if (!AdjustWindowRect(ref rc, windowStyle, false))
            {
                throw new WindowUpdateException(string.Format("AdjustWindowRectEx fails, win32 error: {0}", Marshal.GetLastWin32Error()));
            }

            //now rc is the rect of the window, apply it
            SetWindowPos(window.Pointer, IntPtr.Zero, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, 0x0002/*SWP_NOMOVE*/ | 0x0004/*SWP_NOZORDER*/);
        }

        public Size GetClientSize(IWindow window)
        {
            RECT rect;
            GetClientRect(window.Pointer, out rect);
            return new Size(rect.right - rect.left, rect.bottom - rect.top);
        }

        public void SetClientSize(IWindow window, Size size)
        {
            RECT windowRect;
            GetWindowRect(window.Pointer, out windowRect);

            RECT clientRect;
            GetClientRect(window.Pointer, out clientRect);

            var windowStyle = GetWindowLong(window.Pointer, GWL_STYLE);

            //rect of the desired client area: size adjusted relative to the top-left corner
            RECT rc = new RECT
            {
                left = clientRect.left,
                top = clientRect.top,
                right = clientRect.left + (int)size.Width,
                bottom = clientRect.top + (int)size.Height
            };

            if (!AdjustWindowRect(ref rc, windowStyle, false))
            {
                throw new WindowUpdateException(string.Format("AdjustWindowRectEx fails, win32 error: {0}", Marshal.GetLastWin32Error()));
            }

            //now rc is the rect of the window, apply it
            SetWindowPos(window.Pointer, IntPtr.Zero, rc.left, rc.top, rc.right - rc.left, rc.bottom-rc.top, 0x0002/*SWP_NOMOVE*/ | 0x0004/*SWP_NOZORDER*/);
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
