using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGui.Input;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSImplementation.Windows
{
    internal class Win32Window : IWindow
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

        [Flags]
        enum WindowStylesEx : uint
        {
            /// <summary>Specifies a window that accepts drag-drop files.</summary>
            WS_EX_ACCEPTFILES = 0x00000010,

            /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
            WS_EX_APPWINDOW = 0x00040000,

            /// <summary>Specifies a window that has a border with a sunken edge.</summary>
            WS_EX_CLIENTEDGE = 0x00000200,

            /// <summary>
            /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
            /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
            /// </summary>
            /// <remarks>
            /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
            /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
            /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
            /// Double-buffering allows the window and its descendents to be painted without flicker.
            /// </remarks>
            WS_EX_COMPOSITED = 0x02000000,

            /// <summary>
            /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
            /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
            /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
            /// The Help application displays a pop-up window that typically contains help for the child window.
            /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
            /// </summary>
            WS_EX_CONTEXTHELP = 0x00000400,

            /// <summary>
            /// Specifies a window which contains child windows that should take part in dialog box navigation.
            /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
            /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
            /// </summary>
            WS_EX_CONTROLPARENT = 0x00010000,

            /// <summary>Specifies a window that has a double border.</summary>
            WS_EX_DLGMODALFRAME = 0x00000001,

            /// <summary>
            /// Specifies a window that is a layered window.
            /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            /// </summary>
            WS_EX_LAYERED = 0x00080000,

            /// <summary>
            /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_LAYOUTRTL = 0x00400000,

            /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
            WS_EX_LEFT = 0x00000000,

            /// <summary>
            /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_LEFTSCROLLBAR = 0x00004000,

            /// <summary>
            /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
            /// </summary>
            WS_EX_LTRREADING = 0x00000000,

            /// <summary>
            /// Specifies a multiple-document interface (MDI) child window.
            /// </summary>
            WS_EX_MDICHILD = 0x00000040,

            /// <summary>
            /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
            /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
            /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
            /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
            /// </summary>
            WS_EX_NOACTIVATE = 0x08000000,

            /// <summary>
            /// Specifies a window which does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOINHERITLAYOUT = 0x00100000,

            /// <summary>
            /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,

            /// <summary>
            /// The window does not render to a redirection surface.
            /// This is for windows that do not have visible content or that use mechanisms other than surfaces to provide their visual.
            /// </summary>
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000,

            /// <summary>Specifies an overlapped window.</summary>
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

            /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

            /// <summary>
            /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
            /// The shell language must support reading-order alignment for this to take effect.
            /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
            /// </summary>
            WS_EX_RIGHT = 0x00001000,

            /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            /// <summary>
            /// Specifies a window that displays text using right-to-left reading-order properties.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_RTLREADING = 0x00002000,

            /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
            WS_EX_STATICEDGE = 0x00020000,

            /// <summary>
            /// Specifies a window that is intended to be used as a floating toolbar.
            /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
            /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
            /// If a tool window has a system menu, its icon is not displayed on the title bar.
            /// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
            /// </summary>
            WS_EX_TOOLWINDOW = 0x00000080,

            /// <summary>
            /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
            /// To add or remove this style, use the SetWindowPos function.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,

            /// <summary>
            /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
            /// The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// To achieve transparency without these restrictions, use the SetWindowRgn function.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,

            /// <summary>Specifies a window that has a border with a raised edge.</summary>
            WS_EX_WINDOWEDGE = 0x00000100
        }

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr GetStockObject(int fnObject);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern ushort RegisterClass([In] ref WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
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
        static extern bool AdjustWindowRect(ref RECT lpRect, int dwStyle, bool bMenu);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
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

        [DllImport("user32.dll", EntryPoint = "GetWindowText", SetLastError = true, CharSet = CharSet.Unicode)]
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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll")]
        static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool BringWindowToTop(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);

        private static readonly IntPtr TRUE = new IntPtr(1);
        private static readonly IntPtr FALSE = IntPtr.Zero;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const int LWA_COLORKEY = 0x1;
        public const int LWA_ALPHA = 0x2;
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        #endregion

        IntPtr hwnd;

        #region Window creation

        public Win32Window()
        {
            parentWindow = null;
        }

        public Win32Window(IWindow parent)
        {
            parentWindow = parent;
        }

        static Microsoft.Win32.SafeHandles.SafeProcessHandle processHandle = Process.GetCurrentProcess().SafeHandle;

        private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                #region keyboard
                case 0x100:/*WM_KEYDOWN*/
                    {
                        var keyCode = wParam.ToInt32();
                        if (wParam.ToInt64() < 256)
                        {
                            Keyboard.Instance.lastKeyStates[keyCode] = Keyboard.Instance.keyStates[keyCode];
                            Keyboard.Instance.keyStates[keyCode] = KeyState.Down;
                        }

                        if (keyCode == (int)Key.Enter)
                        {
                            Ime.ImeBuffer.Enqueue('\n');
                            return new IntPtr(1);
                        }

                        //DEBUG only begin
                        if (keyCode == (int)Key.Escape)
                        {
                            Application.Quit();
                        }
                        //DEBUG only end

                        return IntPtr.Zero;
                    }
                case 0x101:/*WM_KEYUP*/
                    {
                        var keyCode = wParam.ToInt32();
                        if (wParam.ToInt64() < 256)
                        {
                            Keyboard.Instance.lastKeyStates[keyCode] = Keyboard.Instance.keyStates[keyCode];
                            Keyboard.Instance.keyStates[keyCode] = KeyState.Up;
                        }
                        return IntPtr.Zero;
                    }
                #endregion
                #region mouse
                case 0x0020: //WM_SETCURSOR
                {
                    if (wParam == hWnd)
                    {
                        //HTCLIENT is 1, see https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest#return-value
                        if (unchecked((short) lParam) == 1/*HTCLIENT*/)
                        {
                            Win32Cursor.ChangeCursor(Mouse.Instance.Cursor);
                            return TRUE;
                        }
                    }
                    break;
                }
                case 0x0201://WM_LBUTTONDOWN
                    Mouse.Instance.LeftButtonState = KeyState.Down;
                    return IntPtr.Zero;
                case 0x0202://WM_LBUTTONUP
                    Mouse.Instance.LeftButtonState = KeyState.Up;
                    return IntPtr.Zero;
                case 0x0203://WM_LBUTTONDBLCLK
                    return IntPtr.Zero;
                case 0x0206://WM_RBUTTONDBLCLK
                    return IntPtr.Zero;
                case 0x0204://WM_RBUTTONDOWN
                    Mouse.Instance.RightButtonState = KeyState.Down;
                    return IntPtr.Zero;
                case 0x0205://WM_RBUTTONUP
                    Mouse.Instance.RightButtonState = KeyState.Up;
                    return IntPtr.Zero;
                case 0x020A://WM_MOUSEWHEEL
                    Mouse.Instance.MouseWheel = ((short)(wParam.ToInt64() >> 16));
                    return IntPtr.Zero;
                case 0x0200://WM_MOUSEMOVE
                    var p = new POINT
                    {
                        X = unchecked((short)lParam),
                        Y = unchecked((short)((uint)lParam >> 16))
                    };
                    Mouse.Instance.Position = new Point(p.X, p.Y);
                    return IntPtr.Zero;
                #endregion
                #region ime
                //http://blog.csdn.net/shuilan0066/article/details/7679825
                case 0x0286:/*WM_IME_CHAR*/
                    {
                        char c = (char)wParam;
                        if (c > 0 && !char.IsControl(c))
                            Ime.ImeBuffer.Enqueue(c);
                        return IntPtr.Zero;
                    }
                case 0x0102:/*WM_CHAR*/
                    {
                        char c = (char)wParam;
                        if (c > 0 && !char.IsControl(c))
                            Ime.ImeBuffer.Enqueue(c);
                        return IntPtr.Zero;
                    }
                #endregion
                case 0x2://WM_DESTROY
                    Close();
                    return IntPtr.Zero;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        //Declare as a field to forbid GC,
        //otherwise it will be GC-collected because no managed object is referencing it.
        // Maybe it's better to use GCHandle.Alloc to pin this object locally?
        private WNDCLASS wndclass;
        
        private IWindow parentWindow;

        public void Init(Point position, Size size, WindowTypes windowType)
        {
            IntPtr hInstance = processHandle.DangerousGetHandle();

            this.wndclass.style = 0x0002 /*CS_HREDRAW*/ | 0x0001/*CS_VREDRAW*/ | 0x0020/*CS_OWNDC*/;
            this.wndclass.lpfnWndProc = WindowProc;
            this.wndclass.cbClsExtra = 0;
            this.wndclass.cbWndExtra = 0;
            this.wndclass.hInstance = hInstance;
            this.wndclass.hIcon = LoadIcon(IntPtr.Zero, new IntPtr(32512/*IDI_APPLICATION*/));
            this.wndclass.hCursor = LoadCursor(IntPtr.Zero, 32512/*IDC_ARROW*/);
            this.wndclass.hbrBackground = IntPtr.Zero;// GetStockObject(0);
            this.wndclass.lpszMenuName = null;
            this.wndclass.lpszClassName = "ImGuiWindow~" + this.GetHashCode();


            ushort atom = RegisterClass(ref this.wndclass);

            if (atom == 0)
            {
                throw new WindowCreateException(string.Format("RegisterClass error: {0}", Marshal.GetLastWin32Error()));
            }

            WindowStylesEx windowExStyle = 0;
            //windowExStyle |= 0x00000008;//(debug only) WS_EX_TOPMOST, window style ex to make the window always on top

            WindowStyles windowStyle;
            switch (windowType)
            {
                case WindowTypes.Regular:
                    windowStyle = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_CLIPCHILDREN;
                    break;
                case WindowTypes.ClientAreaOnly:
                    windowStyle = WindowStyles.WS_POPUP | WindowStyles.WS_SYSMENU;
                    break;
                case WindowTypes.Hidden:
                    windowStyle = 0;
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

            if (!AdjustWindowRect(ref rc, (int)windowStyle, false))
            {
                throw new WindowCreateException(string.Format("AdjustWindowRectEx fails, win32 error: {0}", Marshal.GetLastWin32Error()));
            }

            //now rc is the rect of the window

            IntPtr hwnd = CreateWindowEx(
                (uint)windowExStyle,
                new IntPtr(atom), // window class name
                windowType == WindowTypes.ClientAreaOnly? string.Empty : "ImGuiWindow~", // window caption
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

            this.hwnd = hwnd;
        }

        #endregion

        #region implementation of IWindow

        public object Handle
        {
            get
            {
                return this.hwnd;
            }
        }

        public IntPtr Pointer
        {
            get
            {
                return this.hwnd;
            }
        }

        public Point Position
        {
            get
            {
                RECT rect;
                GetWindowRect(this.Pointer, out rect);
                return new Point(rect.left, rect.top);
            }

            set
            {
                var position = value;
                SetWindowPos(this.Pointer, IntPtr.Zero, (int)position.X, (int)position.Y, 0, 0, 0x0001/*SWP_NOSIZE*/ | 0x0004/*SWP_NOZORDER*/);
            }
        }

        public Size Size
        {
            get
            {
                RECT rect;
                GetWindowRect(this.Pointer, out rect);
                return new Size(rect.right - rect.left, rect.bottom - rect.top);
            }

            set
            {
                var size = value;
                SetWindowPos(this.Pointer, IntPtr.Zero, 0, 0, (int)size.Width, (int)size.Height, 0x0002/*SWP_NOMOVE*/ | 0x0004/*SWP_NOZORDER*/);
            }
        }

        public Point ClientPosition
        {
            get {
                RECT rect;
                GetClientRect(this.Pointer, out rect);
                return new Point(rect.left, rect.top);
            }
            set {
                var position = value;

                RECT windowRect;
                GetWindowRect(this.Pointer, out windowRect);

                RECT clientRect;
                GetClientRect(this.Pointer, out clientRect);
                var clientWidth = clientRect.right - clientRect.left;
                var clientHeight = clientRect.bottom - clientRect.top;

                var windowStyle = GetWindowLong(this.Pointer, GWL_STYLE);

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
                SetWindowPos(this.Pointer, IntPtr.Zero, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, 0x0002/*SWP_NOMOVE*/ | 0x0004/*SWP_NOZORDER*/);

            }
        }

        public Size ClientSize
        {
            get
            {
                RECT rect;
                GetClientRect(this.Pointer, out rect);
                return new Size(rect.right - rect.left, rect.bottom - rect.top);
            }

            set
            {
                var size = value;

                RECT clientRect;
                GetClientRect(this.Pointer, out clientRect);
                //rect of the desired client area: size adjusted relative to the top-left corner
                RECT rc = new RECT
                {
                    left = clientRect.left,
                    top = clientRect.top,
                    right = clientRect.left + (int)size.Width,
                    bottom = clientRect.top + (int)size.Height
                };

                var windowStyle = GetWindowLong(this.Pointer, GWL_STYLE);

                if (!AdjustWindowRect(ref rc, windowStyle, false))
                {
                    throw new WindowUpdateException(
                        $"AdjustWindowRectEx fails, win32 error: {Marshal.GetLastWin32Error()}");
                }

                //now rc is the rect of the window, apply it
                SetWindowPos(this.Pointer, IntPtr.Zero,
                    rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top,
                    0x0002/*SWP_NOMOVE*/ | 0x0004/*SWP_NOZORDER*/);
            }
        }

        public string Title
        {
            get
            {
                return GetWindowText(this.Pointer);
            }

            set
            {
                var title = value;
                SetWindowText(this.Pointer, title);
            }
        }

        public bool Closed { get; private set; } = false;

        public void Close()
        {
            DestroyWindow(this.Pointer);
            this.Closed = true;
        }

        public void Hide()
        {
            ShowWindow(this.Pointer, 0/*SW_HIDE*/);
        }

        public void Show()
        {
            ShowWindow(this.Pointer, 5/*SW_SHOW*/);
        }

        public Point ScreenToClient(Point point)
        {
            POINT p = new POINT { X = (int)point.X, Y = (int)point.Y };
            ScreenToClient(this.Pointer, ref p);
            return new Point(p.X, p.Y);
        }

        public Point ClientToScreen(Point point)
        {
            POINT p = new POINT { X = (int)point.X, Y = (int)point.Y };
            ClientToScreen(this.Pointer, ref p);
            return new Point(p.X, p.Y);
        }

        public bool GetFocus()
        {
            return GetForegroundWindow() == hwnd;
        }

        public void SetFocus()
        {
            BringWindowToTop(hwnd);
            SetForegroundWindow(hwnd);
            SetFocus(hwnd);
        }

        public bool Minimized => IsIconic(hwnd);

        public float Opacity
        {
            set
            {
                Debug.Assert(hwnd != IntPtr.Zero);
                if (value < 0.0f && value > 1.0f)
                {
                    throw new InvalidOperationException("Opacity must be in range [0, 1]");
                }
                
                if (value < 1.0f)
                {
                    var style = GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED;
                    SetWindowLong(hwnd, GWL_EXSTYLE, style);
                    SetLayeredWindowAttributes(hwnd, 0, (byte)(255 * value), LWA_ALPHA);
                }
                else
                {
                    var style = GetWindowLong(hwnd, GWL_EXSTYLE) & ~WS_EX_LAYERED;
                    SetWindowLong(hwnd, GWL_EXSTYLE, style);
                }

            }
        }

        public void ChangeCursor(Cursor cursor)
        {
            //
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
            guiMethod?.Invoke();
        }
        #endregion
    }
}
