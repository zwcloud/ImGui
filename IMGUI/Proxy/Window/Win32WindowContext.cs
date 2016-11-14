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
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);

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

        private IntPtr WindowProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
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
                case 0x0102:/*WM_CHAR*/
                    {
                        char c = (char)wParam;
                        if (c > 0 && !char.IsControl(c))
                            Application.ImeBuffer.Enqueue(c);
                        return IntPtr.Zero;
                    }
                case 0x2://WM_DESTROY
                    PostQuitMessage(0);
                    return IntPtr.Zero;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
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
