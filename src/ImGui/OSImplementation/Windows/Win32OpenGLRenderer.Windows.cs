using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSharpGL;

namespace ImGui.OSImplementation.Windows
{
    internal partial class Win32OpenGLRenderer
    {
        #region Native
        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public void Init()
            {
                this.nSize = (ushort)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>();
                this.nVersion = 1;
                this.dwFlags = PFD_FLAGS.PFD_DRAW_TO_WINDOW | PFD_FLAGS.PFD_SUPPORT_OPENGL | PFD_FLAGS.PFD_DOUBLEBUFFER;
                this.iPixelType = PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
                this.cColorBits = 32;
                this.cAlphaBits = 8;
                this.cDepthBits = 24;
                this.cStencilBits = 8;
                this.iLayerType = PFD_LAYER_TYPES.PFD_MAIN_PLANE;
            }
            ushort nSize;
            ushort nVersion;
            PFD_FLAGS dwFlags;
            PFD_PIXEL_TYPE iPixelType;
            byte cColorBits;
            byte cRedBits;
            byte cRedShift;
            byte cGreenBits;
            byte cGreenShift;
            byte cBlueBits;
            byte cBlueShift;
            byte cAlphaBits;
            byte cAlphaShift;
            byte cAccumBits;
            byte cAccumRedBits;
            byte cAccumGreenBits;
            byte cAccumBlueBits;
            byte cAccumAlphaBits;
            byte cDepthBits;
            byte cStencilBits;
            byte cAuxBuffers;
            PFD_LAYER_TYPES iLayerType;
            byte bReserved;
            uint dwLayerMask;
            uint dwVisibleMask;
            uint dwDamageMask;
        }

        [Flags]
        public enum PFD_FLAGS : uint
        {
            PFD_DOUBLEBUFFER = 0x00000001,
            PFD_STEREO = 0x00000002,
            PFD_DRAW_TO_WINDOW = 0x00000004,
            PFD_DRAW_TO_BITMAP = 0x00000008,
            PFD_SUPPORT_GDI = 0x00000010,
            PFD_SUPPORT_OPENGL = 0x00000020,
            PFD_GENERIC_FORMAT = 0x00000040,
            PFD_NEED_PALETTE = 0x00000080,
            PFD_NEED_SYSTEM_PALETTE = 0x00000100,
            PFD_SWAP_EXCHANGE = 0x00000200,
            PFD_SWAP_COPY = 0x00000400,
            PFD_SWAP_LAYER_BUFFERS = 0x00000800,
            PFD_GENERIC_ACCELERATED = 0x00001000,
            PFD_SUPPORT_DIRECTDRAW = 0x00002000,
            PFD_DIRECT3D_ACCELERATED = 0x00004000,
            PFD_SUPPORT_COMPOSITION = 0x00008000,
            PFD_DEPTH_DONTCARE = 0x20000000,
            PFD_DOUBLEBUFFER_DONTCARE = 0x40000000,
            PFD_STEREO_DONTCARE = 0x80000000
        }

        public enum PFD_LAYER_TYPES : byte
        {
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = 255
        }

        public enum PFD_PIXEL_TYPE : byte
        {
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1
        }

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool DescribePixelFormat(IntPtr hdc, int iPixelFormat, uint nBytes, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern IntPtr wglCreateContext(IntPtr hDC);

        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern bool wglMakeCurrent(IntPtr hDC, IntPtr hRC);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool SwapBuffers(IntPtr hdc);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern bool wglDeleteContext(IntPtr hRC);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #region Wgl

        internal class Wgl
        {
            [DllImport("OPENGL32.DLL", EntryPoint = "wglGetProcAddress", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr GetProcAddress(string lpszProc);

            [DllImport("OPENGL32.DLL", EntryPoint = "wglGetCurrentContext", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr GetCurrentContext();

            [DllImport("OPENGL32.DLL", EntryPoint = "wglCreateContext", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr CreateContext(IntPtr hDc);

            [DllImport("OPENGL32.DLL", EntryPoint = "wglMakeCurrent", ExactSpelling = true, SetLastError = true)]
            internal static extern bool Native_MakeCurrent(IntPtr hDc, IntPtr newContext);

            internal static bool MakeCurrent(IntPtr hDc, IntPtr hglrc)
            {
                return Native_MakeCurrent(hDc, hglrc);
            }

            [DllImport("OPENGL32.DLL", EntryPoint = "wglDeleteContext", ExactSpelling = true, SetLastError = true)]
            internal static extern bool DeleteContext(IntPtr oldContext);

            private static string[] EntryPointNames;

            private static IntPtr[] EntryPoints;

            static Wgl()
            {
                Wgl.EntryPointNames = new string[]
                {
                    "wglCreateContextAttribsARB",//0
                    "wglGetExtensionsStringARB",
                    "wglGetPixelFormatAttribivARB",
                    "wglGetPixelFormatAttribfvARB",
                    "wglChoosePixelFormatARB",//4
                    "wglMakeContextCurrentARB",
                    "wglGetCurrentReadDCARB",
                    "wglCreatePbufferARB",
                    "wglGetPbufferDCARB",
                    "wglReleasePbufferDCARB",
                    "wglDestroyPbufferARB",
                    "wglQueryPbufferARB",
                    "wglBindTexImageARB",
                    "wglReleaseTexImageARB",
                    "wglSetPbufferAttribARB",
                    "wglGetExtensionsStringEXT",
                    "wglSwapIntervalEXT",
                    "wglGetSwapIntervalEXT"
                };
                Wgl.EntryPoints = new IntPtr[Wgl.EntryPointNames.Length];
            }

            private static bool IsValid(IntPtr address)
            {
                long a = address.ToInt64();
                return a < -1L || a > 3L;
            }

            private IntPtr GetAddress(string function_string)
            {
                IntPtr address = Wgl.GetProcAddress(function_string);
                if (!Wgl.IsValid(address))
                {
                    address = Win32.GetProcAddress(function_string);
                }
                return address;
            }
            internal void LoadEntryPoints(IntPtr hdc)
            {
                for (int i = 0; i < Wgl.EntryPointNames.Length; i++)
                {
                    Wgl.EntryPoints[i] = GetAddress(Wgl.EntryPointNames[i]);
                }
            }

            //  Delegates
            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            delegate bool wglChoosePixelFormatARB(IntPtr hdc, [In] int[] piAttribIList, [In] Single[] pfAttribFList, UInt32 nMaxFormats, [Out] out int piFormats, [Out] out UInt32 nNumFormats);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            delegate IntPtr wglCreateContextAttribsARB(IntPtr hdc, IntPtr hShareContext, [In] int[] attribList);

            internal static bool ChoosePixelFormatARB(IntPtr hdc, int[] piAttribIList, Single[] pfAttribFList, UInt32 nMaxFormats, [Out] out int piFormats, [Out] out UInt32 nNumFormats)
            {
                return Marshal.GetDelegateForFunctionPointer<wglChoosePixelFormatARB>(Wgl.EntryPoints[4])(hdc, piAttribIList, pfAttribFList, nMaxFormats, out piFormats, out nNumFormats);
            }

            internal static IntPtr CreateContextAttribsARB(IntPtr hdc, IntPtr hShareContext, [In] int[] attribList)
            {
                return Marshal.GetDelegateForFunctionPointer<wglCreateContextAttribsARB>(Wgl.EntryPoints[0])(hdc, hShareContext, attribList);
            }
        }

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

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

        [DllImport("user32.dll")]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);

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

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);

        enum WGL
        {
            //WGL_ARB_pixel_format
            WGL_NUMBER_PIXEL_FORMATS_ARB = 0x2000,
            WGL_DRAW_TO_WINDOW_ARB = 0x2001,
            WGL_DRAW_TO_BITMAP_ARB = 0x2002,
            WGL_ACCELERATION_ARB = 0x2003,
            WGL_NEED_PALETTE_ARB = 0x2004,
            WGL_NEED_SYSTEM_PALETTE_ARB = 0x2005,
            WGL_SWAP_LAYER_BUFFERS_ARB = 0x2006,
            WGL_SWAP_METHOD_ARB = 0x2007,
            WGL_NUMBER_OVERLAYS_ARB = 0x2008,
            WGL_NUMBER_UNDERLAYS_ARB = 0x2009,
            WGL_TRANSPARENT_ARB = 0x200A,
            WGL_SHARE_DEPTH_ARB = 0x200C,
            WGL_SHARE_STENCIL_ARB = 0x200D,
            WGL_SHARE_ACCUM_ARB = 0x200E,
            WGL_SUPPORT_GDI_ARB = 0x200F,
            WGL_SUPPORT_OPENGL_ARB = 0x2010,
            WGL_DOUBLE_BUFFER_ARB = 0x2011,
            WGL_STEREO_ARB = 0x2012,
            WGL_PIXEL_TYPE_ARB = 0x2013,
            WGL_COLOR_BITS_ARB = 0x2014,
            WGL_RED_BITS_ARB = 0x2015,
            WGL_RED_SHIFT_ARB = 0x2016,
            WGL_GREEN_BITS_ARB = 0x2017,
            WGL_GREEN_SHIFT_ARB = 0x2018,
            WGL_BLUE_BITS_ARB = 0x2019,
            WGL_BLUE_SHIFT_ARB = 0x201A,
            WGL_ALPHA_BITS_ARB = 0x201B,
            WGL_ALPHA_SHIFT_ARB = 0x201C,
            WGL_ACCUM_BITS_ARB = 0x201D,
            WGL_ACCUM_RED_BITS_ARB = 0x201E,
            WGL_ACCUM_GREEN_BITS_ARB = 0x201F,
            WGL_ACCUM_BLUE_BITS_ARB = 0x2020,
            WGL_ACCUM_ALPHA_BITS_ARB = 0x2021,
            WGL_DEPTH_BITS_ARB = 0x2022,
            WGL_STENCIL_BITS_ARB = 0x2023,
            WGL_AUX_BUFFERS_ARB = 0x2024,
            WGL_NO_ACCELERATION_ARB = 0x2025,
            WGL_GENERIC_ACCELERATION_ARB = 0x2026,
            WGL_FULL_ACCELERATION_ARB = 0x2027,
            WGL_SWAP_EXCHANGE_ARB = 0x2028,
            WGL_SWAP_COPY_ARB = 0x2029,
            WGL_SWAP_UNDEFINED_ARB = 0x202A,
            WGL_TYPE_RGBA_ARB = 0x202B,
            WGL_TYPE_COLORINDEX_ARB = 0x202C,
            WGL_TRANSPARENT_RED_VALUE_ARB = 0x2037,
            WGL_TRANSPARENT_GREEN_VALUE_ARB = 0x2038,
            WGL_TRANSPARENT_BLUE_VALUE_ARB = 0x2039,
            WGL_TRANSPARENT_ALPHA_VALUE_ARB = 0x203A,
            WGL_TRANSPARENT_INDEX_VALUE_ARB = 0x203B,

            //WGL_ARB_multisample
            WGL_SAMPLE_BUFFERS_ARB = 0x2041,
            WGL_SAMPLES_ARB = 0x2042,

            //WGL_CONTEXT attributes
            WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091,
            WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092,
            WGL_CONTEXT_LAYER_PLANE_ARB = 0x2093,
            WGL_CONTEXT_FLAGS_ARB = 0x2094,
            WGL_CONTEXT_DEBUG_BIT_ARB = 0x00000001,
            WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x00000002,
            WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126,
            WGL_CONTEXT_CORE_PROFILE_BIT_ARB = 0x00000001,
            WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB = 0x00000002,
            ERROR_INVALID_PROFILE_ARB = 0x2096
        }

        #endregion

        #endregion

        IntPtr hDC;
        IntPtr hglrc;
        IntPtr hwnd;
        private void CreateOpenGLContext(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            this.hDC = GetDC(hwnd);

            var pixelformatdescriptor = new PIXELFORMATDESCRIPTOR();
            pixelformatdescriptor.Init();

            if(!Application.EnableMSAA)
            {
                int pixelFormat;
                pixelFormat = ChoosePixelFormat(this.hDC, ref pixelformatdescriptor);
                SetPixelFormat(this.hDC, pixelFormat, ref pixelformatdescriptor);

                this.hglrc = wglCreateContext(this.hDC);
                wglMakeCurrent(this.hDC, this.hglrc);
            }
            else
            {
                IntPtr tempContext = IntPtr.Zero;
                IntPtr tempHwnd = IntPtr.Zero;
                //Create temporary window
                IntPtr hInstance = Process.GetCurrentProcess().SafeHandle.DangerousGetHandle();

                WNDCLASS wndclass = new WNDCLASS()
                {
                    style = 0x0002 /*CS_HREDRAW*/ | 0x0001/*CS_VREDRAW*/ | 0x0020/*CS_OWNDC*/,
                    lpfnWndProc = (hWnd, msg, wParam, lParam) => DefWindowProc(hWnd, msg, wParam, lParam),
                    hInstance = hInstance,
                    lpszClassName = "tmpWindowForMSAA~"+this.GetHashCode()
                };
                ushort atom = RegisterClassW(ref wndclass);
                if (atom == 0)
                {
                    throw new WindowCreateException(string.Format("RegisterClass error: {0}", Marshal.GetLastWin32Error()));
                }

                tempHwnd = CreateWindowEx(
                    0,
                    new IntPtr(atom),
                    "tmpWindow~",
                    (uint)(WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN),
                    0, 0, 1, 1, hwnd, IntPtr.Zero,
                    hInstance,
                    IntPtr.Zero);
                if (tempHwnd == IntPtr.Zero)
                {
                    throw new WindowCreateException(string.Format("CreateWindowEx for tempContext error: {0}", Marshal.GetLastWin32Error()));
                }

                IntPtr tempHdc = GetDC(tempHwnd);

                var tempPixelFormat = ChoosePixelFormat(tempHdc, ref pixelformatdescriptor);
                if(tempPixelFormat == 0)
                {
                    throw new Exception(string.Format("ChoosePixelFormat failed: error {0}", Marshal.GetLastWin32Error()));
                }

                if (!SetPixelFormat(tempHdc, tempPixelFormat, ref pixelformatdescriptor))
                {
                    throw new Exception(string.Format("SetPixelFormat failed: error {0}", Marshal.GetLastWin32Error()));
                }

                tempContext = Wgl.CreateContext(tempHdc);//Crate temp context to load entry points
                if (tempContext == IntPtr.Zero)
                {
                    throw new Exception(string.Format("wglCreateContext for tempHdc failed: error {0}", Marshal.GetLastWin32Error()));
                }

                if (!Wgl.MakeCurrent(tempHdc, tempContext))
                {
                    throw new Exception(string.Format("wglMakeCurrent for tempContext failed: error {0}", Marshal.GetLastWin32Error()));
                }

                //load wgl entry points for wglChoosePixelFormatARB
                new Wgl().LoadEntryPoints(tempHdc);

                int[] iPixAttribs = {
                    (int)WGL.WGL_SUPPORT_OPENGL_ARB, (int)GL.GL_TRUE,
                    (int)WGL.WGL_DRAW_TO_WINDOW_ARB, (int)GL.GL_TRUE,
                    (int)WGL.WGL_DOUBLE_BUFFER_ARB,  (int)GL.GL_TRUE,
                    (int)WGL.WGL_PIXEL_TYPE_ARB,     (int)WGL.WGL_TYPE_RGBA_ARB,
                    (int)WGL.WGL_ACCELERATION_ARB,   (int)WGL.WGL_FULL_ACCELERATION_ARB,
                    (int)WGL.WGL_COLOR_BITS_ARB,     24,
                    (int)WGL.WGL_ALPHA_BITS_ARB,     8,
                    (int)WGL.WGL_DEPTH_BITS_ARB,     24,
                    (int)WGL.WGL_STENCIL_BITS_ARB,   8,
                    (int)WGL.WGL_SWAP_METHOD_ARB, (int)WGL.WGL_SWAP_EXCHANGE_ARB,
                    //(int)WGL.WGL_SAMPLE_BUFFERS_ARB, (int)GL.GL_TRUE,//Enable MSAA
                    //(int)WGL.WGL_SAMPLES_ARB,        16,
                0};

                int pixelFormat;
                uint numFormats;
                var result = Wgl.ChoosePixelFormatARB(this.hDC, iPixAttribs, null, 1, out pixelFormat, out numFormats);
                if (result == false || numFormats == 0)
                {
                    throw new Exception(string.Format("wglChoosePixelFormatARB failed: error {0}", Marshal.GetLastWin32Error()));
                }

                if(!DescribePixelFormat(this.hDC, pixelFormat, (uint)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>(), ref pixelformatdescriptor))
                {
                    throw new Exception(string.Format("DescribePixelFormat failed: error {0}", Marshal.GetLastWin32Error()));
                }

                if (!SetPixelFormat(this.hDC, pixelFormat, ref pixelformatdescriptor))
                {
                    throw new Exception(string.Format("SetPixelFormat failed: error {0}", Marshal.GetLastWin32Error()));
                }

                int[] attributes = {
                    (int)WGL.WGL_CONTEXT_MAJOR_VERSION_ARB, 4,
                    (int)WGL.WGL_CONTEXT_MINOR_VERSION_ARB, 5,
                    (int)WGL.WGL_CONTEXT_FLAGS_ARB, 0,
                    (int)WGL.WGL_CONTEXT_PROFILE_MASK_ARB, (int)WGL.WGL_CONTEXT_CORE_PROFILE_BIT_ARB
                    , 0
                };

                if ((this.hglrc = Wgl.CreateContextAttribsARB(this.hDC, IntPtr.Zero, attributes)) == IntPtr.Zero)
                {
                    throw new Exception(string.Format("CreateContextAttribsARB failed: error {0}", Marshal.GetLastWin32Error()));
                }

                Wgl.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Wgl.DeleteContext(tempContext);
                ReleaseDC(tempHwnd, tempHdc);
                DestroyWindow(tempHwnd);

                if (!Wgl.MakeCurrent(this.hDC, this.hglrc))
                {
                    throw new Exception(string.Format("Wgl.MakeCurrent failed: error {0}", Marshal.GetLastWin32Error()));
                }

                Utility.CheckGLError();
            }

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_STENCIL, GL.GL_FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE, IntBuffer);
            Utility.CheckGLError();
            var stencilBits = IntBuffer[0];
            if (stencilBits != 8)
            {
                throw new Exception("Failed to set stencilBits to 8.");
            }

            PrintGraphicInfo();
        }

        private void PrintGraphicInfo()
        {
            string version = GL.GetString(GL.GL_VERSION);
            Utility.CheckGLError();
            Debug.WriteLine("OpenGL version info: " + version);

            GL.GetIntegerv(GL.GL_MAX_TEXTURE_SIZE, IntBuffer);
            Utility.CheckGLError();
            int max_texture_size = IntBuffer[0];
            Debug.WriteLine("GL_MAX_TEXTURE_SIZE: " + max_texture_size);

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_BACK_LEFT, GL.GL_FRAMEBUFFER_ATTACHMENT_RED_SIZE, IntBuffer);
            Utility.CheckGLError();
            var redBits = IntBuffer[0];

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_BACK_LEFT, GL.GL_FRAMEBUFFER_ATTACHMENT_GREEN_SIZE, IntBuffer);
            Utility.CheckGLError();
            var greenBits = IntBuffer[0];

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_BACK_LEFT, GL.GL_FRAMEBUFFER_ATTACHMENT_BLUE_SIZE, IntBuffer);
            Utility.CheckGLError();
            var blueBits = IntBuffer[0];

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_BACK_LEFT, GL.GL_FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE, IntBuffer);
            Utility.CheckGLError();
            var alphaBits = IntBuffer[0];

            Debug.WriteLine("R{0} G{1} B{2} A{3}", redBits, greenBits, blueBits, alphaBits);

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_DEPTH, GL.GL_FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE, IntBuffer);
            Utility.CheckGLError();
            var depthBits = IntBuffer[0];
            Debug.WriteLine("Depth: " + depthBits);

            GL.GetFramebufferAttachmentParameteriv(GL.GL_DRAW_FRAMEBUFFER,
                GL.GL_STENCIL, GL.GL_FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE, IntBuffer);
            Utility.CheckGLError();
            var stencilBits = IntBuffer[0];
            Debug.WriteLine("Stencil: " + stencilBits);
        }

        public void SwapBuffers()
        {
            if(!SwapBuffers(this.hDC))
            {
                Log.Error($"SwapBuffers failed: Win32 error {Marshal.GetLastWin32Error()}");
            }
        }

        private void DeleteOpenGLContext()
        {
            Wgl.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
            Wgl.DeleteContext(this.hglrc);
            ReleaseDC(this.hwnd, this.hDC);
        }
    }
}
